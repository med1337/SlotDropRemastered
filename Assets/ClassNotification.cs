using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum ClassId
{
    Fisher = 0,
    Pirate,
    Trojan,
    Logger,
    Egyptian,
    Thief
}

public class ClassNotification : MonoBehaviour
{
    public Image CharacterSprite;

    public Text Line1Text;
    public Text Line2Text;
    public Text Line3Text;
    public Text ActionText;

    private ClassId x;
    [Tooltip("Fisher\nPirate\nTrojan\nlogger\nEgyptian\nThief")]
    [Header("1. Fisher;Pirate;Trojan;logger;Egyptian;Thief")] public List<Sprite> CharacterImages;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdatePhoto(string name)
    {
        var xd = Enum.Parse(typeof(ClassId), name);
        x = (ClassId) xd;
        CharacterSprite.sprite = CharacterImages[(int) x];
    }
    public void SetupClassMessages(string characterClass)
    {
        switch (characterClass)
        {
            case "Trojan":
                Line1Text.text = "Warning";
                Line2Text.text = "Trojan";
                Line3Text.text = "Detected";
                ActionText.text = "Attempting quarantine...";
                break;
            case "Pirate":
                Line1Text.text = "Warning";
                Line2Text.text = "Illegal software detected";
                Line3Text.text = "Detected";
                ActionText.text = "Calling the police...";
                break;
            case "Egyptian":
                Line1Text.text = "Warning";
                Line2Text.text = "Legacy software";
                Line3Text.text = "Detected";
                ActionText.text = "Updating old software...";
                break;
            case "Logger":
                Line1Text.text = "Warning";
                Line2Text.text = "Keylogger";
                Line3Text.text = "Detected";
                ActionText.text = "Removing malware...";
                break;
            case "Thief":
                Line1Text.text = "Warning";
                Line2Text.text = "Ransomware";
                Line3Text.text = "Detected";
                ActionText.text = "Transfering bitcoins...";
                break;
            case "Fisher":
                Line1Text.text = "Warning";
                Line2Text.text = "Phishing attack";
                Line3Text.text = "Detected";
                ActionText.text = "Marking as junk...";
                break;
        }
    }
}