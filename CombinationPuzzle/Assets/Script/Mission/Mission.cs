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
    [SerializeField]
    Text[] MissionTopDescriptionText = null;


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

    string deleteString = "消去";
    string countDeleteString = "回数消去";
    string sameDeleteString = "同時消去";
    string makeString = "生成";

    Color generationColor = Color.blue;
    [SerializeField]
    [ColorHtmlProperty]
    Color sameColor = Color.yellow;
    Color countColor = Color.green;
    Color deleteColor = Color.red;

    #endregion

    // Use this for initialization
    void Awake()
    {
        Debug.Log("ミッションスタート");
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

        switch (missionDataStruct.missionType)
        {
            case MissionType.Delete:
                MissionCountText[num].text = missionData[num].completeNum + "/" + missionData[num].MissionNum + "個";
                MissionTopDescriptionText[num].text = deleteString;
                MissionDescriptionText[num].text = deleteString;
                MissionDescriptionText[num].color = deleteColor;
                break;
            case MissionType.CountDelete:
                MissionCountText[num].text = missionData[num].completeNum + "/" + missionData[num].MissionNum + "回";
                MissionTopDescriptionText[num].text = countDeleteString;
                MissionDescriptionText[num].text = countDeleteString;
                MissionDescriptionText[num].color = countColor;
                break;
            case MissionType.SameDelete:
                MissionCountText[num].text = missionData[num].MissionNum + "個";
                MissionTopDescriptionText[num].text = sameDeleteString;
                MissionDescriptionText[num].text = sameDeleteString;
                MissionDescriptionText[num].color = sameColor;
                break;
            case MissionType.MakePeace:
                MissionCountText[num].text = missionData[num].completeNum + "/" + missionData[num].MissionNum + "個";
                MissionTopDescriptionText[num].text = makeString;
                MissionDescriptionText[num].text = makeString;
                MissionDescriptionText[num].color = generationColor;
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
                int r = Random.Range(0, marathonDataBase.Elements.Count);
                int r2 = Random.Range(0, marathonDataBase.Elements[r].MissionList.Count);
                MissionDataStruct ms = ReturnConstructionMission(marathonDataBase.Elements[r].MissionList[r2]);
                Debug.Log("r=" + r + "  r2=" + r2);
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

    //まとめて
    public void SameDeleteAll(List<PeaceColor> peaceColorList, List<PeaceForm> peaceFormList)
    {
        for (int i = 0; i < ConstMissionNum; i++)
        {
            switch (missionData[i].missionType)
            {
                case MissionType.Delete:
                    if (missionData[i].peaceForm != PeaceForm.None && missionData[i].peaceForm != PeaceForm.Triangle)
                    {//白、黒
                        for (int fCounter = 0; fCounter < peaceFormList.Count; fCounter++)
                        {
                            if (peaceFormList[fCounter] == missionData[i].peaceForm)
                            {
                                missionData[i].completeNum++;
                            }
                        }
                    }
                    else if (missionData[i].peaceForm == PeaceForm.Triangle && missionData[i].peaceColor != PeaceColor.None)
                    {
                        //三角形
                        for (int cCounter = 0; cCounter < peaceColorList.Count; cCounter++)
                        {
                            if (peaceColorList[cCounter] == missionData[i].peaceColor)
                            {
                                missionData[i].completeNum++;
                            }
                        }
                    }
                    else if (missionData[i].peaceForm == PeaceForm.None && missionData[i].peaceColor == PeaceColor.None)
                    {//なんでも
                        missionData[i].completeNum += peaceColorList.Count + peaceFormList.Count;
                    }

                    //文字更新
                    UpdatePercentText(i);
                    if (missionData[i].completeNum >= missionData[i].MissionNum)
                    {
                        ClearMisstion(i);
                    }

                    break;
                case MissionType.CountDelete:

                    if (missionData[i].peaceForm != PeaceForm.None && missionData[i].peaceForm != PeaceForm.Triangle)
                    {//白、黒
                        for (int fCounter = 0; fCounter < peaceFormList.Count; fCounter++)
                        {
                            if (peaceFormList[fCounter] == missionData[i].peaceForm)
                            {
                                missionData[i].completeNum++;
                                break;
                            }
                        }
                    }
                    else if (missionData[i].peaceForm == PeaceForm.Triangle && missionData[i].peaceColor != PeaceColor.None)
                    {
                        //三角形
                        for (int cCounter = 0; cCounter < peaceColorList.Count; cCounter++)
                        {
                            if (peaceColorList[cCounter] == missionData[i].peaceColor)
                            {
                                missionData[i].completeNum++;
                                break;
                            }
                        }
                    }
                    else if (missionData[i].peaceForm == PeaceForm.None && missionData[i].peaceColor == PeaceColor.None)
                    {//なんでも
                        missionData[i].completeNum++;
                    }

                    MissionCountText[i].text = missionData[i].completeNum + "/" + missionData[i].MissionNum + "回";
                    if (missionData[i].completeNum >= missionData[i].MissionNum)
                    {
                        ClearMisstion(i);
                    }

                    break;
                case MissionType.SameDelete:
                    int totalCounter = 0;

                    if (missionData[i].peaceForm != PeaceForm.None && missionData[i].peaceForm != PeaceForm.Triangle)
                    {//白、黒

                        for (int fCounter = 0; fCounter < peaceFormList.Count; fCounter++)
                        {
                            if (peaceFormList[fCounter] == missionData[i].peaceForm)
                            {
                                totalCounter++;
                            }
                        }
                        if (totalCounter >= missionData[i].MissionNum)
                        {
                            ClearMisstion(i);
                            return;
                        }
                    }
                    else if (missionData[i].peaceForm == PeaceForm.Triangle && missionData[i].peaceColor != PeaceColor.None)
                    {
                        //三角形

                        for (int cCounter = 0; cCounter < peaceColorList.Count; cCounter++)
                        {
                            if (peaceColorList[cCounter] == missionData[i].peaceColor)
                            {
                                totalCounter++;
                            }
                        }
                        if (totalCounter >= missionData[i].MissionNum)
                        {
                            ClearMisstion(i);
                            return;
                        }
                    }
                    else if (missionData[i].peaceForm == PeaceForm.None && missionData[i].peaceColor == PeaceColor.None)
                    {//なんでも

                        if (peaceColorList.Count + peaceFormList.Count >= missionData[i].MissionNum)
                        {
                            ClearMisstion(i);
                            return;
                        }
                    }
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

    public void ClearMisstion(int missionNum)
    {
        Debug.Log("ClearMisstion  " + missionNum + "     " + missionData[missionNum].missionType);
        cutMission.SetCutEffect(AllPeaceSprite, missionNum, missionData[missionNum].peaceColor, missionData[missionNum].peaceForm);
        AudioManager.Instance.PlaySE("PAZ_SE_Cut");
        StartCoroutine(ResetMission(missionNum));

        if (missionList.Count == 1)
        {
            GameSystem.Instance.Clear();
            GameSystem.Instance.ChangeDebugText("ClearMisstion");

        }
    }

    private IEnumerator ResetMission(int missionNum)
    {
        MissionDataStruct m = missionData[missionNum];//保存
        //見た目消す、Noneにする
        missionData[missionNum].Init();
        MissionImage[missionNum].gameObject.SetActive(false);
        MissionDescriptionText[missionNum].gameObject.SetActive(false);
        MissionCountText[missionNum].gameObject.SetActive(false);
        yield return new WaitForSeconds(1.0f);

        if (SaveDataManager.Instance.GetMode == Mode.Mission)
        {
            int listNum = -1;
            for (int i = 0; i < missionList.Count; i++)
            {
                if (SameMissionDataStruct(missionList[i], m))
                {
                    listNum = i;
                    break;
                }
            }
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
            {
                GameSystem.Instance.Clear();
                GameSystem.Instance.ChangeDebugText("ミッション数0");
            }
        }
        else
        {
            //Marathonなら補充
            while (true)
            {
                int r = Random.Range(0, marathonDataBase.Elements.Count);
                int r2 = Random.Range(0, marathonDataBase.Elements[r].MissionList.Count);
                MissionDataStruct ms = ReturnConstructionMission(marathonDataBase.Elements[r].MissionList[r2]);
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

        if (missionData[missionNum].missionType == MissionType.None)
            yield break;

        MissionImage[missionNum].gameObject.SetActive(true);
        MissionDescriptionText[missionNum].gameObject.SetActive(true);
        MissionCountText[missionNum].gameObject.SetActive(true);

        //徐々に
        for (float i = 0; i <= 1; i += 0.08f)
        {
            MissionImage[missionNum].color
                = new Color(MissionImage[missionNum].color.r, MissionImage[missionNum].color.g, MissionImage[missionNum].color.b, i);
            MissionTopDescriptionText[missionNum].color = new Color(MissionTopDescriptionText[missionNum].color.r, MissionTopDescriptionText[missionNum].color.g, MissionTopDescriptionText[missionNum].color.b, i);
            MissionDescriptionText[missionNum].color = new Color(MissionDescriptionText[missionNum].color.r, MissionDescriptionText[missionNum].color.g, MissionDescriptionText[missionNum].color.b, i);
            MissionCountText[missionNum].color
                = new Color(MissionCountText[missionNum].color.r, MissionCountText[missionNum].color.g, MissionCountText[missionNum].color.b, i);
            yield return null;
        }
    }

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


    [SerializeField]
    Text text = null;
    int testCutNum = 1;
    public void TestChangeNumberZan()
    {
        testCutNum++;
        if (testCutNum >= 3)
            testCutNum = 0;
        text.text = "↑" + testCutNum;
    }

    public void TextZAN()
    {
        ClearMisstion(testCutNum);
    }

    [SerializeField]
    MissionIcon MissionIconPrefab = null;
    List<MissionIcon> drawRemainingIcon = new List<MissionIcon>();
    public void DrawRemainingMissionIcon(GameObject generationObject, bool drawAgain, Vector2 startPosition, float IntervalX,float scale)
    {
        Debug.Log("miiiiii");
        for (int i = 0; i < missionList.Count; i++)
        {
            MissionIcon missionIcon;

            //一回だけか、また描画するか
            if (drawAgain == false || drawAgain == true && drawRemainingIcon.Count >= i)
            {
                missionIcon = Instantiate(MissionIconPrefab);
                missionIcon.gameObject.transform.SetParent(generationObject.transform);
                missionIcon.transform.localPosition = new Vector3(startPosition.x + IntervalX * i, startPosition.y, 0);
                if (drawAgain == true)
                    drawRemainingIcon.Add(missionIcon);
                missionIcon.gameObject.transform.localScale =new Vector3(scale,scale,1);
            }
            else
            {
                drawRemainingIcon[i].gameObject.SetActive(true);
                missionIcon = drawRemainingIcon[i];
            }
            //スプライト
            if (missionList[i].peaceForm == PeaceForm.Pentagon || missionList[i].peaceForm == PeaceForm.Square)
                missionIcon.PeaceImage.sprite = PeaceGenerator.Instance.PeaceSprites[(int)PeaceColor.None + (int)missionList[i].peaceForm - 1];
            else if (missionList[i].peaceColor != PeaceColor.None)
                missionIcon.PeaceImage.sprite = PeaceGenerator.Instance.PeaceSprites[(int)missionList[i].peaceColor];
            else
                missionIcon.PeaceImage.sprite = AllPeaceSprite;
            //文字、個数適用
            switch (missionList[i].missionType)
            {
                case MissionType.Delete:
                    missionIcon.MissionTypeText.text = deleteString;
                    missionIcon.MissionTypeText.color = deleteColor;
                    missionIcon.BackTypeText.text = deleteString;
                    missionIcon.NumberText.text = (missionList[i].MissionNum - missionList[i].completeNum) + "個";
                    break;

                case MissionType.CountDelete:
                    missionIcon.MissionTypeText.text = countDeleteString;
                    missionIcon.MissionTypeText.color = countColor;
                    missionIcon.BackTypeText.text = countDeleteString;
                    missionIcon.NumberText.text = (missionList[i].MissionNum - missionList[i].completeNum) + "回";
                    break;

                case MissionType.SameDelete:
                    missionIcon.MissionTypeText.text = sameDeleteString;
                    missionIcon.MissionTypeText.color = sameColor;
                    missionIcon.BackTypeText.text = sameDeleteString;
                    missionIcon.NumberText.text = missionList[i].MissionNum + "個";
                    break;

                case MissionType.MakePeace:
                    missionIcon.MissionTypeText.text = makeString;
                    missionIcon.MissionTypeText.color = generationColor;
                    missionIcon.BackTypeText.text = makeString;
                    missionIcon.NumberText.text = (missionList[i].MissionNum - missionList[i].completeNum) + "個";
                    break;
            }
        }

        //消えた分はアクティブを切る
        if(drawRemainingIcon.Count>missionList.Count)
        {
            for(int i= missionList.Count; i< drawRemainingIcon.Count;i++)
            {
                drawRemainingIcon[i].gameObject.SetActive(false);
            }

        }
    }

}
