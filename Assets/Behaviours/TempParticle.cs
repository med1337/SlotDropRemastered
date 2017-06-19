using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempParticle : MonoBehaviour
{
	void Start()
    {
        ParticleSystem system = GetComponent<ParticleSystem>();

        if (system == null)
            system = GetComponentInChildren<ParticleSystem>();

        float duration = system.main.duration;

	    Destroy(gameObject, system.main.duration + 5);
	}

}
