using System.Collections;
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
    public float manaCost;
    private float nextFireTime;

    [Header("Spread settings")]
    public bool useSpread;
    public float spreadAngle;
    public float spreadChance;

    [Header("Bullet settings")]
    public float damage;
    public int bulletCount;
    public float bulletSpreadAngle;

    [Header("Reload settings")]
    public int magazineSize;
    public int currentAmmo;
    public float reloadTime;
    public bool isReloading;

    [Header("Shoot animation")]
    private Vector3 originalScale;
    public float shootAnimationScale;
    public float shootAnimationDuration = 0.1f;

    [Header("Audio settings")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    private AudioSource audioSource;

    private void Start()
    {
        nextFireTime = 0f;
        currentAmmo = magazineSize;
        isReloading = false;
        originalScale = transform.localScale;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (!isPlayerProjectile)
        {
            if (isReloading)
                return;

            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
                return;
            }
        }
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMagazineSize()
    {
        return magazineSize;
    }

    public bool Shoot(bool isPlayerShoot, float currentMana)
    {
        if (isReloading)
        {
            return false;
        }

        if (isPlayerShoot && currentMana < manaCost)
        {
            return false;
        }

        if (Time.time > nextFireTime && currentAmmo > 0)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                float angle = (transform.rotation.eulerAngles.z - (bulletCount - 1) * bulletSpreadAngle / 2) * Mathf.Deg2Rad;

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
            currentAmmo--;

            if (shootSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            StartCoroutine(PlayShootAnimation());

            return true;
        }

        return false;
    }

    public IEnumerator Reload()
    {
        currentAmmo = 0;
        isReloading = true;


        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;

        if (reloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
        
        StartCoroutine(PlayReloadAnimation());
    }

    private IEnumerator PlayReloadAnimation()
    {
        float duration = 0.3f;
        float elapsedTime = 0f;
        Quaternion initialRotation = transform.rotation;

        while (elapsedTime < duration)
        {
            transform.rotation = initialRotation * Quaternion.Euler(0f, 0f, 360f * (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = initialRotation;
        currentAmmo = magazineSize;
        isReloading = false;
    }

    private IEnumerator PlayShootAnimation()
    {
        float elapsedTime = 0f;
        while (elapsedTime < shootAnimationDuration)
        {
            float scale = Mathf.Lerp(originalScale.x, originalScale.x * shootAnimationScale, elapsedTime / shootAnimationDuration);
            transform.localScale = new Vector3(scale, scale, scale);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }
}
