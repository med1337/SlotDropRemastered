using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class JHelper
{
	private static Camera main_camera_var;


    public static Camera main_camera
    {
        get
        {
            if (main_camera_var == null)
                main_camera_var = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

            return main_camera_var;
        }
    }

	
    public static List<string> GetObjectKeys(JsonData data)
    {
        List<string> keys = new List<string>();

        foreach (string key in data.Keys)
        {
            keys.Add(key);
        }

        return keys;
    }
}
