using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PenaltySetup : MonoBehaviour
{
    [SerializeField]
    private Text reason;
    [SerializeField]
    private Text fee;
    [SerializeField]
    private string noFeeString = "NO FEE";

    public void setup(string reason, string animalName, int fee)
    {
        this.reason.text = reason + " - " + animalName;
        this.fee.text = fee == 0 ? noFeeString : fee + "$";
    }
}
