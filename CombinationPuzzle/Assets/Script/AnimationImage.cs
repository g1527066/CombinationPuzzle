using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationImage : MonoBehaviour
{

    [SerializeField]
    List<Sprite> list = new List<Sprite>();
    [SerializeField]
    float Speed = 1;

    [SerializeField]
    Image image = null;

    float nowTime = 0;

    int spriteNumber = 0;

    bool loadAnimation = false;

    // Use this for initialization
    void Start()
    {
    }

    public void StartAnimation()
    {
        loadAnimation = true;
        nowTime = 0;
        image.color = Color.white;
        image.sprite = list[0];
        spriteNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (loadAnimation == true)
        {
            nowTime += Time.deltaTime;

            if (nowTime > Speed)
            {
                nowTime = 0;
                spriteNumber++;
                if (spriteNumber >= list.Count)
                {
                    image.color = Color.clear;
                    loadAnimation = false;
                }
                else
                {
                    image.sprite = list[spriteNumber];
                }
            }
        }
    }
}
