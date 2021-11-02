using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eClearConditions
{
    BreakingBlocks
}

[System.Serializable]
public class BreakingBlockClear
{
    public eBlockType blockType;
    public int count;
}

[System.Serializable]
public struct StageStarPoint
{
    public int[] starPoint;
}

public class GamePlayManager : MonoBehaviour
{
    [SerializeField] private int[] limitCounts = null;
    [SerializeField] private int addScores = 30;

    [SerializeField] private string[] explanations = null;

    [SerializeField] private StageStarPoint[] starScore = null;

    [SerializeField] private Image[] starImage = null;
    [SerializeField] private Sprite starOnImage = null;
    [SerializeField] private Text scoreText = null;
    [SerializeField] private Text limitCountText = null;
    [SerializeField] private Text explanationText = null;

    [SerializeField] private eClearConditions[] clearConditions;
    [SerializeField] private BreakingBlockClear[] breakingblockClears = null;


    private int starCount = 0;
    private int score = 0;

    public int stageNumber = 0;

    private bool isLimitCountTextColorChange = false;

    private event Action<Block, int> clearConditionCheck;

    private static GamePlayManager instance = null;

    public static GamePlayManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log("instance가 null입니다.");

                return null;
            }

            return instance;
        }

        private set
        {
            instance = value;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("중복된 instance 입니다.");
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        switch (clearConditions[stageNumber])
        {
            case eClearConditions.BreakingBlocks:
                clearConditionCheck += (block, count) =>
                {
                    if (block.blockType == breakingblockClears[stageNumber].blockType)
                    {
                        breakingblockClears[stageNumber].count -= count;
                        SetExplanationText(breakingblockClears[stageNumber].count);

                        if (breakingblockClears[stageNumber].count <= 0)
                        {
                            GameClear();
                        }
                    }
                };
                break;

            default:
                clearConditionCheck += (block, count) =>
                {
                    Debug.LogError("클리어 조건이 없습니다.");
                };
                break;
        }

        SetLimitCountText();
        SetExplanationText(breakingblockClears[stageNumber].count);
    }

    public void MoveBlock()
    {
        limitCounts[stageNumber]--;
        SetLimitCountText();

        if (!isLimitCountTextColorChange == limitCounts[stageNumber] <= 3)
        {
            isLimitCountTextColorChange = true;

            LimitCountTextColorChange();
        }

        if (limitCounts[stageNumber] <= 0)
        {
            GameOver();
        }
    }

    public void LimitCountTextColorChange()
    {
        limitCountText.color = Color.red;
    }

    public void BlockBroken(Block block, int count)
    {
        AddScore(count);
        clearConditionCheck(block, count);
    }

    private void AddScore(int count)
    {
        score += count * addScores;
        SetScoreText();
        StarCheck();
    }

    private void SetScoreText()
    {
        scoreText.text = "점수: " + score;
    }

    private void StarCheck()
    {
        if (starCount < 3 && starScore[stageNumber].starPoint[starCount] <= score)
        {
            starImage[starCount].sprite = starOnImage;
            starCount++;

            StarCheck();
        }
    }

    private void SetLimitCountText()
    {
        limitCountText.text = "제한횟수: " + Mathf.Clamp(limitCounts[stageNumber], 0, 100).ToString("00");
    }

    private void SetExplanationText(int count)
    {
        string[] str = explanations[stageNumber].Split('n');
        explanationText.text = str[0] + count + str[1];
    }

    private void GameClear()
    {
        Debug.Log("게임 클리어");
    }

    private void GameOver()
    {
        Debug.Log("게임 오버");
    }
}
