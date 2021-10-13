using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text remainChanceTxt;

    public int remainChanceCnt = 99;

    
    
    public void EnterStage(int stageIndex)
    {
        remainChanceCnt--;
        remainChanceTxt.text = $"99/{remainChanceCnt}";

        SceneManager.LoadScene(stageIndex);
    }
}
