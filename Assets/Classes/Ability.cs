using System.Collections;
using UnityEngine;

public class Ability
{
    float cooldown_time;
    float last_use;
    
    
    bool Ready()
    {
        return (Time.time - last_use) >= cooldown_time;
    }


    public void Activate()
    {
        if (!Ready())
            return;

        last_use = Time.time;

        // Do ability stuff ..
    }

}
