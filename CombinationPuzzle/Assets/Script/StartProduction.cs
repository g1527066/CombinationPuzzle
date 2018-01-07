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
    [SerializeField]
    GameObject Input = null;
    [SerializeField]
    UnityEngine.UI.Button stopButton = null;

    public float stringSize = 2.0f;

    // Use this for initialization
    void Start () {
        stringImage.SetNativeSize();
        stringImage.gameObject.GetComponent<RectTransform>().sizeDelta *= stringSize;
    }
	
	// Update is called once per frame
	void Update () {
        totalTime += Time.deltaTime;
        if (overStayTime == false && totalTime > drawStayTime)
        {
            overStayTime = true;
            stringImage.sprite = startSprite;
            AudioManager.Instance.PlaySE("PAZ_SE_Start");
            stringImage.SetNativeSize();
            stringImage.gameObject.GetComponent<RectTransform>().sizeDelta *= stringSize;
        }
        else if (totalTime > drawStartTime + drawStayTime)
        {
            PeaceManager.SetActive(true);
            MissionImage.SetActive(true);
            MissionManager.SetActive(true);
            stringImage.gameObject.SetActive(false);
            stopButton.interactable = true;
            Input.SetActive(true);

            Destroy(this);
        }
    }
}
