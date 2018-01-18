using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartProduction : MonoBehaviour
{
    //activeではいけないもの
    [SerializeField]
    GameObject PeaceManager = null;
    [SerializeField]
    GameObject MissionManager = null;
    [SerializeField]
    GameObject MissionImage = null;
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
    GameObject MarathonSet = null;


    private void Start()
    {


        if (SaveDataManager.Instance.GetMode == Mode.Mission)
        {
            MissionSet.SetActive(true);
        }
        else
        {
            MarathonSet.SetActive(true);
            //制限時間消す
        }
    }


    private void Update()
    {
        if (tapFlag == false && null == tapStart)
        {
            tapFlag = true;
            StartCoroutine(DeleteStart());
        }
    }

    private IEnumerator DeleteStart()
    {
        AudioManager.Instance.PlayBGM("PAZ_BGM_Game");

        //Activeにしなければならないのをする
        DrawTopWindow.SetActive(false);

        PeaceManager.SetActive(true);
        MissionManager.SetActive(true);
        MissionImage.SetActive(true);
        inputManager.SetActive(true);

        ////////////
        yield return new WaitForSeconds(DrawTime);
        stopButton.interactable = true;

        Destroy(startImage);


        Destroy(this.gameObject);
    }
}
