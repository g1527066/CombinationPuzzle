using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct POINT
{
    public int X;
    public int Y;
    public POINT(int x,int y)
    {
        X = x;
        Y = y;
    }
}


public class Peace : MonoBehaviour
{

    public PeaceType peaceType;
    public POINT point;

    private PieceManager pieaceManager = null;
    public PieceManager SetPeaceManager
    {
        set { pieaceManager = value; }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetSprite(Sprite setSptrite)
    {
        this.GetComponent<UnityEngine.UI.Image>().sprite = setSptrite;
    }
}
