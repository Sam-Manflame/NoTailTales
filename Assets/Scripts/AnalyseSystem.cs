using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnalyseSystem : MonoBehaviour
{
    [SerializeField]
    private RectTransform analyseTemplate;
    [SerializeField]
    private RectTransform analysePrefab;
    [SerializeField]
    private SpawnSystem spawnSystem;

    private int[] analyseValues;
    private int[] analyseTemplateColors;
   
    public void setupAnalyseTemplate(int[] analyseValues, int[] analyseTemplateColors)
    {
        this.analyseValues = analyseValues;
        this.analyseTemplateColors = analyseTemplateColors;

        for (int i = 0; i < analyseTemplate.childCount; i++)
        {
            Image slot = analyseTemplate.GetChild(i).GetComponent<Image>();
            slot.color = analyseTemplateColors[i] == 0 ? Color.black : Color.clear;
            Text slotText = slot.transform.GetChild(0).GetComponent<Text>();
            slotText.color = analyseTemplateColors[i] == 0 ? Color.white : Color.black;
            slotText.text = string.Format(
                "{0}{1}",
                Mathf.Abs(analyseValues[i]),
                analyseValues[i] > 0 ? "+" : "-");
        }
    }

    public void addAnalyse(int[] animalAnalyse)
    {
        RectTransform analyse = spawnSystem.spawnPrefab(analysePrefab);
        for (int i = 0; i < analyse.childCount; i++)
        {
            analyse.GetChild(i).GetComponent<Image>().color =
                animalAnalyse[i] == 0 ? Color.black : Color.clear;
        }
    }

    public bool isAnalyseGood(Animal animal)
    {
        if (animal.analyse == null)
            return true;

        for (int i = 0; i < analyseValues.Length; i++)
        {
            int sum = 0;
            for (int j = 0; j < animal.analyse.Length; j += 6)
                sum += animal.analyse[j];
                
            if (analyseTemplateColors[i] == 0)
            {
                sum = 5 - sum;
            }

            if (analyseValues[i] > 0 && sum >= analyseValues[i])
                continue;
            else if (analyseValues[i] < 0 && sum <= Mathf.Abs(analyseValues[i]))
                continue;
            else
                return false;
        }
        return true;
    }
}
