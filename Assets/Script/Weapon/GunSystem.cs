using UnityEngine;
using UnityEngine.InputSystem;

public class GunSystem : MonoBehaviour
{
    //槍械參數
    public int Damage;
    public float TimeBetweenShooting, spread, range, reloadingTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    private int bulletLeft, bulletShot;

    //射擊判定
    private bool shooting, readyToShoot, reloading;

    //參照物（用於射擊與瞄準）
    public Camera camera;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask Enemy;
    public LayerMask Wall;

    //特效
    public GameObject muzzleFlash, bulletHole;

    private void Awake()
    {
        bulletLeft = magazineSize;
        camera = Camera.main;
        if (camera == null)
        {
            Debug.LogError("找不到攝影機！");
        }
        readyToShoot = true;
    }
    private void Update()
    {
        MyInput();
    }
    //輸入判定
    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (bulletLeft < magazineSize && !reloading) Reload();

        //射擊
        if(readyToShoot && shooting && !reloading && bulletLeft > 0)
        {
            bulletShot = bulletsPerTap;
            Shoot();
        }
    }

    //射擊程式碼
    private void Shoot()
    {
        readyToShoot = false;

        Ray cameraRay = camera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        Vector3 targetPoint;

        if (Physics.Raycast(cameraRay, out RaycastHit hit, range))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = cameraRay.origin + cameraRay.direction * range;
        }

        Vector3 direction = (targetPoint - attackPoint.position).normalized;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        direction += new Vector3(x, y, 0);

        //射線判定
        if (Physics.Raycast(attackPoint.position, direction, out rayHit, range, Enemy) )
        {
            Debug.Log(rayHit.collider.name);
            rayHit.collider.GetComponent<Health>().TakeDamage(Damage);

            if (bulletHole != null)
            {
                Instantiate(bulletHole, rayHit.point, Quaternion.Euler(0, 180, 0));
            }
        }

        if(muzzleFlash != null)
        {
            GameObject GunFire = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
            GunFire.transform.rotation = attackPoint.rotation;
        }

        if (Physics.Raycast(attackPoint.position, direction, out rayHit, range, Wall))
        {
            Debug.Log(rayHit.collider.name);

            if (bulletHole != null)
            {
                Instantiate(bulletHole, rayHit.point, Quaternion.Euler(0, 180, 0));
            }
        }

            bulletLeft--;
        bulletShot--;

        Invoke("ResetShot", TimeBetweenShooting);

        if (bulletShot > 0 &&  bulletLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    //換彈程式碼（用於冷卻系統）
    private void Reload()
    {
        reloading = true;
        Invoke("ReloadingFinished", reloadingTime);
    }

    private void ReloadingFinished()
    {
        bulletLeft = magazineSize;
        reloading = false;
    }
}
