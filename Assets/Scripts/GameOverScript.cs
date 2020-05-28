using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : SettingsScreenScirpt
{
    [SerializeField]
    private Text reason;
    [SerializeField]
    private Image leftIcon;
    [SerializeField]
    private Image rightIcon;
    [SerializeField]
    private Text middleInfo;

    [Header("Icons")]
    public Sprite wolfIcon;
    public Sprite skullIcon;
    public Sprite pigIcon;
    public Sprite billIcon;
    public Sprite waterIcon;
    public Sprite foodIcon;

    private void Start()
    {
        string gameover = PlayerPrefs.GetString("gameover");

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        rightIcon.sprite = skullIcon;

        switch (gameover)
        {
            case "GET FOOD":
                leftIcon.sprite = foodIcon;
                reason.text = "DIED - NO FOOD";
                middleInfo.text = "x     4 DAYS     =";
                break;
            case "GET WATER":
                leftIcon.sprite = waterIcon;
                reason.text = "DIED - NO WATER";
                middleInfo.text = "x     2 DAYS     =";
                break;
            case "KILL BILL":
                leftIcon.sprite = billIcon;
                reason.text = "KILLED BY BILL";
                middleInfo.text = "x     14 DAYS     =";
                break;
            case "PREDATORS":
                leftIcon.sprite = wolfIcon;
                reason.text = "FIRED - MISSED 5 PREDATORS";
                middleInfo.text = "x       5       =";
                break;
            case "HEALTHY":
                leftIcon.sprite = pigIcon;
                reason.text = "FFIRED - MISSED 7 HEALTHY";
                middleInfo.text = "x       -7       =";
                break;
            default:
                throw new System.Exception("WRONG GAME OVER");
        }
    }
}
