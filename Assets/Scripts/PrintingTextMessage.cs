using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrintingTextMessage : MonoBehaviour
{
    private string message = "";

    [SerializeField]
    private float symbolDelta = 0.05f;
    private float lastSymbolTime = 0f;

    [SerializeField]
    private bool activated = false;

    [SerializeField]
    private AudioClip typingSound;

    private Text text;

    public System.Action onEnded { get; set; }

    void Awake()
    {
        text = GetComponent<Text>();
        message = text.text;
    }
    
    void Update()
    {
        if (activated && !text.text.Equals(message) && Time.time - lastSymbolTime > symbolDelta)
        {
            text.text += message.Substring(text.text.Length, 1);
            lastSymbolTime = Time.time;
            if (ended())
            {
                if (GetComponent<AudioSource>() != null)
                {
                    GetComponent<AudioSource>().Stop();
                }

                if (onEnded != null)
                    onEnded();
            }
        }
    }

    public void setMessage(string message)
    {
        this.message = message;
        this.text.text = "";
        this.activated = true;
        this.lastSymbolTime = 0;

        tryPlaySound();
    }

    private void tryPlaySound()
    {
        if (typingSound != null && GetComponent<AudioSource>() != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = typingSound;
            audioSource.Play();
        }
    }

    public void Activate()
    {
        activated = true;
        tryPlaySound();
    }

    public void Disactivate()
    {
        activated = false;
    }

    public bool ended()
    {
        return activated && text.text.Equals(message);
    }
}
