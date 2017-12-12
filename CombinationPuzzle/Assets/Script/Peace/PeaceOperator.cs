﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class PeaceOperator : MonoBehaviour
{

   public static PeaceOperator I=null;

    [SerializeField]
    POINT stratPosition = new POINT(-718, 290);
  public  const int onePeaceSize = 148;

    [SerializeField]
    public float downSpeed = 1.5f;


    // Use this for initialization
    void Awake()
    {
        I = this;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ReSetAllPosition(Dictionary<POINT, Peace> dictionary)
    {
        foreach (var value in dictionary.Values)
        {
            ResetPosition(value);
        }
    }

    public void ResetPosition(Peace peace)
    {
        peace.GetComponent<RectTransform>().anchoredPosition = new Vector2(stratPosition.X + peace.point.X * onePeaceSize, stratPosition.Y - peace.point.Y * onePeaceSize);
    }

    public void MovePeace(Vector2 difference, Peace peace)
    {
        peace.GetComponent<RectTransform>().anchoredPosition += difference;
    }

    public void TradeDictionaryPeace(Dictionary<POINT, Peace> dictionary, Peace p1, Peace p2)
    {
        POINT savePoint = p1.point;
        p1.point = p2.point;
        p2.point = savePoint;

        //保存
        Peace addPeace1 = p1;
        Peace addPeace2 = p2;
        dictionary.Remove(p1.point);
        dictionary.Remove(p2.point);
        dictionary.Add(addPeace1.point, addPeace1);
        dictionary.Add(addPeace2.point, addPeace2);
    }

    public void AddDrop(Peace peace)
    {
        peace.SetDown(DownPosition(new POINT(peace.point.X, peace.point.Y + 1)));
    }

    public float DownPosition(POINT targetPoint)
    {
        return stratPosition.Y - targetPoint.Y * onePeaceSize;
    }

}
