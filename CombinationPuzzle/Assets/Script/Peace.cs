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

    float deleteTime = 0f;
    float flashingTime = 0f;
    bool isNoColor = false;


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
        if(peaceType==PeaceType.None)
        {
            deleteTime += Time.deltaTime;
            flashingTime += Time.deltaTime;
            if(flashingTime>GameSystem.I.flashingTime)
            {
                flashingTime = 0;

                if (isNoColor)
                    this.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                else
                    this.GetComponent<UnityEngine.UI.Image>().color = Color.clear;

                isNoColor = !isNoColor;
            }

            if (deleteTime>GameSystem.I.DeleteTime)
            {
                Destroy(this.gameObject);
            }
        }
    }
    public void SetSprite(Sprite setSptrite)
    {
        this.GetComponent<UnityEngine.UI.Image>().sprite = setSptrite;
    }
}
