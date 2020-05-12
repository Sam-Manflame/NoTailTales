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
    }

    public void Activate()
    {
        activated = true;
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
