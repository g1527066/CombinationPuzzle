using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mission : MonoBehaviour {

    [SerializeField]
    GameObject FirstMission = null;
    [SerializeField]
    Image FirstImage = null;
    [SerializeField]
    Text FirstText = null;

    [SerializeField]
    GameObject SecondMission = null;
    [SerializeField]
    Image SecondImage = null;
    [SerializeField]
    Text SecondText = null;

    [SerializeField]
    GameObject ThirdMission = null;
    [SerializeField]
    Image ThirdImage = null;
    [SerializeField]
    Text ThirdText = null;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void ClearMisstion()
    {
        GameSystem.I.TimerControl(0,0,GameSystem.I.CompleteAddTime);
    }
}
