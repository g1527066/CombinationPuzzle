using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Result : MonoBehaviour
{

    //[SerializeField]
    //GameObject DrawWindow = null;


    //ミッション-------------------------------------
    [Header("ミッション")]

    [SerializeField]
    GameObject MissionWindow = null;

    //お題目、成功画像、クリアタイム」
    [SerializeField]
    Text MissionTitleText = null;
    [SerializeField]
    Image ResultImage = null;
    [SerializeField]
    Sprite ClearSprite = null;
    [SerializeField]
    Sprite MissSprite = null;
    [SerializeField]
    Text MissionClearTime = null;



    //マラソン-------------------------------------
    [Header("マラソン")]

    [SerializeField]
    GameObject MarathonDrawWindow = null;

    [SerializeField]
    Text MarathonTitleText = null;
    [SerializeField]
    Text MarathonScoreText = null;
    [SerializeField]
    Text MarathonTotleTimeText = null;
    [SerializeField]
    Text MarathonHiScoreText = null;



    // Use this for initialization
    void Start()
    {
        //    DrawWindow.SetActive(true);

        Debug.Log("リザルトStart");

        if (SaveDataManager.Instance.GetMode == Mode.Marathon)
        {
            Debug.Log("リザルトMission");

            MarathonDrawWindow.SetActive(true);
            //タイトル、すこあ、プレイ時間、ハイスコアをセットする
            MarathonTitleText.text = "「永久之道」";
            MarathonScoreText.text = GameSystem.Instance.GetScore + "点";

            MarathonTotleTimeText.text = ReturnTime((int)GameSystem.Instance.GetTimer.GetTotalTime);

            //今回のがハイスコアより高ければセット
            Debug.Log(SaveDataManager.Instance.GetMarathonHiScore+"過去のハイスコア    今回＝"+ GameSystem.Instance.GetScore);
            if (SaveDataManager.Instance.GetMarathonHiScore < GameSystem.Instance.GetScore)
                SaveDataManager.Instance.SetMarathonHiScore(GameSystem.Instance.GetScore);
            MarathonHiScoreText.text = SaveDataManager.Instance.GetMarathonHiScore + "点";
        }
        else
        {

            Debug.Log("リザルトMarathon");

            MissionWindow.SetActive(true);
            MissionTitleText.text = "「"+SaveDataManager.Instance.GetMissionData.Elements[SaveDataManager.Instance.GetMissionNumber].MissionName +"」";
            if (GameSystem.Instance.MissionClear)
            {
                ResultImage.sprite = ClearSprite;
                MissionClearTime.text = "クリアタイム    " + ReturnTime((int)GameSystem.Instance.GetTimer.GetTotalTime);
                SaveDataManager.Instance.SaveClearMission();
            }
            else
            {
                ResultImage.sprite = MissSprite;
                MissionClearTime.text = "";
            }

        }


    }


    //秒(int)を渡すと。秒フンつけてかえってくる
    public static string ReturnTime(int time)
    {
        string playTimeString = "";
        if (time / 60 != 0)
        {
            playTimeString += time / 60 + "分";
        }
        playTimeString += time % 60 + "秒";

        return playTimeString;
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            AudioManager.Instance.PlaySE("PAZ_SE_OK");

            SceneManager.LoadScene("Title");
        }

    }
}
