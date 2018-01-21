using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum MissionTypeOld
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
struct MissionDataStruct
{
    public MissionType missionType;
    public int completeNum;//終わったら
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
    #region
    const int ConstMissionNum = 3;

    [SerializeField]
    Image[] MissionImage = null;
    [SerializeField]
    Text[] MissionCountText = null;
    [SerializeField]
    Text[] MissionDescriptionText = null;

    [SerializeField]//全部のほうの最後を参照する
    Sprite AllPeaceSprite = null;

    [SerializeField]
    CutMission cutMission = null;




    MissionDataStruct[] missionData = new MissionDataStruct[3];


    //Missionで使用
    List<MissionDataStruct> missionList = new List<MissionDataStruct>();

    [SerializeField]
    MissionData missionDataBase = null;
    [SerializeField]
    MissionData marathonDataBase = null;

    #endregion

    // Use this for initialization
    void Start()
    {


        SetMission();
    }


    private MissionDataStruct ReturnConstructionMission(MissionData.MissionDetails missionDetails)
    {
        MissionDataStruct data = new MissionDataStruct();
        data.Init();
        data.missionType = missionDetails.missionType;
        data.MissionNum = missionDetails.number;
        data.peaceColor = missionDetails.peaceColor;
        data.peaceForm = missionDetails.peaceForm;

        return data;

    }

    //個数文字、画像、生成

    private void SetDraw(int num, MissionDataStruct missionDataStruct)
    {

        //画像
        //間違えてcolorいれられる可能性があるので先にformをチェックする
        if (missionDataStruct.peaceForm == PeaceForm.Pentagon || missionDataStruct.peaceForm == PeaceForm.Square)
        {
            MissionImage[num].sprite = PeaceGenerator.Instance.PeaceSprites[(int)PeaceColor.None + (int)missionDataStruct.peaceForm - 1];
        }
        else if (missionDataStruct.peaceColor != PeaceColor.None)
        {
            MissionImage[num].sprite = PeaceGenerator.Instance.PeaceSprites[(int)missionDataStruct.peaceColor];
        }
        else
        {
            MissionImage[num].sprite = AllPeaceSprite;
        }

        Debug.Log("目標数 " + missionData[num].MissionNum);
        switch (missionDataStruct.missionType)
        {
            case MissionType.Delete:
                MissionCountText[num].text = missionData[num].completeNum + "/" + missionData[num].MissionNum + "個";
                MissionDescriptionText[num].text = "消去";
                break;
            case MissionType.CountDelete:
                MissionCountText[num].text = missionData[num].completeNum + "/" + missionData[num].MissionNum + "回";
                MissionDescriptionText[num].text = "回数消去";
                break;

            case MissionType.SameDelete:
                MissionCountText[num].text = missionData[num].MissionNum + "個";
                MissionDescriptionText[num].text = "同時消去";
                break;

            case MissionType.MakePeace:
                MissionCountText[num].text = missionData[num].completeNum + "/" + missionData[num].MissionNum + "個";
                MissionDescriptionText[num].text = "生成";
                break;

            default:
                Debug.Log("MissionSetDrawError");
                break;



        }


    }

    private void SetMission()
    {
        if (Mode.Mission == SaveDataManager.Instance.GetMode)
        {

            for (int missionCounter = 0
                ; missionCounter < missionDataBase.Elements[SaveDataManager.Instance.GetMissionNumber].MissionList.Count; missionCounter++)
            {
                missionList.Add(ReturnConstructionMission(missionDataBase.Elements[SaveDataManager.Instance.GetMissionNumber].MissionList[missionCounter]));
            }
            //3つまではセット、表示する
            //それより少ない場合は、他非表示にする

            if (missionList.Count >= ConstMissionNum)
            {
                for (int i = 0; i < ConstMissionNum; i++)
                {
                    missionData[i] = missionList[i];
                    SetDraw(i, missionList[i]);
                }
            }
            else
            {
                Debug.Log("3つより少ない");
                for (int i = 0; i < missionList.Count; i++)
                {
                    Debug.Log("リストから" + i);
                    missionData[i] = missionList[i];
                    SetDraw(i, missionList[i]);
                }
                //余計なのは非表示
                for (int i = missionList.Count; i < ConstMissionNum; i++)
                {
                    Debug.Log("無し" + i);
                    SetNonePeace(i);
                }

            }
        }
        else//Marathon
        {
            //一旦ランダムに決める
            /*
             *Marathonデータから（今はMission）の中からランダムに選択
             * 被ったらやり直し
             *画像もセット
             * 
             */
            for (int i = 0; i < ConstMissionNum; i++)
            {
                int r = Random.Range(0, missionDataBase.Elements.Count);
                int r2 = Random.Range(0, missionDataBase.Elements[r].MissionList.Count);
                Debug.Log(r + "   " + r2 + "  " + missionDataBase.Elements[r].MissionList[r2].number);
                MissionDataStruct ms = ReturnConstructionMission(missionDataBase.Elements[r].MissionList[r2]);
                Debug.Log("ms の数" + ms.MissionNum);
                missionData[i] = ms;
                SetDraw(i, ms);
                for (int j = i - 1; j >= 0; j--)
                {
                    if (SameMissionDataStruct(ms, missionData[j]))//前のとかぶって似ないか
                    {
                        i--;
                        break;
                    }
                }
            }
        }
    }



