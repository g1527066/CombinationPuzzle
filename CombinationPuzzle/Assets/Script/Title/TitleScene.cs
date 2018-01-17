using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour {


    [SerializeField]
    GameObject SelectObject = null;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void PushStartButtone()
    {
        SelectObject.SetActive(true);
    }

    public void PushEndButtone()
    {
        Application.Quit();
       
    }

    public void PushReturn()
    {
        SelectObject.SetActive(false);
    }




 
}
