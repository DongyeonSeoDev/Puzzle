using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class UIManager : MonoBehaviour
{
    public Text remainChanceTxt;

    
    public int remainChanceCnt;

    
    
    public void EnterStage()
    {
        remainChanceCnt--;
        remainChanceTxt.text = $"{remainChanceCnt}";
    }
}
