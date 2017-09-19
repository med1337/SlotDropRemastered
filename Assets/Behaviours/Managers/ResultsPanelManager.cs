using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class ResultsPanelManager : MonoBehaviour
{
    [SerializeField] List<ClassBlock> class_blocks;
    [SerializeField] Text session_duration_text;
    [SerializeField] Image timeline_line;
    [SerializeField] Image timeline_pin;
    [SerializeField] Text continue_text;
    [SerializeField] float pin_speed = 10;

    [Header("Timings")]
    [SerializeField] List<float> class_block_timings;
    [SerializeField] float timeline_timing;

    private float timings_timer;
    private List<Image> spawned_pins = new List<Image>();

    private float session_timer;
    private float session_minutes;
    private float session_seconds;

    private bool pin_done
    {
        get
        {
            return timeline_pin.transform.localPosition.x == timeline_line.rectTransform.rect.width / 2;
        }
    }


    public void UpdateSessionTimer()
    {
        session_timer = Time.realtimeSinceStartup - GameManager.session_start;
        session_minutes = Mathf.Floor(session_timer / 60);
        session_seconds = session_timer % 60;
    }


    void Update()
    {
        timings_timer += Time.unscaledDeltaTime;

        for (int i = 0; i < class_blocks.Count; ++i)
        {
            if (class_blocks[i].enabled || timings_timer < class_block_timings[i])
                continue;

            class_blocks[i].enabled = true;
        }

        if (timings_timer >= timeline_timing && !pin_done)
            UpdateTimeline();

        if (!continue_text.gameObject.activeSelf && pin_done)
            continue_text.gameObject.SetActive(true);

        if (continue_text.gameObject.activeSelf && ReInput.controllers.GetAnyButton())
            this.gameObject.SetActive(false);
    }


    void UpdateTimeline()
    {
        session_duration_text.text = string.Format("{0:00}:{1:00}", session_minutes, session_seconds);

        Vector3 pin_position = timeline_pin.transform.localPosition;
        if (timeline_pin.transform.localPosition.x < timeline_line.rectTransform.rect.width)
        {
            pin_position.x += (timeline_line.rectTransform.rect.width / session_timer) *
                              Time.unscaledDeltaTime * (pin_speed * (1 + session_minutes));
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
