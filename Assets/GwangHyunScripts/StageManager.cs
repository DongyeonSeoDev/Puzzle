using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

using DG.Tweening;

public class StageManager : MonoBehaviour
{
    private int nowStage = 1;
    private int maxStage = 20;

    private int stageIndex = 0;

    public Image curStage;

    public GameObject btnObj;
    public Button[] buttonTrs;

    public List<RectTransform> rect;

    public UIManager uIManager;
    private StageLock stageLock;


    private void Start()
    {
        buttonTrs = btnObj.GetComponentsInChildren<Button>();
        stageLock = GetComponent<StageLock>();

        for (int i = 0; i < buttonTrs.Length; i++)
        {
            rect.Add(buttonTrs[i].GetComponent<RectTransform>());
        }

        StageClear();
    }

    public void StageClear()
    {
        if(GameManager.Instance.stageClear)
        {
            nowStage++;
            PlayerPrefs.SetInt("stageUnlock", nowStage);

            curStage.transform.DOMove(buttonTrs[0].transform.position, 2f);
            stageIndex++;

            curStage.transform.parent = rect[stageIndex].transform.parent.transform;
            MoveStageAnimation();

            uIManager.StageClear(GameManager.Instance.coin);
            uIManager.DrawStar(buttonTrs[uIManager.stageLevel].GetComponent<Image>());
        }
    }


    public void MoveStageAnimation()
    {
        curStage.rectTransform.DOAnchorPos(new Vector3(rect[stageIndex].localPosition.x,
                            rect[stageIndex].localPosition.y + 200f, rect[stageIndex].localPosition.z), 3f);
    }

    public void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("stageUnlock", nowStage);
    }
    public void SelectStage(int dataIndex)
    {
        if (uIManager.heartCnt > 0)
        {
            uIManager.StartStage();
            //데이터 불러오기
            //버튼 이벤트
        }
        else
        {
            return;
        }
    }
}
