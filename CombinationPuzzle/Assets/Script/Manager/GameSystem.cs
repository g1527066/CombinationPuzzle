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
    [SerializeField]
    private Text ResultText = null;

    private bool isGameOver = false;
    public bool IsGameOver
    {
        get { return isGameOver; }
    }

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
    GenerationCollision generationCollision = null;

    public bool MissionClear = false;

    [SerializeField]
    Timer timer = null;
    public Timer GetTimer
    {
        get
        {
            return timer;
        }
    }


    // Use this for initialization
    void Start()
    {
        isGameOver = false;
    }

    public void StartGame()
    {
        timer.SetTimeStop = false;
        generationCollision.GenerationCol();
    }

    // Update is called once per frame
    void Update()
    {
        if (SaveDataManager.Instance.GetMode == Mode.Mission && isGameOver == false && timer.StopTimeFlag == false)
        {
            timer.TimerUpDatMissione();
        }
    }


    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Clear()
    {
        MissionClear = true;
        Debug.Log("くりあ");
        timer.SetTimeStop = true;
        GameOver();

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

    public void GameOver()
    {
        isGameOver = true;

        EndImage.gameObject.SetActive(true);

        AudioManager.Instance.PlaySE("PAZ_SE_Result");

        StartCoroutine(ChangeResult());

    }

    private IEnumerator ChangeResult()
    {
        float stopTime = 2.0f;

        yield return new WaitForSeconds(stopTime);
        ResultObject.SetActive(true);


    }

    [SerializeField]
    Text debugText = null;
    public void ChangeDebugText(string s)
    {
        debugText.text = s;
    }

}
