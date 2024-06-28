using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{
    public GameObject projectilePrefab; // Префаб снаряда
    public Transform firePoint; // Точка, из которой вылетает снаряд
    public float projectileSpeed = 10f; // Скорость снаряда (будет прокачиваться с повышением уровня)

    void Update()
    {
        RotateGunTowardsMouse();
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void RotateGunTowardsMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = mousePosition - transform.position;
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Shoot()
    {

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Vector3 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoint.position).normalized;

        projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

        /* // Воспроизводим анимацию атаки
         anim.SetTrigger("Attack");*/
    }
}
