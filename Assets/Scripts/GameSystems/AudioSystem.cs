using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    [SerializeField]
    private AudioSource mainAudioSource;

    [SerializeField]
    private AudioClip animalSpeakSound;

    [SerializeField]
    private AudioClip dayEndSound;
    
    public void playAnimalVoice()
    {
        mainAudioSource.clip = animalSpeakSound;
        mainAudioSource.Play();
    }

    public void playDayEndSound()
    {
        mainAudioSource.clip = dayEndSound;
        mainAudioSource.Play();
    }
}
