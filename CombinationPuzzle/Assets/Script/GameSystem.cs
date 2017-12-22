using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

static class MyDebug
{
   public static  Text text = null;

    public static void DrawError(string  drawText)
    {
        text.text = drawText;
    }
}


public class GameSystem : MonoBehaviour
{
    public static GameSystem I = null;



    [SerializeField]
    private float SetLimitTime = 10f;

    private float remainingTime = 0;

    [SerializeField]
    private Text TimeText = null;
    [SerializeField]
    private Text ResultText = null;

    [SerializeField]
    Slider TimeSlider = null;


    private bool isGameOver = false;

    private bool isTimeStop = false;


    [SerializeField]
    JudgeManager judgeManager = null;

    //企画さんが設定できるように数値、、、
    //削除までの時間
    public float DeleteTime = 1.6f;
    //peace削除の点滅の時間
    public float flashingTime = 0.2f;

    public bool IsGameOver
    {
        get { return isGameOver; }
    }



    // Use this for initialization
    void Start()
    {
        I = this;
        MyDebug.text = ResultText;
        remainingTime = SetLimitTime;
        isGameOver = false;

        TimeSlider.maxValue = SetLimitTime;
        TimeSlider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver == false&& isTimeStop==false)
        {
            //一旦タイム制限無し
            //if (remainingTime < 0)
            //{
            //    isGameOver = true;
            //    ResultText.text = "TimeOver!";
            //}
            remainingTime -= Time.deltaTime;
            TimeText.text = (int)remainingTime / 60 + ":" + string.Format("{0:D2}", ((int)remainingTime % 60));

            TimeSlider.value = remainingTime;
        }
    }

    public void ResetScene()
    {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Clear()
    {
       // isGameOver = true;
        ResultText.text = "Clear!";
    }

    //いったん操作できてしまう
    public void StopTime()
    {
        isTimeStop = !isTimeStop;
        judgeManager.ChallengeWindowActive();
        ResultText.gameObject.SetActive(false);
    }

}
