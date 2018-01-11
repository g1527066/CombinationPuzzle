using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    //[SerializeField]
    //GameObject s = null;
    //[SerializeField]
    //GameObject e=null;

    //[SerializeField]
    //GameObject cut = null;


    // Use this for initialization
    void Start()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(200,200);

        GetComponent<BoxCollider2D>().size=new Vector2(200, 200);



    }

    // Update is called once per frame
    void Update()
    {



    }
}
