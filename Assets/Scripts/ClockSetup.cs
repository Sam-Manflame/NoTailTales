using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockSetup : MonoBehaviour
{
    private Text text;
    private AudioSource audioSource;

    private void Awake()
    {
        text = GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
    }

    public void setup(int h, int m, bool sound)
    {
        if (h <= 12)
            text.text = string.Format("{0}:{1:D2} AM", h, m);
        else
            text.text = string.Format("{0}:{1:D2} PM", h - 12, m);

        if (sound)
            audioSource.Play();
    }
}
