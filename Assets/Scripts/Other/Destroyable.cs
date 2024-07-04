using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public GameObject effect;
    public Color effectColor;

    public void DestroyObject(){
        var effectObject = Instantiate(effect, transform.position, Quaternion.identity);
        var main = effectObject.GetComponent<ParticleSystem>().main;

        main.startColor = new ParticleSystem.MinMaxGradient(effectColor);

        Destroy(gameObject);
        Destroy(effectObject, 1f);
    }
}
