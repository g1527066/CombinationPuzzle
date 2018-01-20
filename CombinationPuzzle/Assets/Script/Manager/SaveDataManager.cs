using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode
{
    Mission,
    Marathon,
}

public class SaveDataManager : SingletonMonoBehaviour<SaveDataManager>
{

    private Mode mode;
    public Mode GetMode
    {
        get { return mode; }
    }
    public Mode SetMode
    {
        set { mode = value; }
    }



    //マラソンモード----------------

    int MarathonHiScore = 0;
    public int GetMarathonHiScore
    {
        get
        {
            return MarathonHiScore;
        }
    }
    const string MarathonHiScoreKye = "MarathonHiScore";



    //ミッションモード-------------------
    const string MissionNumberKye = "MissionNumber";
    int missionNumer = 0;
    public int GetMissionNumber
    {
        get
        {
            return missionNumer;
        }
    }


    protected override void Awake()
    {
        base.Awake();


        LoadMarathonHiScore();

        DontDestroyOnLoad(this.gameObject);
    }

    //Marathon用
    public void LoadMarathonHiScore()
    {
        MarathonHiScore = PlayerPrefs.GetInt(MarathonHiScoreKye, 0);
    }

    public List<MissionClearState> LoadClearList(int num)
    {
        List<MissionClearState> ClearList = new List<MissionClearState>();
        string kye;
        for (int i = 0; i < num; i++)
        {
            kye = "ClearMission" + num.ToString();
            ClearList.Add((MissionClearState)PlayerPrefs.GetInt(kye, (int)MissionClearState.NotClear));
        }
        return ClearList;
    }

    public void SetMissioinNumber(int num)
    {
        missionNumer = num;
    }
    public void SetMarathonHiScore(int num)
    {
        PlayerPrefs.SetInt(MarathonHiScoreKye, num);
    }
}
