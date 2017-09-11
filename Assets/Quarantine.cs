using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quarantine : MonoBehaviour
{
    private OsScreen _quarantineOsBackgrounds;
    private Slider _quarantineSlider;
    private Text _classTitle;
    private Text _resultText;
    public ClassNotification ClassNotification;

    // Use this for initialization
    void Start()
    {
        ClassNotification = GetComponentInChildren<ClassNotification>();
        _quarantineOsBackgrounds = GetComponent<OsScreen>();
        _quarantineSlider = GetComponentInChildren<Slider>();
        _classTitle = ClassNotification.GetComponentInChildren<Text>();
        _resultText = _quarantineSlider.gameObject.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void AttemptQuarantine()
    {
    }
};