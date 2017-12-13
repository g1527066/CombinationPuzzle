﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public enum ChallengeType
{
    Creation,//何個作成
    Deletion,//何個削除
    ConclusionDeletion,//何個まとめて削除
    MultipleDeletion,         //あれとこれを消せ
    DeletionCount,//何回消せ


}


public struct TargetSet
{
    public PeaceColor peaceType;

    public int count;
    public int remainingCount;
}



public class JudgeManager : MonoBehaviour
{

    ChallengeType challengeType;
    public ChallengeType ChallengeType
    {
        get { return challengeType; }
    }

    [SerializeField]
    Text titleText = null;
    [SerializeField]
    Text ChallengeText = null;
    [SerializeField]
    GameObject ChallengeWindow = null;

    List<TargetSet> targetList = new List<TargetSet>();
    [Space(20), SerializeField]//個数
    int MaxCreateCount = 7;
    [SerializeField]
    int MinCreateCount = 1;

    //まとめて消せる回数
    [SerializeField]
    int MaxConclusionCount = 8;
    [SerializeField]
    int MinConclusionCount = 4;


    //消す回数
    [SerializeField]
    int MaxDeleteCount = 8;
    [SerializeField]
    int MinDeleteCount = 5;

    //同時に消す
    [SerializeField]
    public int MaxMultipleCount = 2;

    int completeCount = 0;
    int deleteCountdown;

    void Start()
    {
        //SetNewChallenge();
        //SetChallengeWindow();

        //deleteCountdown = -1;
        //StartCoroutine(StartStop());

    }

