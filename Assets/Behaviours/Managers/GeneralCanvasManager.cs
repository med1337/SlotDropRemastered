using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralCanvasManager : MonoBehaviour
{
    [SerializeField] GameObject results_panel;

	
    void Update()
    {
        results_panel.SetActive(Input.GetKey(KeyCode.Tab));
    }

}
