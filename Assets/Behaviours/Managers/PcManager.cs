using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum PCState
{
    None = 0,
    Upgrade,
    Titan,
    MeteorRain
}

public enum OSState
{
    Welcome = 0,
    Load = 1,
    Running,
    Freeze,
    BlueScreen,
    Reboot
}

public enum CurrentOS
{
    XP = 0,
    Vista = 1,
    Seven,
    Eight,
    Ten
}

public class PcManager : MonoBehaviour
{
    [Header("DEBUG")] public bool ActivatePopup;
    public bool ActivateQuarantine;
    public bool ActivateReboot;
    public bool ActivateBluescren;

    [Header("GameObjects")] public GameObject CursorGameObject;
    public Quarantine QuarantineGameObject;
    public Slider ProtectionSlider;
    public Slider TemperatureSlider;
    public OsScreen RebootGameObject;
    public Slider RebootSlider;
    public OsScreen WelcomeGameObject;
    public OsScreen BluescreenGameObject;
    public UpgradePC UpgradeManager;
    public OsScreen UpgradeGameObject;
    public OsScreen HealthOsScreen;
    public TitanScreen TitanScreenX;

    [Header("Time settings")] public float CursorFreezeTimeDuration;
    [Space(10)] public float RebootDuration;
    [Space(10)] public float BluescreenDuration;
    private const float LoadDelay = 1.0f;

    [Space(10)] public float QuarantineProcessDuration;
    public float QuarantinePopupCooldown;

    [Header("Slider settings")] [Range(1, 10)] public float ProtectionUpdateRate;
    [Range(0, 1)] public float ProtectionUpdateStep;
    [Range(0, 50)] public float TemperatureStep;


    [Header("Sprites")] [Tooltip("1st element - processing, 2nd - success, 3rd - failure")]
    public List<Sprite> QuarantineSprites;

    [Space(10)] public List<Sprite> PopupImages;

    [Space(10)] public CurrentOS SystemCurrentOs;


    public int TitanScore { get; set; }

    //private
    private float _cursorSpeed;

    private float _protectionTimer = 0;
    private float minimum_failure_chance = 10;

    public PCState PcState;
    private OSState _osState;
    private float _rebootTimer = -1;

    private bool _rebootAfterBsod;

    private List<GameObject> _popups;
    private GameObject _lastGameObject;
    private bool _popupClosed;

    private Vector3 _targetVector3;

    private float _popupTimer;
    private float _protectionUpdateRate;

    private OsScreen _osScreen;


    public void CancelUpgradeState()
    {
        if (PcState != PCState.Upgrade)
            return;

        UpgradeManager.CancelUpgrade();
        UpgradeGameObject.gameObject.SetActive(false);

        Reboot(3);
        ResetProtectionBarToMax();
    }


