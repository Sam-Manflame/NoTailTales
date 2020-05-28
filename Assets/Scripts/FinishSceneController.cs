using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

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
    [SerializeField]
    private Expenses expensesObj;

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

    private Expenses expenses = null;

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

            newLine.GetComponentsInChildren<Text>()[0].text = expenses.expenses[i].name;
            newLine.GetComponentsInChildren<Text>()[1].text = expenses.expenses[i].cost.ToString();
            newLine.GetComponentInChildren<Button>().onClick.AddListener(() => expenseClick(expenses, index));

            newLine.GetComponentsInChildren<Button>()[1].onClick.AddListener(() => StartCoroutine(infoCoroutine(newLine.GetChild(newLine.childCount - 1))));

            newLine.GetChild(newLine.childCount - 1).GetComponentInChildren<Text>().text = string.Format(expenses.expenses[i].info, getExpenseDays(expenses.expenses[i]));

            if (moneyEarned < expenses.expenses[i].cost)
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
            if (expenses.expenses[index].cost <= moneyEarned)
            {
                expensesLines.GetChild(index + 1).GetComponentsInChildren<Image>()[2].sprite = statusOk;
                moneyEarned -= expenses.expenses[index].cost;
                updateExpenses(expenses);
                updateSaved();
            }
        } else
        {
            Debug.Log(index + " ok");
            expensesLines.GetChild(index + 1).GetComponentsInChildren<Image>()[2].sprite = statusX;

            moneyEarned += expenses.expenses[index].cost;
            updateExpenses(expenses);
            updateSaved();
        }
    }

    private IEnumerator infoCoroutine(Transform info)
    {
        info.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        info.gameObject.SetActive(false);
    }

    private void updateExpenses(Expenses expenses)
    {
        if (expenses == null)
            return;

        for (int i = 0; i < expenses.expenses.Length; i++)
        {
            if (moneyEarned < expenses.expenses[i].cost)
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

    private void saveExpenses()
    {
        Expenses expenses = getExpenses();

        for (int i = 0; i < expenses.expenses.Length; i++)
        {
            if (expensesLines.GetChild(i + 1).GetComponentsInChildren<Image>()[2].sprite == statusOk)
            {
                PlayerPrefs.SetInt(expenses.expenses[i].name, PlayerPrefs.GetInt("dayId"));
                Debug.Log(expenses.expenses[i].name + " was bought at day " + PlayerPrefs.GetInt("dayId"));
            }
        }
        PlayerPrefs.Save();
    }

    public void nextDay()
    {
        saveMoney();

        saveExpenses();

        SceneManager.LoadScene(radioScene);
    }

    private LevelResult getLevelResult()
    {
        return JsonUtility.FromJson<LevelResult>(PlayerPrefs.GetString("levelResult"));
    }

    private Expenses getExpenses()
    {
        if (expenses == null)
        {
            expenses = Instantiate(expensesObj) as Expenses;
            List<Expense> left = new List<Expense>();

            foreach (Expense expense in expensesObj.expenses)
            {
                if (!gotExpense(expense) || expense.infinite)
                {
                    left.Add(expense);
                    if (getExpenseDays(expense) == 0)
                    {
                        gameOver(expense.name);
                    }
                }
            }

            expenses.expenses = left.ToArray();
        }

        return expenses;
    }

    private bool gotExpense(Expense expense)
    {
        return PlayerPrefs.GetInt(expense.name, -1) != -1;
    }

    private int getExpenseDays(Expense expense)
    {
        return expense.day - (PlayerPrefs.GetInt("dayId") - PlayerPrefs.GetInt(expense.name, 2));
    }

    private void gameOver(string reason)
    {
        Debug.Log("game over because of " + reason);
    }
}
