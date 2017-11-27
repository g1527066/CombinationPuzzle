﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PeaceType
{

    Red,
    Blue,
    Yellow,
    Green,
    Perple,
    Orange,
    Square,
    Pentagon,
    None,
}

public struct POINT
{
    public int X;
    public int Y;
    public POINT(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public class PeaceManager : MonoBehaviour
{

    public static PeaceManager I = null;

    struct PeaceJudgeStruct
    {
        public PeaceType peaceType;
        public POINT point;
    }


    struct DeletePoint
    {
        public List<POINT> point;
        public int deleteCounter;
        public POINT GeneratePoint;
    }

    List<DeletePoint> DeletionTargetList = new List<DeletePoint>();

    const int BoardSizeX = 10;
    const int BoardSizeY = 6;
    // Peace[,] peaceList = new Peace[BoardSizeY, BoardSizeX];

    //すべて格納
    Dictionary<POINT, Peace> peaceTable = new Dictionary<POINT, Peace>();

    [SerializeField]
    GameObject peacePrefab = null;

    [SerializeField]
    GameObject peaceParent = null;

    [SerializeField]
    List<Sprite> PeaceSprites = new List<Sprite>();

    //  List<POINT> totalDeleteList = new List<POINT>();

    // List<PeaceJudgeStruct> peaceJudgeStructList = new List<PeaceJudgeStruct>();

    //判定用に
    Dictionary<POINT, Peace> peaceJudgeStructList = new Dictionary<POINT, Peace>();


    //Dictionary<POINT, Peace> peaceDictiona = new Dictionary<POINT, Peace>();

    //移動用の変数
    public Peace nowHoldPeace = null;
    //判定に入らないように

    POINT stratPosition = new POINT();

    int onePeaceSize = 160;
    const int DeleteCount = 3;

    //まとめてポイント代入できなかったっけ？、new必要？

    // Use this for initialization
    void Start()
    {
        I = this;
        stratPosition.X = -737;
        stratPosition.Y = 364;

        for (int i = 0; i < BoardSizeY; i++)
        {
            for (int j = 0; j < BoardSizeX; j++)
            {
                PeaceType addPeaceType = (PeaceType)Random.Range(0, (int)PeaceType.Square);
                if (StartJudge(new POINT(j, i), addPeaceType))
                {
                    Peace newPeace = Instantiate(peacePrefab).GetComponent<Peace>();
                    newPeace.peaceType = addPeaceType;
                    newPeace.transform.SetParent(peaceParent.transform, false);
                    newPeace.GetComponent<RectTransform>().anchoredPosition = new Vector2(stratPosition.X + j * onePeaceSize, stratPosition.Y - i * onePeaceSize);
                    newPeace.point = new POINT(j, i);
                    newPeace.SetSprite(PeaceSprites[(int)newPeace.peaceType]);
                    peaceTable.Add(newPeace.point, newPeace);
                }
                else
                    j--;
            }

        }
    }

    /// <summary>
    /// 消える組み合わせだった場合false
    /// </summary>
    /// <param name="newPeace">ここ基準判定</param>
    /// <returns></returns>
    private bool StartJudge(POINT p, PeaceType peaceType)
    {
        //上
        int count = 0;
        //  POINT p = newPeace.point;
        for (int i = p.Y - 1; i >= 0; i--)
        {
            if (peaceTable[new POINT(p.X, i)].peaceType == peaceType)
            {
                count++;
            }
            else
                break;
        }
        if (count >= 2)
        {
            return false;
        }

        //左
        count = 0;
        for (int i = p.X - 1; i >= 0; i--)
        {
            if (peaceTable[new POINT(i, p.Y)].peaceType == peaceType)
            {
                count++;
            }
            else
                break;
        }
        if (count >= 2)
        {
            return false;
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {



    }

    public Sprite ReturnSprite(PeaceType peaceType)
    {
        return PeaceSprites[(int)peaceType];
    }

    public void MoveHoldPeace(Vector2 difference, RaycastHit2D hit)
    {
        if (nowHoldPeace == null) return;
        nowHoldPeace.GetComponent<RectTransform>().anchoredPosition += difference;

        Peace nowPeace = null;

        try
        {
            nowPeace = hit.collider.gameObject.GetComponent<Peace>();
            if (nowHoldPeace.peaceType == PeaceType.None || nowPeace.peaceType == PeaceType.None)//もし持っているのが消えるものか、交換先が消えるものなら交換しようとしたら消える
            {
                ReleasePeace();
                return;
            }//nowPeace.peaceType = PeaceType.None;
        }
        catch { return; }



        //前回とピースがちがかったら入れ替え
        if (nowPeace != null && nowHoldPeace != nowPeace)
        {
            AudioManager.I.PlaySound("Trade");//一旦

            POINT savepoint = nowHoldPeace.point;
            nowHoldPeace.point = nowPeace.point;
            nowPeace.point = savepoint;

            //保存
            Peace p1 = nowPeace;
            Peace p2 = nowHoldPeace;
            peaceTable.Remove(nowPeace.point);
            peaceTable.Remove(nowHoldPeace.point);
            peaceTable.Add(nowPeace.point, p1);
            peaceTable.Add(nowHoldPeace.point, p2);
            //tabelのキーも変更↑できてるか、、、？


            ResetHoldPeacePosition(nowPeace);
            //判定処理
            // peaceJudgeStructList.Clear();
            JudgePeace(nowPeace);
            //違う色のピース判定もある

        }
        //Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //nowPeace.GetComponent<RectTransform>().anchoredPosition = new Vector2(point.x, point.y);

        //Debug.Log("2");
    }

    public void SetHoldPeace(Peace peace)
    {

        nowHoldPeace = peace;
        nowHoldPeace.GetComponent<BoxCollider2D>().enabled = false;
        //  nowHoldPeace.GetComponent<RectTransform>().anchoredPosition += new Vector3(0,0,-nowHoldPeace.GetComponent<RectTransform>().anchoredPosition.x);
    }

    void ResetHoldPeacePosition(Peace peace)
    {
        peace.GetComponent<RectTransform>().anchoredPosition = new Vector2(stratPosition.X + peace.point.X * onePeaceSize, stratPosition.Y - peace.point.Y * onePeaceSize);
    }
    public void ReleasePeace()
    {
        if (nowHoldPeace == null) return;
        nowHoldPeace.GetComponent<BoxCollider2D>().enabled = true;

        ResetHoldPeacePosition(nowHoldPeace);
        JudgePeace(nowHoldPeace);
        nowHoldPeace = null;
    }

    //TODO:いったん判定後ピース省略


    private void JudgePeace(Peace judgePeace)
    {
        //Debug.Log("審査位置" + judgePeace.point.X + " " + judgePeace.point.Y);
        //Debug.Log("DeletionTargetList=" + DeletionTargetList.Count);
        if (judgePeace.peaceType == PeaceType.None)
            return;

        //プレイヤーの持っているのは除外
        POINT judgePoint;// = judgePeace.point;

        List<POINT> temporaryDeletingList = new List<POINT>();

        List<POINT> totalDeleteList = new List<POINT>();
        bool isDelete = false;
        //上下調査------------------------
        int count;// = 1;
        //  POINT generatePeacePoint = new POINT(judgePeace.point.X, judgePeace.point.Y);
        //上へ
        totalDeleteList.Add(new POINT(judgePeace.point.X, judgePeace.point.Y));

        for (int j = 0; j < totalDeleteList.Count; j++)
        {
            judgePoint = totalDeleteList[j];
            count = 1;
            temporaryDeletingList.Clear();
            for (int i = judgePoint.Y - 1; i >= 0; i--)
            {
                if (peaceTable[new POINT(judgePoint.X, i)].peaceType == judgePeace.peaceType)
                {
                    count++;
                    temporaryDeletingList.Add(new POINT(judgePoint.X, i));
                }
                else
                    break;
            }
            for (int i = judgePoint.Y + 1; i < BoardSizeY; i++)
            {
                if (peaceTable[new POINT(judgePoint.X, i)].peaceType == judgePeace.peaceType)
                {
                    count++;
                    temporaryDeletingList.Add(new POINT(judgePoint.X, i));
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
            for (int i = judgePoint.X + 1; i < BoardSizeX; i++)
            {
                if (peaceTable[new POINT(i, judgePoint.Y)].peaceType == judgePeace.peaceType)
                {
                    temporaryDeletingList.Add(new POINT(i, judgePoint.Y));
                    count++;
                }
                else
                    break;
            }
            for (int i = judgePoint.X - 1; i >= 0; i--)
            {
                if (peaceTable[new POINT(i, judgePoint.Y)].peaceType == judgePeace.peaceType)
                {
                    temporaryDeletingList.Add(new POINT(i, judgePoint.Y));
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
            SetDeletePoint(totalDeleteList);
            Debug.Log("セット");
        }
        Debug.Log("審査終了");
    }

    private void SetDeletePoint(List<POINT> deleteList)
    {

        DeletePoint d;
        d.point = new List<POINT>();
        d.deleteCounter = 0;


        //foreach(var b in deleteList)
        //{
        //    Debug.Log("X="+b.X+"  Y="+b.Y);
        //}



        POINT point = ReturnGeneratePoint(deleteList);
        d.GeneratePoint = point;
        PeaceType p = peaceTable[point].peaceType;
        for (int i = 0; i < deleteList.Count; i++)
        {
            if (deleteList[i].X != point.X || deleteList[i].Y != point.Y)
                d.point.Add(deleteList[i]);
            try
            {
                peaceTable[deleteList[i]].peaceType = PeaceType.None;
            }
            catch
            {
                break;
            }
        }
        try
        {
            if (p == PeaceType.Pentagon)//ペンタゴンは全て削除なので生成場所も追加する
            {
                d.point.Add(d.GeneratePoint);
               
            }
            else if (p != PeaceType.Square)//TODO:いったんペンタゴンはスクエアになる設定
            {
                peaceTable[point].nextPeaceType = PeaceType.Square;
            }
            else
            {

                peaceTable[point].nextPeaceType = PeaceType.Pentagon;

            }
            DeletionTargetList.Add(d);

        }
        catch { }


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



    //被っていたらtrue
    private bool CheckSamePeacePoint(List<POINT> pointList, POINT checkPoint)
    {
        for (int i = 0; i < pointList.Count; i++)
        {
            if (pointList[i].X == checkPoint.X && pointList[i].Y == checkPoint.Y)
                return true;
        }
        return false;
    }


    //指定の中から消すポイントを検索
    private POINT ReturnGeneratePoint(List<POINT> checkList)
    {
        POINT returnPoint = checkList[0];
        for (int i = 1; i < checkList.Count; i++)
        {
            //消すポイントしたに統一
            //if (returnPoint.Y <= checkList[i].Y && returnPoint.X > checkList[i].X)
            //{
            //    returnPoint = checkList[i];
            //}

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

    //ピースを削除し、上から追加、ずらす
    //TODO:いったん一瞬で詰める
    public void DeletePeace(Peace deletePeace)
    {
        bool hitSamePoint = false;
        POINT dPoint = deletePeace.point;
        //  peaceTable.Remove(dPoint); 
        for (int i = 0; i < DeletionTargetList.Count; i++)
        {
            for (int j = 0; j < DeletionTargetList[i].point.Count; j++)
            {
                if (DeletionTargetList[i].point[j].X == dPoint.X && DeletionTargetList[i].point[j].Y == dPoint.Y)
                {
                    DeletePoint d = DeletionTargetList[i];
                    d.deleteCounter++;
                    DeletionTargetList[i] = d;
                    hitSamePoint = true;



                    if (DeletionTargetList[i].deleteCounter == DeletionTargetList[i].point.Count)//全て削除済みだったら削除、上から追加、判定する（両方）、ずらす
                    {

                        Debug.Log("全て削除済み");

                        ////全て削除
                        foreach (POINT p in DeletionTargetList[i].point)
                        {
                            peaceTable.Remove(p);
                        }

                        DownDeletingList(DeletionTargetList[i].point,i);

                        List<POINT> DisplacePoint = new List<POINT>();

                        //削除するところの一番下を求める
                        SetDisplacePoint(DisplacePoint, DeletionTargetList[i].point);
                        //foreach (POINT p in DisplacePoint)
                        //{
                        //    Debug.Log("dispoint="+p.X+"  "+p.Y);
                        //}

                        int changeY;
                        for (int k = 0; k < DisplacePoint.Count; k++)//X各座標
                        {
                            Debug.Log("X各座標" + k);
                            //ずらしたい、、、

                            if (DisplacePoint[k].Y == 0)
                            {
                                SetNewPeace(DisplacePoint[k].X, 0);
                                continue;
                            }

                            for (int n = DisplacePoint[k].Y; n > 0; n--)
                            {
                                Debug.Log("n="+n);
                                for (changeY = n - 1; changeY >= 0; changeY--)
                                {
                                    //Debug.Log(changeY);
                                    if (peaceTable.ContainsKey(new POINT(DisplacePoint[k].X, changeY)))
                                    {
                                        Debug.Log("キーが存在したX=" + DisplacePoint[k].X + "  Y=" + changeY);
                                        Peace p = peaceTable[new POINT(DisplacePoint[k].X, changeY)];//保存
                                        //中身を無かった場所に移動
                                        p.point = new POINT(DisplacePoint[k].X, n);
                                        //元の削除
                                        peaceTable.Remove(new POINT(DisplacePoint[k].X, changeY));
                                        //登録しなおし
                                        Debug.Log("登録しなおしポイント X=" + p.point.X + "  Y=" + p.point.Y);
                                        peaceTable.Add(p.point, p);//TODO:エラー
                                        //位置のセット
                                        ResetHoldPeacePosition(p);

                                        if (changeY == 0)
                                            SetNewPeace(DisplacePoint[k].X, changeY);
                                        break;
                                    }
                                    if (changeY == 0)
                                    {
                                        Debug.Log("changeY=" + changeY + "  n" + n);
                                        for (int addPeaceCount = n; addPeaceCount >= 0; addPeaceCount--)
                                        {
                                            SetNewPeace(DisplacePoint[k].X, addPeaceCount);
                                        }
                                        break;
                                    }

                                }
                            }
                        }
                        ////判定
                        List<POINT> deletePoint = new List<POINT>();
                        SetDeletePointList(deletePoint, DeletionTargetList[i]);//ここがエラー
                        for (int k = 0; k < deletePoint.Count; k++)
                        {
                            Debug.Log("--" + deletePoint[k].X + "  " + deletePoint[k].Y);
                            JudgePeace(peaceTable[deletePoint[k]]);//TODO:error
                            Debug.Log(peaceTable[deletePoint[k]].point.X + "  " + peaceTable[deletePoint[k]].point.Y);

                        }
                        ////■とかに変化したものも判定//前がペンタゴンの場合もう一回判定してしまうことになる
                        JudgePeace(peaceTable[DeletionTargetList[i].GeneratePoint]);//変化する前にすべての削除がオア割ってしまうと判定ができないまだNone
                        Debug.Log("場所" + DeletionTargetList[i].GeneratePoint.X + "  " + DeletionTargetList[i].GeneratePoint.Y);
                        Debug.Log("タイプ" + peaceTable[DeletionTargetList[i].GeneratePoint].peaceType);
                        //リストも削除
                        //Debug.Log("削除前"+DeletionTargetList.Count);
                        DeletionTargetList.Remove(DeletionTargetList[i]);
                        Debug.Log("削除後" + peaceTable.Count);


                    }
                    break;
                }
            }
            if (hitSamePoint)
                break;
        }

        //JudgePeace(newPeace);
        //JudgeGameClear();
        //上追加に

    }

    /// <summary>
    /// 今の削除中の以外のリストのY軸を下げる
    /// </summary>
    /// <param name="deleteList"></param>
    /// <param name="deleteNumber">何番目の削除リストか</param>
    private void DownDeletingList(List<POINT> deleteList, int deleteNumber)
    {
        for(int i=0;i<DeletionTargetList.Count;i++)
        {
            if (i == deleteNumber) continue;
            for(int j=0;j<DeletionTargetList[i].point.Count;j++)
            {
                for(int k=0;k<deleteList.Count;k++)
                {
                    if(deleteList[k].X==DeletionTargetList[i].point[j].X&&
                        deleteList[k].Y > DeletionTargetList[i].point[j].Y)
                    {
                        POINT p =DeletionTargetList[i].point[j];
                        p = new POINT(p.X, p.Y+1);
                        DeletionTargetList[i].point[j]=p;
                    }
                }
            }
        }


    }

    private void SetDisplacePoint(List<POINT> displacePoint, List<POINT> deleteList)
    {

        int[] downPoint = new int[BoardSizeX];
        for (int i = 0; i < downPoint.Length; i++)
            downPoint[i] = -1;
        for (int i = 0; i < deleteList.Count; i++)
        {
            if (deleteList[i].Y > downPoint[deleteList[i].X])
            {
                downPoint[deleteList[i].X] = deleteList[i].Y;
            }
        }

        for (int i = 0; i < downPoint.Length; i++)
        {
            if (downPoint[i] != -1)
            {
                displacePoint.Add(new POINT(i, downPoint[i]));
            }
        }
    }


    private void SetDeletePointList(List<POINT> deletePoint, DeletePoint deletePointStruct)
    {
        int downCount;
        for (int i = 0; i < BoardSizeX; i++)
        {
            downCount = -1;
            for (int j = 0; j < deletePointStruct.point.Count; j++)
            {
                if (deletePointStruct.point[j].X == i && downCount < deletePointStruct.point[j].Y)
                {
                    downCount = deletePointStruct.point[j].Y;
                }
            }
            if (downCount != -1)
            {
                for (int j = downCount; j >= 0; j--)
                {
                    deletePoint.Add(new POINT(i, j));
                }
            }
        }
    }




    //上に新しいピースを入れる
    private void SetNewPeace(int x, int y)
    {
        POINT setPoint = new POINT(x, y);
        Peace newPeace = Instantiate(peacePrefab).GetComponent<Peace>();
        newPeace.peaceType = (PeaceType)Random.Range(0, (int)PeaceType.Orange);
        newPeace.transform.SetParent(peaceParent.transform, false);
        newPeace.GetComponent<RectTransform>().anchoredPosition = new Vector2(stratPosition.X + setPoint.X * onePeaceSize, stratPosition.Y - setPoint.Y * onePeaceSize);
        newPeace.point = setPoint;
        newPeace.SetSprite(PeaceSprites[(int)newPeace.peaceType]);
        peaceTable.Add(newPeace.point, newPeace);
    }



    //必要数あるかなどを検索、何個消せとかもありそうだけど、、、点数とか
    //一旦今回ののみに対応させる
    private void JudgeGameClear()
    {
        int count = 0;
        foreach (var key in peaceTable.Keys)
        {
            if (peaceTable[key].peaceType == PeaceType.Pentagon)
                count++;
        }
        if (count >= 2)
            GameSystem.I.Clear();
    }




    public void DebugButtone()
    {
        Debug.Log("--------Debug----------------------");

        Debug.Log("マス数=" + peaceTable.Count);
        Debug.Log("削除リスト数=" + DeletionTargetList.Count);

        for(int i=0;i<DeletionTargetList.Count;i++)
        {
            for(int j=0;j<DeletionTargetList[i].point.Count;j++)
            {
                Debug.Log("i="+i+" j="+j+" X="+ DeletionTargetList[i].point[j].X+" Y="+ DeletionTargetList[i].point[j].Y);

            }

        }
        Debug.Log("------------------------------");


    }
}
