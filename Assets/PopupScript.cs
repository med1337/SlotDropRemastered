using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupScript : MonoBehaviour
{
    public List<Sprite> PopupImages;
    public GameObject CursorGameObject;
    public float CursorFreezeTimeDuration;
    public bool ActivatePopup;

    private bool _closed;
    private float _timer;
    private List<GameObject> _popups = new List<GameObject>();
    private GameObject _lastGameObject;

    private Vector3 _targetVector3;


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (ActivatePopup)
        {
            Popup();
            ActivatePopup = false;
        }
        if (!_closed)
        {
            if (_popups.Count > 0)
            {
                _lastGameObject = _popups[_popups.Count - 1];
                ClosePopup();
            }
        }
        else
        {
            _timer += Time.deltaTime;
            if (_timer > CursorFreezeTimeDuration)
            {
                _timer = 0;
                _closed = false;
            }
        }
    }

    void ClosePopup()
    {
        if (_lastGameObject == null) return;

        _targetVector3 = _lastGameObject.transform.localPosition;

        CursorGameObject.transform.localPosition = Vector3.MoveTowards(CursorGameObject.transform.localPosition,
            _targetVector3, Time.deltaTime);

        if (CursorGameObject.transform.localPosition != _targetVector3) return;

        //if reached position
        _popups.Remove(_lastGameObject);
        GameObject.Destroy(_lastGameObject);
        _closed = true;
    }

    void Popup()
    {
        //remap position accordingly to scale
        float scale = Random.Range(0.01f, 0.02f);
        float posx = Remap(scale, 0.01f, 0.02f, 1.4f, 0.9f);
        float posy = Remap(scale, 0.01f, 0.02f, 0.55f, 0.0f);

        posx = Random.Range(-posx, posx);
        posy = Random.Range(-posy, posy);

        //initialise new popup
        GameObject newpopup_game_object = new GameObject();

        //add sprite and randomize image
        Image x = newpopup_game_object.AddComponent<Image>();
        x.sprite = PopupImages[Random.Range(0, PopupImages.Count)];

        //change transform of new popup
        newpopup_game_object.transform.SetParent(transform);
        newpopup_game_object.transform.localScale = new Vector3(scale, scale);
        newpopup_game_object.transform.localPosition = new Vector3(posx, posy);
        newpopup_game_object.transform.rotation = new Quaternion(0, 0, 0, 0);

        //add popup to array
        _popups.Add(newpopup_game_object);

        //make sure cursor is on top of everything
        CursorGameObject.transform.SetAsLastSibling();
    }

    private static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return ((value - from1) / (to1 - from1) * (to2 - from2)) + from2;
    }
}