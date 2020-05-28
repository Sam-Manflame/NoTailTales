using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationSystem : MonoBehaviour, IGameListener
{
    [SerializeField]
    private AnimalTypes animalTypes;
    [SerializeField]
    private AnalyseSystem analyseSystem;

    public int similarDistance = 2;

    public NamePreset[] presets;

    private string[] generationTypes = { "ill", "healthy", "predator" };

    private List<string> generationHistory = new List<string>();

    private Day currentDay;

    public Animal generate()
    {
        Animal animal = new Animal();

        string generationType = null;
        
        while (generationType == null)
        {
            generationType = generationTypes[Random.Range(0, generationTypes.Length)];
            int i = generationHistory.Count - 1;
            while (i >= 0 && i >= generationHistory.Count - similarDistance)
            {
                if (generationHistory[i].Contains(generationType))
                {
                    generationType = null;
                    break;
                }
                i--;
            }
        }

        switch (generationType)
        {
            case "ill":
                setType(animal, getNotPredator());
                analyseSystem.generateIllAnalyse(animal);
                break;
            case "healthy":
                setType(animal, getNotPredator());
                analyseSystem.generateHealthy(animal);
                break;
            case "predator":
                setType(animal, getNotSpecial());
                analyseSystem.generateHealthy(animal);
                generatePredator(animal);
                break;
            default:
                throw new System.Exception("Unknown generation type: " + generationType);
        }

        generateName(animal);
        generateTypeName(animal);
        generateIcon(animal);

        Debug.Log(string.Format("GENERATED: {0} {1} {2} {3} {4} {5} {6}",
            generationType, animal.typeId, animal.typeName, animal.name, 
            animal.voice, animal.footprints, analyseSystem.isAnalyseGood(animal)));

        generationHistory.Add(generationType);

        return animal;
    }

    private Animal setType(Animal animal, AnimalType animalType)
    {
        animal.typeId = animalType.typeId;
        animal.voice = animalType.typeId;
        animal.footprints = animalType.typeId;

        return animal;
    }

    private Animal generatePredator(Animal animal)
    {
        if (!animalTypes.getTypeById(animal.typeId).predator)
        {
            if (Random.Range(0, 2) == 0)
                animal.voice = getPredator().typeId;
            else
                animal.footprints = getPredator().typeId;
        }
        return animal;
    }

    private Animal generateName(Animal animal)
    {
        List<string> names = new List<string>();
        List<string> surnameParts1 = new List<string>();
        List<string> surnameParts2 = new List<string>();

        foreach (NamePreset preset in presets)
        {
            if (preset.typeId == "common" || preset.typeId == animal.typeId)
            {
                addArrayToList(names, preset.names);
                addArrayToList(surnameParts1, preset.surnameParts1);
                addArrayToList(surnameParts2, preset.surnameParts2);
            }
        }

        animal.name = 
            names[Random.Range(0, names.Count)] 
            + " " 
            + surnameParts1[Random.Range(0, surnameParts1.Count)] 
            + surnameParts2[Random.Range(0, surnameParts2.Count)];
        
        return animal;
    }

    private Animal generateTypeName(Animal animal)
    {
        AnimalType animalType = animalTypes.getTypeById(animal.typeId);
        animal.typeName = animalType.typeNames[Random.Range(0, animalType.typeNames.Length)];

        return animal;
    }

    private Animal generateIcon(Animal animal)
    {
        AnimalType animalType = animalTypes.getTypeById(animal.typeId);
        animal.iconId = animalType.iconsIds[Random.Range(0, animalType.iconsIds.Length)];

        return animal;
    }

    private void addArrayToList(List<string> list, string[] array)
    {
        foreach (string s in array)
        {
            list.Add(s);
        }
    }

    private AnimalType getNotPredator()
    {
        List<AnimalType> notPredators = new List<AnimalType>();
        foreach (AnimalType type in animalTypes.types)
        {
            if (!type.special && !type.predator)
            {
                notPredators.Add(type);
            }
        }

        return notPredators[Random.Range(0, notPredators.Count)];
    }

    private AnimalType getNotSpecial()
    {
        List<AnimalType> notSpecials = new List<AnimalType>();
        foreach (AnimalType type in animalTypes.types)
        {
            if (!type.special)
            {
                notSpecials.Add(type);
            }
        }

        return notSpecials[Random.Range(0, notSpecials.Count)];
    }

    private AnimalType getPredator()
    {
        List<AnimalType> predators = new List<AnimalType>();
        foreach (AnimalType type in animalTypes.types)
        {
            if (type.predator)
            {
                predators.Add(type);
            }
        }

        return predators[Random.Range(0, predators.Count)];
    }

    public void OnGameInit(GameControllerScript game, Day day)
    {
        currentDay = day;
    }

    public void OnAnimalCome(GameControllerScript game, Animal animal)
    {
        
    }

    public void OnChoiceDone(GameControllerScript game, Animal animal)
    {
        
    }
}
