using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;    
    AudioSource BGMSource;
    
    public List<AudioClip> clipList=new List<AudioClip>();
    public AudioClip titleClip;
    public AudioClip bossClip;
    public AudioClip gameOverClip;

    bool isCoroutine=false;

     private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }        
        BGMSource=GetComponent<AudioSource>();
    }

    void Start() 
    {        
        TitleClipPlay();
    }

    public void TitleClipPlay()
    {
        ClipPlay(titleClip);
    }

    public void BossClipPlay()
    {
        ClipPlay(bossClip);
    }

    public void GameOverClipPlay()
    {
        ClipPlay(gameOverClip);
    }

    void ClipPlay(AudioClip clip)
    {
        StopCoroutine(RandomPlay());
        isCoroutine=false;
        BGMSource.Stop();
        BGMSource.clip=clip;
        BGMSource.Play();
    }

    public void BgmClipPlay()
    {
        if(!isCoroutine)
        {
            BGMSource.Stop();
            StartCoroutine(RandomPlay());
        }        
    }

    IEnumerator RandomPlay()
    {
        isCoroutine=true;
        while (true)
        {
            if(!BGMSource.isPlaying)
            {
                BGMSource.Stop();
                BGMSource.clip = clipList[Random.Range(0, clipList.Count)];
                BGMSource.Play();
            }  
            yield return new WaitForSeconds(5f);
        }
    }

    public void BGMAudioControl(float sound)
    {        
        BGMSource.volume=sound;
    }

    public void SFXVolControl(float sound)
    {        
        sFXVol = sound;
    }
    
    [SerializeField][Range(0f, 1f)] private float sFXVol;
    [SerializeField][Range(0f, 1f)] private float sFXPich;

    public static void PlaySFX(AudioClip clip)
    {
        GameObject obj = GameManager.instance.objectPool.SpawnFromPool("SoundSource");
        SoundSource soundSource = obj.GetComponent<SoundSource>();
        soundSource.Play(clip, instance.sFXVol, instance.sFXPich);
    }
}
