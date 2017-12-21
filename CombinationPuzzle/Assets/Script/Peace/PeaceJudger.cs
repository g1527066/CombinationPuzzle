using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DeletePoint
{
    public List<Peace> deletePeaceList;//削除にnextも含まれる
    public Peace nextGenerationPeace;
    public int deleteCounter;
}



public class PeaceJudger : MonoBehaviour
{
    public static PeaceJudger I = null;

    const int DeleteCount = 3;
    List<DeletePoint> DeletionTargetList = new List<DeletePoint>();


    // Use this for initialization
    void Awake()
    {
        I = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool CurrentDeletable(Dictionary<POINT, Peace> dictionary, Peace judgeAddPeace)
    {
        //上
        int count = 0;
        POINT p = judgeAddPeace.point;
        for (int i = p.Y - 1; i >= 0; i--)
        {
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
            Peace changeGenerationPeace = null;
            // Debug.Log("全て削除済み");
            //生成するありなら
            if (nowDeletePoint.nextGenerationPeace != null)
            {
                EffectManager.I.PlayEffect(nowDeletePoint.nextGenerationPeace.gameObject.transform.position, "生成");
                changeGenerationPeace = PeaceManager.I.ChangeForm(nowDeletePoint.nextGenerationPeace);
            }

            //削除した場所以降のピースを落下させる//TODO:生成がnullだと、、？
            List<POINT> downTargetPoint = new List<POINT>();
            SetDownUnderList(downTargetPoint, nowDeletePoint.deletePeaceList, nowDeletePoint.nextGenerationPeace);



            //生成されたもの以外リストから削除
            List<Peace> SetChangeList = new List<Peace>();
            for (int i = 0; i < nowDeletePoint.deletePeaceList.Count; i++)
            {
                if (nowDeletePoint.deletePeaceList[i].GetPeaceForm == PeaceForm.Triangle)
                    EffectManager.I.PlayEffect(nowDeletePoint.deletePeaceList[i].gameObject.transform.position, nowDeletePoint.deletePeaceList[i].peaceColor.DisplayName());
                else if (nowDeletePoint.deletePeaceList[i].GetPeaceForm == PeaceForm.Square)
                    EffectManager.I.PlayEffect(nowDeletePoint.deletePeaceList[i].gameObject.transform.position, "黒");
                else
                    EffectManager.I.PlayEffect(nowDeletePoint.deletePeaceList[i].gameObject.transform.position, "白");


                if (nowDeletePoint.deletePeaceList[i] == nowDeletePoint.nextGenerationPeace) continue;


                SetChangeList.Add(PeaceGenerator.I.ChangeForm(nowDeletePoint.deletePeaceList[i]));
                peaceTable.Remove(nowDeletePoint.deletePeaceList[i].point);
                PeaceManager.I.AddToTopPeace(SetChangeList[SetChangeList.Count - 1]);
            }

            //  動かすのはあと、場所がずれるため
            for (int i = 0; i < downTargetPoint.Count; i++)
            {
                for (int j = downTargetPoint[i].Y; j >= 0; j--)
                {
                    // Debug.Log("上を落下させる " + downTargetPoint[i].X + "  " + j);
                    if (PeaceManager.I.GetPeaceTabel.ContainsKey(new POINT(downTargetPoint[i].X, j)))
                        PeaceOperator.I.AddDrop(PeaceManager.I.GetPeaceTabel[new POINT(downTargetPoint[i].X, j)]);
                }
            }

            //削除リストを削除
            DeletionTargetList.Remove(DeletionTargetList[delteListNumber]);
            //生成後のみ審査??TODO:変える
            if (changeGenerationPeace != null)
                JudgePeace(peaceTable, changeGenerationPeace, PeaceManager.I.nowHoldPeace);

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
            if (returnPoint.X <= checkList[i].X &&
                    checkList[i].X - returnPoint.X < checkList[i].Y - returnPoint.Y)
            {

                returnPoint = checkList[i];
            }
            else if (returnPoint.X > checkList[i].X &&
                returnPoint.X - checkList[i].X >= returnPoint.Y - checkList[i].Y)
            {
                returnPoint = checkList[i];
            }
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
            JudgePeace(PeaceManager.I.GetPeaceTabel, JudgeList[i], PeaceManager.I.nowHoldPeace);
        }
        JudgeList.Clear();
    }



}
