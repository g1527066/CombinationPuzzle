using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PeaceManager : SingletonMonoBehaviour<PeaceManager>
{

    //------設定用
    public const int BoardSizeX = 10;
    public const int BoardSizeY = 6;


    Dictionary<POINT, Peace> peaceTable = new Dictionary<POINT, Peace>();
    public Dictionary<POINT, Peace> GetPeaceTabel
    {
        get { return peaceTable; }
    }

    [HideInInspector]
    public List<Peace> stockPeaceList = new List<Peace>();
    //移動用の変数
    public Peace nowHoldPeace = null;


    [SerializeField]
    GenerationCollision generationCollirion = null;


    void Start()
    {
        PeaceGenerator.Instance.AllGeneration();
        PeaceOperator.Instance.ReSetAllPosition(peaceTable);
    }


    public void MoveHoldPeace(Vector2 difference,Vector2 mousePosition)
    {
        if (nowHoldPeace == null) return;
        PeaceOperator.Instance.MovePeace(difference * 1920 / Screen.width, nowHoldPeace);
        //if (hitPeace == null) return;


        //
        //generationCollirion.SetCollisionActive(true);

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10, 100);
        PointCollision pointCollision = null;

        //画面外想定してない
        //ポイントがあるなら取得

        if (null != hit.collider.gameObject.GetComponent<PointCollision>())
        {
            pointCollision = hit.collider.gameObject.GetComponent<PointCollision>();
            //そこが今と違かったら設定
            if (nowHoldPeace.point.X != pointCollision.point.X || nowHoldPeace.point.Y != pointCollision.point.Y)
            {
                //もしそこにピースがあるなら交換
                if (peaceTable.ContainsKey(pointCollision.point) == true)
                {
                    AudioManager.Instance.PlaySE("testTrade");
         

                    POINT p = nowHoldPeace.point;
                    PeaceOperator.Instance.TradeDictionaryPeace(peaceTable, nowHoldPeace, peaceTable[pointCollision.point]);
                    PeaceOperator.Instance.ResetPosition(peaceTable[p]);//エラー　無し

                    if (PeaceJudger.Instance.CheckPossibleDown(peaceTable, peaceTable[p])&&peaceTable[p].IsDuringFall==false)
                        PeaceOperator.Instance.AddDrop(peaceTable[p]);//これだと先に下があってもやってしまう？？動けるかの判定も先にやった方がいい疑惑
                    else
                        PeaceJudger.Instance.JudgePeace(peaceTable, peaceTable[p], nowHoldPeace);
                    //                    }
                  //  Debug.Log("ピースがあります");
                }
                else//行先にピースがないなら番号のみ交換
                {
                    POINT oldPoint = nowHoldPeace.point;
                    peaceTable.Remove(nowHoldPeace.point);
                    nowHoldPeace.point = new POINT(pointCollision.point.X, pointCollision.point.Y);
                    peaceTable.Add(nowHoldPeace.point, nowHoldPeace);

                    //元の位置の上にピースがあったら落ちるように指示する
                    if (oldPoint.X != nowHoldPeace.point.X )
                    {
                        for (int countY = oldPoint.Y - 1; countY >= 0; countY--)
                        {
                            if (peaceTable.ContainsKey(new POINT(oldPoint.X, countY)) == true)
                            {
                                PeaceOperator.Instance.AddDrop(peaceTable[new POINT(oldPoint.X, countY)]);
                            }
                            else break;
                        }
                    }
                }
            }

        }
    }

    public void SetHoldPeace(PointCollision pointCollision)
    {

        if (peaceTable.ContainsKey(pointCollision.point) == true)
        {
            if (peaceTable[pointCollision.point].isMatching == true)
                return;
        }
        else
            return;

        //落下処理消す
        peaceTable[pointCollision.point].IsDuringFall = false;
        nowHoldPeace = peaceTable[pointCollision.point];

        //nowHoldPeace.GetComponent<BoxCollider2D>().enabled = false;
    }


    public void ReleasePeace(Vector2 pos)
    {
        if (nowHoldPeace == null) return;

        // generationCollirion.SetCollisionActive(true);

        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10, 100);
        PointCollision pointCollision = null;

        //画面外想定してない
        //ポイントがあるなら取得

        if (null != hit.collider.gameObject.GetComponent<PointCollision>())
        {
            pointCollision = hit.collider.gameObject.GetComponent<PointCollision>();
            //そこが今と違かったら設定
            if (nowHoldPeace.point.X != pointCollision.point.X || nowHoldPeace.point.Y != pointCollision.point.Y)
            {
                peaceTable.Remove(nowHoldPeace.point);
                nowHoldPeace.point = new POINT(pointCollision.point.X, pointCollision.point.Y);
                peaceTable.Add(nowHoldPeace.point, nowHoldPeace);
            }

            //  PeaceOperator.Instance.ResetPosition(nowHoldPeace);
            //位置リセット
            //下に一つでも空白があるなら、落下させる
            //if (PeaceJudger.Instance.CheckPossibleDown(peaceTable, nowHoldPeace))
            //    PeaceOperator.Instance.AddDrop(nowHoldPeace);

        }

        //generationCollirion.SetCollisionActive(false);



        //nowHoldPeace.GetComponent<BoxCollider2D>().enabled = true;
        PeaceOperator.Instance.ResetPosition(nowHoldPeace);

        //もし下に間があるなら
        //落下付け、無ければ判定
        if (PeaceJudger.Instance.CheckPossibleDown(peaceTable, nowHoldPeace))
            PeaceOperator.Instance.AddDrop(nowHoldPeace);//これだと先に下があってもやってしまう？？動けるかの判定も先にやった方がいい疑惑
        else
            PeaceJudger.Instance.JudgePeace(peaceTable, nowHoldPeace, nowHoldPeace);
        nowHoldPeace = null;
    }


    private void SetChangePoint(Peace nowPeace, POINT changePoint, bool isJugde = false)
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
            if (isJugde == false)
                StartCoroutine(ExecuteNextFrame(nowPeace, changePoint));
            //SetChangePoint(nowPeace,changePoint);//= (nowPeace )=>SetChangePoint(nowPeace, changePoint);

            Debug.Log("今 x=" + nowPeace.point.X + "  y=" + nowPeace.point.Y);
            Debug.Log("changePoint X=" + changePoint.X + " Y=" + changePoint.Y);
            //  Debug.LogError("すでにピースが存在しています");
            Debug.Log("すでにピースが存在しています");
        }
    }

    //actionがわからなかった、、、
    private IEnumerator ExecuteNextFrame(Peace nowPeace, POINT changePoint)
    {
        yield return null;
        SetChangePoint(nowPeace, changePoint, true);
    }


    public Peace ChangeForm(Peace peace)
    {
        return PeaceGenerator.Instance.ChangeNextForm(peaceTable, peace);
    }



    public void DebugButtone()
    {
        Debug.Log("--------Debug----------------------");

        Debug.Log("マス数=" + peaceTable.Count);
        // Debug.Log("削除リスト数=" + DeletionTargetList.Count);

       foreach(var p in PeaceManager.Instance.GetPeaceTabel )
        {
            Debug.Log("X="+p.Value.point.X+"   Y=" + p.Value.point.Y);

        }

        Debug.Log("------------------------------");


    }
}
