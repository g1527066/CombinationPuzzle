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

    // Use this for initialization
    void Start()
    {
        for(int i=0;i<BoardSizeY;i++)
        {
            for (int j = 0; j <BoardSizeX; j++)
            {
                peaceList[i,j].peaceType = Random.Range(0,((int)PeaceType.None)-1);


            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
