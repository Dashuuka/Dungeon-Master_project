using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Destroyable : MonoBehaviour
{
    public GameObject effect;
    public Color effectColor;

    public GameObject audioGhost;
    public AudioClip destroySound;

    [System.Serializable]
    public class Loot
    {
        public GameObject itemPrefab;
        public float dropChance;
        public int minAmount;
        public int maxAmount;
    }

    public List<Loot> lootTable;
    public float lootSpreadRadius = 1.0f;

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
        foreach (var loot in lootTable)
        {
            if (Random.value * 100f <= loot.dropChance)
            {
                int amountToDrop = Random.Range(loot.minAmount, loot.maxAmount);
                for (int i = 0; i < amountToDrop; i++)
                {
                    Vector3 randomOffset = new Vector3(
                        Random.Range(-lootSpreadRadius, lootSpreadRadius),
                        Random.Range(-lootSpreadRadius, lootSpreadRadius),
                        0
                    );
                    Instantiate(loot.itemPrefab, transform.position + randomOffset, Quaternion.identity);
                }
            }
        }
    }
}
