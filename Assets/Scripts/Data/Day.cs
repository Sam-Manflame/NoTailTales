using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Day
{
    public int id;

    public Animal[] animals;

    public Rule[] rules;

    public int[] analysTemplate;
    public int[] analysTemplateColors;

    public string[] radio;

    public List<string> getRulesAsStrings()
    {
        List<string> result = new List<string>();

        foreach (Rule rule in rules)
        {
            result.Add(rule.rule);
        }

        return result;
    }

    public static Day load(int id)
    {
        string path = string.Format(Application.streamingAssetsPath + "\\day{0}.json", id);

        StreamReader reader = new StreamReader(path);
        string jsonData = reader.ReadToEnd();
        reader.Close();

        return JsonUtility.FromJson<Day>(jsonData);
    }

    public static Day generate(int id)
    {
        Day day = new Day();

        day.id = id;
        day.animals = new Animal[0];
        day.rules = new Rule[3] { new Rule("NO WOLVES", "wolf"), new Rule("NO INFECTED\n ANIMALS", "infected"), new Rule("INTERACT WITH \n ANIMALS WITH \n DISCREPANCIES", "infected") };
        day.analysTemplate = new int[6];
        day.analysTemplateColors = new int[6];
        for (int i = 0; i < day.analysTemplate.Length; i++)
        {
            day.analysTemplate[i] = Random.Range(-5, 6);
            if (day.analysTemplate[i] == 0)
                day.analysTemplate[i] = -1;
            day.analysTemplateColors[i] = Random.Range(0, 2);
        }

        day.radio = new string[1] { string.Format("DAY #{0}\n IF THE FARMER IS RICH - SO IS THE NATION.", id) };

        return day;
    }
}
