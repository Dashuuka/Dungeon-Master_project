using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool isPlayerProjectile;
    public float damage;


    public void Initialize(bool isPlayerProjectile, float damage){
        this.isPlayerProjectile = isPlayerProjectile;
        this.damage = damage;
        if (isPlayerProjectile)
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    { 
        var collidedObject = collision.gameObject;
        Destroy(gameObject);
    }

}
