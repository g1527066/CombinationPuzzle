using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum MissionType
{
    CreateBest,
    CreateAdvanced,
    DeletePeace,
    CollectNum,
    DeleteCount,
    DeleteBestNum,
    None,
}

//どのタイプか、何個達成、目標
struct MisstionData
{
    public MissionType missionType;
    public int completeNum;
    public int MissionNum;
    public PeaceForm peaceForm;
    public PeaceColor peaceColor;
    public void Init()
    {
        missionType = MissionType.None;
        completeNum = 0;
        MissionNum = 0;
        peaceForm = PeaceForm.None;
        peaceColor = PeaceColor.None;
    }

}

public class Mission : MonoBehaviour
{
    const int MissionNum = 3;

    [SerializeField]
    Image[] MissionImage = null;
    [SerializeField]
    Text[] MissionCountText = null;
    [SerializeField]
    Text[] MissionDescriptionText = null;

    [SerializeField]
    Sprite AllPeaceSprite = null;

    [SerializeField]
    CutMission cutMission = null;

    [SerializeField]
    GameObject CharaPool = null;

    [Header("生成系------([0]個から～[1]個の中から)")]


    [SerializeField, Header("最上級何個作るか")]
    int[] CreateBest = { 1, 3 };
    [SerializeField, Header("上級")]
    int[] CreateAdvanced = { 2, 4 };


    [Header("削除系------([0]個から～[1]個の中から)")]

    [SerializeField, Header("何個削除(白以外)")]
    int[] DeletePeace = { 2, 4 };
    [SerializeField, Header("まとめて消す個数")]
    int[] CollectNum = { 4, 5, 7 };
    [SerializeField, Header("何回消せ")]
    int[] DeleteCount = { 3, 6 };
    [SerializeField, Header("最上級を何個消せ")]
    int DeleteBestNum = 1;

    string deleteStr = "消去";
    string generationStr = "生成";


