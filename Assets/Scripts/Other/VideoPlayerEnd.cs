using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VideoPlayerEnd : MonoBehaviour
{
    public int wait;
    public int nextScene;
    void Start(){
        StartCoroutine(videoEnd());
    }

    public IEnumerator videoEnd(){
        yield return new WaitForSeconds(wait);
        SceneManager.LoadScene(nextScene);
    }
}
