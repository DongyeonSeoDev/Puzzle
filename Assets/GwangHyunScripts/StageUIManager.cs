using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StageUIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject nextStagePanel;
    public void nextStageBtn()
    {

    }

    public void reStartBtn()
    {

    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void ReStart()
    {
        nextStagePanel.SetActive(true);
    }
    
}
