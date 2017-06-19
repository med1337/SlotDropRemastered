using System.Collections;
using UnityEngine;

public class USBLoadout
{
                                // Default values.
    public string name          = "";
    public int max_health       = 100;
    public float move_speed     = 15;
    public float snap_distance  = 5;
    public Vector3 scale        = new Vector3(2, 2, 2);

    public GameObject basic_projectile;
    public GameObject special_projectile;

    public Sprite hat;
    public GameObject particle;

}
