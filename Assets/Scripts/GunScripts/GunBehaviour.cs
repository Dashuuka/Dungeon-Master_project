using UnityEngine;

public class GunBehaviour : MonoBehaviour
{
    public GameObject bulletPrefab; // Префаб пули
    public Transform firePoint; // Точка вылета пули
    public float projectileSpeed; // Скорость пули
    public Animator anim; // Аниматор оружия
    public float fireRate=1f; // Скорость стрельбы (выстрелов в секунду)
    private float nextFireTime; // Время следующего выстрела

    // Метод для стрельбы
    public void Shoot()
    {
        // Проверяем, можно ли стрелять
        if (Time.time > nextFireTime)
        {
            // Создаем пулю
            firePoint.rotation = Quaternion.Euler(firePoint.rotation.eulerAngles + new Vector3(0, 0, 270));
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // Получаем направление к курсору
            Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoint.position;
            direction.z = 0; // Убираем z-координату, чтобы пуля летела в 2D плоскости

            // Присваиваем пуле скорость
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction.normalized * projectileSpeed;
            }


            // Обновляем время следующего выстрела
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    // Метод, вызываемый при завершении анимации стрельбы
    private void AnimationEvent_ShootFinished()
    {
        // Удаляем пулю
        GameObject bullet = transform.GetChild(0).gameObject;
        if (bullet != null)
        {
            Destroy(bullet);
        }
    }

    private void RotateGunTowardsMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z; // Устанавливаем z-координату мыши на z-координату объекта "gun"


        // Вычисляем вектор направления от объекта "gun" к мыши
        Vector3 direction = mousePosition - transform.position;

        // Вычисляем угол поворота в градусах
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Инвертируем угол, если лук направлен влево ИЛИ вверх
        if (direction.x < 0.01f)
        {
            angle += 180f;
        }

        // Поворачиваем объект "gun"
        transform.rotation = Quaternion.Euler(0f, 0f, angle); // Поворачиваем только по оси Z
    }


    // Обновление
    void Update()
    {
        RotateGunTowardsMouse();
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }
}