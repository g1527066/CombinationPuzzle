using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    //タイム関係
    [SerializeField]//
    private float SetLimitTime = 10f;

    [SerializeField]
    public float CompleteAddTime = 10f;
    [SerializeField]
    private float BestDeleteAddTime = 1f;
    [SerializeField]
    private float SummaryDeleteAddTime = 0.3f;
    [SerializeField]
    public int SummaryDeleteAddCount = 4;
    //残り時間
    private float remainingTime = 0;
    public float RemainingTime
    {
        get
        {
            return remainingTime;
        }
    }

    [SerializeField]
    private Text TimeText = null;
    [SerializeField]
    Slider TimeSlider = null;
    //企画さんが設定できるように数値、、、
    //削除までの時間
    public float DeleteTime = 1.6f;
    //peace削除の点滅の時間
    public float flashingTime = 0.2f;

    private bool isTimeStop = false;

    public bool StopTimeFlag
    {
        get { return isTimeStop; }
    }
    public bool SetTimeStop
    {
        set { isTimeStop = value; ; }
    }

    [SerializeField]
    GameObject TimerObject = null;

    float totalTime = 0;
    public float GetTotalTime
    {
        get
        {
            return totalTime;
        }
    }

    bool fewTimeUp = false;
    float TimeUpFew = 5f;

    // Use this for initialization
    void Start () {
        if (SaveDataManager.Instance.GetMode == Mode.Mission)
        {
            remainingTime = SaveDataManager.Instance.GetMissionData.Elements[
                SaveDataManager.Instance.GetMissionNumber].LmiteTime;

            TimeSlider.maxValue = SaveDataManager.Instance.GetMissionData.Elements[
                SaveDataManager.Instance.GetMissionNumber].LmiteTime;
            TimeSlider.value = 0;

            //TODO:落下頻度も受けとるようにする

            TimerObject.SetActive(true);
        }
        else
        {
            TimerObject.SetActive(false);
        }
        isTimeStop = true;
    }

    public void StopTime()
    {
        isTimeStop = !isTimeStop;
    }

    void Update () {
        if (isTimeStop == false)
            totalTime += Time.deltaTime;
    }
    public void TimerControl()
    {
        TimeText.text = (int)(remainingTime / 60) + ":" + string.Format("{0:D2}", ((int)remainingTime % 60));
        TimeSlider.value = remainingTime;
    }

    public void TimerUpDatMissione()
    {
        //一旦タイム制限無し
        if (remainingTime < 0)
        {
            GameSystem.Instance.GameOver();
           GameSystem.Instance.ChangeDebugText("タイムアップ");
        }
        remainingTime -= Time.deltaTime;
        TimerControl();

        if (fewTimeUp == false && TimeUpFew >= remainingTime)
        {
            AudioManager.Instance.PlaySE("PAZ_SE_Time");
            fewTimeUp = true;
        }
        else if (fewTimeUp==true&&remainingTime>TimeUpFew)
        {
            fewTimeUp = false;
        }
    }
}
