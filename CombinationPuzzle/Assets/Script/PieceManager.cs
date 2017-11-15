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


public class PieceManager : MonoBehaviour
{

    struct PeaceJudgeStruct
    {
        public PeaceType peaceType;
        public POINT point;
    }

    const int BoardSizeX = 10;
    const int BoardSizeY = 6;
    Peace[,] peaceList = new Peace[BoardSizeY, BoardSizeX];
    [SerializeField]
    GameObject peacePrefab = null;

    [SerializeField]
    GameObject peaceParent = null;

    [SerializeField]
    List<Sprite> PeaceSprites = new List<Sprite>();

  //  List<POINT> totalDeleteList = new List<POINT>();

    List<PeaceJudgeStruct> peaceJudgeStructList = new List<PeaceJudgeStruct>();

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
                newPeace.SetPeaceManager = this;
                peaceList[i, j] = newPeace;
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

    public void MoveHoldPeace(Vector2 difference, RaycastHit2D hit)
    {
        nowHoldPeace.GetComponent<RectTransform>().anchoredPosition += difference;

        Peace nowPeace = null;

        try
        {
            nowPeace = hit.collider.gameObject.GetComponent<Peace>();

        }
        catch { }



        //前回とピースがちがかったら入れ替え
        if (nowPeace != null && nowHoldPeace != nowPeace)
        {
            POINT savepoint = nowHoldPeace.point;
            nowHoldPeace.point = nowPeace.point;
            nowPeace.point = savepoint;
            ResetHoldPeacePosition(nowPeace);
            //判定処理
            peaceJudgeStructList.Clear();
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

        nowHoldPeace.GetComponent<BoxCollider2D>().enabled = true;
        ResetHoldPeacePosition(nowHoldPeace);
    }


    private void JudgePiece(Peace judgePeace)
    {
        //プレイヤーの持っているのは除外
        POINT judgePoint = judgePeace.point;

        List<POINT> deletePointList = new List<POINT>();

        List<POINT> tempDeletePointList = new List<POINT>();

        //上下調査------------------------
        int count = 1;
        //  POINT generatePeacePoint = new POINT(judgePeace.point.X, judgePeace.point.Y);
        //上へ
        for (int i = judgePoint.Y - 1; i >= 0; i--)
        {
            if (peaceList[i, judgePoint.X].peaceType == judgePeace.peaceType)
            {
                count++;
                tempDeletePointList.Add(new POINT(judgePoint.X, i));
            }
            else
                break;
        }
        for (int i = judgePoint.Y + 1; i < BoardSizeY; i++)
        {
            if (peaceList[i, judgePoint.X].peaceType == judgePeace.peaceType)
            {
                count++;
                tempDeletePointList.Add(new POINT(judgePoint.X, i));
                //generatePeacePoint.Y = i;
            }
            else
                break;
        }
        if (count >= DeleteCount)
        {
            for (int i = 0; i < tempDeletePointList.Count; i++)
            {
                deletePointList.Add(new POINT(tempDeletePointList[i].X, tempDeletePointList[i].Y));
            }
        }

        //左右調査------------------------
        count = 1;
        //右
        tempDeletePointList.Clear();
        for (int i = judgePoint.X + 1; i < BoardSizeX; i++)
        {
            if (peaceList[judgePoint.Y, i].peaceType == judgePeace.peaceType)
            {
                tempDeletePointList.Add(new POINT(i, judgePoint.Y));
                count++;
            }
            else
                break;
        }
        for (int i = judgePoint.X - 1; i >= 0; i--)
        {
            if (peaceList[judgePoint.Y, i].peaceType == judgePeace.peaceType)
            {
                tempDeletePointList.Add(new POINT(i, judgePoint.Y));
                count++;
            }
            else
                break;
        }
        if (count >= DeleteCount)
        {
            for (int i = 0; i < tempDeletePointList.Count; i++)
            {
                deletePointList.Add(new POINT(tempDeletePointList[i].X, tempDeletePointList[i].Y));
            }
        }

        //四角形------------------------
        //左上
        if (judgePoint.Y > 0 && judgePoint.X > 0)
        {
            if (peaceList[judgePoint.Y - 1, judgePoint.X].peaceType == judgePeace.peaceType &&
               peaceList[judgePoint.Y - 1, judgePoint.X - 1].peaceType == judgePeace.peaceType &&
                 peaceList[judgePoint.Y, judgePoint.X - 1].peaceType == judgePeace.peaceType)
            {
                deletePointList.Add(new POINT(judgePoint.X, judgePoint.Y - 1));
                deletePointList.Add(new POINT(judgePoint.X - 1, judgePoint.Y - 1));
                deletePointList.Add(new POINT(judgePoint.X - 1, judgePoint.Y));
            }
        }

        //右上
        if (judgePoint.Y > 0 && judgePoint.X < BoardSizeX)
        {
            if (peaceList[judgePoint.Y - 1, judgePoint.X].peaceType == judgePeace.peaceType &&
               peaceList[judgePoint.Y - 1, judgePoint.X + 1].peaceType == judgePeace.peaceType &&
                 peaceList[judgePoint.Y, judgePoint.X + 1].peaceType == judgePeace.peaceType)
            {
                deletePointList.Add(new POINT(judgePoint.X, judgePoint.Y - 1));
                deletePointList.Add(new POINT(judgePoint.X + 1, judgePoint.Y - 1));
                deletePointList.Add(new POINT(judgePoint.X + 1, judgePoint.Y));
            }
        }
        //右下
        if (judgePoint.Y < BoardSizeY && judgePoint.X < BoardSizeX)
        {
            if (peaceList[judgePoint.Y + 1, judgePoint.X].peaceType == judgePeace.peaceType &&
               peaceList[judgePoint.Y + 1, judgePoint.X + 1].peaceType == judgePeace.peaceType &&
                 peaceList[judgePoint.Y, judgePoint.X + 1].peaceType == judgePeace.peaceType)
            {
                deletePointList.Add(new POINT(judgePoint.X, judgePoint.Y + 1));
                deletePointList.Add(new POINT(judgePoint.X + 1, judgePoint.Y + 1));
                deletePointList.Add(new POINT(judgePoint.X + 1, judgePoint.Y));
            }
        }
        //左下    
        if (judgePoint.Y < BoardSizeY && judgePoint.X > 0)
        {
            if (peaceList[judgePoint.Y + 1, judgePoint.X].peaceType == judgePeace.peaceType &&
               peaceList[judgePoint.Y + 1, judgePoint.X - 1].peaceType == judgePeace.peaceType &&
                 peaceList[judgePoint.Y, judgePoint.X - 1].peaceType == judgePeace.peaceType)
            {
                deletePointList.Add(new POINT(judgePoint.X, judgePoint.Y + 1));
                deletePointList.Add(new POINT(judgePoint.X - 1, judgePoint.Y + 1));
                deletePointList.Add(new POINT(judgePoint.X - 1, judgePoint.Y));
            }
        }

        //もしあれば最後に自分加える
        if (deletePointList.Count > 0)
        {
            deletePointList.Add(new POINT(judgePoint.X, judgePoint.Y));
            AddDeletePeace(deletePointList, judgePeace.peaceType);
        }
    }


    private void AddDeletePeace(List<POINT> addPoint,PeaceType peaceType)
    {

        for (int i = 0; i < addPoint.Count; i++)
        {
            for (int j = 0; j < peaceJudgeStructList.Count; j++)
            {
                if (addPoint[i].X == peaceJudgeStructList[j].point.X && addPoint[i].Y == peaceJudgeStructList[j].point.Y)
                {
                    break;
                }
                else if (j == peaceJudgeStructList.Count - 1)//最後まで被らなかったら追加
                {
                  //  peaceJudgeStructList.Add(new PeaceJudgeStruct { addPoint[i], peaceType });
                }
            }
        }
    }


}
