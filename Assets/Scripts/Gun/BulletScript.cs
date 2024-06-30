using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    public int damage; // Урон стрелы

    private void OnCollisionEnter2D(Collision2D collision)
    { 
        // Проверяем, столкнулись ли мы с врагом
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Получаем компонент Enemy из объекта, с которым столкнулись
            EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                // Наносим урон врагу
                enemy.TakeDamage(damage);
            }

            
        }
        Destroy(gameObject);
    }

}
