using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Rule
{
    public string rule;

    public string[] noValues;

    public Rule()
    {

    }

    public Rule(string rule, string noValue)
    {
        this.rule = rule;
        this.noValues = new string[1] { noValue };
    }
}
