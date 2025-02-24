using UnityEngine;

public class GunSystem : MonoBehaviour
{
    //槍械參數
    public int Damage;
    public float TimeBetweenShoting, spread, range,reloadingTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    private int bulletLeft, bulletShot;

    //射擊判定
    private bool shooting, readyToShot, reloading;

    //參照物（用於射擊與瞄準）
    public Camera camera;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask Enemy;
    public LayerMask Wall;

    //特效
    public GameObject muzzleFlash;

    private void Awake()
    {
        bulletLeft = magazineSize;
        camera = Camera.main;
        if (camera == null)
        {
            Debug.LogError("找不到攝影機！");
        }
        readyToShot = true;
    }
    private void Update()
    {
        MyInput();
    }
    //輸入判定
    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetMouseButton(0);
        else shooting = Input.GetMouseButtonDown(0);

        if (bulletLeft < magazineSize && !reloading) Reload();

        //射擊
        if(readyToShot && shooting && !reloading && bulletLeft > 0)
        {
            bulletShot = bulletsPerTap;
            Shoot();
        }
    }

    //射擊程式碼
    private void Shoot()
    {
        if (!readyToShot) return;

        readyToShot = false;

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

            if (rayHit.collider.CompareTag("Enemy"))
                rayHit.collider.GetComponent<Health>().TakeDamage(Damage);
        }

        if(muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        bulletLeft--;
        bulletShot--;

        if(bulletShot > 0 &&  bulletLeft > 0)
        {
            Invoke("Shoot", TimeBetweenShoting);
        }
        Invoke("ResetShot", TimeBetweenShoting);
      
    }

    private void ResetShot()
    {
        readyToShot = true;
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
