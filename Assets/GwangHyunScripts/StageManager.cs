using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    int nowStage = 1;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        PlayerPrefs.SetInt("stageUnlock", nowStage);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }


    void StageClear()
    {
        nowStage++;
        PlayerPrefs.SetInt("stageUnlock", nowStage);
    }
}
