using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinishSceneController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField]
    private Text title;
    [SerializeField]
    private int saveGoal = 300;
    [SerializeField]
    private int radioScene = 4;
    [SerializeField]
    private AnimalTypes animalTypes;

    [Header("Achievements")]
    [SerializeField]
    private Image achievementHuman;
    [SerializeField]
    private Image achievementMonkey;
    [SerializeField]
    private Image achievementVirus;

    [Header("Summary")]
    [SerializeField]
    private RectTransform sheepLine;
    [SerializeField]
    private RectTransform cowsLine;
    [SerializeField]
    private RectTransform pigsLine;
    [SerializeField]
    private RectTransform wildAndGoodLine;
    [SerializeField]
    private RectTransform wolvesReportedLine;
    [SerializeField]
    private RectTransform wolvesAcceptedLine;
    [SerializeField]
    private RectTransform illAcceptedLine;
    [SerializeField]
    private RectTransform healthyDeniedLine;
    [SerializeField]
    private RectTransform totalLine;

    [Header("Expenses")]
    [SerializeField]
    private RectTransform expensesLines;
    [SerializeField]
    private Sprite statusOk;
    [SerializeField]
    private Sprite statusX;

    [Header("Saved")]
    [SerializeField]
    private RectTransform savedLine;
    [SerializeField]
    private Button saveButton;
    
    private int moneyEarned;

    void Start()
    {
        title.text = string.Format("DAY #{0} IS DONE", PlayerPrefs.GetInt("dayId", 0) - 1);
        generateSummary();
        generateExpenses();
        updateSaved();
        updateAchievements();
    }

    private void generateSummary()
    {
        LevelResult levelResult = getLevelResult();

        setLineData(sheepLine, levelResult.getCountOf("sheep"), levelResult.getCountOf("sheep") * animalTypes.getTypeById("sheep").cost);
        setLineData(cowsLine, levelResult.getCountOf("cow"), levelResult.getCountOf("cow") * animalTypes.getTypeById("cow").cost);
        setLineData(pigsLine, levelResult.getCountOf("pig"), levelResult.getCountOf("pig") * animalTypes.getTypeById("pig").cost);
        setLineData(wildAndGoodLine, levelResult.getCountOf("goodAccepted"), levelResult.getCountOf("goodAccepted") * animalTypes.getTypeById("goodAccepted").cost);
        setLineData(wolvesReportedLine, levelResult.getCountOf("predatorCalled"), levelResult.getCountOf("predatorCalled") * animalTypes.getTypeById("predatorCalled").cost);
        setLineData(wolvesAcceptedLine, levelResult.getCountOf("predatorAccepted"), levelResult.getCountOf("predatorAccepted") * animalTypes.getTypeById("predatorAccepted").cost);
        setLineData(illAcceptedLine, levelResult.getCountOf("illAccepted"), levelResult.getCountOf("illAccepted") * animalTypes.getTypeById("illAccepted").cost);
        setLineData(healthyDeniedLine, levelResult.getCountOf("healthyDenied"), levelResult.getCountOf("healthyDenied") * animalTypes.getTypeById("healthyDenied").cost);

        int sum = 0;
        for (int i = 0; i < levelResult.types.Length; i++)
        {
            sum += levelResult.counts[i] * animalTypes.getTypeById(levelResult.types[i]).cost;
        }
        setLineData(totalLine, -1, sum);

        moneyEarned = sum;
    }

    private void setLineData(RectTransform line, int count, int money)
    {
        line.GetChild(1).GetComponent<Text>().text = count.ToString();
        line.GetChild(2).GetComponent<Text>().text = string.Format("{0}$", money);
        
        if (count == 0)
        {
            line.gameObject.SetActive(false);
            moveNextSiblings(line);
        }
    }

    private void moveNextSiblings(RectTransform line)
    {
        for (int i = line.GetSiblingIndex() + 1; i < line.parent.childCount; i++)
        {
            Vector3 pos = line.parent.GetChild(i).localPosition;
            pos.y += line.rect.height - 5;
            line.parent.GetChild(i).localPosition = pos;
        }
    }

    private void generateExpenses()
    {
        Expenses expenses = getExpenses();
        if (expenses == null)
        {
            expensesLines.parent.gameObject.SetActive(false);
            return;
        }

        RectTransform line = expensesLines.GetChild(0) as RectTransform;
        for (int i = 0; i < expenses.expenses.Length; i++)
        {
            int index = i;
            RectTransform newLine = Instantiate(line, line.parent);
            Vector3 pos = newLine.localPosition;
            pos.y -= (line.rect.height - 5) * i;
            newLine.localPosition = pos;

            newLine.GetComponentsInChildren<Text>()[0].text = expenses.expenses[i];
            newLine.GetComponentsInChildren<Text>()[1].text = expenses.cost[i].ToString();
            newLine.GetComponentInChildren<Button>().onClick.AddListener(() => expenseClick(expenses, index));

            if (moneyEarned < expenses.cost[i])
            {
                newLine.GetComponentsInChildren<Text>()[1].color = Color.red;
                newLine.GetComponentsInChildren<Image>()[2].color = Color.red;
            }
        }

        line.gameObject.SetActive(false);
    }

    private void expenseClick(Expenses expenses, int index)
    {        
        if (expensesLines.GetChild(index + 1).GetComponentsInChildren<Image>()[2].sprite == statusX)
        {
            Debug.Log(index + " x");
            if (expenses.cost[index] <= moneyEarned)
            {
                expensesLines.GetChild(index + 1).GetComponentsInChildren<Image>()[2].sprite = statusOk;
                moneyEarned -= expenses.cost[index];
                updateExpenses(expenses);
                updateSaved();
            }
        } else
        {
            Debug.Log(index + " ok");
            expensesLines.GetChild(index + 1).GetComponentsInChildren<Image>()[2].sprite = statusX;

            moneyEarned += expenses.cost[index];
            updateExpenses(expenses);
            updateSaved();
        }
    }

    private void updateExpenses(Expenses expenses)
    {
        if (expenses == null)
            return;

        for (int i = 0; i < expenses.expenses.Length; i++)
        {
            if (moneyEarned < expenses.cost[i])
            {
                if (expensesLines.GetChild(i + 1).GetComponentsInChildren<Image>()[2].sprite == statusX)
                {
                    expensesLines.GetChild(i + 1).GetComponentsInChildren<Text>()[1].color = Color.red;
                    expensesLines.GetChild(i + 1).GetComponentsInChildren<Image>()[2].color = Color.red;
                }
            } else
            {
                expensesLines.GetChild(i + 1).GetComponentsInChildren<Text>()[1].color = Color.white;
                expensesLines.GetChild(i + 1).GetComponentsInChildren<Image>()[2].color = Color.white;
            }
        }
    }

    private void updateSaved()
    {
        savedLine.GetComponentsInChildren<Text>()[1].text = string.Format("{0}$", PlayerPrefs.GetInt("saved"));
        savedLine.GetComponentsInChildren<Text>()[2].text = string.Format("{0}$", Mathf.Max(0, saveGoal - PlayerPrefs.GetInt("saved")));
        saveButton.GetComponentInChildren<Text>().text =
            (moneyEarned >= 0 ? "SAVE FOR FUTURE: " : "PAY FINES: ") + moneyEarned + "$";
    }

    private void updateAchievements()
    {

    }

    public void saveMoney()
    {
        PlayerPrefs.SetInt("saved", PlayerPrefs.GetInt("saved") + moneyEarned);
        PlayerPrefs.Save();
        moneyEarned = 0;
        updateExpenses(getExpenses());
        updateSaved();
    }

    public void nextDay()
    {
        saveMoney();

        // TODO save expenses

        SceneManager.LoadScene(radioScene);
    }

    private LevelResult getLevelResult()
    {
        return JsonUtility.FromJson<LevelResult>(PlayerPrefs.GetString("levelResult"));
    }

    private Expenses getExpenses()
    {
        Day day = Day.load(PlayerPrefs.GetInt("dayId") - 1);

        Expenses expenses = new Expenses();
        if (day.expenses == null)
            return null;
        expenses.expenses = new string[day.expenses.Length];
        expenses.cost = new int[day.expenses.Length];

        for (int i = 0; i < day.expenses.Length; i++)
        {
            expenses.expenses[i] = day.expenses[i].name;
            expenses.cost[i] = day.expenses[i].cost;
        }
        return expenses;
    }
}
