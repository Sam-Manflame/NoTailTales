using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalType
{
    public string typeId;

    public int voiceMin = 1;

    public int voiceMax = 10;

    public Sprite footprintsImage;

    public int cost;
}
