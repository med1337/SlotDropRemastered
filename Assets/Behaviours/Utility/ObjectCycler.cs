using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCycler : MonoBehaviour
{
    [SerializeField] List<GameObject> objects;
    [SerializeField] float cycle_speed = 0.1f;

    private float timer;
    private int index;


    void Start()
    {
        if (objects == null || objects.Count == 0)
        {
            Destroy(this);
        }
        else
        {
            foreach (GameObject obj in objects)
            {
                obj.SetActive(false);
            }

            objects[0].SetActive(true);
        }
    }


    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= cycle_speed)
        {
            timer = 0;
            CycleNext();
        }
    }


    void CycleNext()
    {
        objects[index].SetActive(false);

        ++index;

        if (index >= objects.Count)
            index = 0;

        objects[index].SetActive(true);
    }


}
