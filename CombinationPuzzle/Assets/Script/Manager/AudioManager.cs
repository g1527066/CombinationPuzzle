using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager I = null;
  


    [SerializeField]
    AudioSource tradeAudioSource;

    [SerializeField]
    AudioSource audioSource;




    [SerializeField]
    AudioClip Trade = null;
    [SerializeField]
    AudioClip DeletePeace = null;

    [SerializeField]
    AudioClip CutSuccess = null;
    [SerializeField]
    AudioClip EndSound = null;
    [SerializeField]
    AudioClip StageDelete1 = null;
    [SerializeField]
    AudioClip StageDelete2 = null;
    [SerializeField]
    AudioClip StageDelete3 = null;
    [SerializeField]
    AudioClip StartSE = null;
    [SerializeField]
    AudioClip Decision = null;
    [SerializeField]
    AudioClip LessTime = null;


    // Use this for initialization
    void Start () {
        I = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayTrade()
    {
        tradeAudioSource.clip = Trade;
        tradeAudioSource.Play();
    }


    //いったんてきとう、生成でやらないと被った時聞こえない
    public  void PlaySound(string SEName)
    {
        switch(SEName)
        {
            case "CutSuccess":
                audioSource.clip = CutSuccess;
                break;
            case "EndSound"://そこまで！のとき
                audioSource.clip =EndSound;
                break;
            case "1StageDelete":
                audioSource.clip =StageDelete1;
                break;
            case "2StageDelete":
                audioSource.clip =StageDelete2;
                break;
            case "3StageDelete":
                audioSource.clip =StageDelete3;
                break;
            case "Start":
                audioSource.clip =StartSE;
                break;
            case "Decision":
                audioSource.clip = Decision;
                break;
            case "LessTime"://残り５秒
                audioSource.clip =LessTime;
                break;
        }
        audioSource.Play();
    }
}
