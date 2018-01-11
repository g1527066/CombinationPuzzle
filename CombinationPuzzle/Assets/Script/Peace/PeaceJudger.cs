using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DeletePoint
{
    public List<Peace> deletePeaceList;//削除にnextも含まれる
    public Peace nextGenerationPeace;
    public int deleteCounter;
}


//public class GameManager : SingletonMonoBehaviour<GameManager>
//{
public class PeaceJudger : SingletonMonoBehaviour<PeaceJudger>
{
    const int DeleteCount = 3;

    List<DeletePoint> DeletionTargetList = new List<DeletePoint>();

    [SerializeField]
    Mission mission = null;

    public bool CurrentDeletable(Dictionary<POINT, Peace> dictionary, Peace judgeAddPeace)
    {
        //上
        int count = 0;
        POINT p = judgeAddPeace.point;
        for (int i = p.Y - 1; i >= 0; i--)
        {
            if (dictionary.ContainsKey(new POINT(p.X, i)) == false) break;
            if (dictionary[new POINT(p.X, i)].peaceColor == judgeAddPeace.peaceColor)
            {
                count++;
            }
            else
                break;
        }
        if (count >= 2)
        {
            return true;
        }

        //左
        count = 0;
        for (int i = p.X - 1; i >= 0; i--)
        {
            if (dictionary.ContainsKey(new POINT(p.X, i)) == false) break;
            if (dictionary[new POINT(i, p.Y)].peaceColor == judgeAddPeace.peaceColor)
            {
                count++;
            }
            else
                break;
        }
        if (count >= 2)
        {
            return true;
        }
        return false;
    }

