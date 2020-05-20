using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    [SerializeField]
    private AudioSource mainAudioSource;

    [SerializeField]
    private AudioClip animalSpeakSound;
    
    public void playAnimalVoice()
    {
        mainAudioSource.clip = animalSpeakSound;
        mainAudioSource.Play();
    }
}
