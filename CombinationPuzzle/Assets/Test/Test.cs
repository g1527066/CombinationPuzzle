﻿using System.Collections;
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

    [SerializeField]
    GameObject aaa = null;


    // Use this for initialization
    void Start()
    {
        //GetComponent<RectTransform>().sizeDelta = new Vector2(200,200);

        //GetComponent<BoxCollider2D>().size=new Vector2(200, 200);

        aaa.SetActive(true);
        aaa.GetComponent<Animator>().Play("Mission_Slise_Animation");
        StartCoroutine(StayProsess());
       

    }


    private IEnumerator StayProsess()
    {
      
            yield return new WaitForSeconds(2f);
        aaa.SetActive(false);
        yield return new WaitForSeconds(2f);

        aaa.SetActive(true);
        aaa.GetComponent<Animator>().Play("Mission_Slise_Animation");
    }



    bool aa = false;
    // Update is called once per frame
    void Update()
    {
        if (aa == false)
            Click();


        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10, 100);

        //if (hit.collider.gameObject==this.gameObject)
        //{

        //    Debug.Log("hit white");

        ////    PeaceManager.Instance.SetHoldPeace(hit.collider.gameObject.GetComponent<PointCollision>());
        //}

    }


    public void Click()
    {
        aa = true;
        Debug.Log("click");
        SpriteSlicer2D.ExplodeSprite(this.gameObject, 3, 10);
    }
}
