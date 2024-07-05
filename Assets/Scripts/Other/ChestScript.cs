using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ChestScript : MonoBehaviour
{
    public GameObject effect;
    public Color effectColor;

    public GameObject audioGhost;
    public AudioClip destroySound;

    public int cost;

    public List<GameObject> loot = new List<GameObject>();

    void Awake(){
        cost = Random.Range(5, 50);
        GetComponentInChildren<TMP_Text>().text = cost.ToString() + " coins";
    }

    public void DestroyObject()
    {
        var effectObject = Instantiate(effect, transform.position, Quaternion.identity);
        var main = effectObject.GetComponent<ParticleSystem>().main;
        main.startColor = new ParticleSystem.MinMaxGradient(effectColor);
        var audio = Instantiate(audioGhost, transform.position, Quaternion.identity);
        audio.GetComponent<AudioSource>().PlayOneShot(destroySound);
        Destroy(audio, destroySound.length);

        DropLoot();

        Destroy(gameObject);
        Destroy(effectObject, 1f);
    }

    private void DropLoot()
    {
        Instantiate(loot[Random.Range(0, loot.Count)], transform.position, Quaternion.identity);
    }
}
