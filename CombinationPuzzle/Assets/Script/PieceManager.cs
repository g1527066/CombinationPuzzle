using System.Collections;
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

public class PieceManager : MonoBehaviour
{

    public static PieceManager I = null;

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
                ReleasePiece();
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
    public void ReleasePiece()
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
                    if (JudgeDoublePoint(totalDeleteList, temporaryDeletingList[i]))
                        totalDeleteList.Add(new POINT(temporaryDeletingList[i].X, temporaryDeletingList[i].Y));
                }
                isDelete = true;

            }

            //左右調査------------------------
            count = 1;
            //右
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
        }
    }

    private void SetDeletePoint(List<POINT> deleteList)
    {

        DeletePoint d;
        d.point = new List<POINT>();
        d.deleteCounter = 0;



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
                return;
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
                        //全て削除
                        foreach (POINT p in DeletionTargetList[i].point)
                        {
                            peaceTable.Remove(p);
                        }

                        List<POINT> DisplacePoint = new List<POINT>();

                        SetDisplacePoint(DisplacePoint, DeletionTargetList[i].point);

                        int changeY;
                        for (int k = 0; k < DisplacePoint.Count; k++)//X各座標
                        {
                           
                            //ずらしたい、、、
                            for(int n=DisplacePoint[k].Y;n>0;n--)
                            {
                                for (changeY = DisplacePoint[k].Y - 1; changeY >= 0; changeY--)
                                {
                                    if (peaceTable[new POINT(DisplacePoint[k].X,changeY)] != null)
                                    {
                                        Peace p = peaceTable[new POINT(DisplacePoint[k].X, changeY)];//保存
                                        //中身を無かった場所に移動
                                        p.point =new POINT(DisplacePoint[k].X,n);
                                        //元の削除
                                        peaceTable.Remove(new POINT(DisplacePoint[k].X, changeY));
                                        //登録しなおし
                                        peaceTable.Add(p.point, p);
                                        //位置のセット
                                        ResetHoldPeacePosition(p);
                                        break;

                                    }
                                    if(changeY==0)
                                    {
                                        for(int addPeaceCount=n;addPeaceCount>=0; addPeaceCount--)
                                        {
                                            SetNewPeace(DisplacePoint[k].X, addPeaceCount);
                                        }
                                        break;
                                    }

                                }



                            }

                            //最後は追加のみ
                           // SetNewPeace(DisplacePoint[k].X ,0);


                            ////ずらす
                            //for (int n = DeletionTargetList[i].point[k].Y - 1; n >= 0; n--)
                            //{
                            //    POINT modificationPoint = DeletionTargetList[i].point[k];
                            //    //TODO:途中
                            //    //縦三つの時も考慮してつくる！

                            //    //peaceTable[new POINT(modificationPoint.X, n)].point = new POINT(modificationPoint.X, n + 1);
                            //    //Peace p = peaceTable[new POINT(modificationPoint.X, n)];
                            //    //peaceTable.Remove(new POINT(modificationPoint.X, n));
                            //    //peaceTable.Add(new POINT(modificationPoint.X, n + 1), p);
                            //    //ResetHoldPeacePosition(p);

                            //    ////上に追加
                            //    //SetNewPeace(deletePointList[i].point[k].X, 0);



                            //}
                        }
                        //判定
                        List<POINT> deletePoint = new List<POINT>();
                        SetDeletePointList(deletePoint, DeletionTargetList[i]);
                        for (int k = 0; k < deletePoint.Count; k++)
                        {
                            JudgePeace(peaceTable[deletePoint[k]]);
                        }
                        //■とかに変化したものも判定//前がペンタゴンの場合もう一回判定してしまうことになる
                        JudgePeace(peaceTable[DeletionTargetList[i].GeneratePoint]);

                        //リストも削除
                        DeletionTargetList.Remove(DeletionTargetList[i]);
                    }
                    break;
                }
            }
            if (hitSamePoint)
                break;
        }



        ////削除
        //POINT dPoint = deletePeace.point;
        //peaceTable.Remove(dPoint);


        ////  List<Peace> dowPeace = new List<Peace>();
        ////そのポイントから↑下にずらす

        ////ずらした後の全てのピースを消す
        ////for(int i=0;i< dowPeace.Count; i++)
        ////{
        ////    JudgePiece(dowPeace[i]);
        ////}


        //JudgePeace(newPeace);
        //JudgeGameClear();
        //上追加に

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

        int count = 0;
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
                for (int j = downCount; downCount >= 0; j--)
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
}
