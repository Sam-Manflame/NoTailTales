using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControllerScript : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField]
    private float minutesInSecond = 2;
    [SerializeField]
    private AnimalTypes animalTypes;
    [SerializeField]
    private Images images;
    [SerializeField]
    [Multiline]
    private string haveNothingToSayText;
    
    [Header("Rules Window")]
    [SerializeField]
    private Text rulesTitle;
    [SerializeField]
    private Text rules;
    [SerializeField]
    private Text rulesButtonText;
    
    [Header("Top Panel")]
    [SerializeField]
    private Text dayCounter;
    [SerializeField]
    private Text watches;
    
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
    private RectTransform analyseTemplate;
    [SerializeField]
    private RectTransform rulesTemplate;

    [Header("Other")]
    [SerializeField]
    private VoiceDiagramGenerator voiceDiagramPrefab;
    [SerializeField]
    private Image footprintPrefab;
    [SerializeField]
    private RectTransform analysePrefab;
    [SerializeField]
    private Image textCardPrefab;
    [SerializeField]
    private Image penaltyPrefab;

    private Day currentDay;
    private int animalCounter = 0;
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

    private int h = 9;
    private int m = 0;

    void Start()
    {
        loadDay();
        setupRules(rulesTitle, rules);
        setupTopPanel();
        setupAnimalWindow();
        setupAnalyseTemplate();
        setupRulesTemplate();
    }

    private void loadDay()
    {
        if (!PlayerPrefs.HasKey("dayId"))
        {
            PlayerPrefs.SetInt("dayId", 1);
            PlayerPrefs.Save();
        }

        int dayId = PlayerPrefs.GetInt("dayId");
        currentDay = Day.load(dayId);
    }

    private Animal getCurrentAnimal()
    {
        return currentDay.animals[animalCounter];
    }

    private void setupRules(Text title, Text rules)
    {
        title.text = string.Format("RULESET: DAY #{0}", currentDay.id);

        rules.text = "";

        List<string> rulesStrings = getRules();
        foreach (string rule in rulesStrings)
        {
            rules.text += "- " + rule + (rulesStrings.IndexOf(rule) != rulesStrings.Count - 1 ? "\n" : "");
        }
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
        Animal animal = currentDay.animals[animalCounter];
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
    
    private void setupAnalyseTemplate()
    {
        for (int i = 0; i < analyseTemplate.childCount; i++)
        {
            Image slot = analyseTemplate.GetChild(i).GetComponent<Image>();
            slot.color = currentDay.analysTemplateColors[i] == 0 ? Color.black : Color.clear;
            Text slotText = slot.transform.GetChild(0).GetComponent<Text>();
            slotText.color = currentDay.analysTemplateColors[i] == 0 ? Color.white : Color.black;
            slotText.text = string.Format(
                "{0}{1}",
                Mathf.Abs(currentDay.analysTemplate[i]),
                currentDay.analysTemplate[i] > 0 ? "+" : "-");
        }
    }

    private void setupRulesTemplate()
    {
        Text templateTitle = rulesTemplate.GetChild(0).GetComponent<Text>();
        Text templateRules = rulesTemplate.GetChild(1).GetComponent<Text>();

        setupRules(templateTitle, templateRules);
    }

    public void startDayTimer()
    {
        StartCoroutine(dayTimer());
    }

    private IEnumerator dayTimer()
    {
        while (h < 18)
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
        if (!analyseCardAdded && analyseAdded && !isAnalyseGood(getCurrentAnimal()) && getCurrentAnimal().infoAnalyse != null)
        {
            addTextCard(getCurrentAnimal().infoAnalyse);
            analyseCardAdded = true;
        } else
        if (!infoNow)
        {
            addTextCard(haveNothingToSayText);
        }
    }

    private bool isAnalyseGood(Animal animal)
    {
        for (int i = 0; i < currentDay.analysTemplate.Length; i++)
        {
            int sum =
                animal.analyse[i] +
                animal.analyse[i + 6] +
                animal.analyse[i + 12] +
                animal.analyse[i + 18] +
                animal.analyse[i + 24];

            if (currentDay.analysTemplateColors[i] == 0)
            {
                sum = 5 - sum;
            }

            if (currentDay.analysTemplate[i] > 0 && sum >= currentDay.analysTemplate[i])
                continue;
            else if (currentDay.analysTemplate[i] < 0 && sum <= Mathf.Abs(currentDay.analysTemplate[i]))
                continue;
            else
                return false;
        }
        return true;
    }

    private void addTextCard(string text)
    {
        if (text == null)
            return;

        Image textCard = Instantiate(textCardPrefab, analyseTemplate.parent);
        textCard.GetComponentsInChildren<Text>()[0].text = string.Format("MESSAGE FROM {0}", getCurrentAnimal().name);
        textCard.GetComponentsInChildren<Text>()[1].text = text;
    }

    public void addVoiceDiagram()
    {
        AnimalType animalType = animalTypes.getTypeById(currentDay.animals[animalCounter].typeId);
        VoiceDiagramGenerator voiceDiagram = Instantiate(voiceDiagramPrefab, analyseTemplate.parent);
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
            AnimalType animalType = animalTypes.getTypeById(currentDay.animals[animalCounter].footprints);
            Image footprint = Instantiate(footprintPrefab, analyseTemplate.parent);
            footprint.sprite = animalType.footprintsImage;

            footprintsAdded = true;
        }
    }

    public void addAnimalAnalyse()
    {
        if (waitNext)
            return;

        if (!analyseAdded)
        {
            RectTransform analyse = Instantiate(analysePrefab, analyseTemplate.parent);
            for (int i = 0; i < analyse.childCount; i++)
            {
                //Debug.Log(currentDay.animals[animalCounter].analyse);
                analyse.GetChild(i).GetComponent<Image>().color = 
                    currentDay.animals[animalCounter].analyse[i] == 0 ? Color.black : Color.clear;
            }

            analyseAdded = true;
            //Debug.Log(isAnalyseGood());
        }
    }

    private void addToHistory(Animal animal, string action)
    {
        history.Add(new HistoryEntry(animal, action));
        if (!isChoiceGood(animal, action) && !penaltyAdded)
        {
            addPenaltyCard();
            penaltyAdded = true;
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

                if (noValue.Equals("infected") && !isAnalyseGood(animal) && action.Equals("accept"))
                    return false;
                else if (noValue.Equals("infected") && !isAnalyseGood(animal) && action.Equals("deny"))
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
        Instantiate(penaltyPrefab, analyseTemplate.parent);
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
        animalCounter++;
        setupAnimalWindow();
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

    }
}
