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
    public PeaceType nextPeaceType = PeaceType.None;
    public bool isMatching = false;
    public bool IsDuringFall = false;
    RectTransform rectTransform;
    public RectTransform RectTransform
    {
        get { return rectTransform; }
    }

    private void Start()
    {
        Initialization();
        rectTransform = this.gameObject.GetComponent<RectTransform>();
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
                if (nextPeaceType != PeaceType.None)
                {

                    peaceType = nextPeaceType;
//                    nextPeaceType = PeaceType.None;
                  
                    flashingTime = deleteTime = 0;
                    SetSprite(PeaceManager.I.ReturnSprite(peaceType));
                    PeaceManager.I.DeletePeace(this);
                   // nextPeaceType = PeaceType.None;


                    return;
                }
                PeaceManager.I.DeletePeace(this);//ストックに追加
                AudioManager.I.PlaySound("DeletePeace");
               // this.gameObject.SetActive(false);
                // Destroy(this.gameObject);
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
        nextPeaceType = PeaceType.None;
        isMatching = false;
    }
    public void SetNewType()
    {
       peaceType= (PeaceType)Random.Range(0, (int)PeaceType.Square);
    }
}
