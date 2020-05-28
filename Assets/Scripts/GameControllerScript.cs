using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour, IGameListener
{
    [Header("Main Settings")]
    [SerializeField]
    private int radioScene;
    [SerializeField]
    private int levelFinishScene;
    [SerializeField]
    private float minutesInSecond = 2;
    [SerializeField]
    private AnimalTypes animalTypes;
    [SerializeField]
    private Images images;

    [Header("Systems")]
    [SerializeField]
    private SpawnSystem spawnSystem;
    [SerializeField]
    private AnalyseSystem analyseSystem;
    [SerializeField]
    private VoiceSystem voiceSystem;
    [SerializeField]
    private FootprintsSystem footprintsSystem;
    [SerializeField]
    private InteractionSystem interactionSystem;
    [SerializeField]
    private GenerationSystem generationSystem;

    [Header("UI Elements")]
    [SerializeField]
    private RulesSetup rulesWindow;
    [SerializeField]
    private RulesSetup rulesTemplate;
    [SerializeField]
    private AnimalWindowSetup animalWindowSetup;
    [SerializeField]
    private Image overlay;

    [Header("Top Panel")]
    [SerializeField]
    private Text dayCounter;
    [SerializeField]
    private Text watches;
    [SerializeField]
    private Button callButton;
    
    [Header("Tutorials")]
    [SerializeField]
    private GameObject voiceTutorial;
    [SerializeField]
    private GameObject footrpintsTutorial;
    [SerializeField]
    private GameObject analyseTutorial;

    private List<IGameListener> listeners = new List<IGameListener>();

    private Day currentDay;
    private int animalCounter = -1;
    private List<HistoryEntry> history = new List<HistoryEntry>();
    private List<Animal> generatedAnimals = new List<Animal>();

    private bool gotFreePenalty = true;
    
    private int dayEndTime = 18;
    private int h = 9;
    private int m = 0;

    void Start()
    {
        listeners.Add(analyseSystem);
        listeners.Add(voiceSystem);
        listeners.Add(footprintsSystem);
        listeners.Add(interactionSystem);

        listeners.Add(animalWindowSetup);
        listeners.Add(rulesWindow);
        listeners.Add(rulesTemplate);

        loadDay();
        
        setupTopPanel();
        OnGameInit(this, currentDay);
    }

    private void loadDay()
    {
        int dayId = PlayerPrefs.GetInt("dayId");
        currentDay = Day.load(dayId);
    }

    private Animal getCurrentAnimal()
    {
        if (animalCounter >= currentDay.animals.Length)
        {
            if (generatedAnimals.Count <= animalCounter - currentDay.animals.Length)
            {
                generatedAnimals.Add(generationSystem.generate());
            }
            return generatedAnimals[generatedAnimals.Count - 1];
        }
        return currentDay.animals[animalCounter];
    }

    private void setupTopPanel()
    {
        dayCounter.text = string.Format("DAY #{0}", currentDay.id);
        setupWatches(h, m);
    }

    public int getAnimalCounter()
    {
        return animalCounter;
    }

    public void startDayTimer()
    {
        StartCoroutine(dayTimer());
    }

    private IEnumerator dayTimer()
    {
        while (h < dayEndTime || currentDay.id == 1)
        {
            m += 1;
            if (m == 60)
            {
                m = 0;
                h += 1;
            }
            if (m % minutesInSecond == 0)
                setupWatches(h, m);
            yield return new WaitForSeconds(1.0f / minutesInSecond);
        }
        endDay();
    }

    private void setupWatches(int h, int m)
    {
        if (h <= 12)
            watches.text = string.Format("{0}:{1:D2} AM", h, m);
        else
            watches.text = string.Format("{0}:{1:D2} PM", h - 12, m);
    }

    private void addToHistory(Animal animal, string action)
    {
        string choiceResult = getChoiceResult(animal, action);
        if (choiceResult != null)
        {
            AnimalType animalType = animalTypes.getTypeById(choiceResult);
            spawnSystem.addPenaltyCard(animalType.penaltyString, animal.name, gotFreePenalty ? 0 : animalType.cost);

            if (gotFreePenalty)
            {
                gotFreePenalty = false;
                return;
            }
        }

        history.Add(new HistoryEntry(animal, action));
    }

    private string getChoiceResult(Animal animal, string action)
    {
        List<HistoryEntry> animals = new List<HistoryEntry>();
        animals.Add(new HistoryEntry(animal, action));
        LevelResult result = LevelResult.generateLevelResult(currentDay, animals, analyseSystem, animalTypes);
        string animalType = result.types[0];

        switch (animalType)
        {
            case "illAccepted":
            case "predatorAccepted":
            case "healthyDenied":
                return animalType;

        }
        return null;
    }


    public void doChoice(string choice)
    {
        if (currentDay.id == 1 && animalCounter == 2)
        {
            StartCoroutine(firstDayEnd());
            return;
        }

        addToHistory(currentDay.animals[animalCounter], choice);
        makeRemoveable();
    }

    public void nextAnimal()
    {
        if (currentDay.id == 1)
        {
            callButton.gameObject.SetActive(false);
        }

        if (animalCounter == -1)
        {
            startDayTimer();
        }

        animalCounter++;

        OnAnimalCome(this, getCurrentAnimal());
    }

    public void makeRemoveable()
    {
        MovableElement[] movableElements = FindObjectsOfType<MovableElement>();
        foreach (MovableElement element in movableElements)
        {
            if (element.isCanBeRemovedLater())
            {
                element.setRemoveable(true);
            }
        }
    }

    private void endDay()
    {
        PlayerPrefs.SetString("levelResult", JsonUtility.ToJson(LevelResult.generateLevelResult(currentDay, history, analyseSystem, animalTypes)));
        PlayerPrefs.SetInt("dayId", currentDay.id + 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(levelFinishScene);
    }

    private IEnumerator firstDayEnd()
    {
        overlay.gameObject.SetActive(true);
        while (overlay.color.a < 1f)
        {
            Color color = overlay.color;
            color.a += 0.01f;
            overlay.color = color;
            yield return new WaitForSeconds(0.01f);
        }
        PlayerPrefs.SetInt("dayId", 2);
        PlayerPrefs.Save();
        SceneManager.LoadScene(radioScene);
    }

    public void addListener(IGameListener listener)
    {
        listeners.Add(listener);
    }

    public void removeListener(IGameListener listener)
    {
        listeners.Remove(listener);
    }

    public void OnAnimalCome(GameControllerScript game, Animal animal)
    {
        foreach(IGameListener listener in listeners)
        {
            listener.OnAnimalCome(game, animal);
        }
    }

    public void OnGameInit(GameControllerScript game, Day day)
    {
        foreach (IGameListener listener in listeners)
        {
            listener.OnGameInit(game, day);
        }
    }
}
