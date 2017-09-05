using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyLightController : MonoBehaviour
{
    [SerializeField] USBCharacter owner;
    [SerializeField] Light energy_light;
    [SerializeField] float pulse_rate;
    [SerializeField] float max_light_range;


    void Update()
    {
        float titan_proximity = CalculateTitanProximity();

        if (owner.is_titan || titan_proximity < owner.energy_on_slot * 2)
        {
            energy_light.range = 0;
            energy_light.intensity = 0;
        }
        else
        {
            energy_light.range = Mathf.PingPong(Time.time * pulse_rate, max_light_range);
            energy_light.intensity = titan_proximity / 50;
        }
    }


    float CalculateTitanProximity()
    {
        return owner.stats.energy + (owner.stats.score * owner.energy_score_factor) + owner.energy_on_slot;
    }

}
