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
                Peace newPeace = Instantiate(peacePrefab).GetComponent<Peace>();
                newPeace.peaceType = (PeaceType)Random.Range(0, (int)PeaceType.Orange);
                newPeace.transform.SetParent(peaceParent.transform, false);
                newPeace.GetComponent<RectTransform>().anchoredPosition = new Vector2(stratPosition.X + j * onePeaceSize, stratPosition.Y - i * onePeaceSize);
                newPeace.point = new POINT(j, i);
                newPeace.SetSprite(PeaceSprites[(int)newPeace.peaceType]);
                peaceTable.Add(newPeace.point, newPeace);

                //  peaceList[i, j] = newPeace;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

        //移動中、、
        if (nowHoldPeace != null)
        {

        }


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

                Debug.Log("どちらかNONEなので消えます、、はず、、");
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
            JudgePiece(nowPeace);
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
        JudgePiece(nowHoldPeace);
        nowHoldPeace = null;
    }

    //TODO:いったん判定後ピース省略


    private void JudgePiece(Peace judgePeace)
    {
        //プレイヤーの持っているのは除外
        POINT judgePoint = judgePeace.point;

        List<POINT> deletePointList = new List<POINT>();

        List<POINT> tempDeletePointList = new List<POINT>();
        bool isDelete = false;
        //上下調査------------------------
        int count = 1;
        //  POINT generatePeacePoint = new POINT(judgePeace.point.X, judgePeace.point.Y);
        //上へ
        tempDeletePointList.Add(new POINT(judgePoint.X, judgePoint.Y));
        for (int i = judgePoint.Y - 1; i >= 0; i--)
        {
            if (peaceTable[new POINT(judgePoint.X, i)].peaceType == judgePeace.peaceType)
            {
                count++;
                deletePointList.Add(new POINT(judgePoint.X, i));
            }
            else
                break;
        }
        for (int i = judgePoint.Y + 1; i < BoardSizeY; i++)
        {
            if (peaceTable[new POINT(judgePoint.X, i)].peaceType == judgePeace.peaceType)
            {
                count++;
                deletePointList.Add(new POINT(judgePoint.X, i));
                //generatePeacePoint.Y = i;
            }
            else
                break;
        }
        if (count >= DeleteCount)
        {
            //if (isDelete == false)
            //{
                for (int i = 0; i < deletePointList.Count; i++)
                {
                    tempDeletePointList.Add(new POINT(deletePointList[i].X, deletePointList[i].Y));
                }
                isDelete = true;
            //}
            //else//被っているのが無いかチェック
            //{
            //    for (int i = 0; i < deletePointList.Count; i++)
            //    {
            //        if (CheckSamePeacePoint(tempDeletePointList, deletePointList[i]) == false)
            //            tempDeletePointList.Add(new POINT(deletePointList[i].X, deletePointList[i].Y));
            //    }
            //}

            // SetDeletePoint(tempDeletePointList);
        }

        //左右調査------------------------
        count = 1;
        //右
        deletePointList.Clear();
        //tempDeletePointList.Add(new POINT(judgePoint.X, judgePoint.Y));
        for (int i = judgePoint.X + 1; i < BoardSizeX; i++)
        {
            if (peaceTable[new POINT(i, judgePoint.Y)].peaceType == judgePeace.peaceType)
            {
                deletePointList.Add(new POINT(i, judgePoint.Y));
                count++;
            }
            else
                break;
        }
        for (int i = judgePoint.X - 1; i >= 0; i--)
        {
            if (peaceTable[new POINT(i, judgePoint.Y)].peaceType == judgePeace.peaceType)
            {
                deletePointList.Add(new POINT(i, judgePoint.Y));
                count++;
            }
            else
                break;
        }
        if (count >= DeleteCount)
        {
            //if (isDelete == false)
            //{
                for (int i = 0; i < deletePointList.Count; i++)
                {
                    tempDeletePointList.Add(new POINT(deletePointList[i].X, deletePointList[i].Y));
                }
                isDelete = true;
            //}
            //else//被っているのが無いかチェック
            //{
            //    for (int i = 0; i < deletePointList.Count; i++)
            //    {
            //        if (CheckSamePeacePoint(tempDeletePointList, deletePointList[i]) == false)
            //        tempDeletePointList.Add(new POINT(deletePointList[i].X, deletePointList[i].Y));
            //    }
            //}

            // SetDeletePoint(tempDeletePointList);
        }

        //四角形------------------------
        //左上
        //   tempDeletePointList.Clear();
        if (judgePoint.Y > 0 && judgePoint.X > 0)
        {
            if (peaceTable[new POINT(judgePoint.X, judgePoint.Y - 1)].peaceType == judgePeace.peaceType &&
               peaceTable[new POINT(judgePoint.X - 1, judgePoint.Y - 1)].peaceType == judgePeace.peaceType &&
                 peaceTable[new POINT(judgePoint.X - 1, judgePoint.Y)].peaceType == judgePeace.peaceType)
            {
                    tempDeletePointList.Add(new POINT(judgePoint.X, judgePoint.Y - 1));
                    tempDeletePointList.Add(new POINT(judgePoint.X - 1, judgePoint.Y - 1));
                    tempDeletePointList.Add(new POINT(judgePoint.X - 1, judgePoint.Y));
                //  SetDeletePoint(tempDeletePointList);
                isDelete = true;
            }
        }

        //右上
        //     tempDeletePointList.Clear();
        if (judgePoint.Y > 0 && judgePoint.X < BoardSizeX - 1)
        {
            if (peaceTable[new POINT(judgePoint.X, judgePoint.Y - 1)].peaceType == judgePeace.peaceType &&
               peaceTable[new POINT(judgePoint.X + 1, judgePoint.Y - 1)].peaceType == judgePeace.peaceType &&
                 peaceTable[new POINT(judgePoint.X + 1, judgePoint.Y)].peaceType == judgePeace.peaceType)
            {
                // tempDeletePointList.Add(new POINT(judgePoint.X, judgePoint.Y));
                tempDeletePointList.Add(new POINT(judgePoint.X, judgePoint.Y - 1));
                tempDeletePointList.Add(new POINT(judgePoint.X + 1, judgePoint.Y - 1));
                tempDeletePointList.Add(new POINT(judgePoint.X + 1, judgePoint.Y));
                //  SetDeletePoint(tempDeletePointList);
                isDelete = true;
            }
        }
        //右下
        //   tempDeletePointList.Clear();
        if (judgePoint.Y < BoardSizeY - 1 && judgePoint.X < BoardSizeX - 1)
        {
            if (peaceTable[new POINT(judgePoint.X, judgePoint.Y + 1)].peaceType == judgePeace.peaceType &&
               peaceTable[new POINT(judgePoint.X + 1, judgePoint.Y + 1)].peaceType == judgePeace.peaceType &&
                 peaceTable[new POINT(judgePoint.X + 1, judgePoint.Y)].peaceType == judgePeace.peaceType)
            {
                //     tempDeletePointList.Add(new POINT(judgePoint.X, judgePoint.Y));
                tempDeletePointList.Add(new POINT(judgePoint.X, judgePoint.Y + 1));
                tempDeletePointList.Add(new POINT(judgePoint.X + 1, judgePoint.Y + 1));
                tempDeletePointList.Add(new POINT(judgePoint.X + 1, judgePoint.Y));
                //     SetDeletePoint(tempDeletePointList);
                isDelete = true;
            }
        }
        //左下    
        //   tempDeletePointList.Clear();
        if (judgePoint.Y < BoardSizeY - 1 && judgePoint.X > 0)
        {
            if (peaceTable[new POINT(judgePoint.X, judgePoint.Y + 1)].peaceType == judgePeace.peaceType &&
               peaceTable[new POINT(judgePoint.X - 1, judgePoint.Y + 1)].peaceType == judgePeace.peaceType &&
                 peaceTable[new POINT(judgePoint.X - 1, judgePoint.Y)].peaceType == judgePeace.peaceType)
            {
                //   tempDeletePointList.Add(new POINT(judgePoint.X, judgePoint.Y));
                tempDeletePointList.Add(new POINT(judgePoint.X, judgePoint.Y + 1));
                tempDeletePointList.Add(new POINT(judgePoint.X - 1, judgePoint.Y + 1));
                tempDeletePointList.Add(new POINT(judgePoint.X - 1, judgePoint.Y));
                //  SetDeletePoint(tempDeletePointList);
                isDelete = true;
            }
        }

        //もしあれば最後に自分加える
        if (isDelete)
        {
            SetDeletePoint(tempDeletePointList);
        }
        //    tempDeletePointList.Add(new POINT(judgePoint.X, judgePoint.Y));
        //    //AddDeletePeace(deletePointList, judgePeace.peaceType);
        //}
    }

    private void SetDeletePoint(List<POINT> deleteList)
    {
        POINT pint = ReturnPoint(deleteList);
        PeaceType p = peaceTable[pint].peaceType;
        for (int i = 0; i < deleteList.Count; i++)
        {
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
            if (p == PeaceType.Pentagon) return;

            if (p != PeaceType.Square)//TODO:いったんペンタゴンはスクエアになる設定
            {
                peaceTable[pint].nextPeaceType = PeaceType.Square;
            }
            else
            {

                peaceTable[pint].nextPeaceType = PeaceType.Pentagon;

            }
        }
        catch { }
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
    private POINT ReturnPoint(List<POINT> checkList)
    {
        POINT returnPoint = checkList[0];
        for (int i = 1; i < checkList.Count; i++)
        {
            //TODO:消すポイントいったんしたに統一
            if (returnPoint.Y <= checkList[i].Y && returnPoint.X > checkList[i].X)
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
        //削除
        POINT dPoint = deletePeace.point;
        peaceTable.Remove(dPoint);


      //  List<Peace> dowPeace = new List<Peace>();
        //そのポイントから↑下にずらす
        for (int i = dPoint.Y - 1; i >= 0; i--)
        {
            POINT modificationPoint = new POINT(dPoint.X, i);
            peaceTable[new POINT(dPoint.X, i)].point = new POINT(dPoint.X, i + 1);
            Peace p = peaceTable[new POINT(dPoint.X, i)];
         //   dowPeace.Add(p);
            peaceTable.Remove(new POINT(dPoint.X, i));
            peaceTable.Add(new POINT(dPoint.X, i + 1), p);
            ResetHoldPeacePosition(p);
        }
        //ずらした後の全てのピースを消す
        //for(int i=0;i< dowPeace.Count; i++)
        //{
        //    JudgePiece(dowPeace[i]);
        //}


        //Peace newPeace
        //peaceTable.Add(new POINT(),);
        POINT setPoint = new POINT(dPoint.X, 0);
        Peace newPeace = Instantiate(peacePrefab).GetComponent<Peace>();
        newPeace.peaceType = (PeaceType)Random.Range(0, (int)PeaceType.Orange);
        newPeace.transform.SetParent(peaceParent.transform, false);
        newPeace.GetComponent<RectTransform>().anchoredPosition = new Vector2(stratPosition.X + setPoint.X * onePeaceSize, stratPosition.Y - setPoint.Y * onePeaceSize);
        newPeace.point = setPoint;
        newPeace.SetSprite(PeaceSprites[(int)newPeace.peaceType]);
        peaceTable.Add(newPeace.point, newPeace);

        JudgePiece(newPeace);
        JudgeGameClear();
        //上追加に

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


    //private void AddDeletePeace(List<POINT> addPoint,PeaceType peaceType)
    //{
    //    for (int i = 0; i < addPoint.Count; i++)
    //    {
    //        for (int j = 0; j < peaceJudgeStructList.Count; j++)
    //        {
    //            if (addPoint[i].X == peaceJudgeStructList[j].point.X && addPoint[i].Y == peaceJudgeStructList[j].point.Y)
    //            {
    //                break;
    //            }
    //            else if (j == peaceJudgeStructList.Count - 1)//最後まで被らなかったら追加
    //            {
    //              //  peaceJudgeStructList.Add(new PeaceJudgeStruct { addPoint[i], peaceType });
    //            }
    //        }
    //    }
    //}


}
