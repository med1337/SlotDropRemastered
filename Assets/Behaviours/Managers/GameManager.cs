using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerManager player_manager;
    [SerializeField] AudioManager audio_manager;
    [SerializeField] RespawnManager respawn_manager;
    [SerializeField] LoadoutFactory loadout_factory;

    private static GameManager instance;


    void Awake()
    {
        if (instance == null)
        {
            InitSingleton();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    void InitSingleton()
    {
        instance = this;
    }

}
