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
        //GetComponent<RectTransform>().sizeDelta = new Vector2(200,200);

        //GetComponent<BoxCollider2D>().size=new Vector2(200, 200);



    }

    // Update is called once per frame
    void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10, 100);

        if (hit.collider.gameObject==this.gameObject)
        {

            Debug.Log("hit white");

        //    PeaceManager.Instance.SetHoldPeace(hit.collider.gameObject.GetComponent<PointCollision>());
        }

    }
}
