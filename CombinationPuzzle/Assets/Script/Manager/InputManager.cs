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
   // bool noePeaceMove = false;
    Vector2 savePosition;
    string[] MaskNameList;

    Vector2 oldPosition;

    //Rayの長さ
    float maxDistance = 10;

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

            RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, maxDistance, 100);
            Peace peace = null;
            if (hit.collider != null)
                peace = hit.collider.gameObject.GetComponent<Peace>();

                    peaceManager.MoveHoldPeace(d, peace);

            oldPosition = Input.mousePosition;
          
        }
        else if (Input.GetMouseButtonUp(0))
        {
            peaceManager.ReleasePeace();
        }
    }

}