    /// <summary>
    /// 同じミッションは生成しない
    /// </summary>
    /// <param name="targetNum"></param>
    /// <returns></returns>
    private bool CheckSuffering(int targetNum)//TODO:今は同じミッションはできない
    {
        for (int i = 0; i < ConstMissionNum; i++)
        {
            if (i == targetNum) continue;
            if (missionData[i].missionType == missionData[targetNum].missionType)
                return false;
        }
        return true;
    }


    public void CheckMission(List<Peace> deletePeaceList, Peace generationPeace)
    {

        bool addCount = false;

        for (int i = 0; i < ConstMissionNum; i++)
        {
            switch (missionData[i].missionType)
            {
                case MissionType.MakePeace:
                    if (generationPeace != null)
                    {
                        if (generationPeace.GetPeaceForm == missionData[i].peaceForm - 1)
                        {
                            missionData[i].completeNum++;
                            UpdatePercentText(i);
                            if (missionData[i].completeNum >= missionData[i].MissionNum)
                            {
                                ClearMisstion(i);
                            }
                        }
                    }
                    break;

                case MissionType.Delete:

                    addCount = false;
                    if (missionData[i].peaceForm == PeaceForm.Square && deletePeaceList[0].GetPeaceForm == PeaceForm.Square ||
                        missionData[i].peaceForm == PeaceForm.Pentagon && deletePeaceList[0].GetPeaceForm == PeaceForm.Pentagon)
                    {
                        addCount = true;
                    }//三角形//三角形で、おんなじ色だったら
                    else if (deletePeaceList[0].GetPeaceColor == missionData[i].peaceColor &&
                        deletePeaceList[0].GetPeaceForm == PeaceForm.Triangle)
                    {
                        addCount = true;
                    }
                    else if (missionData[i].peaceForm == PeaceForm.None && missionData[i].peaceColor == PeaceColor.None)//どの種類でもいい
                    {
                        addCount = true;
                    }

                    if (addCount == true)
                    {
                        missionData[i].completeNum += deletePeaceList.Count;
                        UpdatePercentText(i);
                        if (missionData[i].completeNum >= missionData[i].MissionNum)
                        {
                            ClearMisstion(i);
                        }
                    }


                    break;
                case MissionType.CountDelete:

                    addCount = false;
                    if (missionData[i].peaceForm == PeaceForm.Square && deletePeaceList[0].GetPeaceForm == PeaceForm.Square ||
                        missionData[i].peaceForm == PeaceForm.Pentagon && deletePeaceList[0].GetPeaceForm == PeaceForm.Pentagon)
                        addCount = true;
                    else if (deletePeaceList[0].GetPeaceColor == missionData[i].peaceColor &&
                   deletePeaceList[0].GetPeaceForm == PeaceForm.Triangle)
                        addCount = true;
                    else if (missionData[i].peaceForm == PeaceForm.None && missionData[i].peaceColor == PeaceColor.None)//どの種類でもいい
                        addCount = true;

                    if (addCount == true)
                    {
                        missionData[i].completeNum++;
                        MissionCountText[i].text = missionData[i].completeNum + "/" + missionData[i].MissionNum + "回";
                        if (missionData[i].completeNum >= missionData[i].MissionNum)
                        {
                            ClearMisstion(i);
                        }
                    }
                    break;

                case MissionType.SameDelete:

                    addCount = false;
                    if (missionData[i].peaceForm == PeaceForm.Square && deletePeaceList[0].GetPeaceForm == PeaceForm.Square ||
                        missionData[i].peaceForm == PeaceForm.Pentagon && deletePeaceList[0].GetPeaceForm == PeaceForm.Pentagon)
                        addCount = true;
                    else if (deletePeaceList[0].GetPeaceColor == missionData[i].peaceColor &&
                   deletePeaceList[0].GetPeaceForm == PeaceForm.Triangle)
                        addCount = true;
                    else if (missionData[i].peaceForm == PeaceForm.None && missionData[i].peaceColor == PeaceColor.None)//どの種類でもいい
                        addCount = true;

                    if (addCount == true && deletePeaceList.Count >= missionData[i].MissionNum)
                    {
                        ClearMisstion(i);
                    }
                    break;

                default:
                    Debug.Log("default" + i);
                    break;
            }

        }
    }

