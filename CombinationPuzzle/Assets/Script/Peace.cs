﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Peace : MonoBehaviour
{

    public PeaceType peaceType;
    public POINT point;

    float deleteTime = 0f;
    float flashingTime = 0f;
    bool isNoColor = false;
    PeaceType nextPeaceType = PeaceType.None; 

    //private PieceManager pieaceManager = null;
    //public PieceManager SetPeaceManager
    //{
    //    set { pieaceManager = value; }
    //}

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
                if(nextPeaceType!=PeaceType.None)
                {
                    //マネージェーからスプライト受け取る
                    peaceType = nextPeaceType;
                    nextPeaceType = PeaceType.None;
                    this.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                    flashingTime = deleteTime = 0;
                    return;
                }
                Destroy(this.gameObject);
            }
        }
    }
    public void SetSprite(Sprite setSptrite)
    {
        this.GetComponent<UnityEngine.UI.Image>().sprite = setSptrite;
    }
}
