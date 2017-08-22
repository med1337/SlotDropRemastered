using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class PopupScript : MonoBehaviour
{
    [Header("DEBUG")] public bool ActivatePopup;
    public bool ActivateQuarantine;

    [Header("Settings")] public GameObject CursorGameObject;
    public GameObject QuarantineGameObject;
    public Slider ProtectionSlider;
    public Slider TemperatureSlider;

    [Space(10)] public float CursorFreezeTimeDuration;

    [Space(10)]
    public float QuarantineProcessDuration;
    public float QuarantinePopupCooldown;
    public float ChanceOfQuarantineSuccess;

    [Space(10)]
    [Range(1, 10)]
    public float ProtectionUpdateRate;
    [Range(0, 1)]
    public float ProtectionUpdateStep;

    [Tooltip("1st element - processing, 2nd - success, 3rd - failure")] public List<Sprite> QuarantineSprites;

    [Space(10)] public List<Sprite> PopupImages;

    //private
    private float _cursorSpeed;

    private float _protectionTimer = 0;

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
    }

    // Update is called once per frame
    void Update()
    {
        //editor

        _protectionUpdateRate = 1 / ProtectionUpdateRate;

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
        _cursorSpeed = _popups.Count;
        ProcessQuarantine();
        if (_quarantineStatus != QuarantineStatus.Idle) return;
        ProcessPopups();
        ProcessProtectionBar();
    }

    private void ProcessPopups()
    {
        if (_quarantineStatus != QuarantineStatus.Idle) return;
        if (!_popupClosed)
        {
            if (_popups.Count <= 0) return;
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

    private void ClosePopup()
    {
        if (_lastGameObject == null) return;

        _targetVector3 = _lastGameObject.transform.localPosition;

        //TODO: X position calculation, which does not work sadly [couldn't wrap my head around]
        {
//_lastGameObject.GetComponent<RectTransform>().
            //float offsetx = _lastGameObject.GetComponent<Image>().sprite.rect.width *
            //                _lastGameObject.transform.localScale.x;
            //float offsety = _lastGameObject.GetComponent<Image>().sprite.rect.height *
            //                _lastGameObject.transform.localScale.y;

            //Vector3 offset = new Vector3(offsetx, offsety);

            //Debug.Log(offset);
            //_targetVector3 += offset;
            //Debug .Log( _lastGameObject.GetComponent<SpriteRenderer>().bounds);
        }

        CursorGameObject.transform.localPosition = Vector3.MoveTowards(CursorGameObject.transform.localPosition,
            _targetVector3, Time.deltaTime * _cursorSpeed);


        if (CursorGameObject.transform.localPosition != _targetVector3) return;

        _popups.Remove(_lastGameObject);
        GameObject.Destroy(_lastGameObject);
        _popupClosed = true;
    }

    public void ProcessProtectionBar()
    {
        _protectionTimer += Time.deltaTime;
        if (!(_protectionTimer >= _protectionUpdateRate)) return;
        if (ProtectionSlider.value > 0)
        {
            ProtectionSlider.value -= ProtectionUpdateStep;
            ChanceOfQuarantineSuccess = ProtectionSlider.value / 100;
        }
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


            i++;
        }

        //make sure cursor is on top of everything
        CursorGameObject.transform.SetAsLastSibling();
    }

    public void AttemptQuarantine()
    {
        if (_quarantineStatus != QuarantineStatus.Idle) return;

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
                break;
            case QuarantineStatus.Processing:
                _quarantineTimer += Time.deltaTime;
                if (_quarantineTimer >= QuarantineProcessDuration)
                {
                    _quarantineTimer = 0;
                    var random = Random.Range(0.0f, 1.0f);
                    Debug.Log(random);
                    _quarantineSuccess = random < ChanceOfQuarantineSuccess;
                    QuarantineGameObject.GetComponent<Image>().sprite =
                        _quarantineSuccess ? QuarantineSprites[1] : QuarantineSprites[2];

                    _quarantineStatus = QuarantineStatus.Result;
                }
                break;
            case QuarantineStatus.Result:
                _quarantineTimer += Time.deltaTime;
                if (_quarantineTimer >= QuarantinePopupCooldown)
                {
                    _quarantineTimer = 0;
                    QuarantineGameObject.SetActive(false);
                    QuarantineGameObject.GetComponent<Image>().sprite = QuarantineSprites[0];
                    _quarantineStatus = QuarantineStatus.Idle;
                    if (!_quarantineSuccess)
                    {
                        Popup(3);
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