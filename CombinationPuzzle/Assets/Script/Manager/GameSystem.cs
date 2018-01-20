using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

static class MyDebug
{
    public static Text text = null;

    public static void DrawError(string drawText)
    {
        text.text = drawText;
    }
}



public class GameSystem : SingletonMonoBehaviour<GameSystem>
{
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


    [SerializeField]
    private Text ResultText = null;

    private bool isGameOver = false;
    public bool IsGameOver
    {
        get { return isGameOver; }
    }
    private bool isTimeStop = false;


    //得点
    private int score = 0;
    public int GetScore
    {
        get
        {
            return score;
        }
    }
    [SerializeField, Header("通常消し加算ポイント")]
    private int CompleteMissionPoint = 100;
    [SerializeField, Header("一気削除得点倍[0四[1五")]
    private int[] CollectDoublePoint = { 2, 4 };
    [SerializeField, Header("一気削除追加得点")]
    private int CollectDeletePoint = 10;


    [SerializeField]
    Text ScoreText = null;
    [SerializeField]
    GameObject ChallengeWindow = null;


    [SerializeField]
    GameObject Timer = null;

    float totalTime = 0;
    public float GetTotalTime
    {
        get
        {
            return totalTime;
        }
    }

    [SerializeField]
    GenerationCollision generationCollision = null;

    // Use this for initialization
    void Start()
    {

        MyDebug.text = ResultText;
        remainingTime = SetLimitTime;
        isGameOver = false;

        TimeSlider.maxValue = SetLimitTime;
        TimeSlider.value = 0;
        if (SaveDataManager.Instance.GetMode == Mode.Marathon)
            Timer.SetActive(false);
        isTimeStop = true;
    }

    public void StartGame()
    {
        isTimeStop = false;
        generationCollision.GenerationCol();
    }

    // Update is called once per frame
    void Update()
    {
        if (SaveDataManager.Instance.GetMode == Mode.Mission && isGameOver == false && isTimeStop == false)
        {
            //一旦タイム制限無し
            if (remainingTime < 0)
            {
                GameOver();
            }
            remainingTime -= Time.deltaTime;
            TimerControl(0, 0, 0);
        }

        if (isTimeStop == false)
            totalTime += Time.deltaTime;
    }

    public void TimerControl(int SummaryCount, int BestCount, float addTime)
    {
        remainingTime += SummaryCount * SummaryDeleteAddTime + BestDeleteAddTime * BestCount + addTime;
        TimeText.text = (int)remainingTime / 60 + ":" + string.Format("{0:D2}", ((int)remainingTime % 60));
        TimeSlider.value = remainingTime;
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Clear()
    {
        // isGameOver = true;
        ResultText.text = "Clear!";
        StopTime();
    }

    //いったん操作できてしまう
    public void StopTime()
    {
        isTimeStop = !isTimeStop;
        ChallengeWindowActive();
        // ResultText.gameObject.SetActive(false);
    }

    private void UpdateScore()
    {
        ScoreText.text = score.ToString();
    }

    public void AddScorePoint(int deleteCount, PeaceForm peaceForm)
    {
        int add = 100;
        if (deleteCount > 3)
        {
            add += (deleteCount - 3) * CollectDeletePoint;
        }
        if (peaceForm == PeaceForm.Square)
            add *= CollectDoublePoint[0];
        else if (peaceForm == PeaceForm.Pentagon)
            add *= CollectDoublePoint[1];
        score += add;
        UpdateScore();
    }

    //表示する
    public void ChallengeWindowActive()
    {
        Debug.Log(ChallengeWindow.activeInHierarchy);
        if (ChallengeWindow.activeInHierarchy == false)
            ChallengeWindow.SetActive(true);
        else
            ChallengeWindow.SetActive(false);
    }




    public void TestDebugPeace()
    {
        Debug.Log("---------------------------------------------------");
        foreach (var key in PeaceManager.Instance.GetPeaceTabel.Keys)
        {
            Debug.Log("X=" + key.X + "  Y=" + key.Y);
        }
        Debug.Break();
    }

    [SerializeField]
    Image EndImage = null;
    [SerializeField]
    GameObject ResultObject = null;
    [SerializeField]
    Sprite EndSprite = null;

    public void GameOver()
    {
        isGameOver = true;
        //if (isTimeStop == false)
        //    StopTime();//一旦テスト用にコメント
        //  ResultText.text = "GameOver!";

        EndImage.gameObject.SetActive(true);

        EndImage.sprite = EndSprite;

        StartCoroutine(ChangeResult());

    }


    private IEnumerator ChangeResult()
    {
        float stopTime = 2.0f;

        yield return new WaitForSeconds(stopTime);
        ResultObject.SetActive(true);


    }


}
