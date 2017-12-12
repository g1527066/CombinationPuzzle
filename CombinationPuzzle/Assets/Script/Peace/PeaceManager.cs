using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


public class PeaceManager : MonoBehaviour
{
    public static PeaceManager I = null;

    //------設定用
    public const int BoardSizeX = 10;
    public const int BoardSizeY = 6;


    Dictionary<POINT, Peace> peaceTable = new Dictionary<POINT, Peace>();
    public Dictionary<POINT, Peace> GetPeaceTabel
    {
        get { return peaceTable; }
    }


    List<Peace> stockPeaceList = new List<Peace>();
    //移動用の変数
    public Peace nowHoldPeace = null;
    
    //ToDo:判定に入らないようにする！


    //各機能
    [SerializeField]
    PeaceJudger peaceJudger = null;
    [SerializeField]
    PeaceGenerator peaceGenerator = null;
    
    [SerializeField]
    PeaceOperator peaceOperator = null;
    public PeaceOperator PeaceOperator
    {
        get { return peaceOperator; }
    }


    [SerializeField]
    GameObject peaceParent = null;



    //-------test
    [SerializeField]
    UnityEngine.UI.Text text = null;


    //public void ChangeDrop()
    //{

    //    Sequence sequence = DOTween.Sequence().
    //        Append(

    //            peaceTable[new POINT(0, 0)].RectTransform.DOLocalMove(
    //               new Vector3(stratPosition.X + 0 * onePeaceSize, stratPosition.Y - 1 * onePeaceSize, 0), 0.1f)
    //        )
    //        .Append(
    //        peaceTable[new POINT(0, 0)].RectTransform.DOLocalMove(
    //               new Vector3(stratPosition.X + 0 * onePeaceSize, stratPosition.Y - 2 * onePeaceSize, 0), 0.1f)

    //        )
    //        .Append(
    //        peaceTable[new POINT(0, 0)].RectTransform.DOLocalMove(
    //               new Vector3(stratPosition.X + 0 * onePeaceSize, stratPosition.Y - 3 * onePeaceSize, 0), 0.1f)

    //        )
    //        .InsertCallback(2, () => text.text = "down");
    //    testSpriteCount++;
  //  }
    //まとめてポイント代入できなかったっけ？、new必要？

    // Use this for initialization
    void Start()
    {
        I = this;
        peaceGenerator.AllGeneration(peaceTable, peaceJudger);
        peaceOperator.ReSetAllPosition(peaceTable);
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

    public void MoveHoldPeace(Vector2 difference, Peace hitPeace)
    {
        if (nowHoldPeace == null) return;
        peaceOperator.MovePeace(difference,nowHoldPeace);

        if (hitPeace == null) return;

        //前回とピースがちがかったら入れ替え
        if (hitPeace != null)
        {
            AudioManager.I.PlaySound("Trade");//一旦

            peaceOperator.TradeDictionaryPeace(peaceTable,nowHoldPeace,hitPeace);
            peaceOperator.ResetPosition(hitPeace);
            peaceJudger.JudgePeace(peaceTable, hitPeace,nowHoldPeace);
        }
    }

    public void SetHoldPeace(Peace peace)
    {
        nowHoldPeace = peace;
        nowHoldPeace.GetComponent<BoxCollider2D>().enabled = false;
    }

  
    public void ReleasePeace()
    {
        if (nowHoldPeace == null) return;
        nowHoldPeace.GetComponent<BoxCollider2D>().enabled = true;
        peaceOperator.ResetPosition(nowHoldPeace);
        peaceJudger.JudgePeace(peaceTable,nowHoldPeace,nowHoldPeace);
        nowHoldPeace = null;
    }

    public GameObject GetPeacePrefab()
    {
        return peaceGenerator.peacePrefab;      
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


 

    public void ChangePeaceClass(Peace oldPeace,Peace newPeace)
    {
        Peace old = oldPeace;
        peaceTable.Remove(oldPeace.point);
        peaceTable.Add(newPeace.point,newPeace);
    }

    public void ChangeForm(Peace peace)
    {
        peaceGenerator.ChangeForm(peaceTable,peace);
    }



    //ピースを削除し、上から追加、ずらす
    //TODO:いったん一瞬で詰める
    public void DeletePeace(Peace deletePeace)
    {
       peaceJudger.DeletePeace(peaceTable,deletePeace);
    }


    /// <summary>
    /// 削除されるピースと同じX軸の上に追加
    /// </summary>
    /// <param name="deletePeace"></param>
    public void AddToTopPeace(Peace deletePeace)
    {
       peaceGenerator.AddToTopPeace(peaceTable,deletePeace);
        //場所移動
        peaceOperator.ResetPosition(deletePeace);
        //落下付与する
        peaceOperator.AddDrop(deletePeace);
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
            GameSystem.I.Clear();
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
