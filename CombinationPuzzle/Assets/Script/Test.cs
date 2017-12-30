using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    [SerializeField]
    GameObject s = null;
    [SerializeField]
    GameObject e=null;

    [SerializeField]
    GameObject cut = null;


    // Use this for initialization
    void Start()
    {
  




    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(s.transform.position, e.transform.position, Color.red);
        Debug.Log(s.transform.position);
        Debug.Log("終了"+e.transform.position);
        SpriteSlicer2D.SliceSprite(s.transform.position, e.transform.position, cut);
    }
}
