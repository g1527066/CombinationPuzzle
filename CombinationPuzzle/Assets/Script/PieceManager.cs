using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PeaceType
{

    Red,
    Blue,
    Yellow,
    Green,
    Perple,
    Orange,
    Square,
    Pentagon,
    None,
}


public class PieceManager : MonoBehaviour
{
    const int BoardSizeX = 10;
    const int BoardSizeY = 6;
    Peace[,] peaceList = new Peace[BoardSizeY, BoardSizeX];
    [SerializeField]
    GameObject peacePrefab = null;

    [SerializeField]
    GameObject peaceParent = null;

    [SerializeField]
    List<Sprite> PeaceSprites = new List<Sprite>();


    //移動用の変数
    public Peace nowHoldPeace = null;

    POINT stratPosition = new POINT();

    int onePeaceSize = 150;


    //まとめてポイント代入できなかったっけ？、new必要？

    // Use this for initialization
    void Start()
    {
        stratPosition.X = -519;
        stratPosition.Y = 329;

        for (int i = 0; i < BoardSizeY; i++)
        {
            for (int j = 0; j < BoardSizeX; j++)
            {
                Peace newPeace = Instantiate(peacePrefab).GetComponent<Peace>();
                newPeace.peaceType = (PeaceType)Random.Range(0, (int)PeaceType.Orange);
                newPeace.transform.SetParent(peaceParent.transform, false);
                newPeace.GetComponent<RectTransform>().anchoredPosition = new Vector2(stratPosition.X + j * onePeaceSize, stratPosition.Y - i * onePeaceSize);
                newPeace.point = new POINT(j, i);
                newPeace.SetSprite(PeaceSprites[(int)newPeace.peaceType]);
                newPeace.SetPeaceManager = this;
                peaceList[i, j] = newPeace;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

        //移動中、、
        if (nowHoldPeace != null)
        {

        }


    }

    public void MoveHoldPeace( Peace nowPeace)
    {
        //前回とピースがちがかったら入れ替え
        if (nowHoldPeace != nowPeace)
        {
            POINT savePoint = nowHoldPeace.point;
            nowHoldPeace.point = nowPeace.point;
            nowPeace.point = savePoint;
            ResetHoldPeacePosition(nowPeace);

            Debug.Log("1");
        }
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        nowPeace.GetComponent<RectTransform>().anchoredPosition = new Vector2(point.x,point.y);

        Debug.Log("2");
    }

    public void SetHoldPeace(Peace peace)
    {
        nowHoldPeace = peace;
    }

    void ResetHoldPeacePosition(Peace peace)
    {
        peace.GetComponent<RectTransform>().anchoredPosition = new Vector2(stratPosition.X + peace.point.X * onePeaceSize, stratPosition.Y - peace.point.Y * onePeaceSize);
    }

}
