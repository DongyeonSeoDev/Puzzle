using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
public class SetBarricade
{
    public Vector2Int[] position;
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

    [SerializeField] private CanvasGroup feverCanvasGroup = null;
    [SerializeField] private GameObject feverPanel = null;

    [SerializeField] private CanvasGroup gameClearPanel;
    [SerializeField] private CanvasGroup gameOverPanel;
    [SerializeField] private CanvasGroup developPanel;

    [SerializeField] private Button[] closeButtons;
    [SerializeField] private Button developButton;

    public SetBarricade[] barricade;

    private int starCount = 0;
    private int score = 0;

    public int stageNumber = 0;

    private bool isLimitCountTextColorChange = false;

    private event Action<Block, int> clearConditionCheck;

    public bool gameEnd = false;

    private bool isDevelopPanelChanging = false;

    private static GamePlayManager instance = null;

    public event Action gameClearEvent = () =>
    {

    };

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

        foreach (var button in closeButtons)
        {
            button.onClick.AddListener(() =>
            {
                Debug.Log("닫기");
                //TODO: Scene 이동
            });
        }

        developButton.onClick.AddListener(() =>
        {
            DevelopPanelClose();
        });
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

        SoundManager.SoundPlay(eSoundType.TOUCHSOUND);
    }

    public bool limitCountCheck()
    {
        limitCounts[stageNumber]--;
        SetLimitCountText();

        if (limitCounts[stageNumber] <= 0)
        {
            return false;
        }

        return true;
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

    public void BlockParticle(Block block)
    {
        ParticleManager.StartParticle(BlockPosition(block), block.effectTexture);
    }

    private Vector3 BlockPosition(Block block)
    {
        return block.transform.position;
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
        SetScoreText();

        string[] str = explanations[stageNumber].Split('n');
        count = Mathf.Clamp(count, 0, 1000);
        explanationText.text = str[0] + count + str[1];
    }

    private void GameClear()
    {
        if (gameEnd)
        {
            return;
        }

        Debug.Log("게임 클리어");

        gameEnd = true;

        FeverTime();
    }

    private void GameOver()
    {
        if (gameEnd)
        {
            return;
        }

        gameEnd = true;

        Debug.Log("게임 오버");

        GameOverPanel();
        SoundManager.SoundPlay(eSoundType.GAMEOVER);
    }

    private void FeverTime()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(feverCanvasGroup.DOFade(1f, 0.5f));
        sequence.Join(feverCanvasGroup.transform.DOMoveY(1f, 0.5f).SetRelative());
        sequence.Append(feverCanvasGroup.DOFade(0f, 0.5f).SetDelay(0.5f));
        sequence.Join(feverCanvasGroup.transform.DOMoveY(-1f, 0.5f).SetRelative().OnComplete(() =>
        {
            gameClearEvent();
        }));
    }

    public void GameClearPanel()
    {
        gameClearPanel.interactable = true;
        gameClearPanel.blocksRaycasts = true;

        gameClearPanel.DOFade(1, 0.5f);

        SoundManager.SoundPlay(eSoundType.GAMECLEAR);
    }

    private void GameOverPanel()
    {
        gameOverPanel.interactable = true;
        gameOverPanel.blocksRaycasts = true;

        gameOverPanel.DOFade(1, 0.5f);
    }

    public void DevelopPanelOpen()
    {
        if (isDevelopPanelChanging)
        {
            return;
        }

        isDevelopPanelChanging = true;

        developPanel.interactable = true;
        developPanel.blocksRaycasts = true;

        developPanel.DOFade(1, 0.5f).OnComplete(() =>
        {
            isDevelopPanelChanging = false;
        });
    }

    private void DevelopPanelClose()
    {
        if (isDevelopPanelChanging)
        {
            return;
        }

        isDevelopPanelChanging = true;

        developPanel.DOFade(0, 0.5f).OnComplete(() =>
        {
            developPanel.interactable = false;
            developPanel.blocksRaycasts = false;

            isDevelopPanelChanging = false;
        });
    }
}
