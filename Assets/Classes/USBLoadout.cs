using System.Collections;
using UnityEngine;

public class USBLoadout
{
    public string name          = "";
    public int max_health       = 100;
    public float move_speed     = 15;
    public float snap_distance  = 5;
    public Vector3 scale        = new Vector3(2, 2, 2);

    public Sprite hat;
    public GameObject particle;

    public Ability basic;
    public Ability special;
}
