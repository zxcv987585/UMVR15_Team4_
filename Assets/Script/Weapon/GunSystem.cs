using UnityEngine;

public class GunSystem : MonoBehaviour
{
    //槍械參數
    private int Damage = 15;
    public float TimeBetweenShoting, spread, range,reloadingTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    private bool allowButtonHold;
    private int bulletLeft, bulletShot;

    //射擊判定
    private bool shooting, readyToShot, reloading;

    //參照物（用於射擊與瞄準）
    public Camera camera;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask Enemy;
    public LayerMask Wall;

    private void Awake()
    {
        bulletLeft = magazineSize;
        camera = Camera.main;
        if (camera == null)
        {
            Debug.LogError("找不到 MainCamera，請確認場景中有標記為 'MainCamera' 的攝影機！");
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
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

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
        readyToShot = false;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 direction = camera.transform.forward + new Vector3(x,y,0);

        //射線判定
        if (Physics.Raycast(camera.transform.position, direction, out rayHit, range, Enemy) )
        {
            Debug.Log(rayHit.collider.name);

            if (rayHit.collider.CompareTag("Enemy"))
                rayHit.collider.GetComponent<Health>().TakeDamage(Damage);
            
        }
        if (Physics.Raycast(camera.transform.position, direction, out rayHit, range, Wall))
        {
            if (rayHit.collider.CompareTag("Wall"))
                Debug.Log($"你打中了牆壁!");

        }
        bulletLeft--;
        bulletShot--;

        Invoke("ResetShot", TimeBetweenShoting);
        if(bulletShot > 0 && bulletLeft > 0)
        Invoke("Shoot", TimeBetweenShoting);
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