    //TODO:いったん判定後ピース省略
    public void JudgePeace(Dictionary<POINT, Peace> peaceTable, Peace judgePeace, Peace nowHoldPeace)
    {
        if (judgePeace.peaceColor == PeaceColor.None)
            return;

        //プレイヤーの持っているのは除外
        POINT judgePoint;

        List<POINT> temporaryDeletingList = new List<POINT>();

        List<POINT> totalDeleteList = new List<POINT>();
        bool isDelete = false;
        //上下調査------------------------
        int count;// = 1;
        //上へ
        totalDeleteList.Add(new POINT(judgePeace.point.X, judgePeace.point.Y));

        //連鎖中で無い可能性もあるので一回一回チェックする
        POINT checkPoint;

        for (int j = 0; j < totalDeleteList.Count; j++)
        {
            judgePoint = totalDeleteList[j];
            count = 1;
            temporaryDeletingList.Clear();
            for (int i = judgePoint.Y - 1; i >= 0; i--)
            {
                checkPoint = new POINT(judgePoint.X, i);
                if (peaceTable.ContainsKey(checkPoint) == false) break;
                if (IsDeletable(peaceTable[checkPoint], nowHoldPeace) == false) break;
                if (CheckSameElement(peaceTable[checkPoint], judgePeace))
                {
                    count++;
                    temporaryDeletingList.Add(checkPoint);
                }
                else
                    break;
            }
            for (int i = judgePoint.Y + 1; i < PeaceManager.BoardSizeY; i++)
            {
                checkPoint = new POINT(judgePoint.X, i);
                if (peaceTable.ContainsKey(checkPoint) == false) break;
                if (IsDeletable(peaceTable[checkPoint], nowHoldPeace) == false) break;
                if (CheckSameElement(peaceTable[checkPoint], judgePeace))
                {
                    count++;
                    temporaryDeletingList.Add(checkPoint);
                }
                else
                    break;
            }
            if (count >= DeleteCount)
            {
                for (int i = 0; i < temporaryDeletingList.Count; i++)
                {
                    // Debug.Log("X=" + temporaryDeletingList[i].X + "  Y=" + temporaryDeletingList[i].Y);
                    if (JudgeDoublePoint(totalDeleteList, temporaryDeletingList[i]))
                        totalDeleteList.Add(new POINT(temporaryDeletingList[i].X, temporaryDeletingList[i].Y));
                }
                isDelete = true;

            }

            //左右調査------------------------
            count = 1;

            temporaryDeletingList.Clear();
            for (int i = judgePoint.X + 1; i < PeaceManager.BoardSizeX; i++)
            {
                checkPoint = new POINT(i, judgePoint.Y);
                if (peaceTable.ContainsKey(checkPoint) == false) break;
                if (IsDeletable(peaceTable[checkPoint], nowHoldPeace) == false) break;
                if (CheckSameElement(peaceTable[checkPoint], judgePeace))
                {
                    temporaryDeletingList.Add(checkPoint);
                    count++;
                }
                else
                    break;
            }
            for (int i = judgePoint.X - 1; i >= 0; i--)
            {
                checkPoint = new POINT(i, judgePoint.Y);
                if (peaceTable.ContainsKey(checkPoint) == false) break;
                if (IsDeletable(peaceTable[checkPoint], nowHoldPeace) == false) break;
                if (CheckSameElement(peaceTable[checkPoint], judgePeace))
                {
                    temporaryDeletingList.Add(checkPoint);
                    count++;
                }
                else
                    break;
            }
            if (count >= DeleteCount)
            {
                for (int i = 0; i < temporaryDeletingList.Count; i++)
                {
                    //  Debug.Log("X=" + temporaryDeletingList[i].X + "  Y=" + temporaryDeletingList[i].Y);
                    if (JudgeDoublePoint(totalDeleteList, temporaryDeletingList[i]))
                        totalDeleteList.Add(new POINT(temporaryDeletingList[i].X, temporaryDeletingList[i].Y));
                }
                isDelete = true;
            }
        }

        #region 四角形判定（オミット）
        //四角形------------------------
        //左上
        //if (judgePoint.Y > 0 && judgePoint.X > 0)
        //{
        //    if (peaceTable[new POINT(judgePoint.X, judgePoint.Y - 1)].peaceType == judgePeace.peaceType &&
        //       peaceTable[new POINT(judgePoint.X - 1, judgePoint.Y - 1)].peaceType == judgePeace.peaceType &&
        //         peaceTable[new POINT(judgePoint.X - 1, judgePoint.Y)].peaceType == judgePeace.peaceType)
        //    {
        //        tempDeletePointList.Add(new POINT(judgePoint.X, judgePoint.Y - 1));
        //        tempDeletePointList.Add(new POINT(judgePoint.X - 1, judgePoint.Y - 1));
        //        tempDeletePointList.Add(new POINT(judgePoint.X - 1, judgePoint.Y));
        //        isDelete = true;
        //    }
        //}

        ////右上
        //if (judgePoint.Y > 0 && judgePoint.X < BoardSizeX - 1)
        //{
        //    if (peaceTable[new POINT(judgePoint.X, judgePoint.Y - 1)].peaceType == judgePeace.peaceType &&
        //       peaceTable[new POINT(judgePoint.X + 1, judgePoint.Y - 1)].peaceType == judgePeace.peaceType &&
        //         peaceTable[new POINT(judgePoint.X + 1, judgePoint.Y)].peaceType == judgePeace.peaceType)
        //    {
        //        tempDeletePointList.Add(new POINT(judgePoint.X, judgePoint.Y - 1));
        //        tempDeletePointList.Add(new POINT(judgePoint.X + 1, judgePoint.Y - 1));
        //        tempDeletePointList.Add(new POINT(judgePoint.X + 1, judgePoint.Y));
        //        isDelete = true;
        //    }
        //}
        ////右下
        //if (judgePoint.Y < BoardSizeY - 1 && judgePoint.X < BoardSizeX - 1)
        //{
        //    if (peaceTable[new POINT(judgePoint.X, judgePoint.Y + 1)].peaceType == judgePeace.peaceType &&
        //       peaceTable[new POINT(judgePoint.X + 1, judgePoint.Y + 1)].peaceType == judgePeace.peaceType &&
        //         peaceTable[new POINT(judgePoint.X + 1, judgePoint.Y)].peaceType == judgePeace.peaceType)
        //    {
        //        tempDeletePointList.Add(new POINT(judgePoint.X, judgePoint.Y + 1));
        //        tempDeletePointList.Add(new POINT(judgePoint.X + 1, judgePoint.Y + 1));
        //        tempDeletePointList.Add(new POINT(judgePoint.X + 1, judgePoint.Y));
        //        isDelete = true;
        //    }
        //}
        ////左下    
        //if (judgePoint.Y < BoardSizeY - 1 && judgePoint.X > 0)
        //{
        //    if (peaceTable[new POINT(judgePoint.X, judgePoint.Y + 1)].peaceType == judgePeace.peaceType &&
        //       peaceTable[new POINT(judgePoint.X - 1, judgePoint.Y + 1)].peaceType == judgePeace.peaceType &&
        //         peaceTable[new POINT(judgePoint.X - 1, judgePoint.Y)].peaceType == judgePeace.peaceType)
        //    {
        //        tempDeletePointList.Add(new POINT(judgePoint.X, judgePoint.Y + 1));
        //        tempDeletePointList.Add(new POINT(judgePoint.X - 1, judgePoint.Y + 1));
        //        tempDeletePointList.Add(new POINT(judgePoint.X - 1, judgePoint.Y));
        //        isDelete = true;
        //    }
        //}
        #endregion

        //もしあれば最後に自分加える
        if (isDelete)
        {
            SetDeletePoint(peaceTable, totalDeleteList);
        }
    }

