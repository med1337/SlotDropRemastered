using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRespawner : MonoBehaviour
{
    private Vector3 start_pos;
    private Quaternion start_rot;
    private Rigidbody prop_rigidbody;
    private bool respawning = false;

    [Tooltip("How high it will spawn above its original position")]
    public float respawn_height = 40;
    public Vector3 respawn_velocity = new Vector3(0, -5, 0);
    public float respawn_delay = 5;
    public LayerMask respawn_triggers;


    void Start()
    {
        start_pos = transform.position;
        start_rot = transform.rotation;
        prop_rigidbody = GetComponent<Rigidbody>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (ContainsLayer(respawn_triggers, other.gameObject.layer) && !respawning)
        {
            respawning = true;
            Invoke("RespawnProp", respawn_delay);
        }
    }


    bool ContainsLayer(LayerMask layer_mask, int layer)
    {
        return layer_mask != (layer_mask | (1 << layer));//return true if layer mask has layer
    }


    public void RespawnProp()
    {
        transform.rotation = start_rot;
        transform.position = new Vector3(start_pos.x, start_pos.y +
            respawn_height, start_pos.z);

        prop_rigidbody.velocity = respawn_velocity;//reset velocity
        respawning = false;
    }


    void OnDrawGizmosSelected()
    {
        Vector3 respawn_location = new Vector3(transform.position.x,
            transform.position.y + respawn_height, transform.position.z);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, respawn_location);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(respawn_location, respawn_location + respawn_velocity);
    }
}
