using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    List<GameObject> enemiesList = new List<GameObject>();

    public void addToList(GameObject enemy){
        enemiesList.Add(enemy);
    }

    public void removeFromList(GameObject enemy){
        enemiesList.Remove(enemy);
    }

    public bool checkEnemies(){
        return enemiesList.Count == 0;
    }
}
