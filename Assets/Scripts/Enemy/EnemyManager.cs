using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemiesList = new List<GameObject>();
    public GameObject keyPrefab;

    public void AddToList(GameObject enemy)
    {
        enemiesList.Add(enemy);
    }

    public void RemoveFromList(GameObject enemy)
    {
        enemiesList.Remove(enemy);
        
        if(enemiesList.Count == 0){
            Instantiate(keyPrefab, enemy.transform.position, Quaternion.identity);
        }
    }
}
