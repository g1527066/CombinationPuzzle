//http://naichilab.blogspot.jp/2013/11/unityaudiomanager.html
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public enum VolumeType
{
    None = 0,
    Small = 33,
    Middle = 66,
    Big = 100,
}

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    public List<AudioClip> BGMList;
    public List<AudioClip> SEList;
    public int MaxSE = 10;

    float SEVolume = 1;
    float BGMVolume = 1;

    private AudioSource bgmSource = null;
    [SerializeField]
    private List<AudioSource> seSources = null;
    private Dictionary<string, AudioClip> bgmDict = null;
    private Dictionary<string, AudioClip> seDict = null;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this.gameObject);

        //create listener
        if (FindObjectsOfType(typeof(AudioListener)).All(o => !((AudioListener)o).enabled))
        {
            this.gameObject.AddComponent<AudioListener>();
        }
        //create audio sources
        this.bgmSource = this.gameObject.AddComponent<AudioSource>();
        this.seSources = new List<AudioSource>();
        this.bgmSource.loop = true;

        //create clip dictionaries
        this.bgmDict = new Dictionary<string, AudioClip>();
        this.seDict = new Dictionary<string, AudioClip>();

        Action<Dictionary<string, AudioClip>, AudioClip> addClipDict = (dict, c) =>
        {
            if (!dict.ContainsKey(c.name))
            {
                dict.Add(c.name, c);
            }
        };

        this.BGMList.ForEach(bgm => addClipDict(this.bgmDict, bgm));
        this.SEList.ForEach(se => addClipDict(this.seDict, se));
    }

    public void PlaySE(string seName)
    {
        if (!this.seDict.ContainsKey(seName)) throw new ArgumentException(seName + " not found", "seName");

        AudioSource source = this.seSources.FirstOrDefault(s => !s.isPlaying);
        if (source == null)
        {
            if (this.seSources.Count >= this.MaxSE)
            {
                Debug.Log("SE AudioSource is full");
                return;
            }

            source = this.gameObject.AddComponent<AudioSource>();
            source.volume = SEVolume;
            this.seSources.Add(source);
        }

        source.clip = this.seDict[seName];
        source.Play();
    }

    public void StopSE()
    {
        this.seSources.ForEach(s => s.Stop());
    }

    public void PlayBGM(string bgmName)
    {
        if (!this.bgmDict.ContainsKey(bgmName)) throw new ArgumentException(bgmName + " not found", "bgmName");
        if (this.bgmSource.clip == this.bgmDict[bgmName]) return;
        this.bgmSource.Stop();
        this.bgmSource.clip = this.bgmDict[bgmName];
        this.bgmSource.Play();
    }

    public void StopBGM()
    {
        this.bgmSource.Stop();
        this.bgmSource.clip = null;
    }

    //新規のもその音量にする
    public void SetSEVolume(VolumeType volumeType)
    {
        for (int i = 0; i < seSources.Count; i++)
        {
            seSources[i].volume = (int)volumeType * 0.01f;
            Debug.Log("volume = " + (int)volumeType * 0.01f);
        }
        SEVolume = (int)volumeType * 0.01f;
    }
    
    public void SetBGMVolume(VolumeType volumeType)
    {
        bgmSource.volume = (int)volumeType * 0.01f;
        BGMVolume = (int)volumeType * 0.01f;
    }

    public VolumeType GetSeVolume()
    {
        float se = SEVolume + 0.01f;
        return (VolumeType)se;
    }

    public VolumeType GetBgmVolume()
    {
        float bgm = BGMVolume + 0.01f;
        return (VolumeType)bgm;
    }
}