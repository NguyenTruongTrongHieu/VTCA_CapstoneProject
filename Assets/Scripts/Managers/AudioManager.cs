using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] musics;
    public Sound[] sfx;

    public AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMusic(string name, bool isLoop)
    {
        var music = Array.Find(musics, x => x.name == name);

        if (music == null)
        {
            Debug.Log("Khong tim thay am thanh");
            return;
        }

        musicSource.clip = music.audioClip;
        musicSource.loop = isLoop;
        musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        var soundEffect = Array.Find(sfx, x => x.name == name);

        if (soundEffect == null)
        {
            Debug.Log("Khong tim thay am thanh");
            return;
        }

        sfxSource.PlayOneShot(soundEffect.audioClip);
    }
}
