using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager I = null;
  


    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    AudioClip Trade = null;

    [SerializeField]
    AudioClip DeletePeace = null;

    // Use this for initialization
    void Start () {
        I = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //いったんてきとう
    public  void PlaySound(string SEName)
    {
        if (SEName == "Trade")
            audioSource.clip = Trade;
        if (SEName == "DeletePeace")
            audioSource.clip = DeletePeace;

        audioSource.Play();
       
    }
}
