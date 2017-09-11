using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassNotification : MonoBehaviour
{
    private Sprite _characterSprite;
    public CharacterClasses CharacterClass
    {
        get { return CharacterClass; }
        set
        {
            _characterSprite = CharacterImages[(int) value];
            CharacterClass = value;
        }
    }

    public List<Sprite> CharacterImages;

    public List<string> CharacterDescriptions;

    public string GetCharacterDescription()
    {
        return CharacterDescriptions[(int) CharacterClass];
    }

    // Use this for initialization
	void Start ()
	{
	    _characterSprite = GetComponent<Image>().sprite;
	}
	
	// Update is called once per frame
	void Update () {
	   
	}

    
}
