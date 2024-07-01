using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkScript : MonoBehaviour
{
    public List<Sprite> markSprites = new List<Sprite>();

    public void SetMark(int id){
        GetComponent<SpriteRenderer>().sprite = markSprites[id];
    }
}
