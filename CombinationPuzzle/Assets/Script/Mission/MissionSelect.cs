using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum MissionClearState
{
    NotClear,
    Clear
}
public class MissionSelect : MonoBehaviour
{

    [SerializeField]
    List<MissionMenuSet> missionList = new List<MissionMenuSet>();

    [SerializeField]
    List<Sprite> MissionSprites = new List<Sprite>();

    int viewNumber = 0;//今見せている番号

    const int totalDraw = 5;

    List<MissionClearState> clearState = new List<MissionClearState>();

    // Use this for initialization


    //ミッション決まるまでコメントアウト
    void Start()
    {
        viewNumber = 0;
       clearState = SaveDataManager.Instance.LoadClearList(MissionSprites.Count);

        SetMission(0);

    }


    public void ClickMissionNumber(int num)
    {
        Debug.Log(num+ "番クリック   viewNumber=" + viewNumber);
       
        SaveDataManager.Instance.SetMissioinNumber(num + viewNumber * totalDraw);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    private void SetMission(int page)
    {
        for (int i = 0; i < totalDraw; i++)
        {
            missionList[i].ChangeMissionSprite(MissionSprites[totalDraw * page + i]);
            Debug.Log(i+"=は　クリア状態="+clearState[i]);
            if (clearState[totalDraw * page + i] == MissionClearState.NotClear)
                missionList[i].SetClearActive(false);
            else
                missionList[i].SetClearActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ClickRightButton()//クリアかどうかなどで、成功も表示
    {
        AudioManager.Instance.PlaySE("PAZ_SE_OK");
        viewNumber--;

        if (viewNumber < 0)
            viewNumber = MissionSprites.Count / totalDraw - 1;
        SetMission(viewNumber);

    }
    public void ClickLeftButton()
    {
        AudioManager.Instance.PlaySE("PAZ_SE_OK");
        viewNumber++;

        if (viewNumber >= MissionSprites.Count / totalDraw)
            viewNumber = 0;
        SetMission(viewNumber);

    }
    public void ClickRightMission()//一旦
    {
        SaveDataManager.Instance.SetMode = Mode.Mission;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }
    


}
