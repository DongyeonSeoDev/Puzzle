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

    private Button[] buttons;

    void Start()
    {
        buttons = StageBtn.GetComponentsInChildren<Button>();

        StageUnLock();
    }
    

    public void StageUnLock()
    {
        currentStage = PlayerPrefs.GetInt("stageUnlock");

        for (int i = currentStage; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        Debug.Log(currentStage);
    }

}
