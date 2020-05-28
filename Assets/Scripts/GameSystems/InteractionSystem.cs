using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSystem : MonoBehaviour, IGameListener
{
    [SerializeField]
    private VoiceSystem voiceSystem;
    [SerializeField]
    private FootprintsSystem footprintsSystem;
    [SerializeField]
    private AnalyseSystem analyseSystem;
    [SerializeField]
    private SpawnSystem spawnSystem;

    [SerializeField]
    private RectTransform interactionButton;

    [SerializeField]
    [Multiline]
    private string haveNothingToSayText;

    private bool infoCardAdded = true;
    private bool voiceCardAdded = true;
    private bool footprintsCardAdded = true;
    private bool analyseCardAdded = true;

    private Animal currentAnimal = null;

    public void OnAnimalCome(GameControllerScript game, Animal animal)
    {
        analyseCardAdded = false;
        footprintsCardAdded = false;
        infoCardAdded = false;
        voiceCardAdded = false;

        currentAnimal = animal;
    }

    public void OnGameInit(GameControllerScript game, Day day)
    {
        if (day.id <= 1)
        {
            interactionButton.gameObject.SetActive(false);
        }
    }

    public void OnChoiceDone(GameControllerScript game, Animal animal)
    {
        currentAnimal = null;
    }

    public void animalInteract()
    {
        if (currentAnimal == null)
            return;

        bool infoNow = false;
        if (!infoCardAdded && currentAnimal.info != null)
        {
            spawnSystem.addTextCard(currentAnimal.info, currentAnimal.name);
            infoCardAdded = true;
            infoNow = true;
        }

        if (!voiceCardAdded && voiceSystem.isDiagramAdded() && currentAnimal.voice != currentAnimal.typeId && currentAnimal.infoVoice != null)
        {
            spawnSystem.addTextCard(currentAnimal.infoVoice, currentAnimal.name);
            voiceCardAdded = true;
        }
        else
        if (!footprintsCardAdded && footprintsSystem.isFootrpintsAdded() && currentAnimal.footprints != currentAnimal.typeId && currentAnimal.infoFootprints != null)
        {
            spawnSystem.addTextCard(currentAnimal.infoFootprints, currentAnimal.name);
            footprintsCardAdded = true;
        }
        else
        if (!analyseCardAdded && analyseSystem.isAnalyseAdded() && !analyseSystem.isAnalyseGood(currentAnimal) && currentAnimal.infoAnalyse != null)
        {
            spawnSystem.addTextCard(currentAnimal.infoAnalyse, currentAnimal.name);
            analyseCardAdded = true;
        }
        else
        if (!infoNow)
        {
            spawnSystem.addTextCard(haveNothingToSayText, currentAnimal.name);
        }
    }
}