    public void DeletePeace(Dictionary<POINT, Peace> peaceTable, Peace deletePeace)
    {

        int delteListNumber = ReturnSameDeleteListNumber(deletePeace, DeletionTargetList);
        POINT dPoint = deletePeace.point;

        if (delteListNumber == -1)
        {
            Debug.LogError("リストに見つからなかった");
            return;//見つからなかった
        }
        DeletePoint nowDeletePoint = DeletionTargetList[delteListNumber];
        nowDeletePoint.deleteCounter++;
        DeletionTargetList[delteListNumber] = nowDeletePoint;

        //全て削除済みで、生成も変わっていたら
        if (nowDeletePoint.deleteCounter == nowDeletePoint.deletePeaceList.Count)//全て削除済みだったら、上から追加、判定する（両方）、ずらす
        {

            //サウンド
            if (nowDeletePoint.nextGenerationPeace != null)
            {
                if (nowDeletePoint.nextGenerationPeace.GetPeaceForm == PeaceForm.Triangle)
                    AudioManager.Instance.PlaySE("PAZ_SE_EraseSt");
                else
                    AudioManager.Instance.PlaySE("PAZ_SE_EraseNd");
            }
            else
                AudioManager.Instance.PlaySE("PAZ_SE_EraseRd");

            //時間追加
            int summaryCount = 0, bestCount = 0;
            if (nowDeletePoint.deleteCounter >= GameSystem.Instance.SummaryDeleteAddCount)
                summaryCount = nowDeletePoint.deleteCounter;
            if (nowDeletePoint.deletePeaceList[0].GetPeaceForm == PeaceForm.Pentagon)
                bestCount = nowDeletePoint.deleteCounter;
            GameSystem.Instance.TimerControl(summaryCount, bestCount, 0);

            //判定
            mission.CheckMission(nowDeletePoint.deletePeaceList, nowDeletePoint.nextGenerationPeace);
            //得点追加
            GameSystem.Instance.AddScorePoint(nowDeletePoint.deletePeaceList.Count, nowDeletePoint.deletePeaceList[0].GetPeaceForm);

            Peace changeGenerationPeace = null;
            // Debug.Log("全て削除済み");
            //生成するありなら
            if (nowDeletePoint.nextGenerationPeace != null)
            {
                EffectManager.Instance.PlayEffect(nowDeletePoint.nextGenerationPeace.gameObject.transform.position, "生成");
                changeGenerationPeace = PeaceManager.Instance.ChangeForm(nowDeletePoint.nextGenerationPeace);

            }

            //削除した場所以降のピースを落下させる//TODO:生成がnullだと、、？
            List<POINT> downTargetPoint = new List<POINT>();
            SetDownUnderList(downTargetPoint, nowDeletePoint.deletePeaceList, nowDeletePoint.nextGenerationPeace);



            //生成されたもの以外リストから削除
            List<Peace> SetChangeList = new List<Peace>();
            for (int i = 0; i < nowDeletePoint.deletePeaceList.Count; i++)
            {
                //if(nowDeletePoint.deletePeaceList[i].gameObject==null)//error
                //{
                //    Debug.Break();
                //    Debug.LogError("nowDeletePoint.deletePeaceList[i].gameObject==null");
                //}


                if (nowDeletePoint.deletePeaceList[i].GetPeaceForm == PeaceForm.Triangle)
                {
                    //error↓
                    EffectManager.Instance.PlayEffect(nowDeletePoint.deletePeaceList[i].gameObject.transform.position, nowDeletePoint.deletePeaceList[i].peaceColor.DisplayName());
                }
                else if (nowDeletePoint.deletePeaceList[i].GetPeaceForm == PeaceForm.Square)
                    EffectManager.Instance.PlayEffect(nowDeletePoint.deletePeaceList[i].gameObject.transform.position, "黒");
                else
                    EffectManager.Instance.PlayEffect(nowDeletePoint.deletePeaceList[i].gameObject.transform.position, "白");


                if (nowDeletePoint.deletePeaceList[i] == nowDeletePoint.nextGenerationPeace)
                {
                    if (CheckPossibleDown(peaceTable, nowDeletePoint.nextGenerationPeace))
                    {
                        Debug.Log("生成を落下させます");

                        PeaceOperator.Instance.AddDrop(nowDeletePoint.nextGenerationPeace);
                    }
                    continue;
                }

                //SetChangeList.Add();
                //リストから削除
                PeaceManager.Instance.stockPeaceList.Add(PeaceGenerator.Instance.ChangeForm(nowDeletePoint.deletePeaceList[i]));
                peaceTable.Remove(nowDeletePoint.deletePeaceList[i].point);

                //見えない位置に移動
                PeaceOperator.Instance.HidePeace(PeaceManager.Instance.stockPeaceList[PeaceManager.Instance.stockPeaceList.Count - 1]);
                //PeaceManager.Instance.AddToTopPeace(SetChangeList[SetChangeList.Count - 1]);
            }

            //  動かすのはあと、場所がずれるため
            for (int i = 0; i < downTargetPoint.Count; i++)
            {
                for (int j = downTargetPoint[i].Y; j >= 0; j--)
                {
                    // Debug.Log("上を落下させる " + downTargetPoint[i].X + "  " + j);
                    if (PeaceManager.Instance.GetPeaceTabel.ContainsKey(new POINT(downTargetPoint[i].X, j)))
                    {
                        if (PeaceManager.Instance.GetPeaceTabel[new POINT(downTargetPoint[i].X, j)] != PeaceManager.Instance.nowHoldPeace)
                            PeaceOperator.Instance.AddDrop(PeaceManager.Instance.GetPeaceTabel[new POINT(downTargetPoint[i].X, j)]);
                    }
                }
            }

            //削除リストを削除
            DeletionTargetList.Remove(DeletionTargetList[delteListNumber]);
            //生成後のみ審査??TODO:変える
            if (changeGenerationPeace != null)
                JudgePeace(peaceTable, changeGenerationPeace, PeaceManager.Instance.nowHoldPeace);

            Debug.Log(PeaceManager.Instance.GetPeaceTabel.Count+"   総数");
           // Debug.Break();
        }
    }

