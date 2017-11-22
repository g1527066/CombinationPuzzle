using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up,
    Rigth,
    Down,
    Left,
}


public class InputManager : MonoBehaviour
{


    [SerializeField]
    PeaceManager peaceManager = null;
    bool noePeaceMove = false;
    Vector2 savePosition;
    string[] MaskNameList;

    Vector2 oldPosition;
    // Use this for initialization
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {
        if (GameSystem.I.IsGameOver == true) return;



        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Rayの長さ
            float maxDistance = 10;

            RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, maxDistance, 100);

            if (hit.collider != null)
            {
                peaceManager.SetHoldPeace(hit.collider.gameObject.GetComponent<Peace>());
            }

            oldPosition = Input.mousePosition;

        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 d = new Vector2(Input.mousePosition.x - oldPosition.x, Input.mousePosition.y - oldPosition.y);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //Rayの長さ
            float maxDistance = 10;

            RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, maxDistance, 100);

            //Debug.Log(hit.collider);
            // peaceManager.MoveHoldPeace(hit.collider.gameObject.GetComponent<Peace>());
            peaceManager.MoveHoldPeace(d, hit);

            oldPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {


            peaceManager.ReleasePeace();
        }





        //if (Input.GetMouseButton(0))
        //{
        //    noePeaceMove = true;
        //    savePosition = Input.mousePosition;
        //}
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    noePeaceMove = false;
        //}


        ////移動中、、、
        //if (noePeaceMove)
        //{
        //    //一番長い上下左右で、一定離れていたら　渡す
        //    //方向を受け取る、



        //}
    }

    //      //押された瞬間にピースをマナージェーに
    //public void SetPushPeace(Peace peace)
    //{
    //    peaceManager.nowHoldPeace = peace;
    //}
}
