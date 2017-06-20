using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;


public class LoadoutFactory : MonoBehaviour
{
    public List<GameObject> projectile_prefabs;
    public List<Sprite> hat_sprites;

    private Dictionary<string, USBLoadout> starter_loadouts = new Dictionary<string, USBLoadout>();
    private Dictionary<string, USBLoadout> general_loadouts = new Dictionary<string, USBLoadout>();
    private Dictionary<string, USBLoadout> misc_loadouts = new Dictionary<string, USBLoadout>();
    private Dictionary<string, USBLoadout> titan_loadouts = new Dictionary<string, USBLoadout>();


	void Start()
    {
        EnumerateLoadouts();
	}


    void EnumerateLoadouts()
    {
        string file_name = Application.streamingAssetsPath + "/loadouts.json";
        JsonData loadouts_data = JsonMapper.ToObject(File.ReadAllText(file_name));

        var current_array = loadouts_data["Starter"];
        for (int i = 0; i < current_array.Count; ++i)
        {
            PushLoadout(current_array[i], starter_loadouts);
        }

        current_array = loadouts_data["General"];
        for (int i = 0; i < current_array.Count; ++i)
        {
            PushLoadout(current_array[i], general_loadouts);
        }

        current_array = loadouts_data["Misc"];
        for (int i = 0; i < current_array.Count; ++i)
        {
            PushLoadout(current_array[i], misc_loadouts);
        }

        current_array = loadouts_data["Titan"];
        for (int i = 0; i < current_array.Count; ++i)
        {
            PushLoadout(current_array[i], titan_loadouts);
        }
    }


    void PushLoadout(JsonData elem, Dictionary<string, USBLoadout> dictionary)
    {
        USBLoadout loadout = new USBLoadout();

        loadout.name = elem["name"].ToString();
        string hat_name = loadout.name + "Hat";
        loadout.hat = hat_sprites.Find(item => item.name.Substring(0) == hat_name);

        loadout.max_health = int.Parse(elem["max_health"].ToString());
        loadout.move_speed = float.Parse(elem["move_speed"].ToString());
        loadout.snap_distance = float.Parse(elem["snap_distance"].ToString());
        loadout.scale = new Vector3(float.Parse(elem["scale"][0].ToString()),
                                    float.Parse(elem["scale"][1].ToString()),
                                    float.Parse(elem["scale"][2].ToString()));

        loadout.basic_projectile = projectile_prefabs.Find(item => item.name.Substring(0) == elem["basic"].ToString());
        loadout.special_projectile = projectile_prefabs.Find(item => item.name.Substring(0) == elem["special"].ToString());

        dictionary.Add(loadout.name, loadout);
    }


    USBLoadout FindLoadout(string loadout_name)
    {
        if (starter_loadouts.ContainsKey(loadout_name))
        {
            return starter_loadouts[loadout_name];
        }
        else if (general_loadouts.ContainsKey(loadout_name))
        {
            return general_loadouts[loadout_name];
        }
        else if (misc_loadouts.ContainsKey(loadout_name))
        {
            return misc_loadouts[loadout_name];
        }
        else if (titan_loadouts.ContainsKey(loadout_name))
        {
            return titan_loadouts[loadout_name];
        }
        else
        {
            return null;
        }
    }


    public void AssignLoadout(USBCharacter _character, string _loadout_name)
    {
        USBLoadout loadout = FindLoadout(_loadout_name);

        if (loadout != null)
            _character.AssignLoadout(loadout);
    }


    public void AssignRandomLoadout(USBCharacter _character)
    {
        List<string> loadout_names = new List<string>(general_loadouts.Keys);
        string new_loadout_name;

        do
        {
            new_loadout_name = loadout_names[Random.Range(0, loadout_names.Count)];
        } while (new_loadout_name == _character.loadout.name);

        AssignLoadout(_character, new_loadout_name);
    }

}
