using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SelectScene : MonoBehaviour {

    [SerializeField]
    GameObject TitleSet = null;

    [SerializeField]
    GameObject MenuSet = null;

    [SerializeField]
    GameObject OptionSet = null;

    [SerializeField]
    GameObject MissionSet = null;
    private enum SceneType
    {
        Title,
        Menu,
        Mission,
        Option,
    }

    SceneType sceneType;


    private void Start()
    {
        AudioManager.Instance.PlayBGM("PAZ_BGM_Menu");
    }


    public void PushMarathonMode()
    {
        PlayerPrefs.SetString("GameMode", "Marathon");
        SceneManager.LoadScene("Main");
    }
    public void ClickReturnButton()
    {
        AudioManager.Instance.PlaySE("PAZ_SE_OK");
        if (sceneType == SceneType.Menu)
        {
            sceneType = SceneType.Title;
            MenuSet.SetActive(false);
            TitleSet.SetActive(true);
           
        }
        else if (sceneType == SceneType.Option)
        {
            sceneType = SceneType.Menu;
            MenuSet.SetActive(true);
            OptionSet.SetActive(false);
            //音

        }
        else if (sceneType == SceneType.Mission)
        {
            sceneType = SceneType.Menu;
            MenuSet.SetActive(true);
            MissionSet.SetActive(false);
            //音
        }
    }

    public void ClickStartButton()
    {
        sceneType = SceneType.Menu;
        TitleSet.SetActive(false);
        MenuSet.SetActive(true);
        Debug.Log("Start!");
        AudioManager.Instance.PlaySE("PAZ_SE_OK");
    }

    public void ClickEndButtone()
    {
        AudioManager.Instance.PlaySE("PAZ_SE_OK");
        Application.Quit();
    }

    public void ClickMissionButton()
    {
        AudioManager.Instance.PlaySE("PAZ_SE_OK");
        sceneType = SceneType.Mission;
        MissionSet.SetActive(true);
        MenuSet.SetActive(false);
        Debug.Log("Mission");
    }

    //オプション
    public void ClickOptionButton()
    {
        AudioManager.Instance.PlaySE("PAZ_SE_OK");
        sceneType = SceneType.Option;
        OptionSet.SetActive(true);
        MenuSet.SetActive(false);
        Debug.Log("Option!");
    }


    //public void ClickOptionButton()
    //{
    //    OptionSet.SetActive(true);
    //    MenuSet.SetActive(false);
    //    Debug.Log("Option!");
    //}

    
}
