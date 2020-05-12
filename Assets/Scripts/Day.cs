using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Day
{
    public int id;

    public Animal[] animals;

    public Rule[] rules;

    public int[] analysTemplate;
    public int[] analysTemplateColors;

    public Expense[] expenses;

    public string[] radio;

    public static Day load(int id)
    {
        string path = string.Format("Assets/Resources/day{0}.json", id);

        StreamReader reader = new StreamReader(path);
        string jsonData = reader.ReadToEnd();
        reader.Close();

        return JsonUtility.FromJson<Day>(jsonData);
    }
}
