using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour{
    public GameObject floor;

    void Start(){
        for(int i = 0; i < 10; i++){
            for(int j = 0; j < 15; j++){
                Instantiate(floor, new Vector3(i, j, 0), Quaternion.identity);
            }
        }
    }
    

    void Update(){
        
    }
}
