using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PeaceColor
{
    Red,
    Blue,
    Yellow,
    Green,
    Perple,
    Orange,
    None,
}

public enum PeaceForm
{
    Triangle,
    Square,
    Pentagon,
    None,
}

public abstract class Peace : MonoBehaviour
{
    public PeaceColor peaceColor;

    public abstract PeaceForm GetPeaceForm
    {
        get;
    }

    public abstract PeaceColor GetPeaceColor
    {
        get;
    }
    public POINT point;

    protected float deleteTime = 0f;
    protected float flashingTime = 0f;
    protected bool isNoColor = false;
    public PeaceForm nextPeaceForm = PeaceForm.None;
    public abstract PeaceForm GetNextPeaceForm
    {
        get;
    }

    public bool isMatching = false;
    public bool IsDuringFall = false;
    RectTransform rectTransform;
    public float downPosition = 0f;
    int particleCounter = 0;
    int particleWave = 2;

    public RectTransform RectTransform
    {
        get
        {
            if (rectTransform == null)
            {
                rectTransform = transform as RectTransform;
            }
            return rectTransform;
        }
    }

    private void Awake()
    {
        Initialization();
    }

    void Update()
    {
        if (isMatching&&GameSystem.Instance.GetTimer.StopTimeFlag==false)
        {
            deleteTime += Time.deltaTime;
            flashingTime += Time.deltaTime;
            if (flashingTime > GameSystem.Instance.GetTimer.flashingTime)
            {
                flashingTime = 0;

                if (isNoColor)
                    this.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                else
                    this.GetComponent<UnityEngine.UI.Image>().color = Color.clear;

                isNoColor = !isNoColor;
            }

            if (deleteTime > GameSystem.Instance.GetTimer.DeleteTime)
            {
                Debug.Log("マッチ終了　X="+point.X+"  Y="+point.Y);
                isMatching = false;
                this.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                PeaceJudger.Instance.DeletePeace(PeaceManager.Instance.GetPeaceTabel, this);
            }
        }
    }

    public void SetSprite(Sprite setSptrite)
    {
        this.GetComponent<UnityEngine.UI.Image>().sprite = setSptrite;
    }

    //消して次の追加する前の初期化用
    public void Initialization()
    {
        deleteTime = 0f;
        flashingTime = 0f;
        isNoColor = false;
        nextPeaceForm = PeaceForm.None;
        isMatching = false;
    }

    public void SetNewType()
    {
        peaceColor = (PeaceColor)Random.Range(0, (int)PeaceColor.None);
    }

    public void SetDown(float yPosition)
    {
        //    Debug.Log("最初の位置" + RectTransform.anchoredPosition);
        //    Debug.Log("目標地点=" + yPosition);
         if (IsDuringFall == true) return;
        IsDuringFall = true;
        downPosition = yPosition;
        StartCoroutine(DownMovePeace());//erorrたくさん消した時？
        if (PeaceGenerator.Instance.SetPeaceList(this, new POINT(point.X, point.Y + 1)) == false)
        {
            //    Debug.Log("目標地点=" + yPosition);
            StartCoroutine(NextCheck());//まだ下にいるかもしれないから、移動はするがPOINTは変えない
        }
    }

    public IEnumerator DownMovePeace()
    {
        while (IsDuringFall)
        {
            while (GameSystem.Instance.GetTimer.StopTimeFlag == true)
            {
                yield return null;
            }
            RectTransform.anchoredPosition -= new Vector2(0, PeaceOperator.Instance.downSpeed);
            particleCounter++;
            if (particleCounter>particleWave)
            {
                PeaceGenerator.Instance.FallParticle(this.transform.localPosition);
                particleCounter = 0;
            }
            if (downPosition >= RectTransform.anchoredPosition.y)
            {
                // Debug.Log("セットしなおし");
                //Debug.Break();
                //場所戻す
                PeaceOperator.Instance.ResetPosition(this);
                //審査して下がなければ落ちるように設定
                if (PeaceJudger.Instance.CheckPossibleDown(PeaceManager.Instance.GetPeaceTabel, this))
                {
                    //ポイント更新//もし無理だったら次のフレームで審査する
                    if (PeaceGenerator.Instance.SetPeaceList(this, new POINT(point.X, point.Y + 1)) == false)
                    {
                        yield return NextCheck();
                    }
                    //新しく座標設定
                    //   Debug.Log("前downPosition" + downPosition);
                    downPosition = PeaceOperator.Instance.DownPosition(new POINT(point.X, point.Y));
                    // Debug.Log("現在downPosition" + downPosition);
                }
                else
                {
                    IsDuringFall = false;
                    PeaceJudger.Instance.DownJudge(this);
                    PeaceJudger.Instance.CheckGameOver();
                    break;
                }
            }
            yield return null;
        }
    }

    private IEnumerator NextCheck()
    {
        int test = 0;//削除
        while (IsDuringFall == true)
        {
            test++;
            if (test > 100)
                Debug.Log("NextCheckコルーチン内　　X=" + point.X + "  Y=" + point.Y);

            bool tes = PeaceManager.Instance.GetPeaceTabel.ContainsKey(new POINT(point.X, point.Y + 1));
            //if(tes==true)
            //{
            //    Debug.Log("X="+point.X+ "  Y=" + point.Y +"の下有り");
            //}
            //else
            //{
            //    Debug.Log("X=" + point.X + "  Y=" + point.Y + "の下無し");
            //}

            if (PeaceGenerator.Instance.SetPeaceList(this, new POINT(point.X, point.Y + 1)) == true)
            {
                // Debug.Log("NextCheckコルーチン内　見つかりました  X="+point.X+" Y="+point.Y);
                break;
            }
            yield return null;
        }
    }
}
