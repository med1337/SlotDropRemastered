using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OsScreen : MonoBehaviour
{
    private Image _image;
    public List<Sprite> OsSprites;
    private PcManager _pcManager;

    void Start()
    {
        _pcManager = GetComponentInParent<PcManager>();
        _image = GetComponent<Image>();
    }

    void Update()
    {

    }

    public void ChangeOS(int version)
    {
        _image = GetComponent<Image>();
        _image.sprite = OsSprites[version];
    }

    


}