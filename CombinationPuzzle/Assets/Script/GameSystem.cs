using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystem : MonoBehaviour
{


    [SerializeField]
    private float SetLimitTime = 90f;

    private float remainingTime = 0;

    [SerializeField]
    private Text TimeText = null;

    private bool isGameOver = false;
    public bool IsGameOver
    {
        get { return isGameOver; }
    }



    // Use this for initialization
    void Start()
    {
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
