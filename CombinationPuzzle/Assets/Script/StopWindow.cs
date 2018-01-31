using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopWindow : MonoBehaviour
{
    //Mission
    [SerializeField]
    GameObject MissionSet = null;

    [SerializeField]
    Text MissionLimitTime = null;
    [SerializeField]
    Text MissionTitle = null;

    //マラソン
    //スコアじかんはいすこあ
    [SerializeField]
    GameObject MarathonSet = null;

    [SerializeField]
    Text MarathonScore = null;
    [SerializeField]
    Text MarathonTotalTime = null;
    [SerializeField]
    Text MarathonHiScore = null;


    [SerializeField]
    GameObject StopTimeWindow = null;
    [SerializeField]
    GameObject EffectPool = null;

    // Use this for initialization
    void Start()
    {
        if(SaveDataManager.Instance.GetMode==Mode.Marathon)
        {
            MarathonHiScore.text = SaveDataManager.Instance.GetMarathonHiScore+"点";
        }
        else
        {
            MissionTitle.text="「"+SaveDataManager.Instance.GetMissionData.Elements[SaveDataManager.Instance.GetMissionNumber].MissionName +"」";
        }
    }

    public void ClickStopButton()
    {
        AudioManager.Instance.PlaySE("PAZ_SE_OK");
        if (StopTimeWindow.activeInHierarchy == false)
        {
            EffectPool.SetActive(false);
            GameSystem.Instance.GetTimer.StopTime();
            StopTimeWindow.SetActive(true);

            if (SaveDataManager.Instance.GetMode == Mode.Marathon)
            {
                MarathonSet.SetActive(true);
                //スコア、プレイ時間　表示
                MarathonScore.text = GameSystem.Instance.GetScore+"点";
                MarathonTotalTime.text = Result.ReturnTime(GameSystem.Instance.GetTimer.GetTotalTime);
            }
            else
            {
                MissionSet.SetActive(true);
                //ピース生成、残り時間
                MissionLimitTime.text= Result.ReturnTime(GameSystem.Instance.GetTimer.RemainingTime);
                PeaceJudger.Instance.mission.DrawRemainingMissionIcon(MissionSet, true, new Vector2(-507, 72), 200,0.7f);
            }
        }
        else
        {
            //消すのみ
            MarathonSet.SetActive(false);
            StopTimeWindow.SetActive(false);
            MissionSet.SetActive(false);
            StopTimeWindow.SetActive(false);
            GameSystem.Instance.GetTimer.StopTime();
            EffectPool.SetActive(true);
        }
    }

    public void ClickEndButton()
    {
        AudioManager.Instance.PlaySE("PAZ_SE_OK");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }

    public void ClickKeepButton()
    {
        AudioManager.Instance.PlaySE("PAZ_SE_OK");
        MarathonSet.SetActive(false);
        StopTimeWindow.SetActive(false);
        MissionSet.SetActive(false);
        StopTimeWindow.SetActive(false);
        GameSystem.Instance.GetTimer.StopTime();
        EffectPool.SetActive(true);
    }
}
