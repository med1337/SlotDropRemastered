using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SpeedModifier
{
    public float modifier;
    public float duration;
    public float timeout { get; private set; }

    public SpeedModifier(float _modifier, float _duration)
    {
        modifier = _modifier;
        duration = _duration;
        timeout = Time.time + duration;
    }
}

public class MovementCalculator 
{
    private float base_speed;
    private List<SpeedModifier> speed_modifiers = new List<SpeedModifier>();


    public void SetBaseSpeed(float _base_speed)
    {
        base_speed = _base_speed;
    }


    public void AddSpeedModifier(float _modifier, float _duration)
    {
        speed_modifiers.Add(new SpeedModifier(_modifier, _duration));
    }


    public float CalculateMoveSpeed()
    {
        CleanUpModifiers();

        float biggest_bonus = 1;
        float biggest_penalty = 1;

        foreach (SpeedModifier mod in speed_modifiers)
        {
            if (mod.modifier > 1)
            {
                // Speed Bonus.
                if (mod.modifier <= biggest_bonus)
                    continue;

                biggest_bonus = mod.modifier;
            }
            else if (mod.modifier < 1)
            {
                // Speed Penalty.
                if (mod.modifier >= biggest_penalty)
                    continue;

                biggest_penalty = mod.modifier;
            }
        }

        return base_speed * SanityCheckModifier(biggest_penalty) * SanityCheckModifier(biggest_bonus);
    }


    void CleanUpModifiers()
    {
        speed_modifiers.RemoveAll(elem => Time.time >= elem.timeout);
    }


    float SanityCheckModifier(float _modifier)
    {
        return (_modifier <= 0 ? 1 : _modifier);
    }

}
