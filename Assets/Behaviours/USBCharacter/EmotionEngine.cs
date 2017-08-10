using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

[ExecuteInEditMode]
public class EmotionEngine : MonoBehaviour
{
    const float MIN_HEIGHT = 0.1f;
    const float MAX_HEIGHT = 0.359f;

    [Header("Eye References")]
    public Image left_eye;
    public Image right_eye;

    [Header("Eye Openers")]
    [Range(MIN_HEIGHT, MAX_HEIGHT)]
    public float left_eye_open = MAX_HEIGHT;
    [Range(MIN_HEIGHT, MAX_HEIGHT)]
    public float right_eye_open = MAX_HEIGHT;

	// Update is called once per frame
	void Update ()
    {
		left_eye.rectTransform.sizeDelta = new Vector2(left_eye.rectTransform.sizeDelta.x, left_eye_open);
        right_eye.rectTransform.sizeDelta = new Vector2(right_eye.rectTransform.sizeDelta.x, right_eye_open);
    }
}
