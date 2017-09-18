using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsPanelManager : MonoBehaviour
{
    [SerializeField] List<ClassBlock> class_blocks;
    [SerializeField] Text session_duration_text;
    [SerializeField] Image timeline_line;
    [SerializeField] Image timeline_pin;
    [SerializeField] float pin_speed = 10;

    private float session_start;
    private float session_timer;

    private List<Image> spawned_pins = new List<Image>();


    void Awake()
    {
        session_start = Time.time;
    }


    void Update()
    {
        session_timer = Time.time - session_start;
        float minutes = Mathf.Floor(session_timer / 60);
        float seconds = session_timer % 60;

        session_duration_text.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        Vector3 pin_position = timeline_pin.transform.localPosition;
        if (timeline_pin.transform.localPosition.x < timeline_line.rectTransform.rect.width)
        {
            pin_position.x += (timeline_line.rectTransform.rect.width / session_timer) *
                Time.unscaledDeltaTime * (pin_speed * (1 + minutes));
        }

        pin_position.x = Mathf.Clamp(pin_position.x, -(timeline_line.rectTransform.rect.width / 2), timeline_line.rectTransform.rect.width / 2);
        timeline_pin.transform.localPosition = pin_position;
    }


    void OnDisable()
    {
        timeline_pin.rectTransform.localPosition = Vector3.zero - new Vector3(timeline_line.rectTransform.rect.width / 2, 0, 0);

        foreach (Image pin in spawned_pins)
            Destroy(pin.gameObject);
    }

}
