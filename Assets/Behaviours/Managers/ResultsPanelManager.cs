using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class ResultsPanelManager : MonoBehaviour
{
    [Header("Timings")]
    [SerializeField] List<float> class_block_timings;
    [SerializeField] float timeline_timing;

    [Header("References")]
    [SerializeField] List<ClassBlock> class_blocks;
    [SerializeField] Text session_duration_text;
    [SerializeField] Image timeline_line;
    [SerializeField] RectTransform other_pins_transform;
    [SerializeField] Image timeline_pin;
    [SerializeField] Text continue_text;
    [SerializeField] float pin_speed = 10;
    
    [Header("Timeline Pins")]
    [SerializeField] GameObject titan_pin;
    [SerializeField] GameObject meteor_pin;
    [SerializeField] GameObject upgrade_fail_pin;
    [SerializeField] GameObject upgrade_success_pin;

    private float timings_timer;
    private float session_timer;
    private float session_minutes;
    private float session_seconds;
    private float pin_progress;

    private bool pin_done
    {
        get
        {
            return timeline_pin.transform.localPosition.x == timeline_line.rectTransform.rect.width / 2;
        }
    }


    void Start()
    {
        UpdateSessionTimer();
    }


    void UpdateSessionTimer()
    {
        session_timer = Time.realtimeSinceStartup - GameManager.session_start;
        session_minutes = Mathf.Floor(session_timer / 60);
        session_seconds = session_timer % 60;

        session_duration_text.text = string.Format("{0:00}:{1:00}", session_minutes, session_seconds);
    }


    void Update()
    {
        timings_timer += Time.unscaledDeltaTime;

        for (int i = 0; i < class_blocks.Count; ++i)
        {
            if (class_blocks[i].enabled || timings_timer < class_block_timings[i])
                continue;

            class_blocks[i].enabled = true;
            class_blocks[i].Flash();
            AudioManager.PlayOneShot("wink");
        }

        if (timings_timer >= timeline_timing && !pin_done)
            UpdateTimeline();

        if (!continue_text.gameObject.activeSelf && pin_done)
            continue_text.gameObject.SetActive(true);

        if (continue_text.gameObject.activeSelf && ReInput.controllers.GetAnyButton())
            this.enabled = false;
    }


    void UpdateTimeline()
    {
        Vector3 pin_position = timeline_pin.transform.localPosition;
        if (!pin_done)
        {
            float prev_progress = pin_progress;

            float adjustment = (timeline_line.rectTransform.rect.width / session_timer) *
                               Time.unscaledDeltaTime * (pin_speed * (1 + session_minutes));

            pin_progress += Time.unscaledDeltaTime * (pin_speed * (1 + session_minutes));
            pin_position.x += adjustment;

            CreateTimestampPins(prev_progress);
        }

        pin_position.x = Mathf.Clamp(pin_position.x, -(timeline_line.rectTransform.rect.width / 2), timeline_line.rectTransform.rect.width / 2);
        timeline_pin.transform.localPosition = pin_position;
    }


    void CreateTimestampPins(float _prev_progress)
    {
        foreach (float f in GameManager.scene.stat_tracker.titan_timestamps)
        {
            if (pin_progress >= f && _prev_progress < f)
                CreatePin(titan_pin);
        }

        foreach (float f in GameManager.scene.stat_tracker.meteor_timestamps)
        {
            if (pin_progress >= f && _prev_progress < f)
                CreatePin(meteor_pin);
        }

        foreach (float f in GameManager.scene.stat_tracker.upgrade_fail_timestamps)
        {
            if (pin_progress >= f && _prev_progress < f)
                CreatePin(upgrade_fail_pin);;
        }

        foreach (UpgradeTimestamp t in GameManager.scene.stat_tracker.upgrade_timestamps)
        {
            if (pin_progress >= t.time && _prev_progress < t.time)
            {
                var pin = CreatePin(upgrade_success_pin);
                pin.GetComponentInChildren<Text>().text = "WIN " + t.name + "\n" +
                    string.Format("{0:00}:{1:00}", Mathf.Floor(t.time / 60), t.time % 60);
            }
        }
    }


    GameObject CreatePin(GameObject _pin)
    {
        AudioManager.PlayOneShot("timeline_pop");

        var pin = GameObject.Instantiate(_pin, timeline_pin.transform.position, Quaternion.identity);
        pin.transform.SetParent(other_pins_transform);

        return pin;
    }

}
