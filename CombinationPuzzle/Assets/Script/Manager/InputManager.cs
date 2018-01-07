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
    Vector2 savePosition;
    string[] MaskNameList;

    Vector2 oldPosition;

    //Rayの長さ
    float maxDistance = 10;

    // Update is called once per frame
    void Update()
    {
        if (GameSystem.Instance.IsGameOver == true) return;



        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, maxDistance, 100);

            if (hit.collider != null)
            {
                PeaceManager.Instance.SetHoldPeace(hit.collider.gameObject.GetComponent<Peace>());
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

            PeaceManager.Instance.MoveHoldPeace(d, peace);

            oldPosition = Input.mousePosition;
          
        }
        else if (Input.GetMouseButtonUp(0))
        {
            PeaceManager.Instance.ReleasePeace();
        }
    }

}
