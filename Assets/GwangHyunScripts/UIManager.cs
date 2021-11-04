using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [Header("스테이지")]
    [SerializeField]
    private Text stageLevelTxt;
    public GameObject StartBtnPanel;
    private int stageLevel;

    [Header("하트")]
    [SerializeField]
    private Transform heartPos;
    [SerializeField]
    public GameObject heart;
    public Transform canvasTr;
    [SerializeField]
    private Text heartTxt;
    public int heartCnt = 5;
    private int maxChance = 99;
    [SerializeField]
    private GameObject StorePanel;

    [Header("골드")]
    [SerializeField]
    private Text goldText;
    private int gold = 1000;

    public void StartStage()
    {
        heartCnt--;

        GameObject heartImage = Instantiate(heart, heartPos);
        heartImage.transform.SetParent(canvasTr.transform);
        heart.transform.DOScale(new Vector3(2, 2, 2), 1f);
        SoundManager.SoundPlay(eSoundType.TOUCHSOUND);
        heartImage.transform.DOMove(StartBtnPanel.transform.position, 1f).OnComplete(() =>
        {
            SceneManager.LoadScene("Main");
        });
    }
    private void Update()
    {
        goldText.text = $"{gold}";
        heartTxt.text = $"{heartCnt}/{maxChance}";
    }

    public void ExitBtn()
    {
        StartBtnPanel.SetActive(false);
    }

    public void OnStartBtn(int stageIndex)
    {
        StartBtnPanel.SetActive(true);
        stageLevel = stageIndex;
        stageLevelTxt.text = $"Level {stageLevel}";
        SoundManager.SoundPlay(eSoundType.TOUCHSOUND);
    }
    
    public void OnStorePanel()
    {
        StorePanel.SetActive(true);
        StorePanel.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0f);
        StorePanel.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0f);
    }

    public void ExitStore()
    {
        StorePanel.SetActive(false);
    }

    public void BuyHeart(int buyGold)
    {
        if (gold >= buyGold)
        {
            int buyHeartCnt = buyGold / 1000;
            gold -= buyGold;
            heartCnt += buyHeartCnt;
            Debug.LogError(heartCnt);
        }
    }

    public void StageClear(int coin)
    {
        gold += coin;
    }
}