    private void SetDeletePoint(Dictionary<POINT, Peace> peaceTable, List<POINT> deleteList)
    {

        DeletePoint d;
        d.deletePeaceList = new List<Peace>();
        d.deleteCounter = 0;
        d.nextGenerationPeace = null;

        POINT generationPoint = ReturnGeneratePoint(deleteList);
        PeaceForm p = peaceTable[generationPoint].GetPeaceForm;

        for (int i = 0; i < deleteList.Count; i++)
        {
            if (peaceTable.ContainsKey(deleteList[i]) == false)
                Debug.LogError("キーがありません！");

            d.deletePeaceList.Add(peaceTable[deleteList[i]]);
            peaceTable[deleteList[i]].isMatching = true;
        }

        //  Debug.Break();
        if (p == PeaceForm.Pentagon)
        {
            d.nextGenerationPeace = null;
        }
        else
        {
            d.nextGenerationPeace = peaceTable[generationPoint];
            //   peaceTable[generationPoint].nextPeaceForm = p + 1;
        }
        DeletionTargetList.Add(d);
    }

    private bool IsDeletable(Peace peace, Peace nowHoldPeace)
    {
        if (peace.IsDuringFall == true) return false;
        if (nowHoldPeace == peace) return false;
        if (peace.isMatching == true) return false;

        return true;
    }

