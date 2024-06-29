using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour
{
    public Image healthBar;
    public float healthAmount;
    public float maxHealth;

    void Awake(){
        healthAmount = maxHealth;
    }

    public void TakeDamage (float damage)
    {
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount/maxHealth;

        if(healthAmount <= 0f){
            SceneManager.LoadScene(0);
        }

    }
    public void Heal (float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);
        healthBar.fillAmount = healingAmount/maxHealth;  
    }
}
