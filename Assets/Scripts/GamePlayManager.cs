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

public class GamePlayManager : MonoBehaviour
{
    [SerializeField] private int limitCount = 18;
    [SerializeField] private int[] starScore = null;
    [SerializeField] private int addScore = 20;

    [SerializeField] private Image[] starImage = null;
    [SerializeField] private Text scoreText = null;
    [SerializeField] private Text limitCountText = null;

    [SerializeField] private eClearConditions clearCondition;
    [SerializeField] private BreakingBlockClear breakingblockClear = null;

    private int starCount = 0;
    private int score = 0;

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
        switch (clearCondition)
        {
            case eClearConditions.BreakingBlocks:
                clearConditionCheck += (block, count) =>
                {
                    if (block.blockType == breakingblockClear.blockType)
                    {
                        breakingblockClear.count -= count;

                        if (breakingblockClear.count <= 0)
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
    }

    public void MoveBlock()
    {
        limitCount--;
        SetLimitCountText();

        if (limitCount <= 0)
        {
            GameOver();
        }
    }

    public void BlockBroken(Block block, int count)
    {
        AddScore(count);
        clearConditionCheck(block, count);
    }

    private void AddScore(int count)
    {
        score += count * addScore;
        SetScoreText();
        StarCheck();
    }

    private void SetScoreText()
    {
        scoreText.text = "점수: " + score;
    }

    private void StarCheck()
    {
        if (starCount < 3 && starScore[starCount] <= score)
        {
            starImage[starCount].color = Color.yellow;
            starCount++;

            StarCheck();
        }
    }

    private void SetLimitCountText()
    {
        limitCountText.text = "제한횟수: " + Mathf.Clamp(limitCount, 0, 100).ToString("00");
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
