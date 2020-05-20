﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulesSetup : MonoBehaviour
{
    [SerializeField]
    private Text titleText;
    [SerializeField]
    private Text rulesText;

    [Space]

    [SerializeField]
    private string dayCounterString = "RULESET: DAY #{0}";

    public void setup(int day, List<string> rules)
    {
        titleText.text = string.Format(dayCounterString, day);

        rulesText.text = "";
        foreach (string rule in rules)
        {
            rulesText.text += "- " + rule + (rules.IndexOf(rule) != rules.Count - 1 ? "\n" : "");
        }

        if (rulesText.GetComponent<PrintingTextMessage>() != null)
            rulesText.GetComponent<PrintingTextMessage>().setMessage(rulesText.text);
    }
}