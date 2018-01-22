using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartProduction : MonoBehaviour
{
    //activeではいけないもの

    [SerializeField]
    GameObject inputManager = null;
    [SerializeField]
    Button stopButton = null;



    //始め！,表示する時間
    [SerializeField]
    Image startImage = null;
    float DrawTime = 2.0f;

    [SerializeField]
    Flashing tapStart = null;
    bool tapFlag = false;

    //表示するミッション、マラソン
    [SerializeField]
    GameObject DrawTopWindow = null;

    [SerializeField]
    GameObject MissionSet = null;
    [SerializeField]
    Text MarathonHiScoreText = null;

    [SerializeField]
    GameObject MarathonSet = null;

    //Mission関係
    //Missionタイトル、各絵、制限時間
    [SerializeField]
    MissionData missionData = null;

    [SerializeField]
    Text MissionTitle = null;
    [SerializeField]
    Text MissionLimitTime = null;
    [SerializeField]
    MissionStartIcon missionStartPrefab = null;
    [SerializeField]
    RectTransform rectTransform;
    [SerializeField]
    float SlideX = 10;


    private void Start()
    {
        if (SaveDataManager.Instance.GetMode == Mode.Mission)
        {

            MissionSet.SetActive(true);
            MissionTitle.text = "「" + missionData.Elements[SaveDataManager.Instance.GetMissionNumber].MissionName + "」";
            int limit = missionData.Elements[SaveDataManager.Instance.GetMissionNumber].LmiteTime;
            string timeString = "";
            if (limit >= 60)
            {
                timeString += limit / 60 + "分";
            }
            if (limit % 60 != 0)
            {
                timeString += limit % 60 + "秒";
            }
            MissionLimitTime.text = timeString;



        }
        else
        {
            MarathonSet.SetActive(true);

            MarathonHiScoreText.text = SaveDataManager.Instance.GetMarathonHiScore.ToString() + "点";
        }
    }

    void SetDrawIcon(MissionData.MissionParams details, RectTransform rect, float RightSlide)
    {
        for (int i = 0; i < details.MissionList.Count; i++)
        {
           //画像、個数、消去


        }
    }



    private void Update()
    {
        if (tapFlag == false && null == tapStart)
        {
            AudioManager.Instance.PlaySE("PAZ_SE_Start2");
            tapFlag = true;
            StartCoroutine(DeleteStart());
        }
    }

    private IEnumerator DeleteStart()
    {
        AudioManager.Instance.PlayBGM("PAZ_BGM_Game");

        //Activeにしなければならないのをする
        DrawTopWindow.SetActive(false);

   
        inputManager.SetActive(true);
        GameSystem.Instance.StartGame();
        ////////////
        yield return new WaitForSeconds(DrawTime);
        stopButton.interactable = true;

        startImage.gameObject.SetActive(false);


        Destroy(this.gameObject);
    }
}
