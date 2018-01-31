using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour {

    private float totalTime = 0;
    public float DestroyTime=1;

    void Update () {
        totalTime += Time.deltaTime;
        if(DestroyTime<totalTime)
        {
            Destroy(this.gameObject);
        }
	}
}