    //今までのリストに同じものが無いかチェック
    private bool JudgeDoublePoint(List<POINT> originalPointList, POINT CheckPoint)
    {
        for (int i = 0; i < originalPointList.Count; i++)
        {
            if (CheckPoint.X == originalPointList[i].X && CheckPoint.Y == originalPointList[i].Y)
                return false;
        }
        return true;
    }

    private bool CheckSameElement(Peace p1, Peace p2)
    {
        if (p1.GetPeaceForm != p2.GetPeaceForm) return false;
        if (p1.GetPeaceForm != PeaceForm.Triangle) return true;

        if (p1.GetPeaceColor == p2.GetPeaceColor)
            return true;

        return false;
    }

    //指定の中から消すポイントを検索
    private POINT ReturnGeneratePoint(List<POINT> checkList)
    {
        POINT returnPoint = checkList[0];
        for (int i = 1; i < checkList.Count; i++)
        {
            if (checkList[i].X < returnPoint.X)
                returnPoint = checkList[i];
            else if (checkList[i].X == returnPoint.X && returnPoint.Y < checkList[i].Y)
                returnPoint = checkList[i];
        }
        return returnPoint;
    }

    //DeleteListの何番目にあるか
    private int ReturnSameDeleteListNumber(Peace checkPeace, List<DeletePoint> deletionTargetList)
    {
        for (int i = 0; i < deletionTargetList.Count; i++)
        {
            for (int j = 0; j < deletionTargetList[i].deletePeaceList.Count; j++)
            {
                if (deletionTargetList[i].deletePeaceList[j] == checkPeace)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    //下にピースが無ければtrue
    public bool CheckPossibleDown(Dictionary<POINT, Peace> peaceTable, Peace peace)
    {
        // Debug.Log("下に行けるか検索" + peace.point.X + "  " + peace.point.Y);
        for (int i = peace.point.Y + 1; i < PeaceManager.BoardSizeY; i++)
        {
            if (peaceTable.ContainsKey(new POINT(peace.point.X, i)) == false)
                return true;
        }
        return false;
    }

    private void SetDownUnderList(List<POINT> ResultList, List<Peace> deleteList, Peace ignorePoint)
    {
        //foreach (var p in deleteList)
        //{
        //    Debug.Log("SetDownUnderList" + p.point.X + " " + p.point.Y);
        //}


        int[] downPoint = new int[PeaceManager.BoardSizeX];
        for (int i = 0; i < downPoint.Length; i++)
            downPoint[i] = -1;
        for (int i = 0; i < deleteList.Count; i++)
        {
            if (ignorePoint != null && deleteList[i].point.X == ignorePoint.point.X && deleteList[i].point.Y == ignorePoint.point.Y)
                continue;

            if (deleteList[i].point.Y > downPoint[deleteList[i].point.X])
            {
                //  Debug.Log("配列更新 " + deleteList[i].point.X + "  " + deleteList[i].point.Y);
                downPoint[deleteList[i].point.X] = deleteList[i].point.Y;
            }
        }

        for (int i = 0; i < downPoint.Length; i++)
        {
            if (downPoint[i] != -1)
            {
                // Debug.Log("落下リストに格納" + i + "  " + downPoint[i]);
                ResultList.Add(new POINT(i, downPoint[i]));
            }
        }
    }

    public void DebugLog()
    {
        Debug.Log("消されていない");
        for (int i = 0; i < DeletionTargetList.Count; i++)
        {
            for (int j = 0; j < DeletionTargetList[i].deletePeaceList.Count; j++)
            {
                Debug.Log(DeletionTargetList[i].deletePeaceList[j].point.X + "  " + DeletionTargetList[i].deletePeaceList[j].point.Y);
            }
        }


    }

    List<Peace> JudgeList = new List<Peace>();

    public void DownJudge(Peace peace)
    {
        JudgeList.Add(peace);
        //落下したら　リストに格納、数フレーム待ってから判定
        StartCoroutine(WaitJudge());
        if (PeaceManager.Instance.GetPeaceTabel.Count >= PeaceManager.BoardSizeX * PeaceManager.BoardSizeY)
            GameSystem.Instance.GameOver();
    }

    private IEnumerator WaitJudge()
    {
        int waitFrame = 5;
        for (int waitCount = 0; waitCount < waitFrame; waitCount++)
        {
            if (waitCount < waitFrame)
                yield return null;
        }

        for (int i = 0; i < JudgeList.Count; i++)
        {
            JudgePeace(PeaceManager.Instance.GetPeaceTabel, JudgeList[i], PeaceManager.Instance.nowHoldPeace);
        }
        JudgeList.Clear();
    }

    //上から落下を生成するときの
    public int GenerationPositionX()
    {
        int JudgeProportion = 90;//判定方法を分ける、ピース個数の％で
        int MaxPeace = PeaceManager.BoardSizeX * PeaceManager.BoardSizeY;
        int NowProportion = PeaceManager.Instance.GetPeaceTabel.Count / MaxPeace * 100;

        if (NowProportion < JudgeProportion)
        {
            //もし少なかったら
            //ランダムでどこのXに生成するか決め、そこに空きがあったら決定、無かったらやり直し    
            while (true)
            {
                int generarX = Random.Range(0, PeaceManager.BoardSizeX);
                for (int countY = 0; countY < PeaceManager.BoardSizeY; countY++)
                {
                    if (PeaceManager.Instance.GetPeaceTabel.ContainsKey(new POINT(generarX, countY)) == false)
                    {
                        return generarX;
                    }
                }
            }
        }
        else//多かったら
        {
            //無いXのリストを作りそこから抽出
            List<int> Xcollection = new List<int>();
            for (int countX = 0; countX < PeaceManager.BoardSizeX; countX++)
            {
                for (int countY = 0; countY < PeaceManager.BoardSizeY; countY++)
                {
                    if (PeaceManager.Instance.GetPeaceTabel.ContainsKey(new POINT(countX, countY)) == false)
                    {
                        Xcollection.Add(countX);
                        break;
                    }
                }
            }
            if(Xcollection.Count>0)
            {
                int r = Random.Range(0,Xcollection.Count);
                return Xcollection[r];
            }
        }
        return -1;
    }
}
