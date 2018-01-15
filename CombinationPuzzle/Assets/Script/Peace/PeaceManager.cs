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

    //ToDo:判定に入らないようにする！

    ////各機能
    //[SerializeField]
    //PeaceJudger peaceJudger = null;
    //[SerializeField]
    //PeaceGenerator peaceGenerator = null;

    //[SerializeField]
    //PeaceOperator peaceOperator = null;
    //public PeaceOperator PeaceOperator
    //{
    //    get { return peaceOperator; }
    //}

    void Start()
    {
        PeaceGenerator.Instance.AllGeneration();
        PeaceOperator.Instance.ReSetAllPosition(peaceTable);
    }

    /// <summary>
    /// 消える組み合わせだった場合false
    /// </summary>
    /// <param name="newPeace">ここ基準判定</param>
    /// <returns></returns>
    private bool StartJudge(POINT p, PeaceColor peaceType)
    {
        //上
        int count = 0;
        //  POINT p = newPeace.point;
        for (int i = p.Y - 1; i >= 0; i--)
        {
            if (peaceTable[new POINT(p.X, i)].peaceColor == peaceType)
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
            if (peaceTable[new POINT(i, p.Y)].peaceColor == peaceType)
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
                    //if (hitPeace.IsDuringFall)//落下時、触っているのが下の場合
                    //{
                    //    //下に行くようにする、もしその下に空きがなければ予約するように
                    //    if (peaceTable.ContainsKey(new POINT(nowHoldPeace.point.X, nowHoldPeace.point.Y + 1)))
                    //    {
                    //        PeaceOperator.Instance.AddDrop(hitPeace);//ここらえん変える！！
                    //    }
                    //    else
                    //    {
                    //        PeaceGenerator.Instance.SetPeaceList(hitPeace, new POINT(nowHoldPeace.point.X, nowHoldPeace.point.Y + 1));
                    //        PeaceOperator.Instance.ResetPosition(hitPeace);
                    //        if (PeaceJudger.Instance.CheckPossibleDown(peaceTable, hitPeace))
                    //            PeaceOperator.Instance.AddDrop(hitPeace);
                    //        else
                    //            hitPeace.IsDuringFall = false;
                    //    }
                    //}
                    //else
                    //{

                    POINT p = nowHoldPeace.point;
                    PeaceOperator.Instance.TradeDictionaryPeace(peaceTable, nowHoldPeace, peaceTable[pointCollision.point]);
                    PeaceOperator.Instance.ResetPosition(peaceTable[p]);

                    if (PeaceJudger.Instance.CheckPossibleDown(peaceTable, peaceTable[p])&&peaceTable[p].IsDuringFall==false)
                        PeaceOperator.Instance.AddDrop(peaceTable[p]);//これだと先に下があってもやってしまう？？動けるかの判定も先にやった方がいい疑惑
                    else
                        PeaceJudger.Instance.JudgePeace(peaceTable, peaceTable[p], nowHoldPeace);
                    //                    }
                  //  Debug.Log("ピースがあります");
                }
                else//行先にピースがないなら番号のみ交換
                {
                    Debug.Log("ピースがありません");
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

            //  PeaceOperator.Instance.ResetPosition(nowHoldPeace);
            //位置リセット
            //下に一つでも空白があるなら、落下させる
            //if (PeaceJudger.Instance.CheckPossibleDown(peaceTable, nowHoldPeace))
            //    PeaceOperator.Instance.AddDrop(nowHoldPeace);

        }

        //  generationCollirion.SetCollisionActive(false);



        //



        //前回とピースがちがかったら入れ替え
        //if (hitPeace != null)
        //{
            //  Debug.Log("前回と違います X="+hitPeace.point.X+" Y=" + hitPeace.point.Y);
            //if (hitPeace.IsDuringFall)//落下時、触っているのが下の場合
            //{
            //    //下に行くようにする、もしその下に空きがなければ予約するように
            //    if (peaceTable.ContainsKey(new POINT(nowHoldPeace.point.X, nowHoldPeace.point.Y + 1)))
            //    {
            //        PeaceOperator.Instance.AddDrop(hitPeace);//ここらえん変える！！
            //    }
            //    else
            //    {
            //        PeaceGenerator.Instance.SetPeaceList(hitPeace, new POINT(nowHoldPeace.point.X, nowHoldPeace.point.Y + 1));
            //        PeaceOperator.Instance.ResetPosition(hitPeace);
            //        if (PeaceJudger.Instance.CheckPossibleDown(peaceTable, hitPeace))
            //            PeaceOperator.Instance.AddDrop(hitPeace);
            //        else
            //            hitPeace.IsDuringFall = false;
            //    }
            //}
            //else
            //{
            //    PeaceOperator.Instance.TradeDictionaryPeace(peaceTable, nowHoldPeace, hitPeace);
            //    PeaceOperator.Instance.ResetPosition(hitPeace);

            //    if (PeaceJudger.Instance.CheckPossibleDown(peaceTable, hitPeace))
            //        PeaceOperator.Instance.AddDrop(hitPeace);//これだと先に下があってもやってしまう？？動けるかの判定も先にやった方がいい疑惑
            //    else
            //        PeaceJudger.Instance.JudgePeace(peaceTable, hitPeace, nowHoldPeace);
            //}
    //    }
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

    public GameObject GetPeacePrefab()
    {
        return PeaceGenerator.Instance.peacePrefab;
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




    public void ChangePeaceClass(Peace oldPeace, Peace newPeace)
    {
        Peace old = oldPeace;
        peaceTable.Remove(oldPeace.point);
        peaceTable.Add(newPeace.point, newPeace);
    }

    public Peace ChangeForm(Peace peace)
    {
        return PeaceGenerator.Instance.ChangeNextForm(peaceTable, peace);
    }



    //ピースを削除し、上から追加、ずらす
    //TODO:いったん一瞬で詰める
    public void DeletePeace(Peace deletePeace)
    {
        PeaceJudger.Instance.DeletePeace(peaceTable, deletePeace);
    }


    /// <summary>
    /// 削除されるピースと同じX軸の上に追加
    /// </summary>
    /// <param name="deletePeace"></param>
    public void AddToTopPeace(Peace deletePeace)
    {
        PeaceGenerator.Instance.AddToTopPeace(peaceTable, deletePeace);
        //場所移動
        PeaceOperator.Instance.ResetPosition(deletePeace);
        //落下付与する
        PeaceOperator.Instance.AddDrop(deletePeace);
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
                    //peaceTable[peace.point].IsDuringFall = true;
                    ////そのポイントより下のポイントで一個でも空きマスがあればポイントが無ければ動かす
                    ////番号は途中から変える?
                    ////  new Vector2(stratPosition.X + peace.point.X * onePeaceSize, stratPosition.Y - peace.point.Y * onePeaceSize);
                    //Sequence sequence = DOTween.Sequence().
                    //    OnStart(() =>
                    //    {
                    //        peaceTable[peace.point].RectTransform.DOLocalMove(new Vector2(stratPosition.X + peace.point.X * onePeaceSize, stratPosition.Y - (peace.point.Y + 1) * onePeaceSize), 1);//移動
                    //    }).InsertCallback(0.5f, () => SetChangePoint(peace, new POINT(peace.point.X, peace.point.Y + 1)))
                    //    .OnComplete(() =>
                    //        CheckFallingPeace(peace)
                    //    );//ここで半分の時番号変える、もし交換して何も無いなら終了

                    //return;

                }
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
            //  if (peaceTable[key].peaceType == PeaceColor.Pentagon)
            count++;
        }
        if (count >= 2)
            GameSystem.Instance.Clear();
    }




    public void DebugButtone()
    {
        Debug.Log("--------Debug----------------------");

        Debug.Log("マス数=" + peaceTable.Count);
        // Debug.Log("削除リスト数=" + DeletionTargetList.Count);

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
