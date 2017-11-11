using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct POINT
{
    public int X;
    public int Y;
}


public class Peace : MonoBehaviour
{

    public PeaceType peaceType;
    public POINT point;

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
