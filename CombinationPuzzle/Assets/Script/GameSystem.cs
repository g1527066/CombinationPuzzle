﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{
    public static GameSystem I = null;

    [SerializeField]
    private float SetLimitTime = 90f;

    private float remainingTime = 0;

    [SerializeField]
    private Text TimeText = null;

    private bool isGameOver = false;


    //企画さんが設定できるように数値、、、
    //削除までの時間
    public float DeleteTime = 1.6f;
    //peace削除の点滅の時間
    public float flashingTime = 0.2f;

    public bool IsGameOver
    {
        get { return isGameOver; }
    }



    // Use this for initialization
    void Start()
    {
        I = this;

        remainingTime = SetLimitTime;
        isGameOver = false;
    }

    // Update is called once per frame
    void Update()
    {

        remainingTime -= Time.deltaTime;
        TimeText.text = (int)remainingTime / 60 + ":" + (int)remainingTime % 60;
        if (remainingTime < 0)
            isGameOver = true;

    }
}
