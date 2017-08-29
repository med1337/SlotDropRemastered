using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


enum PCState
{
    Welcome = 0,
    Load,
    Running,
    Freeze,
    BlueScreen,
    Reboot
}

public class PcManager : MonoBehaviour
{
    [Header("DEBUG")] public bool ActivatePopup;
    public bool ActivateQuarantine;
    public bool ActivateReboot;
    public bool ActivateBluescren;

    [Header("GameObjects")] public GameObject CursorGameObject;
    public GameObject QuarantineGameObject;
    public Slider ProtectionSlider;
    public Slider TemperatureSlider;
    public GameObject RebootGameObject;
    public Slider RebootSlider;
    public GameObject WelcomeGameObject;
    public GameObject BluescreenGameObject;

    [Header("Time settings")] public float CursorFreezeTimeDuration;
    [Space(10)] public float RebootDuration;
    [Space(10)] public float BluescreenDuration;
    public float loadDelay;

    [Space(10)] public float QuarantineProcessDuration;
    public float QuarantinePopupCooldown;

    [Header("Slider settings")] [Range(1, 10)] public float ProtectionUpdateRate;
    [Range(0, 1)] public float ProtectionUpdateStep;
    [Range(0, 50)] public float TemperatureStep;


    [Header("Sprites")] [Tooltip("1st element - processing, 2nd - success, 3rd - failure")]
    public List<Sprite> QuarantineSprites;

    [Space(10)] public List<Sprite> PopupImages;

    //private
    private float _cursorSpeed;

    private float _protectionTimer = 0;

    private PCState _pcState;
    private float _rebootTimer = -1;

    private bool _rebootAfterBsod;

    private QuarantineStatus _quarantineStatus;
    private bool _quarantineSuccess; // could do in enum but whatever

    private List<GameObject> _popups;
    private GameObject _lastGameObject;
    private bool _popupClosed;

    private Vector3 _targetVector3;

    private float _popupTimer;
    private float _quarantineTimer;
    private float _protectionUpdateRate;
    