    private IEnumerator StartStop()
    {
        yield return null;
        GameSystem.I.StopTime();
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void CheckCreate(Dictionary<POINT, Peace> dictionary)
    {
        if (challengeType != ChallengeType.Creation) return;
        List<Peace> keyList = new List<Peace>(dictionary.Values);
        if (CheckCreatePeaceCount(keyList, targetList) == true)
            ClearChallange();
    }


    private bool CheckCreatePeaceCount(List<Peace> list, List<TargetSet> tList)
    {
        int count = 0;
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < tList.Count; j++)
            {
                if (tList[j].peaceType == list[i].peaceColor)//複数種類に未対応
                {
                    count++;
                    break;
                }
            }
        }
        if (targetList[0].count <= count)
        {

            return true;
        }
        else
            return false;
    }


    private void ClearChallange()
    {
        completeCount++;
        //いったんタイマー止める
        GameSystem.I.StopTime();
        //クリアー表示
        GameSystem.I.Clear();
        deleteCountdown = -1;
        //次の課題設定
      //  SetNewChallenge();
        StartCoroutine(ClearCoRoutine());

    }

    private IEnumerator ClearCoRoutine()
    {

        yield return new WaitForSeconds(0.5f);
        SetChallengeWindow();

    }

    private string PeaceTypeString(PeaceColor peaceType)
    {
        switch (peaceType)
        {
            case PeaceColor.Red:
                return "赤";
            case PeaceColor.Blue:
                return "青";
            case PeaceColor.Yellow:
                return "黄色";
            case PeaceColor.Green:
                return "緑";
            case PeaceColor.Perple:
                return "紫";
            case PeaceColor.Orange:
                return "橙";
            //case PeaceColor.Square:
            //    return "四角";
            //case PeaceColor.Pentagon:
            //    return "五角";
            default:
                Debug.LogError("色変更時エラー");
                return "error";
        }
    }

    //文字とかを決める
    private void SetChallengeWindow()
    {
        titleText.text = "第" + (completeCount + 1) + "課題";
        string text = "";

        switch (challengeType)
        {
            case ChallengeType.Creation:
                text = PeaceTypeString(targetList[0].peaceType) + "を" + targetList[0].count + "個作れ";

                break;
            case ChallengeType.Deletion:
                text = PeaceTypeString(targetList[0].peaceType) + "を" + targetList[0].count + "個消せ";

                break;
            case ChallengeType.ConclusionDeletion:
                text = "まとめて" + targetList[0].count + "個消せ";

                break;
            case ChallengeType.MultipleDeletion:
                text = PeaceTypeString(targetList[0].peaceType) + "を" + targetList[0].count + "個、" +
                     PeaceTypeString(targetList[1].peaceType) + "を" + targetList[1].count + "個消せ";

                break;
            case ChallengeType.DeletionCount:
                text = targetList[0].count + "回消せ";
                break;

        }


        ChallengeText.text = text;

    }
    //表示する
    public void ChallengeWindowActive()
    {
        Debug.Log(ChallengeWindow.activeInHierarchy);
        if (ChallengeWindow.activeInHierarchy == false)
            ChallengeWindow.SetActive(true);
        else
            ChallengeWindow.SetActive(false);
    }
    public void DeleteCount()
    {
        if (ChallengeType.DeletionCount == challengeType)
        {
            TargetSet t = targetList[0];
            t.count--;
            targetList[0] = t;
            if (t.count == 0)
                ClearChallange();

        }
        else
        {
            deleteCountdown--;
            if (deleteCountdown == 0)
                ClearChallange();
        }
    }

    public void DleteCheck(List<PeaceColor> list, int deleteCount)
    {
        if (challengeType == ChallengeType.Creation || ChallengeType.DeletionCount == challengeType) return;
        if (challengeType == ChallengeType.ConclusionDeletion)
        {
            if (list.Count >= targetList[0].count)
            {
                deleteCountdown = deleteCount;
            }
        }
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < targetList.Count; j++)
                {
                    if (list[i] == targetList[j].peaceType)
                    {
                        TargetSet t = targetList[j];
                        t.remainingCount--;
                        targetList[j] = t;
                    }
                }

            }

            for (int i = 0; i < targetList.Count; i++)
            {
                if (targetList[i].remainingCount > 0)
                    return;
            }
            deleteCountdown = deleteCount;
        }

        //    Deletion,//何個削除
        //ConclusionDeletion,//何個まとめて削除
        //MultipleDeletion,         //あれとこれを消せ
    }

    private void SetNewChallenge()
    {
        targetList.Clear();
        TargetSet target;
        challengeType = (ChallengeType)Random.Range(0, (int)ChallengeType.DeletionCount + 1);
        bool setOK = false;
       
        while (setOK == false)
        {
            switch (challengeType)
            {
                case ChallengeType.Creation:

                    target.peaceType = (PeaceColor)Random.Range(0, (int)PeaceColor.None);
                    target.count = Random.Range(MinCreateCount, MaxCreateCount);
                    target.remainingCount = -1;

                   // List<Peace> keyList = new List<Peace>(PeaceManager.I.PeaceTable.Values);
                    List<TargetSet> t = new List<TargetSet>();
                    t.Add(target);
                    //if (CheckCreatePeaceCount(keyList, t) == true)
                    //{
                    //    setOK = true;
                    //}
                    //else
                    //    targetList.Add(target);

                    //  CheckCreate(PeaceManager.I.PeaceTable);

                    break;
                case ChallengeType.Deletion:
                    setOK = true;
                    target.peaceType = (PeaceColor)Random.Range(0, (int)PeaceColor.None);
                    target.remainingCount = target.count = Random.Range(1, MaxCreateCount);
                    targetList.Add(target);
                    break;
                case ChallengeType.ConclusionDeletion://まとめて消す
                    setOK = true;
                    target.peaceType = 0;
                    target.count = Random.Range(MinConclusionCount, MaxConclusionCount);
                    target.remainingCount = -1;
                    targetList.Add(target);
                    break;

                case ChallengeType.DeletionCount:
                    setOK = true;
                    target.peaceType = 0;
                    target.count = Random.Range(MinDeleteCount, MaxDeleteCount);
                    target.remainingCount = -1;
                    targetList.Add(target);
                    break;
                case ChallengeType.MultipleDeletion:
                    setOK = true;
                    int deleteCount = 2;// Random.Range(1, MaxMultipleCount + 1);
                    bool isContinu = false;
                    for (int i = 0; i < deleteCount; i++)
                    {
                        target.peaceType = (PeaceColor)Random.Range(0, (int)PeaceColor.None);
                        target.remainingCount = target.count = Random.Range(MinDeleteCount, MaxDeleteCount);
                        isContinu = false;
                        if (targetList.Count > 0)
                        {

                            for (int j = 0; j < targetList.Count; j++)
                            {
                                if (targetList[j].peaceType == target.peaceType)
                                {
                                    i--;
                                    isContinu = true;
                                    break;
                                }


                            }
                        }
                        if (isContinu == true)
                            continue;

                        targetList.Add(target);
                    }
                    break;
            }
        }

    }

}