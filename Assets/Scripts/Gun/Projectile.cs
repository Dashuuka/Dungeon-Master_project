using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public bool isPlayerProjectile;
    public float damage;

    [Header("Bullet destroy effect")]
    public GameObject effect;


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
        if(collidedObject.CompareTag("Destroyable")){
            collidedObject.GetComponent<Destroyable>().DestroyObject();
        }else{
            if(collidedObject.CompareTag("Player") && !isPlayerProjectile){
                collidedObject.GetComponent<PlayerBehaviourScript>().TakeDamage(damage);
            }else if(collidedObject.CompareTag("Enemy") && isPlayerProjectile){
                collidedObject.GetComponent<EnemyAI>().TakeDamage(damage);
            }
            var effectObject = Instantiate(effect, transform.position, Quaternion.identity);
            Destroy(effectObject, 1f);
        }
        Destroy(gameObject);
        
    }

}
