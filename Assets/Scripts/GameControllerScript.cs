using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour
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
    [SerializeField]
    [Multiline]
    private string haveNothingToSayText;

    [Header("Systems")]
    [SerializeField]
    private SpawnSystem spawnSystem;
    [SerializeField]
    private AnalyseSystem analyseSystem;

    [Header("Audio")]
    [SerializeField]
    private AudioSource mainAudioSource;
    [SerializeField]
    private AudioClip animalSpeakSound;

    [Header("Special Costs")]
    [SerializeField]
    private int wildAndGoodCost = 5;
    [SerializeField]
    private int wolfReportedCost = 10;
    [SerializeField]
    private int wolfAcceptedCost = 10;
    [SerializeField]
    private int illAcceptedCost = 10;
    [SerializeField]
    private int healthyDeniedCost = 10;

    [Header("Rules")]
    [SerializeField]
    private RulesSetup rulesWindow;
    [SerializeField]
    private RulesSetup rulesTemplate;

    [Header("Top Panel")]
    [SerializeField]
    private Text dayCounter;
    [SerializeField]
    private Text watches;
    [SerializeField]
    private Button callButton;
    
    [Header("Animal Window")]
    [SerializeField]
    private Text animalTitle;
    [SerializeField]
    private Text animalName;
    [SerializeField]
    private Text animalType;
    [SerializeField]
    private Image animalImage;

    [Header("Templates")]
    [SerializeField]
    private RectTransform voiceTemplate;
    [SerializeField]
    private RectTransform footrpintsTemplate;

    [Header("Other")]
    [SerializeField]
    private VoiceDiagramGenerator voiceDiagramPrefab;
    [SerializeField]
    private Image footprintPrefab;
    [SerializeField]
    private Image textCardPrefab;
    [SerializeField]
    private Image penaltyPrefab;
    [SerializeField]
    private Image overlay;
    [SerializeField]
    private Button analyseButton;
    

    [Header("FirstDay")]
    [SerializeField]
    private RectTransform firstDayFootrpints;
    [SerializeField]
    private Sprite firstDayVoice;

    [Header("Tutorials")]
    [SerializeField]
    private GameObject voiceTutorial;
    [SerializeField]
    private GameObject footrpintsTutorial;
    [SerializeField]
    private GameObject analyseTutorial;

    private Day currentDay;
    private int animalCounter = -1;
    private bool footprintsAdded;
    private bool analyseAdded;
    private bool voiceDiagramAdded;
    private bool infoCardAdded;
    private bool voiceCardAdded;
    private bool footprintsCardAdded;
    private bool analyseCardAdded;
    private bool waitNext;
    private bool penaltyAdded = false;
    private List<HistoryEntry> history = new List<HistoryEntry>();

    private int dayEndTime = 18;
    private int h = 9;
    private int m = 0;

    void Start()
    {
        loadDay();

        if (currentDay.id == 1)
        {
            voiceTutorial.SetActive(true);
            footrpintsTutorial.SetActive(true);
        }

        rulesWindow.setup(currentDay.id, getRules());
        rulesTemplate.setup(currentDay.id, getRules());
        setupTopPanel();

        analyseSystem.setupAnalyseTemplate(currentDay.analysTemplate, currentDay.analysTemplateColors);
        
        specialLevelSettings();
    }

    private void loadDay()
    {
        int dayId = 3;  PlayerPrefs.GetInt("dayId");
        currentDay = Day.load(dayId);
    }

    private Animal getCurrentAnimal()
    {
        return currentDay.animals[animalCounter];
    }

    private List<string> getRules()
    {
        List<string> rules = new List<string>();
        
        foreach (Rule rule in currentDay.rules)
        {
            rules.Add(rule.rule);
        }

        return rules;
    }

    private void setupTopPanel()
    {
        dayCounter.text = string.Format("DAY #{0}", currentDay.id);
        setupWatches(h, m);
    }

    private void setupAnimalWindow()
    {
        Animal animal = getCurrentAnimal();
        animalTitle.text = string.Format("ANIMAL #{0}", animalCounter + 1);
        animalName.text = animal.name;
        animalType.text = animal.typeName;
        animalImage.sprite = images.getImageById(animal.iconId);

        waitNext = false;

        footprintsAdded = false;
        analyseAdded = false;
        voiceDiagramAdded = false;

        analyseCardAdded = false;
        footprintsCardAdded = false;
        infoCardAdded = false;
        voiceCardAdded = false;
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
            if (m % 10 == 0)
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

    public void animalInteract()
    {
        if (waitNext)
            return;

        bool infoNow = false;
        if (!infoCardAdded && getCurrentAnimal().info != null)
        {
            addTextCard(getCurrentAnimal().info);
            infoCardAdded = true;
            infoNow = true;
        }

        if (!voiceCardAdded && voiceDiagramAdded && getCurrentAnimal().voice != getCurrentAnimal().typeId && getCurrentAnimal().infoVoice != null)
        {
            addTextCard(getCurrentAnimal().infoVoice);
            voiceCardAdded = true;
        } else
        if (!footprintsCardAdded && footprintsAdded && getCurrentAnimal().footprints != getCurrentAnimal().typeId && getCurrentAnimal().infoFootprints != null)
        {
            addTextCard(getCurrentAnimal().infoFootprints);
            footprintsCardAdded = true;
        } else
        if (!analyseCardAdded && analyseAdded && !analyseSystem.isAnalyseGood(getCurrentAnimal()) && getCurrentAnimal().infoAnalyse != null)
        {
            addTextCard(getCurrentAnimal().infoAnalyse);
            analyseCardAdded = true;
        } else
        if (!infoNow)
        {
            addTextCard(haveNothingToSayText);
        }
    }

    private void addTextCard(string text)
    {
        if (text == null)
            return;

        Image textCard = spawnSystem.spawnPrefab(textCardPrefab.GetComponent<RectTransform>()).GetComponent<Image>();
        textCard.GetComponentsInChildren<Text>()[0].text = string.Format("MESSAGE FROM {0}", getCurrentAnimal().name);
        textCard.GetComponentsInChildren<Text>()[1].text = text;
    }

    public void addVoiceDiagram()
    {
        mainAudioSource.clip = animalSpeakSound;
        mainAudioSource.Play();

        AnimalType animalType = animalTypes.getTypeById(getCurrentAnimal().typeId);

        VoiceDiagramGenerator voiceDiagram = spawnSystem.spawnPrefab(voiceDiagramPrefab.GetComponent<RectTransform>()).GetComponent<VoiceDiagramGenerator>();
        voiceDiagram.init(animalType.voiceMin, animalType.voiceMax, true, 2);
        voiceDiagram.generate();

        voiceDiagramAdded = true;
    }

    public void addAnimalFootrpint()
    {
        if (waitNext)
            return;
        
        if (!footprintsAdded)
        {
            AnimalType animalType = animalTypes.getTypeById(getCurrentAnimal().footprints);

            Image footprint = spawnSystem.spawnPrefab(footprintPrefab.GetComponent<RectTransform>()).GetComponent<Image>();
            footprint.sprite = animalType.footprintsImage;
            
            footprintsAdded = true;
        }
    }

    public void addAnimalAnalyse()
    {
        if (waitNext)
            return;

        //if (currentDay.id == 3 && animalCounter == 0)
        {
            //analyseTutorial.SetActive(true);
        }

        if (!analyseAdded)
        {
            analyseSystem.addAnalyse(getCurrentAnimal().analyse);
            analyseAdded = true;
        }
    }

    private void addToHistory(Animal animal, string action)
    {
        if (currentDay.id == 1 && animalCounter == 2)
        {
            overlay.gameObject.SetActive(true);
            StartCoroutine(firstDayEnd());
        }
        if (!isChoiceGood(animal, action) && !penaltyAdded)
        {
            addPenaltyCard();
            penaltyAdded = true;
        }
        else
        {
            history.Add(new HistoryEntry(animal, action));
        }
    }

    private bool isChoiceGood(Animal animal, string action)
    {
        string trueType = animal.typeId;
        if (animal.voice != animal.typeId)
            trueType = animal.voice;
        if (animal.footprints != animal.typeId)
            trueType = animal.footprints;

        foreach (Rule rule in currentDay.rules)
        {
            foreach(string noValue in rule.noValues)
            {
                if (animal.typeId.Equals(noValue) && action.Equals("accept"))
                    return false;
                else if (animal.typeId.Equals(noValue) && action.Equals("deny"))
                    return true;

                if (noValue.Equals("infected") && !analyseSystem.isAnalyseGood(animal) && action.Equals("accept"))
                    return false;
                else if (noValue.Equals("infected") && !analyseSystem.isAnalyseGood(animal) && action.Equals("deny"))
                    return true;
            }
        }

        bool shouldCall = animal.typeId == "wolf" || animal.voice == "wolf" || animal.footprints == "wolf";
        if (action == "call" && shouldCall)
            return true;

        if (action == "deny" && shouldCall)
            return true;
        if (action == "accept" && !shouldCall)
            return true;
        return false;
    }

    private void addPenaltyCard()
    {
        spawnSystem.spawnPrefab(penaltyPrefab.GetComponent<RectTransform>());
    }

    public void denyAnimal()
    {
        addToHistory(currentDay.animals[animalCounter], "deny");
        makeRemoveable();
    }

    public void callAnimal()
    {
        addToHistory(currentDay.animals[animalCounter], "call");
        makeRemoveable();
    }

    public void acceptAnimal()
    {
        addToHistory(currentDay.animals[animalCounter], "accept");
        makeRemoveable();
    }

    public void nextAnimal()
    {
        if (animalCounter == -1)
        {
            startDayTimer();
        }

        animalCounter++;
        setupAnimalWindow();
        specialLevelSettings();
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
        PlayerPrefs.SetString("levelResult", JsonUtility.ToJson(generateLevelResult()));
        PlayerPrefs.SetInt("dayId", currentDay.id + 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(levelFinishScene);
    }

    private LevelResult generateLevelResult()
    {
        LevelResult levelResult = new LevelResult();
        levelResult.counts = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        levelResult.moneyChanges = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

        foreach (HistoryEntry entry in history)
        {
            int type = countAnimal(entry.animal, entry.action);
            if (type >= 0 && type < levelResult.counts.Length)
                levelResult.counts[type] += 1;
        }

        levelResult.moneyChanges[0] = levelResult.counts[0] * animalTypes.getTypeById("sheep").cost;
        levelResult.moneyChanges[1] = levelResult.counts[1] * animalTypes.getTypeById("cow").cost;
        levelResult.moneyChanges[2] = levelResult.counts[2] * animalTypes.getTypeById("pig").cost;
        levelResult.moneyChanges[3] = levelResult.counts[3] * wildAndGoodCost;
        levelResult.moneyChanges[4] = levelResult.counts[4] * wolfReportedCost;
        levelResult.moneyChanges[5] = - levelResult.counts[5] * wolfAcceptedCost;
        levelResult.moneyChanges[6] = - levelResult.counts[6] * illAcceptedCost;
        levelResult.moneyChanges[7] = - levelResult.counts[7] * healthyDeniedCost;

        return levelResult;
    }

    private int countAnimal(Animal animal, string action)
    {
        bool isWolf = animal.typeId == "wolf" || animal.voice == "wolf" || animal.footprints == "wolf";
        if (action == "call" && isWolf)
            return 4;
        if (action == "accept" && isWolf)
            return 5;

        foreach (Rule rule in currentDay.rules)
        {
            foreach (string noValue in rule.noValues)
            {
                if (animal.typeId.Equals(noValue) && action.Equals("accept"))
                    return 5;

                if (noValue.Equals("infected") && !analyseSystem.isAnalyseGood(animal) && action.Equals("accept"))
                    return 6;
            }
        }

        if (!isWolf)
        {
            if (analyseSystem.isAnalyseGood(animal) && (action.Equals("deny") || action.Equals("call")))
                return 7;
            switch (animal.typeId)
            {
                case "sheep":
                    return 0;
                case "cow":
                    return 1;
                case "pig":
                    return 2;
                default:
                    return 3;
            }
        }
        return -1;
        //throw new System.Exception("Unhandled animal case: " + animal.name + " " + action);
    }

    public void specialLevelSettings()
    {
        if (currentDay.id == 1)
        {
            //analyseTemplate.gameObject.SetActive(false);
            analyseButton.gameObject.SetActive(false);
            callButton.gameObject.SetActive(false);

            footrpintsTemplate.gameObject.SetActive(false);
            firstDayFootrpints.gameObject.SetActive(true);

            voiceTemplate.GetComponent<Image>().sprite = firstDayVoice;
        }

        if (currentDay.id == 2)
        {
            //analyseTemplate.gameObject.SetActive(false);
            analyseButton.gameObject.SetActive(false);
        }
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
}
