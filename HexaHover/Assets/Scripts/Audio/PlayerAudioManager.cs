using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    public static float effectVolume = 1.0f;
    public AudioClip Bounce;
    public AudioClip Boost;

    private AudioSource _source;
    static float GameVolume = 0.5f;


    // Use this for initialization
    void Start()
    {
        _source = GetComponent<AudioSource>();
        _source.volume = GameVolume;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Play(AudioClip clip)
    {
        _source.clip = clip;
        _source.Play();
    }

    public float GetVolume()
    {
        return GameVolume;
    }
    public void SetVolume(float value)
    {
        GameVolume = value;
    }
}