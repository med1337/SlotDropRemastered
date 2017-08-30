using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRespawner : MonoBehaviour
{
    private Vector3 start_pos;
    private Quaternion start_rot;
    private Rigidbody prop_rigidbody;
    private const float MAX_SPAWN_HEIGHT = 60;

    [Tooltip("How high it will spawn above its original position")]
    public float respawn_height = 40;
    public Vector3 respawn_velocity = new Vector3(0, -5, 0);


    public void RespawnProp()
    {
        transform.rotation = start_rot;

        float respawn_y = start_pos.y + respawn_height;
        respawn_y = Mathf.Clamp(respawn_y, 0, MAX_SPAWN_HEIGHT);
        transform.position = new Vector3(start_pos.x, respawn_y, start_pos.z);

        //reset velocity
        prop_rigidbody.velocity = respawn_velocity;
        prop_rigidbody.angularVelocity = Vector3.zero;
    }


    void Start()
    {
        start_pos = transform.position;
        start_rot = transform.rotation;

        prop_rigidbody = GetComponent<Rigidbody>();
    }


    void OnDrawGizmosSelected()
    {
        Vector3 respawn_location = new Vector3(transform.position.x,
            transform.position.y + respawn_height, transform.position.z);
        respawn_location.y = Mathf.Clamp(respawn_location.y, 0, MAX_SPAWN_HEIGHT);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, respawn_location);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(respawn_location, respawn_location + respawn_velocity);
    }

}
