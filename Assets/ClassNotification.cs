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
}