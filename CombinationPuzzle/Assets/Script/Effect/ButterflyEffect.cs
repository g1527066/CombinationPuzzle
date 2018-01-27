using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButterflyEffect : MonoBehaviour
{

    [SerializeField]
    Image butterfly = null;

    [SerializeField]
    float genearationTime = 20f;
    float generationTotalTime = 0;
    [SerializeField]
    List<Sprite> spriteList = new List<Sprite>();
    int spriteCount = 0;
    float spriteTotalTime = 0;
    float spriteChange = 0.5f;



    Vector2 startPosition;
    //移動
    float upTime = 0.4f;
    float downTime = 0.6f;
    bool upflag = false;
    float totalTime = 0;

    int MaxSize = 170;
    int MinSize = 100;



    //   // Use this for initialization
    void Start()
    {
        startPosition = butterfly.rectTransform.anchoredPosition;
        int r = Random.Range(MinSize, MaxSize);
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(r,r);
    }

    // Update is called once per frame
    void Update()
    {
        generationTotalTime += Time.deltaTime;
        if (generationTotalTime > genearationTime)
        {
            totalTime += Time.deltaTime;
            spriteTotalTime += Time.deltaTime;

            float moveX = 2;
            float moveY = 3;

            if (upflag)
            {
                butterfly.rectTransform.anchoredPosition += new Vector2(moveX, moveY);
                if (upTime < totalTime)
                {
                    upflag = false;
                    totalTime = 0;
                }

            }
            else
            {
                butterfly.rectTransform.anchoredPosition += new Vector2(moveX, -moveY);
                if (downTime < totalTime)
                {
                    upflag = true;
                    totalTime = 0;
                }
            }

            if (spriteTotalTime > spriteChange)
            {
                spriteCount++;
                spriteTotalTime = 0;
                if (spriteCount == spriteList.Count)
                {
                    spriteCount = 0;
                }
                butterfly.sprite = spriteList[spriteCount];
            }

        }
        if (generationTotalTime > genearationTime + 50)
        {
            generationTotalTime = 0;
            butterfly.rectTransform.anchoredPosition = startPosition;
            int r = Random.Range(MinSize, MaxSize);
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(r, r);

        }
    }
}