    private void UpdatePercentText(int missionNum)
    {
        MissionCountText[missionNum].text = missionData[missionNum].completeNum + "/" + missionData[missionNum].MissionNum + "個";
    }

    private void SetNonePeace(int missionNum)
    {
        MissionImage[missionNum].gameObject.SetActive(false);
        MissionDescriptionText[missionNum].gameObject.SetActive(false);
        MissionCountText[missionNum].gameObject.SetActive(false);
        //判定しないように残りはNone入れる
        MissionDataStruct dataStruct = new MissionDataStruct();
        dataStruct.Init();
        missionData[missionNum] = dataStruct;
    }

    private void ClearMisstion(int missionNum)
    {

        Debug.Log("ClearMisstion  " + missionNum + "     " + missionData[missionNum].missionType);
        cutMission.SetCutEffect(AllPeaceSprite, missionNum, missionData[missionNum].peaceColor, missionData[missionNum].peaceForm);
        if (SaveDataManager.Instance.GetMode == Mode.Mission)
        {
            int listNum = -1;
            for (int i = 0; i < missionList.Count; i++)
            {
                if (SameMissionDataStruct(missionList[i], missionData[missionNum]))
                {
                    listNum = i;
                    break;
                }
            }
            Debug.Log("削除" + listNum);
            missionList.RemoveAt(listNum);

            if (missionList.Count >= ConstMissionNum)
            {
                missionData[missionNum] = missionList[ConstMissionNum - 1];
                SetDraw(missionNum, missionData[missionNum]);
            }
            else
            {
                SetNonePeace(missionNum);
            }

            if (missionList.Count == 0)
                GameSystem.Instance.Clear();
        }
        else
        {
            //Marathonなら補充

            while (true)
            {
                int r = Random.Range(0, missionDataBase.Elements.Count);
                int r2 = Random.Range(0, missionDataBase.Elements[r].MissionList.Count);
                Debug.Log(r + "   " + r2 + "  " + missionDataBase.Elements[r].MissionList[r2].number);
                MissionDataStruct ms = ReturnConstructionMission(missionDataBase.Elements[r].MissionList[r2]);
                missionData[missionNum] = ms;
                SetDraw(missionNum, ms);

                bool Same = false;
                for (int i = 0; i < ConstMissionNum; i++)
                {
                    if (i == missionNum)
                        continue;
                    if (SameMissionDataStruct(ms, missionData[i]))//前のとかぶって似ないか
                    {
                        Same = true;
                    }
                }
                if (Same == false)
                    break;
            }
        }

        GameSystem.Instance.TimerControl(0, 0, GameSystem.Instance.CompleteAddTime);

        for (int i = 0; i < ConstMissionNum; i++)
        {
            // if (missionData[i].missionType != MissionTypeOld.None) return;

        }
        //すべてNoneだったらクリア
        // GameSystem.Instance.Clear();
    }


    //public MissionType missionType;
    //public int completeNum;//終わったら
    //public int MissionNum;
    //public PeaceForm peaceForm;


    private bool SameMissionDataStruct(MissionDataStruct missionDataStruct1, MissionDataStruct missionDataStruct2)
    {

        //今の個数以外が合ってればいいのでは

        if (missionDataStruct1.missionType != missionDataStruct2.missionType)
            return false;
        //if (missionDataStruct1.completeNum != missionDataStruct2.completeNum)
        //    return false;
        if (missionDataStruct1.MissionNum != missionDataStruct2.MissionNum)
            return false;
        if (missionDataStruct1.peaceForm != missionDataStruct2.peaceForm)
            return false;
        if (missionDataStruct1.peaceColor != missionDataStruct2.peaceColor)
            return false;

        return true;
    }

}
