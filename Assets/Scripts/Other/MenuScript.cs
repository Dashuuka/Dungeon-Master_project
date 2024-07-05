using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    float volume;
    public GameObject SettingsPanel;

    public void StartNewGame(){
        SceneManager.LoadScene(1);
    }

    public void Settings(){
        if(SettingsPanel.activeSelf){
            SettingsPanel.SetActive(false);
        }else{
            SettingsPanel.SetActive(true);
        }
    }

    public void Exit(){
        Application.Quit();
    }

    public void High(){
        QualitySettings.SetQualityLevel(5);
    }

    public void Medium(){
        QualitySettings.SetQualityLevel(2);
    }

    public void Low(){
        QualitySettings.SetQualityLevel(0);
    }

    public void volumeChange(float value){
        volume = value;
    }
}
