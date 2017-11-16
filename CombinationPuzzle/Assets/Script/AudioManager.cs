using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager I = null;
  

    [SerializeField]
    AudioSource audioSource;


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

        audioSource.Play();
    }
}
