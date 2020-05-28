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
    private int mainMenuScene = 0;
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
    [SerializeField]
    private AudioSystem audioSystem;

    [Header("UI Elements")]
    [SerializeField]
    private RulesSetup rulesWindow;
    [SerializeField]
    private RulesSetup rulesTemplate;
    [SerializeField]
    private AnimalWindowSetup animalWindowSetup;
    [SerializeField]
    private Image overlay;
    [SerializeField]
    private ClockSetup clock;
    [SerializeField]
    private GameObject pauseMenu;

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
    private bool dayEnded = false;
    private bool paused = false;
    
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
        try
        {
            currentDay = Day.load(dayId);
        } catch (System.Exception e)
        {
            currentDay = Day.generate(dayId);
        }
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
        clock.setup(h, m, false);
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
        while (h < dayEndTime)
        {
            if (paused)
            {
                yield return new WaitForSeconds(1.0f / minutesInSecond);
                continue;
            }

            m += 1;
            if (m == 60)
            {
                m = 0;
                h += 1;
            }
            if (m % minutesInSecond == 0)
                clock.setup(h, m, true);
            yield return new WaitForSeconds(1.0f / minutesInSecond);
        }
        if (currentDay.id == 1)
        {
            StartCoroutine(firstDayEnd());
        } else
        {
            audioSystem.playDayEndSound();
            dayEnded = true;
            if (!animalWindowSetup.gameObject.activeSelf)
            {
                endDay();
            }
        }
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
        string animalType = result.types.Length > 0 ? result.types[0] : "";

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

        addToHistory(getCurrentAnimal(), choice);
        makeRemoveable();

        OnChoiceDone(this, getCurrentAnimal());

        if (dayEnded)
        {
            endDay();
        }
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
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
                unpause();
            else
                pause();
        }
    }

    private void OnApplicationPause(bool isPause)
    {
        if (isPause)
            pause();
        else
            unpause();
    }

    public void pause()
    {
        overlay.gameObject.SetActive(true);
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);
        pauseMenu.SetActive(true);
        paused = true;
    }

    public void unpause()
    {
        overlay.gameObject.SetActive(false);
        pauseMenu.SetActive(false);
        paused = false;
    }

    public void goToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
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

    public void OnChoiceDone(GameControllerScript game, Animal animal)
    {
        foreach (IGameListener listener in listeners)
        {
            listener.OnChoiceDone(game, animal);
        }
    }
}
