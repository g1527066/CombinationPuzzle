using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartProduction : MonoBehaviour {


    [SerializeField]
    Sprite startSprite = null;

    [SerializeField]
    float drawStayTime = 1f;
    [SerializeField]
    float drawStartTime = 1f;

    float totalTime = 0f;
    [SerializeField]
    UnityEngine.UI.Image stringImage = null;

    bool overStayTime = false;

    //activeではいけないもの
    [SerializeField]
    GameObject PeaceManager = null;
    [SerializeField]
    GameObject MissionManager = null;
    [SerializeField]
    GameObject MissionImage = null;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        totalTime += Time.deltaTime;
        if (overStayTime == false && totalTime > drawStayTime)
        {
            overStayTime = true;
            stringImage.sprite = startSprite;
        }
        else if (totalTime > drawStartTime + drawStayTime)
        {
            PeaceManager.SetActive(true);
            MissionImage.SetActive(true);
            MissionManager.SetActive(true);
            stringImage.gameObject.SetActive(false);
            Destroy(this);
        }
    }
}
