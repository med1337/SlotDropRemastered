using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadoutStats
{
    public int score_earned;
    public int score_deposited;
    public int kills;
    public int deaths;
}

public class UpgradeTimestamp
{
    public string name;
    public float time;
}

public class StatTracker : MonoBehaviour
{
    public bool debug_log_enabled = true;

    public Dictionary<string, LoadoutStats> loadout_stats = new Dictionary<string, LoadoutStats>();
    public LoadoutStats largest_stats = new LoadoutStats();

    public List<UpgradeTimestamp> upgrade_timestamps = new List<UpgradeTimestamp>();
    public List<float> upgrade_fail_timestamps = new List<float>();
    public List<float> titan_timestamps = new List<float>();
    public List<float> meteor_timestamps = new List<float>();

    private float awake_time;
    private bool initialised;


    public void Init(List<string> _loadout_names)
    {
        if (initialised)
            return;

        foreach (string loadout_name in _loadout_names)
        {
            loadout_stats.Add(loadout_name, new LoadoutStats());
        }

        initialised = true;
    }


    public void LogDeath(string _loadout_name)
    {
        if (!loadout_stats.ContainsKey(_loadout_name))
            return;

        LoadoutStats stats = loadout_stats[_loadout_name];
        ++stats.deaths;

        if (stats.deaths > largest_stats.deaths)
            largest_stats.deaths = stats.deaths;

        if (debug_log_enabled)
        {
            Debug.Log("Loadout: " + _loadout_name + " died. " +
                "[Total Deaths for Loadout: " + stats.deaths.ToString() + "]");
        }
    }


    public void LogKill(string _loadout_name)
    {
        if (!loadout_stats.ContainsKey(_loadout_name))
            return;

        LoadoutStats stats = loadout_stats[_loadout_name];
        ++stats.kills;

        if (stats.kills > largest_stats.kills)
            largest_stats.kills = stats.kills;

        if (debug_log_enabled)
        {
            Debug.Log("Loadout: " + _loadout_name + " earned a kill. " +
                "[Total Kills for Loadout: " + stats.kills.ToString() + "]");
        }
    }


    public void LogScoreIncrease(string _loadout_name, int _amount)
    {
        if (!loadout_stats.ContainsKey(_loadout_name))
            return;

        LoadoutStats stats = loadout_stats[_loadout_name];
        stats.score_earned += _amount;

        if (stats.score_earned > largest_stats.score_earned)
            largest_stats.score_earned = stats.score_earned;

        if (debug_log_enabled)
        {
            Debug.Log("Loadout: " + _loadout_name + " earned " + _amount.ToString() + " score. " +
                "[Total Earned Score for Loadout: " + stats.score_earned.ToString() + "]");
        }
    }


    public void LogScoreDeposited(string _loadout_name, int _amount)
    {
        if (!loadout_stats.ContainsKey(_loadout_name))
            return;

        LoadoutStats stats = loadout_stats[_loadout_name];
        stats.score_deposited += _amount;

        if (stats.score_deposited > largest_stats.score_deposited)
            largest_stats.score_deposited = stats.score_deposited;

        if (debug_log_enabled)
        {
            Debug.Log("Loadout: " + _loadout_name + " deposited " + _amount.ToString() + " score. " +
                "[Total Deposited Score for Loadout: " + stats.score_deposited.ToString() + "]");
        }
    }


    public void LogPCUpgrade(string _upgrade_name)
    {
        UpgradeTimestamp timestamp = new UpgradeTimestamp();

        timestamp.name = _upgrade_name;
        timestamp.time = Time.time - awake_time;

        upgrade_timestamps.Add(timestamp);

        if (debug_log_enabled)
        {
            Debug.Log("PC Upgraded to: " + _upgrade_name + " at " + timestamp.time.ToString());
        }
    }


    public void LogFailedUpgrade()
    {
        float timestamp = Time.time - awake_time;

        upgrade_fail_timestamps.Add(timestamp);

        if (debug_log_enabled)
        {
            Debug.Log("Failed Upgrade at: " + timestamp.ToString() +
                      " [Total failed Upgrades this Session: " + upgrade_fail_timestamps.Count.ToString() + "]");
        }
    }


    public void LogTitanAchieved()
    {
        float timestamp = Time.time - awake_time;

        titan_timestamps.Add(timestamp);

        if (debug_log_enabled)
        {
            Debug.Log("Titan Achieved at: " + timestamp.ToString() +
                " [Total Titans this Session: " + titan_timestamps.Count.ToString() + "]");
        }
    }


    public void LogMeteorSwarm()
    {
        float timestamp = Time.time - awake_time;

        meteor_timestamps.Add(timestamp);

        if (debug_log_enabled)
        {
            Debug.Log("Meteor Swarm triggered at: " + timestamp.ToString() +
                " [Total Meteor Swarms this Session: " + meteor_timestamps.Count.ToString() + "]");
        }
    }


    void Awake()
    {
        awake_time = Time.time;

        Init(LoadoutFactory.instance.general_loadouts.Keys.ToList());
    }

}
