﻿using System.Collections;
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


    public RectTransform RectTransform
    {
        get
        {
            if(rectTransform == null)
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


    // Update is called once per frame
    void Update()
    {
        if (isMatching)
        {
            deleteTime += Time.deltaTime;
            flashingTime += Time.deltaTime;
            if (flashingTime > GameSystem.I.flashingTime)
            {
                flashingTime = 0;

                if (isNoColor)
                    this.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                else
                    this.GetComponent<UnityEngine.UI.Image>().color = Color.clear;

                isNoColor = !isNoColor;
            }

            if (deleteTime > GameSystem.I.DeleteTime)
            {
                isMatching = false;
                this.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                PeaceJudger.I.DeletePeace(PeaceManager.I.GetPeaceTabel, this);
                //AudioManager.I.PlaySound("DeletePeace");
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
      //  if (IsDuringFall == true) return;
        IsDuringFall = true;
        downPosition = yPosition;
        StartCoroutine(DownMovePeace());//erorrたくさん消した時？
        if (PeaceGenerator.I.SetPeaceList(this, new POINT(point.X, point.Y + 1)) == false)
        {
            //    Debug.Log("目標地点=" + yPosition);
            StartCoroutine(NextCheck());
        }
    }


    private IEnumerator DownMovePeace()
    {

        while (IsDuringFall)
        {
            RectTransform.anchoredPosition -= new Vector2(0, PeaceOperator.I.downSpeed);

            if (downPosition >= RectTransform.anchoredPosition.y)
            {
                // Debug.Log("セットしなおし");
                //Debug.Break();
                //場所戻す
                PeaceOperator.I.ResetPosition(this);
                //審査して下がなければ落ちるように設定
                if (PeaceJudger.I.CheckPossibleDown(PeaceManager.I.GetPeaceTabel, this))
                {
                    //ポイント更新//もし無理だったら次のフレームで審査する
                    if (PeaceGenerator.I.SetPeaceList(this, new POINT(point.X, point.Y + 1)) == false)
                    {
                        yield return NextCheck();
                    }
                    //新しく座標設定
                    //   Debug.Log("前downPosition" + downPosition);
                    downPosition = PeaceOperator.I.DownPosition(new POINT(point.X, point.Y));
                    // Debug.Log("現在downPosition" + downPosition);

                }
                else
                {
                    IsDuringFall = false;
                    PeaceJudger.I.DownJudge(this);
                    break;
                }
            }

            yield return null;
        }
    }

    private IEnumerator NextCheck()
    {
        while (true&&IsDuringFall==true)
        {
             Debug.Log("NextCheckコルーチン内");
            if (PeaceGenerator.I.SetPeaceList(this, new POINT(point.X, point.Y + 1)) == true)
            {
                 // Debug.Log("NextCheckコルーチン内　見つかりました  X="+point.X+" Y="+point.Y);
                break;
            }
            yield return null;
        }

    }
}
