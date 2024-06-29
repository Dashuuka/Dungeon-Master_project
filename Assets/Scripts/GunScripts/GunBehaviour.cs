using UnityEngine;

public class GunBehaviour : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float projectileSpeed;
    public Animator anim;
    public float fireRate = 1f;
    private float nextFireTime;

    public void Shoot()
    {

        if (Time.time > nextFireTime)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z;

            Vector3 direction = mousePosition - transform.parent.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


            firePoint.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 270f));
            
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction.normalized * projectileSpeed;
            }

            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void RotateGunTowardsMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;

        Vector3 direction = mousePosition - transform.parent.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (direction.x < 0f)
        {
            angle += 180f;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }


    void Update()
    {
        RotateGunTowardsMouse();
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }
}