    MisstionData[] missionData = new MisstionData[3];

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < MissionNum; i++)
        {
            SetMission(i);
        }
    }


    private void SetMission(int missionNum)
    {
        while (true)
        {
            missionData[missionNum].Init();
            missionData[missionNum].missionType = (MissionType)Random.Range(0, (int)MissionType.None);
            if (CheckSuffering(missionNum) == true)
                break;
        }
        switch (missionData[missionNum].missionType)
        {
            case MissionType.CreateBest:
                //文字とスプライトセット
                MissionDescriptionText[missionNum].text = generationStr;
                missionData[missionNum].MissionNum = Random.Range(CreateBest[0], CreateBest[1] + 1);
                UpdateCreateBestText(missionNum);
                missionData[missionNum].peaceForm = PeaceForm.Pentagon;
                MissionImage[missionNum].sprite = PeaceGenerator.Instance.PeaceSprites[(int)PeaceColor.None + 1];
                break;
            case MissionType.CreateAdvanced:
                MissionDescriptionText[missionNum].text = generationStr;
                missionData[missionNum].MissionNum = Random.Range(CreateAdvanced[0], CreateAdvanced[1] + 1);
                UpdateCreateBestText(missionNum);
                missionData[missionNum].peaceForm = PeaceForm.Square;
                MissionImage[missionNum].sprite = PeaceGenerator.Instance.PeaceSprites[(int)PeaceColor.None];
                break;
            case MissionType.DeletePeace:
                MissionDescriptionText[missionNum].text = deleteStr;
                missionData[missionNum].MissionNum = Random.Range(DeletePeace[0], DeletePeace[1] + 1);
                UpdateCreateBestText(missionNum);
                int r = Random.Range(0, ((int)PeaceColor.None + 1));
                if (r != (int)PeaceColor.None)
                {
                    missionData[missionNum].peaceColor = (PeaceColor)r;
                    MissionImage[missionNum].sprite = PeaceGenerator.Instance.PeaceSprites[r];
                }
                else
                {
                    MissionImage[missionNum].sprite = PeaceGenerator.Instance.PeaceSprites[(int)PeaceColor.None];
                    missionData[missionNum].peaceForm = PeaceForm.Square;
                }
                break;
            case MissionType.CollectNum:
                MissionDescriptionText[missionNum].text = "まとめて消去";
                int num= Random.Range(0, CollectNum.Length);
                missionData[missionNum].MissionNum = CollectNum[num];
                MissionCountText[missionNum].text = missionData[missionNum].MissionNum + "個";
                MissionImage[missionNum].sprite = AllPeaceSprite;
                break;
            case MissionType.DeleteCount:

                MissionDescriptionText[missionNum].text = "回数消去";
                missionData[missionNum].MissionNum = Random.Range(DeleteCount[0], DeleteCount[1]);
                UpdateCreateBestText(missionNum);
                MissionImage[missionNum].sprite = AllPeaceSprite;
                break;
            case MissionType.DeleteBestNum:
                MissionDescriptionText[missionNum].text = "消去";
                MissionCountText[missionNum].text = DeleteBestNum + "個";
                MissionImage[missionNum].sprite = PeaceGenerator.Instance.PeaceSprites[(int)PeaceColor.None + 1];
                missionData[missionNum].peaceForm = PeaceForm.Pentagon;
                break;
        }

    }


    /// <summary>
    /// 同じミッションは生成しない
    /// </summary>
    /// <param name="targetNum"></param>
    /// <returns></returns>
    private bool CheckSuffering(int targetNum)//TODO:今は同じミッションはできない
    {
        for (int i = 0; i < MissionNum; i++)
        {
            if (i == targetNum) continue;
            if (missionData[i].missionType == missionData[targetNum].missionType)
                return false;
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void CheckMission(List<Peace> deletePeaceList, Peace generationPeace)
    {
        for (int i = 0; i < MissionNum; i++)
        {
            switch (missionData[i].missionType)
            {
                #region 生成
                case MissionType.CreateBest:
                    if (generationPeace != null)
                    {
                        if (generationPeace.GetPeaceForm == PeaceForm.Square)
                        {
                            missionData[i].completeNum++;
                            UpdateCreateBestText(i);
                            if (missionData[i].completeNum >= missionData[i].MissionNum)
                            {
                                ClearMisstion(i);
                            }
                        }
                    }
                    break;
                case MissionType.CreateAdvanced:
                    if (generationPeace != null)
                    {
                        if (generationPeace.GetPeaceForm == PeaceForm.Triangle)
                        {
                            missionData[i].completeNum++;
                            UpdateCreateBestText(i);
                            if (missionData[i].completeNum >= missionData[i].MissionNum)
                            {
                                ClearMisstion(i);
                            }
                        }
                    }
                    break;
                #endregion
                #region 削除
                case MissionType.DeletePeace:
                    if (missionData[i].peaceColor == deletePeaceList[0].peaceColor && deletePeaceList[0].GetPeaceForm == PeaceForm.Triangle
                        || missionData[i].peaceForm == deletePeaceList[0].GetPeaceForm)
                    {
                        missionData[i].completeNum += deletePeaceList.Count;
                        UpdateCreateBestText(i);
                        if (missionData[i].completeNum >= missionData[i].MissionNum)
                        {
                            ClearMisstion(i);
                        }
                    }

                    break;
                case MissionType.CollectNum:
                    if(deletePeaceList.Count>=missionData[i].MissionNum)
                    {
                        ClearMisstion(i);
                    }
                    break;
                case MissionType.DeleteCount:
                    missionData[i].completeNum++;
                    UpdateCreateBestText(i);
                    if (missionData[i].completeNum >= missionData[i].MissionNum)
                    {
                        ClearMisstion(i);
                    }
                    break;
                case MissionType.DeleteBestNum://TODO:4個以上に対応してない
                    if(deletePeaceList[0].GetPeaceForm==PeaceForm.Pentagon)
                    {
                        ClearMisstion(i);
                    }
                    break;
                default:
                    break;
                    #endregion
            }

        }

    }

    private void UpdateCreateBestText(int missionNum)
    {
        MissionCountText[missionNum].text = missionData[missionNum].completeNum + "/" + missionData[missionNum].MissionNum + "個";
    }

    private void ClearMisstion(int missionNum)
    {

        SpriteSlicer2D.SliceSprite(MissionImage[0].gameObject.transform.position+new Vector3(-1000, 0), MissionImage[0].gameObject.transform.position +new Vector3(1000, 0), MissionImage[0].gameObject);
        SpriteSlicer2D.ShatterSprite(MissionImage[0].gameObject,100);
        cutMission.SetCutEffect(AllPeaceSprite,missionNum,missionData[missionNum].peaceColor, missionData[missionNum].peaceForm);
        if (GameSystem.Instance.gameMode == Mode.Mission)
        {
            MissionImage[missionNum].gameObject.SetActive(false);
            missionData[missionNum].missionType = MissionType.None;
        }
        else
        {
            SetMission(missionNum);
                //Marathonなら補充

        }

        GameSystem.Instance.TimerControl(0, 0, GameSystem.Instance.CompleteAddTime);

        for (int i = 0; i < MissionNum; i++)
        {
            if (missionData[i].missionType != MissionType.None) return;

        }
        //すべてNoneだったらクリア
        GameSystem.Instance.Clear();
    }
}
