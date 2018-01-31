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

    [SerializeField]
    RectTransform PuzzleSpace = null;

    Vector2 puzzleSpaceStartPosition = Vector2.zero;

    //Rayの長さ
    float maxDistance = 10;

    private void Start()
    {
        //左下(0,0)スクリーン座標での位置
        puzzleSpaceStartPosition = new Vector2(
            1920/2+PuzzleSpace.anchoredPosition.x-PuzzleSpace.sizeDelta.x/2,
            1080/2+ PuzzleSpace.anchoredPosition.y+PuzzleSpace.sizeDelta.y/2);

    }

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
                PeaceManager.Instance.SetHoldPeace(hit.collider.gameObject.GetComponent<PointCollision>());
            }

            oldPosition = Input.mousePosition;

        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 pos = PuzzleSpaceRange();

            Vector2 d = new Vector2(pos.x - oldPosition.x, pos.y - oldPosition.y);
            PeaceManager.Instance.MoveHoldPeace(d,pos);

            oldPosition = pos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            PeaceManager.Instance.ReleasePeace(oldPosition);
        }
    }

    //もしピースを掴んでいて範囲外に出ていたら、範囲内に修正する
    private Vector2 PuzzleSpaceRange()
    {
        Vector2 screenSize = new Vector2(1920, 1080);

        Vector2 convertPostion;// = new Vector2(screenSize.x * Input.mousePosition.x / Screen.width, screenSize.y * Input.mousePosition.y / Screen.height);
        if (Screen.width == screenSize.x)
        {
            convertPostion = Input.mousePosition;
        }
        else
            convertPostion = new Vector2(screenSize.x * Input.mousePosition.x / Screen.width, screenSize.y * Input.mousePosition.y / Screen.height);

        Vector2 returnPosition = Input.mousePosition;

        float correctionSize = 45;

        //範囲内ならマウスの位置を返す
        if (puzzleSpaceStartPosition.x+ correctionSize < convertPostion.x &&
            puzzleSpaceStartPosition.x+PuzzleSpace.sizeDelta.x-correctionSize > convertPostion.x &&
            puzzleSpaceStartPosition.y-correctionSize > convertPostion.y &&
            puzzleSpaceStartPosition.y - PuzzleSpace.sizeDelta.y+correctionSize < convertPostion.y)
        {
         //   Debug.Log("範囲内");
            return Input.mousePosition;
        }
        else//範囲外なら修正（その時の解像度でやる）
        {
            if (puzzleSpaceStartPosition.x+correctionSize > convertPostion.x)
            {
                returnPosition.x = (Screen.width * puzzleSpaceStartPosition.x) / screenSize.x+ correctionSize;
            }
            else if (puzzleSpaceStartPosition.x + PuzzleSpace.sizeDelta.x-correctionSize < convertPostion.x)
            {
                returnPosition.x = (Screen.width * (puzzleSpaceStartPosition.x + PuzzleSpace.sizeDelta.x)) / screenSize.x- correctionSize;
            }

            if (puzzleSpaceStartPosition.y-correctionSize < convertPostion.y)
            {
                returnPosition.y = (Screen.height * puzzleSpaceStartPosition.y) / screenSize.y- correctionSize;
            }
            else if (puzzleSpaceStartPosition.y - PuzzleSpace.sizeDelta.y+correctionSize > convertPostion.y)
            {
                returnPosition.y = (Screen.height *(puzzleSpaceStartPosition.y - PuzzleSpace.sizeDelta.y)) / screenSize.y+ correctionSize;
            }
        }
        return returnPosition;
    }
}
