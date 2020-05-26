using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FootprintsSystem : MonoBehaviour, IGameListener
{
    [SerializeField]
    private RectTransform footrpintsTemplate;

    [SerializeField]
    private Image footprintPrefab;

    [SerializeField]
    private SpawnSystem spawnSystem;

    [SerializeField]
    private AnimalTypes animalTypes;

    [SerializeField]
    private RectTransform footprintsButton;

    private bool footprintsAdded = true;
    private Animal currentAnimal = null;

    public void addAnimalFootrpint()
    {
        if (footprintsAdded)
            return;

        AnimalType animalType = animalTypes.getTypeById(currentAnimal.footprints);

        Image footprint = spawnSystem.spawnPrefab(footprintPrefab.GetComponent<RectTransform>()).GetComponent<Image>();
        footprint.sprite = animalType.footprintsImage;

        footprintsAdded = true;
        
    }

    public void OnAnimalCome(GameControllerScript game, Animal animal)
    {
        footprintsAdded = false;
        currentAnimal = animal;
    }

    public void OnGameInit(GameControllerScript game, Day day)
    {
        if (day.id <= 1)
        {
            footrpintsTemplate.gameObject.SetActive(false);
            footprintsButton.gameObject.SetActive(false);
        }
    }

    public bool isFootrpintsAdded()
    {
        return footprintsAdded;
    }
}
