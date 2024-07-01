using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool isPlayerProjectile;
    public int damage;

    private void OnCollisionEnter2D(Collision2D collision)
    { 
        var collidedObject = collision.gameObject;
        if(isPlayerProjectile){
            if(collidedObject.CompareTag("Projectile")){
                if(!collidedObject.GetComponent<Projectile>().isPlayerProjectile){
                    Destroy(gameObject);
                }
            }else if(collidedObject.CompareTag("Enemy")){
                Destroy(gameObject);
            }
        }else{
            if(collidedObject.CompareTag("Projectile")){
                if(collidedObject.GetComponent<Projectile>().isPlayerProjectile){
                    Destroy(gameObject);
                }
            }else if(collidedObject.CompareTag("Player")){
                Destroy(gameObject);
            }
        }

        if(collidedObject.CompareTag("Wall")){
            Destroy(gameObject);
        }
    }

}
