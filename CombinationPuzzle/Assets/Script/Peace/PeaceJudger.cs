using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaceJudger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public bool CurrentDeletable(Dictionary<POINT,Peace> dictionary,Peace judgeAddPeace)
    {
            //上
            int count = 0;
              POINT p = judgeAddPeace.point;
            for (int i = p.Y - 1; i >= 0; i--)
            {
                if (dictionary[new POINT(p.X, i)].peaceType == judgeAddPeace.peaceType)
                {
                    count++;
                }
                else
                    break;
            }
            if (count >= 2)
            {
                return true;
            }

            //左
            count = 0;
            for (int i = p.X - 1; i >= 0; i--)
            {
                if (dictionary[new POINT(i, p.Y)].peaceType == judgeAddPeace.peaceType)
                {
                    count++;
                }
                else
                    break;
            }
            if (count >= 2)
            {
                return true;
            }
            return false;
    }



}
