using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] GameObject hud_mouse;
    [SerializeField] float sensitivity = 0.1f;

    private Vector3 mouse_pos;
    private Vector3 prev_mouse_pos;


    void Start()
    {
        if (hud_mouse == null)
            Destroy(this);
    }


	void Update()
    {
		mouse_pos = transform.position;

        if (mouse_pos != prev_mouse_pos)
        {
            Vector3 difference = mouse_pos - prev_mouse_pos;
            difference.y = difference.z;
            difference.z = 0;


            Vector3 new_mouse_pos = hud_mouse.transform.localPosition + (difference * sensitivity);
            new_mouse_pos.x = Mathf.Clamp(new_mouse_pos.x, -1.9f, 1.9f);
            new_mouse_pos.y = Mathf.Clamp(new_mouse_pos.y, -1, 1);

            hud_mouse.transform.localPosition = new_mouse_pos;
        }

        prev_mouse_pos = mouse_pos;
	}


}
