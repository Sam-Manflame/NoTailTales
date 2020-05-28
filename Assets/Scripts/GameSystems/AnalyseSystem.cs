using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnalyseSystem : MonoBehaviour, IGameListener
{
    [SerializeField]
    private RectTransform analyseTemplate;
    [SerializeField]
    private RectTransform analysePrefab;
    [SerializeField]
    private SpawnSystem spawnSystem;

    private int[] analyseValues;
    private int[] analyseTemplateColors;

    private Animal currentAnimal = null;
    private bool analyseAdded = true;
   
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

    public void addAnalyse()
    {
        // не добавляем карточку с анализом повторно
        if (analyseAdded)
            return;

        RectTransform analyse = spawnSystem.spawnPrefab(analysePrefab);
        if (currentAnimal.analyse == null)
            return;

        for (int i = 0; i < analyse.childCount; i++)
        {
            analyse.GetChild(i).GetComponent<Image>().color =
                currentAnimal.analyse[i] == 0 ? Color.black : Color.clear;
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

    public void OnAnimalCome(GameControllerScript game, Animal animal)
    {
        currentAnimal = animal;
        analyseAdded = false;
    }

    public void OnGameInit(GameControllerScript game, Day day)
    {
        setupAnalyseTemplate(day.analysTemplate, day.analysTemplateColors);
    }

    public bool isAnalyseAdded()
    {
        return analyseAdded;
    }

    // generation stuff

    public Animal generateIllAnalyse(Animal animal)
    {
        animal.analyse = new int[30];

        bool hasIllColumn = false;
        for (int i = 0; i < analyseValues.Length; i++)
        {
            int[] column;
            if (Random.Range(0, 2) == 0)
            {
                column = generateIllColumn(analyseValues[i], analyseTemplateColors[i]);
                hasIllColumn = true;
            }
            else
            {
                column = generateHealthyColumn(analyseValues[i], analyseTemplateColors[i]);
            }
            for (int j = 0; j < 5; j++)
            {
                animal.analyse[i * 5 + j] = column[j];
            }
        }

        if (!hasIllColumn)
        {
            int i = Random.Range(0, 7);
            int[] column = generateIllColumn(analyseValues[i], analyseTemplateColors[i]);
            for (int j = 0; j < 5; j++)
            {
                animal.analyse[i * 5 + j] = column[j];
            }
        }

        return animal;
    }

    public Animal generateHealthy(Animal animal)
    {
        animal.analyse = new int[30];

        for (int i = 0; i < analyseValues.Length; i++)
        {
            int[] column = generateHealthyColumn(analyseValues[i], analyseTemplateColors[i]);
            for (int j = 0; j < 5; j++)
            {
                animal.analyse[i * 5 + j] = column[j];
            }
        }

        return animal;
    }

    private int[] generateHealthyColumn(int val, int color)
    {
        if (val > 0)
        {
            return generateColumn(Random.Range(val, 6), color);
        }
        else
        {
            return generateColumn(Random.Range(0, val + 1), color);
        }
    }

    private int[] generateIllColumn(int val, int color)
    {
        if (val > 0)
        {
            return generateColumn(Random.Range(0, val), color);
        }
        else
        {
            return generateColumn(Random.Range(val + 1, 6), color);
        }
    }

    private int[] generateColumn(int count, int color)
    {
        int[] columns = new int[5];

        for (int i = 0; i < 5; i++)
        {
            columns[i] = Mathf.Abs(color - 1);
        }

        while (count > 0)
        {
            int i = Random.Range(0, 5);
            if (columns[i] != color)
            {
                columns[i] = color;
                count -= 1;
            }
        }

        return columns;
    }
}
