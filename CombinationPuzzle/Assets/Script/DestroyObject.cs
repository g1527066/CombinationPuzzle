using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour {

    private float totalTime = 0;
    public float DestroyTime=1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        totalTime += Time.deltaTime;
        if(DestroyTime<totalTime)
        {
            Destroy(this.gameObject);
        }

	}
}
