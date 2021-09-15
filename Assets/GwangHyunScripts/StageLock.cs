using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine;

public class StageLock : MonoBehaviour
{
    int currentStage;
    public GameObject StageBtn;

    void Start()
    {
        Button[] buttons = StageBtn.GetComponentsInChildren<Button>();

        currentStage = PlayerPrefs.GetInt("stageUnlock");

        for (int i = currentStage; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
    }
    void Update()
    {
        
    }
}
