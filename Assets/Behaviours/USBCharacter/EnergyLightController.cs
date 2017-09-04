using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyLightController : MonoBehaviour
{
    [SerializeField] USBCharacter owner;
    [SerializeField] Light energy_light;


    void Update()
    {
        if (owner.is_titan)
            return;

        float titan_proximity = CalculateTitanProximity();

        if (titan_proximity < 20)
        {
            energy_light.range = 0;
            return;
        }

        energy_light.range = Mathf.PingPong(titan_proximity / 10, 10);
    }


    float CalculateTitanProximity()
    {
        return owner.energy + (owner.score * owner.energy_score_factor);
    }

}
