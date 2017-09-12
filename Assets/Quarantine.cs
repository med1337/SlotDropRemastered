using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class Quarantine : OsScreen
{
    public Slider QuarantineSlider;
    public Text ClassTitle;
    public Text ResultText;
    public ClassNotification ClassNotification;
    private QuarantineStatus _quarantineStatus;
    private float _quarantineProcessDuration;
    public float _quarantineTimer;
    private float _osDelay;
    public PcManager PcManagerRef;

    public float QuarantineResultCooldown;
    private bool _quarantineSuccess;

    private float _chanceOfQuarantineSuccess;

    // Use this for initialization
    // ReSharper disable once UnusedMember.Local
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _quarantineTimer += Time.deltaTime;
        switch (_quarantineStatus)
        {
            case QuarantineStatus.Idle:
                break;
            case QuarantineStatus.Processing:
                Process();
                break;
            case QuarantineStatus.Result:
                DisplayResult();
                break;
            default:
                break;
        }
    }

    private void DisplayResult()
    {
        if (!(_quarantineTimer >= QuarantineResultCooldown)) return;

        //reset timer
        _quarantineTimer = 0;

        //deactivate text
        ResultText.gameObject.SetActive(false);

        _quarantineStatus = QuarantineStatus.Idle;

        //send result to the manager
        PcManagerRef.QuarantineResult(_quarantineSuccess);
    }

    public bool AttemptQuarantine(float quarantineProcessDuration, float loadDelay, float procectionLevel,
        USBCharacter aCharacter)
    {
        if (_quarantineStatus != QuarantineStatus.Idle) return false;

        ClassTitle.text = aCharacter.loadout_name;
        ClassNotification.UpdatePhoto(aCharacter.loadout_name);

        _quarantineStatus = QuarantineStatus.Processing;

        _osDelay = loadDelay;
        _quarantineProcessDuration = quarantineProcessDuration;
        _chanceOfQuarantineSuccess = procectionLevel / 100;

        SetupQuarantineBar();

        _quarantineTimer = -_osDelay;

        return true;
    }

    private void SetupQuarantineBar()
    {
        QuarantineSlider.gameObject.SetActive(true);
        QuarantineSlider.maxValue = _quarantineProcessDuration;
        QuarantineSlider.value = QuarantineSlider.minValue;
    }

    public void Process()
    {
        QuarantineSlider.value = _quarantineTimer;
        if (!(_quarantineTimer >= _quarantineProcessDuration)) return;

        //reset timer
        _quarantineTimer = 0;

        //random chance
        var random = Random.Range(0.0f, 1.0f);
        _quarantineSuccess = random < _chanceOfQuarantineSuccess;

        //disable slider
        QuarantineSlider.gameObject.SetActive(false);

        //activate result text and adjust the text
        ResultText.gameObject.SetActive(true);

        if (_quarantineSuccess)
        {
            ResultText.text = "SUCCESS";
            ResultText.color = Color.green;
        }
        else
        {
            ResultText.text = "FAILURE";
            ResultText.color = Color.red;
        }

        //change quarantine state
        _quarantineStatus = QuarantineStatus.Result;
    }
};