    // Use this for initialization
    void Start()
    {
        _popups = new List<GameObject>();
        _protectionUpdateRate = 1 / ProtectionUpdateRate;
        RebootSlider.maxValue = RebootDuration;
        _osScreen = GetComponent<OsScreen>();

        ChangeSettings();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.O))
        {
            UpgradeOs();
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            Bluescreen(BluescreenDuration, true);
        }
        if (Input.GetKeyUp(KeyCode.Q)) //debug
        {
            if (PcState == PCState.None)
            {
                PcState = PCState.Upgrade;
                StartCoroutine(TriggerUpgrade());
            }
        }
        if (Input.GetKeyUp(KeyCode.U))
        {
            if (PcState == PCState.None && _osState == OSState.Running)
            {
                UpgradeOs();
            }
        }

        ProcessPC();
        switch (PcState)
        {
            case PCState.None:
                break;
            case PCState.Upgrade:
                //process upgrade screen
                break;
            case PCState.Titan:
                //process titan screen
                break;
            case PCState.MeteorRain:
                break;
        }
    }


    private void ProcessPC()
    {
        switch (_osState)
        {
            case OSState.Running:
                DebugOptions();
                ProcessPopups();
                ProcessProtectionBar();
                ProcessTemperatureBar();
                break;
            case OSState.Freeze:
                break;
            case OSState.BlueScreen:
                ProcessBluescreen();
                break;
            case OSState.Reboot:
                ProcessReboot();
                break;
            case OSState.Welcome:
                ProcessWelcome();
                break;
            case OSState.Load:
                ProcessLoad();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ProcessTemperatureBar()
    {
        if (TemperatureSlider.value >= 100 && PcState == PCState.None)
        {
            GameManager.scene.stat_tracker.LogMeteorSwarm();

            PcState = PCState.MeteorRain;
            StartCoroutine(TriggerCataclysm());
        }
    }


    public void TriggerTitanState(USBCharacter _character)
    {
        if (PcState == PCState.Titan)
            return;

        PcState = PCState.Titan;
        _osState = OSState.Freeze;

        TitanScreenX.gameObject.SetActive(true);
        TitanScreenX.transform.SetAsLastSibling();

        StartCoroutine(TitanState(_character));
    }


    IEnumerator TitanState(USBCharacter _character)
    {
        Light dir_light = GameObject.Find("DirectionalLight").GetComponent<Light>();
        Color prev_color = dir_light.color;
        dir_light.color = new Color(0.5f, 0.5f, 0);

        AudioManager.PlayOneShot("alarm");

        Time.timeScale = 0.5f;

        yield return new WaitForSeconds(0.5f);

        Time.timeScale = 1;
        GameManager.scene.slot_manager.ActivateTitanSlots();

        yield return new WaitUntil(() => !GameManager.scene.respawn_manager.titan_exists);

        dir_light.color = prev_color;

        TitanScreenX.gameObject.SetActive(false);
        PcState = PCState.None;
        _osState = OSState.Running;

        BarBuffer();
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
            //AttemptQuarantine();
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

        if (!(_rebootTimer > BluescreenDuration)) return;

        //reinitialise timer
        _rebootTimer = -LoadDelay;

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
        _rebootTimer = -LoadDelay;


        //change pc state
        _osState = OSState.Load;
        WelcomeGameObject.gameObject.SetActive(false);
    }

    private void ProcessLoad()
    {
        //update timer
        _rebootTimer += Time.deltaTime;

        if (!(_rebootTimer > 0)) return;

        //reinitialise timer
        _rebootTimer = -LoadDelay;

        //prevent ticking cataclysm events from triggering too close to one another.
        BarBuffer();

        //update pc state and enable pc health scanner
        _osState = OSState.Running;
        PcState = PCState.None;
        GameManager.scene.slot_manager.enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }


    void BarBuffer()
    {
        if (ProtectionSlider.value <= 25)
            ProtectionSlider.value = 25;

        if (TemperatureSlider.value >= 100 - TemperatureStep)
            TemperatureSlider.value = 100 - TemperatureStep;
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

        if (!(_rebootTimer > RebootDuration)) return;

        //reinitialise timer
        _rebootTimer = -LoadDelay;

        //disable reboot screen
        RebootGameObject.gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);

        //reload bars
        RestartBars();

        //proceed to next state and enable welcome screen
        _osState = OSState.Welcome;
        WelcomeGameObject.gameObject.SetActive(true);
        WelcomeGameObject.transform.SetAsLastSibling();
    }

    public void Bluescreen(float duration, bool rebootAfterBsod)
    {
        _rebootAfterBsod = rebootAfterBsod;
        BluescreenDuration = duration;

        //change state
        _osState = OSState.BlueScreen;

        //activate image
        BluescreenGameObject.gameObject.SetActive(true);
        BluescreenGameObject.transform.SetAsLastSibling();
    }


    public void Reboot(float rebootDuration)
    {
        RebootDuration = rebootDuration;

        //change pc state
        _osState = OSState.Reboot;

        //update screen
        RebootGameObject.gameObject.SetActive(true);
        RebootGameObject.transform.SetAsLastSibling();
        BluescreenGameObject.gameObject.SetActive(false);
    }

    private void ProcessPopups()
    {
        if (_osState != OSState.Running) return;

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

        RebootSlider.value = 0;
    }

    public void SetProtectionBar(int _fill_amount)
    {
        ProtectionSlider.value = _fill_amount;
    }

    public void ResetProtectionBarToMax()
    {
        ProtectionSlider.value = ProtectionSlider.maxValue;
    }

    private float IncreaseTemperature()
    {
        if (PcState == PCState.None)
        {
            if (TemperatureSlider.value + TemperatureStep <= 100)
            {
                TemperatureSlider.value += TemperatureStep;
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
        GameManager.scene.focus_camera.Focus(GameManager.scene.pc_manager.transform.position, 9, 1f);

        yield return new WaitUntil(() => PcState == PCState.None);

        dir_light.color = prev_color;
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

        if (popup_rect == null)
            return Vector3.zero;

        const float border_offset_percent_x = 0.93f;
        const float border_offset_percent_y = 0.87f;

        float x = (popup_rect.sizeDelta.x * _lastGameObject.transform.localScale.x) *
                  0.5f * border_offset_percent_x;

        float y = (popup_rect.sizeDelta.y * _lastGameObject.transform.localScale.y) *
                  0.5f * border_offset_percent_y;

        return new Vector3(x, y);
    }


    public void ProcessProtectionBar()
    {
        //update timer
        _protectionTimer += Time.deltaTime;

        if (!(_protectionTimer >= _protectionUpdateRate) || GameManager.scene.respawn_manager.MorePlayersNeeded() &&
            !Input.GetKey(KeyCode.P))
        {
            return;
        }

        //update timer if appropriate
        if (ProtectionSlider.value - ProtectionUpdateStep >= 0)
        {
            ProtectionSlider.value -= ProtectionUpdateStep;
        }

        //reset timer
        _protectionTimer = 0;

        if (UpgradeManager != null)
        {
            if (ProtectionSlider.value <= 1 && PcState == PCState.None)
            {
                PcState = PCState.Upgrade;
                StartCoroutine(TriggerUpgrade());
            }
        }
    }


    IEnumerator TriggerUpgrade()
    {
        Light dir_light = GameObject.Find("DirectionalLight").GetComponent<Light>();
        Color prev_color = dir_light.color;
        dir_light.color = Color.blue;

        UpgradeManager.TriggerUpgrade();
        GameManager.scene.slot_manager.enabled = false;

        AudioManager.PlayOneShot("alarm");
        GameManager.scene.focus_camera.Focus(GameManager.scene.pc_manager.transform.position, 9, 1f);

        yield return new WaitUntil(() => PcState == PCState.None);

        dir_light.color = prev_color;
        GameManager.scene.slot_manager.enabled = true;
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

            //overlay of os frame
            var oSoverlayGameObject = new GameObject();
            var y = oSoverlayGameObject.AddComponent<Image>();
            y.sprite = HealthOsScreen.OsSprites[(int) SystemCurrentOs];

            oSoverlayGameObject.transform.SetParent(newpopupGameObject.transform);
            oSoverlayGameObject.transform.localScale = Vector3.one;
            oSoverlayGameObject.transform.localPosition = Vector3.zero;
            oSoverlayGameObject.transform.rotation = new Quaternion(0, 0, 0, 0);

            //add popup to array
            _popups.Add(newpopupGameObject);

            //increase counter
            i++;
        }

        //make sure cursor is on top of everything
        CursorGameObject.transform.SetAsLastSibling();
    }

    public void AttemptQuarantine(USBCharacter aCharacter)
    {
        if (_osState != OSState.Running || PcState != PCState.None) return;

        if (SystemCurrentOs == CurrentOS.Ten)
        {
            IncreaseTemperature();
            return;
        }

        //activate quarantine popup and make sure it is on top
        QuarantineGameObject.gameObject.SetActive(true);
        QuarantineGameObject.transform.SetAsLastSibling();

        //attempt quarantine
        if (!QuarantineGameObject.AttemptQuarantine(QuarantineProcessDuration, LoadDelay,
            ProtectionSlider.value - minimum_failure_chance, aCharacter))
        {
            QuarantineGameObject.gameObject.SetActive(false);
            return;
        }

        //freeze PC
        _osState = OSState.Freeze;

        //but doesn't cover the cursor ;)
        CursorGameObject.transform.SetAsLastSibling();
    }


    private static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return ((value - from1) / (to1 - from1) * (to2 - from2)) + from2;
    }

    public void UpgradePc()
    {
        UpgradeGameObject.gameObject.SetActive(true);
        UpgradeGameObject.transform.SetAsLastSibling();
    }

    public void UpgradeOs()
    {
        //disable upgrade popup
        UpgradeGameObject.gameObject.SetActive(false);

        //don't upgrade above \/
        if (SystemCurrentOs == CurrentOS.Ten) return;

        ++SystemCurrentOs;

        //log timestamp of update
        GameManager.scene.stat_tracker.LogPCUpgrade(SystemCurrentOs.ToString());

        //update look of OS related objects
        _osScreen.ChangeOS((int) SystemCurrentOs);
        WelcomeGameObject.ChangeOS((int) SystemCurrentOs);
        RebootGameObject.ChangeOS((int) SystemCurrentOs);
        HealthOsScreen.ChangeOS((int) SystemCurrentOs);
        QuarantineGameObject.ChangeOS((int) SystemCurrentOs);

        //reboot PC
        Reboot(3);

        //reset protection
        ProtectionSlider.value = ProtectionSlider.maxValue;

        //update OS settings
        ChangeSettings();
    }

    private void ChangeSettings()
    {
        switch (SystemCurrentOs)
        {
            case CurrentOS.XP:
                TemperatureStep = 25;
                ProtectionUpdateStep = 0.5f;
                minimum_failure_chance = 50f;
                break;
            case CurrentOS.Vista:
                TemperatureStep = 25;
                ProtectionUpdateStep = 0.4f;
                minimum_failure_chance = 40f;
                break;
            case CurrentOS.Seven:
                TemperatureStep = 25;
                ProtectionUpdateStep = 0.3f;
                minimum_failure_chance = 30f;
                break;
            case CurrentOS.Eight:
                TemperatureStep = 25;
                ProtectionUpdateStep = 0.2f;
                minimum_failure_chance = 20f;
                break;
            case CurrentOS.Ten:
                TemperatureStep = 25;
                ProtectionUpdateStep = 0;
                minimum_failure_chance = 10f;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void QuarantineResult(bool quarantineSuccess)
    {
        _osState = OSState.Running;
        QuarantineGameObject.gameObject.SetActive(false);

        if (quarantineSuccess)
        {
            //whatever on successful quarantine
        }
        else
        {
            //disable quarantine popup

            //increase temperature
            if (PcState != PCState.None) return;
            if (IncreaseTemperature() < 100)
            {
                Popup(3);
            }
        }
    }

    //private void ProcessQuarantine()
    //{
    //    switch (_quarantineStatus)
    //    {
    //        case QuarantineStatus.Idle:
    //            _osState = OSState.Running;
    //            break;
    //        case QuarantineStatus.Processing:
    //            //update timer

    //            _quarantineTimer += Time.deltaTime;
    //            QuarantineSlider.value = _quarantineTimer;
    //            if (_quarantineTimer >= QuarantineProcessDuration)
    //            {
    //                //reset timer
    //                _quarantineTimer = 0;

    //                float ChanceOfQuarantineSuccess = ProtectionSlider.value / 100;

    //                //random chance
    //                var random = Random.Range(0.0f, 1.0f);
    //                _quarantineSuccess = random < ChanceOfQuarantineSuccess;

    //                //update image
    //                //QuarantineGameObject.GetComponent<Image>().sprite =
    //                //    _quarantineSuccess ? QuarantineSprites[1] : QuarantineSprites[2];

    //                QuarantineText.gameObject.SetActive(true);
    //                QuarantineSlider.gameObject.SetActive(false);

    //                if (_quarantineSuccess)
    //                {
    //                    QuarantineText.text = "SUCCESS";
    //                    QuarantineText.color = Color.green;
    //                }
    //                else
    //                {
    //                    QuarantineText.text = "FAILURE";
    //                    QuarantineText.color = Color.red;
    //                }
    //                //change quarantine state
    //                _quarantineStatus = QuarantineStatus.Result;
    //            }
    //            break;
    //        case QuarantineStatus.Result:
    //            //update timer
    //            _quarantineTimer += Time.deltaTime;

    //            if (_quarantineTimer >= QuarantinePopupCooldown)
    //            {
    //                //reset timer
    //                _quarantineTimer = 0;

    //                //deactivate image and set to idle
    //                QuarantineGameObject.GetComponentInChildren<Text>().gameObject.SetActive(false);
    //                QuarantineGameObject.gameObject.SetActive(false);
    //                //QuarantineGameObject.GetComponent<Image>().sprite = QuarantineSprites[0];
    //                _quarantineStatus = QuarantineStatus.Idle;

    //                //oh no :(
    //                if (!_quarantineSuccess)
    //                {
    //                    if (IncreaseTemperature() < 100)
    //                    {
    //                        Popup(3);
    //                    }
    //                }
    //            }
    //            break;
    //    }
    //}
    public void DepositScore(int statsScore)
    {
        if (statsScore == 0) return;

        float target = TitanScore + statsScore;

        TitanScore += statsScore;
        TitanScreenX.UpdateScreen();

        StartCoroutine(TitanSlotEvent(target));
    }


    IEnumerator TitanSlotEvent(float _target_score)
    {
        // Begin focus.
        GameManager.scene.focus_camera.Focus(GameManager.scene.pc_manager.transform.position, 9, 999f);

        // Wait until the point slider has finished incrementing.
        yield return new WaitUntil(() => TitanScreenX.PointSlider.value >= _target_score ||
                                         TitanScreenX.PointSlider.value >= TitanScreenX.PointSlider.maxValue);

        // End focus.
        GameManager.scene.focus_camera.Focus(GameManager.scene.pc_manager.transform.position, 9, 0.5f,
            TitanScreenX.PointSlider.value < TitanScreenX.PointSlider.maxValue);
    }
}