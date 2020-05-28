using System;
using UnityEngine;

[Serializable]
public class AnimalType
{
    public string typeId;

    public int voiceMin = 1;

    public int voiceMax = 10;

    public Sprite footprintsImage;

    public int cost = 0;

    public bool predator = false;

    public bool special = false;

    public string penaltyString = "";

    [Header("Generation")]
    public string[] iconsIds;
    public string[] typeNames;
}
