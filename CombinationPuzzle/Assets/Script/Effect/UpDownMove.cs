using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDownMove : MonoBehaviour {

    [SerializeField]
    float UpDownTime = 1;
    
    [SerializeField]
    float moveY = 1;

    float totalTime = 0;

    bool upFlag = false;

	void Update () {

        totalTime += Time.deltaTime;
        if(totalTime>UpDownTime)
        {
            totalTime = 0;
            upFlag = !upFlag;
        }

        if(upFlag==true)
        {
            transform.localPosition += new Vector3(0, moveY, 0);
        }
        else
        {
            transform.localPosition += new Vector3(0, -moveY, 0);
        }
	}
}
