using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


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
        public List<Peace> deletePeaceList;
        public int deleteCounter;
    }


    //------設定用
    const int BoardSizeX = 10;
    const int BoardSizeY = 6;
    POINT stratPosition = new POINT();
    const int onePeaceSize = 148;
    const int DeleteCount = 3;



    //
    Dictionary<POINT, Peace> peaceTable = new Dictionary<POINT, Peace>();
    public Dictionary<POINT, Peace> PeaceTable
    {
        get { return peaceTable; }
    }
    List<DeletePoint> DeletionTargetList = new List<DeletePoint>();
    List<Peace> stockPeaceList = new List<Peace>();
    //移動用の変数
    public Peace nowHoldPeace = null;
    //ToDo:判定に入らないようにする！



    [SerializeField]
    JudgeManager judgeManager = null;

    [SerializeField]
    GameObject peacePrefab = null;

    [SerializeField]
    GameObject peaceParent = null;

    [SerializeField]
    List<Sprite> PeaceSprites = new List<Sprite>();





    //-------test
    [SerializeField]
    UnityEngine.UI.Text text = null;
    [SerializeField]
    List<Sprite> testDrop = new List<Sprite>();
    int testSpriteCount = 0;





    public void ChangeDrop()
    {

        Sequence sequence = DOTween.Sequence().
            Append(

                peaceTable[new POINT(0, 0)].RectTransform.DOLocalMove(
                   new Vector3(stratPosition.X + 0 * onePeaceSize, stratPosition.Y - 1 * onePeaceSize, 0), 0.1f)
            )
            .Append(
            peaceTable[new POINT(0, 0)].RectTransform.DOLocalMove(
                   new Vector3(stratPosition.X + 0 * onePeaceSize, stratPosition.Y - 2 * onePeaceSize, 0), 0.1f)

            )
            .Append(
            peaceTable[new POINT(0, 0)].RectTransform.DOLocalMove(
                   new Vector3(stratPosition.X + 0 * onePeaceSize, stratPosition.Y - 3 * onePeaceSize, 0), 0.1f)

            )




            .InsertCallback(2, () => text.text = "down");





        testSpriteCount++;

    }
    //まとめてポイント代入できなかったっけ？、new必要？

    // Use this for initialization
    void Start()
    {
        I = this;
        stratPosition.X = -718;
        stratPosition.Y = 290;

        for (int i = 0; i < BoardSizeY; i++)
        {
            for (int j = 0; j < BoardSizeX; j++)
            {
                PeaceType addPeaceType = (PeaceType)UnityEngine.Random.Range(0, (int)PeaceType.Square);
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
            //if (nowHoldPeace.peaceType == PeaceType.None || nowPeace.peaceType == PeaceType.None)//もし持っているのが消えるものか、交換先が消えるものなら交換しようとしたら消える
            //{
            //    ReleasePeace();
            //    return;
            //}//nowPeace.peaceType = PeaceType.None;
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


            ResetPeacePosition(nowPeace);
            //判定処理
            JudgePeace(nowPeace);
            //違う色のピース判定もある

        }
    }

    public void SetHoldPeace(Peace peace)
    {
        nowHoldPeace = peace;
        nowHoldPeace.GetComponent<BoxCollider2D>().enabled = false;
    }

    void ResetPeacePosition(Peace peace)
    {
        peace.GetComponent<RectTransform>().anchoredPosition = new Vector2(stratPosition.X + peace.point.X * onePeaceSize, stratPosition.Y - peace.point.Y * onePeaceSize);
    }
    public void ReleasePeace()
    {
        if (nowHoldPeace == null) return;
        nowHoldPeace.GetComponent<BoxCollider2D>().enabled = true;

        ResetPeacePosition(nowHoldPeace);
        JudgePeace(nowHoldPeace);
        nowHoldPeace = null;
    }

    //TODO:いったん判定後ピース省略
    private void JudgePeace(Peace judgePeace)
    {
        if (judgePeace.peaceType == PeaceType.None)
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
                if (peaceTable[checkPoint].peaceType == judgePeace.peaceType)
                {
                    count++;
                    temporaryDeletingList.Add(checkPoint);
                }
                else
                    break;
            }
            for (int i = judgePoint.Y + 1; i < BoardSizeY; i++)
            {
                checkPoint = new POINT(judgePoint.X, i);
                if (peaceTable.ContainsKey(checkPoint) == false) break;
                if (peaceTable[checkPoint].peaceType == judgePeace.peaceType)
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
            for (int i = judgePoint.X + 1; i < BoardSizeX; i++)
            {
                checkPoint = new POINT(i, judgePoint.Y);
                if (peaceTable.ContainsKey(checkPoint) == false) break;
                if (peaceTable[checkPoint].peaceType == judgePeace.peaceType)
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
                if (peaceTable[checkPoint].peaceType == judgePeace.peaceType)
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
            SetDeletePoint(totalDeleteList);
            // Debug.Log("セット");
        }
        //    Debug.Log("審査終了");
    }

    private void SetDeletePoint(List<POINT> deleteList)
    {

        DeletePoint d;
        d.deletePeaceList = new List<Peace>();
        d.deleteCounter = 0;


        //foreach(var b in deleteList)
        //{
        //    Debug.Log("X="+b.X+"  Y="+b.Y);
        //}

        List<PeaceType> deletePeace = new List<PeaceType>();

        POINT point = ReturnGeneratePoint(deleteList);
        //  d.GeneratePoint = point;
        PeaceType p = peaceTable[point].peaceType;
        for (int i = 0; i < deleteList.Count; i++)
        {
            // if (deleteList[i].X != point.X || deleteList[i].Y != point.Y)
            d.deletePeaceList.Add(PeaceTable[deleteList[i]]);
            try
            {
                deletePeace.Add(peaceTable[deleteList[i]].peaceType);
                peaceTable[deleteList[i]].isMatching = true;  // = PeaceType.None;
            }
            catch
            {
                Debug.LogError("削除リスト作成中エラー");
                break;
            }
        }

        try
        {
            //if (p == PeaceType.Pentagon)//ペンタゴンは全て削除なので生成場所も追加する
            //{
            //    d.point.Add(d.GeneratePoint);

            //}
            if (p != PeaceType.Square && p != PeaceType.Pentagon)
                peaceTable[point].nextPeaceType = PeaceType.Square;
            else if (p == PeaceType.Square)
                peaceTable[point].nextPeaceType = PeaceType.Pentagon;
            DeletionTargetList.Add(d);

        }
        catch { Debug.LogError("削除リスト作成中エラー"); }
        // judgeManager.DleteCheck(deletePeace, DeletionTargetList.Count);

    }


    private void SetChangePoint(Peace nowPeace, POINT changePoint,bool isJugde=false)
    {
        if (peaceTable.ContainsKey(changePoint) == false)
        {
            Peace p = nowPeace;
            peaceTable.Remove(nowPeace.point);
            p.point = changePoint;
            peaceTable.Add(p.point, p);
            
        }
        else//無理だったら次のフレームで挑戦する
        {
            //Action<Peace, POINT> action = new Action<Peace, POINT>(SetChangePoint);
            //action(nowPeace,changePoint);
            if(isJugde==false)
            StartCoroutine(ExecuteNextFrame(nowPeace,changePoint));
            //SetChangePoint(nowPeace,changePoint);//= (nowPeace )=>SetChangePoint(nowPeace, changePoint);

            Debug.Log("今 x="+nowPeace.point.X+"  y="+nowPeace.point.Y);
            Debug.Log("changePoint X="+ changePoint.X+" Y="+changePoint.Y);
            //  Debug.LogError("すでにピースが存在しています");
            Debug.Log("すでにピースが存在しています");
        }
    }

    //actionがわからなかった、、、
    private IEnumerator ExecuteNextFrame(Peace nowPeace, POINT changePoint)
    {
        yield return null;
        SetChangePoint(nowPeace, changePoint,true);
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



    //DeleteListの何番目にあるか
    private int ReturnSameDeleteListNumber(Peace checkPeace)
    {
        for (int i = 0; i < DeletionTargetList.Count; i++)
        {
            for (int j = 0; j < DeletionTargetList[i].deletePeaceList.Count; j++)
            {
                if (DeletionTargetList[i].deletePeaceList[j] == checkPeace)
                {
                    return i;
                }
            }
        }
        return -1;
    }



    //ピースを削除し、上から追加、ずらす
    //TODO:いったん一瞬で詰める
    public void DeletePeace(Peace deletePeace)
    {
        POINT dPoint = deletePeace.point;

        int delteListNumber = ReturnSameDeleteListNumber(deletePeace);
        if (delteListNumber == -1)
        {
            Debug.LogError("リストに見つからなかった");
            return;//見つからなかった
        }
        DeletePoint nowDeletePoint = DeletionTargetList[delteListNumber];
        nowDeletePoint.deleteCounter++;
        DeletionTargetList[delteListNumber] = nowDeletePoint;

        //全て削除済みで、生成も変わっていたら
        if (nowDeletePoint.deleteCounter == nowDeletePoint.deletePeaceList.Count/* && peaceTable[nowDeletePoint.GeneratePoint].peaceType != PeaceType.None*/)//全て削除済みだったら削除、上から追加、判定する（両方）、ずらす
        {
            //  judgeManager.DeleteCount();
           // Debug.Log("全て削除済み");
            ////生成されたもの以外リストから削除、ストックに追加
            //foreach (POINT p in nowDeletePoint.point)
            //{
            //    peaceTable.Remove(p);
            //}
            Peace generationPeace = null;

            List<POINT> nowDeletePointList = new List<POINT>();
            for (int i = 0; i < nowDeletePoint.deletePeaceList.Count; i++)
            {
              //  Debug.Log("構造体  X=" + nowDeletePoint.deletePeaceList[i].point.X + " Y=" + nowDeletePoint.deletePeaceList[i].point.Y);
                nowDeletePointList.Add(nowDeletePoint.deletePeaceList[i].point);
            }

            for (int i = 0; i < nowDeletePoint.deletePeaceList.Count; i++)
            {
                POINT p = nowDeletePoint.deletePeaceList[i].point;

                if (peaceTable[p].nextPeaceType == PeaceType.None)
                {
                    ////ストックに追加
                    //stockPeaceList.Add(peaceTable[p]);
                    Peace peace = peaceTable[p];//一旦逃がす

                    //削除
                    peaceTable.Remove(p);
                    //消したところに上に追加
                    AddToTopPeace(peace);
                }
                else
                {
                    generationPeace = peaceTable[p];
                }
            }


            //今の以外の削除リストを下げる//参照になったからたぶんいらない
            //DownDeletingList(nowDeletePoint.point, delteListNumber);
            List<POINT> DisplacePoint = new List<POINT>();
            //構造体のpeaceのpoint を格納
        

            //削除するところの一番下を求める
            SetDisplacePoint(DisplacePoint, nowDeletePointList);
            foreach (POINT p in DisplacePoint)
            {
                Debug.Log("削除格納dispoint=" + p.X + "  " + p.Y);
            }


            for (int k = 0; k < DisplacePoint.Count; k++)//X各座標
            {
                bool isGeneration = false;
                //上を全て下げるフラグを立てる
                for (int i = DisplacePoint[k].Y; i >= 0; i--)
                {
                    if (peaceTable.ContainsKey(new POINT(DisplacePoint[k].X, i)))
                    {
                        //フラグが経っているか生成時なら残りは確認しながら落ちる

                        if (isGeneration || peaceTable[new POINT(DisplacePoint[k].X, i)].nextPeaceType != PeaceType.None)
                        {
                            isGeneration = true;
                            if (peaceTable[new POINT(DisplacePoint[k].X, i)].IsDuringFall == false)
                            {
                             //   Debug.Log("落とす　X="+ DisplacePoint[k].X+" Y="+i);
                                CheckFallingPeace(peaceTable[new POINT(DisplacePoint[k].X, i)]);
                            }
                        }
                        else
                        {
                            if (peaceTable[new POINT(DisplacePoint[k].X, i)].IsDuringFall == false)
                            {
                               // Debug.Log("落とす　X=" + DisplacePoint[k].X + " Y=" + i);
                                SetFallPeace(peaceTable[new POINT(DisplacePoint[k].X, i)]);
                            }
                        }
                    }
                }

            }
         
            generationPeace.nextPeaceType = PeaceType.None;//最後に直す
            //判定は生成したところだけ
            JudgePeace(generationPeace);


            DeletionTargetList.Remove(nowDeletePoint);

            // judgeManager.CheckCreate(peaceTable);
        }
    }


    /// <summary>
    /// 削除されるピースと同じX軸の上に追加
    /// </summary>
    /// <param name="deletePeace"></param>
    private void AddToTopPeace(Peace deletePeace)
    {
        for (int i = -1; i >= -BoardSizeY; i--)
        {
            if (peaceTable.ContainsKey(new POINT(deletePeace.point.X, i)) == false)
            {
                //初期化
                deletePeace.Initialization();
                deletePeace.SetNewType();
                deletePeace.SetSprite(ReturnSprite(deletePeace.peaceType));
                //point
                deletePeace.point = new POINT(deletePeace.point.X, i);
                //テーブルに追加
                peaceTable.Add(deletePeace.point, deletePeace);
                //場所を移動
                ResetPeacePosition(deletePeace);
                //落下をつける
                SetFallPeace(deletePeace);
                return;
            }
        }
    }

    //今の場所の一つ下がなかったら動かす
    public void CheckFallingPeace(Peace peace)
    {
        Debug.Log("CheckFallingPeace  x=" + peace.point.X + "　Y=" + peace.point.Y);
        if (peace.point.Y < BoardSizeY - 1) //一番下より小さく、
        {
            for (int i = peace.point.Y + 1; i < BoardSizeY; i++)
            {
                if (peaceTable.ContainsKey(new POINT(peace.point.X, i)) == false)
                {
                    peaceTable[peace.point].IsDuringFall = true;
                    //そのポイントより下のポイントで一個でも空きマスがあればポイントが無ければ動かす
                    //番号は途中から変える?
                    //  new Vector2(stratPosition.X + peace.point.X * onePeaceSize, stratPosition.Y - peace.point.Y * onePeaceSize);
                    Sequence sequence = DOTween.Sequence().
                        OnStart(() =>
                        {
                            peaceTable[peace.point].RectTransform.DOLocalMove(new Vector2(stratPosition.X + peace.point.X * onePeaceSize, stratPosition.Y - (peace.point.Y + 1) * onePeaceSize), 1);//移動
                        }).InsertCallback(0.5f, () => SetChangePoint(peace, new POINT(peace.point.X, peace.point.Y + 1)))
                        .OnComplete(() =>
                            CheckFallingPeace(peace)
                        );//ここで半分の時番号変える、もし交換して何も無いなら終了

                    return;

                }
            }
        }
    }

    /// <summary>
    /// 落下させつつピースの番号を変える
    /// </summary>
    private void SetFallPeace(Peace peace)
    {

        peace.IsDuringFall = true;
        Sequence sequence = DOTween.Sequence().
                     OnStart(() =>
                     {
                         peaceTable[peace.point].RectTransform.DOLocalMove(new Vector2(stratPosition.X + peace.point.X * onePeaceSize, stratPosition.Y - (peace.point.Y + 1) * onePeaceSize), 1);//移動
                     }).InsertCallback(0.5f, () => SetChangePoint(peace, new POINT(peace.point.X, peace.point.Y + 1)))
                     .OnComplete(() =>
                         CheckFallingPeace(peace)
                     );//ここで半分の時番号変える、もし交換して何も無いなら終了
    }


    private void SetDisplacePoint(List<POINT> displacePoint, List<POINT> deleteList)
    {

        int[] downPoint = new int[BoardSizeX];
        for (int i = 0; i < downPoint.Length; i++)
            downPoint[i] = -1;
        for (int i = 0; i < deleteList.Count; i++)
        {
     //       Debug.Log("deleteList[i].Y > downPoint[deleteList[i].X]  " + deleteList[i].Y + ">" + downPoint[deleteList[i].X]);
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

        //for (int i = 0; i < DeletionTargetList.Count; i++)
        //{
        //    for (int j = 0; j < DeletionTargetList[i].point.Count; j++)
        //    {
        //        Debug.Log("i=" + i + " j=" + j + " X=" + DeletionTargetList[i].point[j].X + " Y=" + DeletionTargetList[i].point[j].Y);

        //    }

        //}
        Debug.Log("------------------------------");


    }
}