    // Use this for initialization
    void Start()
    {
        _popups = new List<GameObject>();
        _protectionUpdateRate = 1 / ProtectionUpdateRate;
        RebootSlider.maxValue = RebootDuration;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_pcState)
        {
            case PCState.Running:
                DebugOptions();
                ProcessPopups();
                ProcessProtectionBar();
                break;
            case PCState.Freeze:
                ProcessQuarantine();
                break;
            case PCState.BlueScreen:
                ProcessBluescreen();
                break;
            case PCState.Reboot:
                ProcessReboot();
                break;
            case PCState.Welcome:
                ProcessWelcome();
                break;
            case PCState.Load:
                ProcessLoad();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void DebugOptions()
    {
        //editor
        _protectionUpdateRate = 1 / ProtectionUpdateRate;
        RebootSlider.maxValue = RebootDuration;

        //debug
        if (ActivatePopup)
        {
            Popup(1);
            ActivatePopup = false;
        }

        if (ActivateQuarantine)
        {
            AttemptQuarantine();
            ActivateQuarantine = false;
        }

        if (ActivateBluescren)
        {
            Bluescreen(BluescreenDuration, true);
            ActivateBluescren = false;
        }
        if (ActivateReboot)
        {
            Reboot(RebootDuration);
            ActivateReboot = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            IncreaseTemperature();
    }

    private void ProcessBluescreen()
    {
        //update timer
        _rebootTimer += Time.deltaTime;

        if (!(_rebootTimer > BluescreenDuration - loadDelay)) return;

        //reinitialise timer
        _rebootTimer = -loadDelay;

        //reboot if needed
        if (_rebootAfterBsod)
            Reboot(RebootDuration);
    }

    private void ProcessWelcome()
    {
        //update timer
        _rebootTimer += Time.deltaTime;

        if (!(_rebootTimer > 0)) return;

        //reinitialise timer
        _rebootTimer = -loadDelay;


        //change pc state
        _pcState = PCState.Load;
        WelcomeGameObject.SetActive(false);
    }

    private void ProcessLoad()
    {
        //update timer
        _rebootTimer += Time.deltaTime;

        if (!(_rebootTimer > 0)) return;

        //reinitialise timer
        _rebootTimer = -loadDelay;

        //update pc state and enable pc health scanner
        _pcState = PCState.Running;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void ProcessReboot()
    {
        //update timer
        _rebootTimer += Time.deltaTime;

        //update timer after initial delay
        if (_rebootTimer >= 0)
        {
            RebootSlider.value = _rebootTimer;
        }

        if (!(_rebootTimer > RebootDuration - loadDelay)) return;

        //reinitialise timer
        _rebootTimer = -loadDelay;

        //disable reboot screen
        RebootGameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);

        //reload bars
        RestartBars();

        //proceed to next state and enable welcome screen
        _pcState = PCState.Welcome;
        WelcomeGameObject.SetActive(true);
        WelcomeGameObject.transform.SetAsLastSibling();
    }

    public void Bluescreen(float duration, bool rebootAfterBsod)
    {
        _rebootAfterBsod = rebootAfterBsod;
        BluescreenDuration = duration;

        //change state
        _pcState = PCState.BlueScreen;

        //activate image
        BluescreenGameObject.SetActive(true);
        BluescreenGameObject.transform.SetAsLastSibling();
    }


    public void Reboot(float rebootDuration)
    {
        RebootDuration = rebootDuration;

        //change pc state
        _pcState = PCState.Reboot;

        //update screen
        RebootGameObject.SetActive(true);
        RebootGameObject.transform.SetAsLastSibling();
        BluescreenGameObject.SetActive(false);
    }

    private void ProcessPopups()
    {
        if (_quarantineStatus != QuarantineStatus.Idle) return;

        if (!_popupClosed)
        {
            if (_popups.Count <= 0) return;
            _cursorSpeed = _popups.Count;
            _lastGameObject = _popups[_popups.Count - 1];
            ClosePopup();
        }
        else
        {
            _popupTimer += Time.deltaTime;
            if (!(_popupTimer > CursorFreezeTimeDuration)) return;
            _popupTimer = 0;
            _popupClosed = false;
        }
    }

    public void RestartBars()
    {
        TemperatureSlider.value = 0;
        ProtectionSlider.value = 100;
        RebootSlider.value = 0;
    }

    private float IncreaseTemperature()
    {
        if (TemperatureSlider.value + TemperatureStep <= 100)
        {
            TemperatureSlider.value += TemperatureStep;

            if (TemperatureSlider.value >= 100)
            {
                StartCoroutine(TriggerCataclysm());
            }
        }
        return TemperatureSlider.value;
    }


    private IEnumerator TriggerCataclysm()
    {
        Light dir_light = GameObject.Find("DirectionalLight").GetComponent<Light>();
        Color prev_color = dir_light.color;
        dir_light.color = Color.red;

        GameManager.scene.meteor_manager.SpawnMeteors(BluescreenDuration);
        GameManager.scene.slot_manager.enabled = false;

        AudioManager.PlayOneShot("alarm");

        Bluescreen(BluescreenDuration, true);
        GameManager.scene.focus_camera.Focus(GameManager.scene.pc_manager.transform.position, 9, 0.5f);

        yield return new WaitForSeconds(BluescreenDuration);

        dir_light.color = prev_color;
        yield return new WaitForSeconds(RebootDuration);

        GameManager.scene.slot_manager.enabled = true;
    }


    private void ClosePopup()
    {
        if (_lastGameObject == null) return;

        _targetVector3 = _lastGameObject.transform.localPosition;
        _targetVector3 += CalculatePopupCloseOffset(_lastGameObject);

        CursorGameObject.transform.localPosition = Vector3.MoveTowards(CursorGameObject.transform.localPosition,
            _targetVector3, Time.deltaTime * _cursorSpeed);

        if (CursorGameObject.transform.localPosition != _targetVector3) return;

        _popups.Remove(_lastGameObject);
        GameObject.Destroy(_lastGameObject);
        _popupClosed = true;
    }


    Vector3 CalculatePopupCloseOffset(GameObject _popup)
    {
        RectTransform popup_rect = _lastGameObject.GetComponent<Image>().rectTransform;
        if (popup_rect)
        {
            const float border_offset_percent_x = 0.93f;
            const float border_offset_percent_y = 0.87f;

            float x = (popup_rect.sizeDelta.x * _lastGameObject.transform.localScale.x) *
                0.5f * border_offset_percent_x;

            float y = (popup_rect.sizeDelta.y * _lastGameObject.transform.localScale.y) *
                0.5f * border_offset_percent_y;

            return new Vector3(x, y);
        }

        return Vector3.zero;
    }


    public void ProcessProtectionBar()
    {
        //update timer
        _protectionTimer += Time.deltaTime;

        if (!(_protectionTimer >= _protectionUpdateRate)) return;

        //update timer if appropriate
        if (ProtectionSlider.value-ProtectionUpdateStep >= 0)
        {
            ProtectionSlider.value -= ProtectionUpdateStep;
        }

        //reset timer
        _protectionTimer = 0;
    }

    private void Popup(int amount)
    {
        var i = 0;
        while (i < amount)
        {
            //remap position accordingly to scale
            var scale = Random.Range(0.01f, 0.02f);
            var posx = Remap(scale, 0.01f, 0.02f, 1.4f, 0.9f);
            var posy = Remap(scale, 0.01f, 0.02f, 0.55f, 0.0f);

            posx = Random.Range(-posx, posx);
            posy = Random.Range(-posy, posy);

            //initialise new popup
            var newpopupGameObject = new GameObject();

            //add sprite and randomize image
            var x = newpopupGameObject.AddComponent<Image>();
            x.sprite = PopupImages[Random.Range(0, PopupImages.Count)];

            //change transform of new popup
            newpopupGameObject.transform.SetParent(transform);
            newpopupGameObject.transform.localScale = new Vector3(scale, scale);
            newpopupGameObject.transform.localPosition = new Vector3(posx, posy);
            newpopupGameObject.transform.rotation = new Quaternion(0, 0, 0, 0);

            //add popup to array
            _popups.Add(newpopupGameObject);

            //increase counter
            i++;
        }

        //make sure cursor is on top of everything
        CursorGameObject.transform.SetAsLastSibling();
    }

    public void AttemptQuarantine()
    {
        if (_pcState != PCState.Running) return;

        //freeze pc
        _pcState = PCState.Freeze;

        //activate quarantine popup
        _quarantineStatus = QuarantineStatus.Processing;
        QuarantineGameObject.SetActive(true);

        //set appropriate image [0] for checking image
        QuarantineGameObject.GetComponent<Image>().sprite = QuarantineSprites[0];

        //make sure it is on top
        QuarantineGameObject.transform.SetAsLastSibling();

        //but doesn't cover the cursor ;)
        CursorGameObject.transform.SetAsLastSibling();
    }

    private void ProcessQuarantine()
    {
        switch (_quarantineStatus)
        {
            case QuarantineStatus.Idle:
                _pcState=PCState.Running;
                break;
            case QuarantineStatus.Processing:
                //update timer
                _quarantineTimer += Time.deltaTime;

                if (_quarantineTimer >= QuarantineProcessDuration)
                {
                    //reset timer
                    _quarantineTimer = 0;

                     float ChanceOfQuarantineSuccess = ProtectionSlider.value/100;

                    //random chance
                    var random = Random.Range(0.0f, 1.0f);
                    _quarantineSuccess = random < ChanceOfQuarantineSuccess;

                    //update image
                    QuarantineGameObject.GetComponent<Image>().sprite =
                        _quarantineSuccess ? QuarantineSprites[1] : QuarantineSprites[2];

                    //change quarantine state
                    _quarantineStatus = QuarantineStatus.Result;
                }
                break;
            case QuarantineStatus.Result:
                //update timer
                _quarantineTimer += Time.deltaTime;

                if (_quarantineTimer >= QuarantinePopupCooldown)
                {
                    //reset timer
                    _quarantineTimer = 0;

                    //deactivate image and set to idle
                    QuarantineGameObject.SetActive(false);
                    QuarantineGameObject.GetComponent<Image>().sprite = QuarantineSprites[0];
                    _quarantineStatus = QuarantineStatus.Idle;

                    //oh no :(
                    if (!_quarantineSuccess)
                    {
                        if(IncreaseTemperature()<100)
                        {
                            Popup(3);
                        }

                    }
                }
                break;
        }
    }

    private static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return ((value - from1) / (to1 - from1) * (to2 - from2)) + from2;
    }
}