using UnityEngine;

public class GunBehaviour : MonoBehaviour
{
    [Header("Projectile settings")]
    public GameObject projectilePrefab;
    public Sprite projectileSprite;
    [Header("Weapon settings")]
    public bool isPlayerProjectile;
    public Vector3 projectileOffset;
    public float projectileSpeed;
    public float fireRate;
    private float nextFireTime;

    [Header("Spread settings")]
    public bool useSpread;
    public float spreadAngle;
    public float spreadChance;

    [Header("Bullet settings")]
    public float damage;
    public int bulletCount;
    public float bulletSpreadAngle;

    private void Start()
    {
        nextFireTime = 0f;
    }

    public void Shoot(bool isPlayerShoot)
    {
        if (Time.time > nextFireTime)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;

                if (useSpread && Random.value <= spreadChance)
                {
                    angle += Random.Range(-spreadAngle * Mathf.Deg2Rad, spreadAngle * Mathf.Deg2Rad);
                }

                angle += i * bulletSpreadAngle * Mathf.Deg2Rad;

                GameObject projectile = Instantiate(projectilePrefab, transform.position + projectileOffset, Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg));
                projectile.GetComponent<SpriteRenderer>().sprite = projectileSprite;
                projectile.GetComponent<Projectile>().Initialize(isPlayerProjectile, damage);
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * projectileSpeed;
                }
            }

            nextFireTime = Time.time + 1f / fireRate;
        }
    }
}
