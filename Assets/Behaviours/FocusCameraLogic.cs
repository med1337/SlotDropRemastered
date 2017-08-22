using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FocusCameraLogic : MonoBehaviour
{

    [SerializeField] float min_zoom_in = 5;
    [SerializeField] float max_zoom_out = 60;
    [SerializeField] List<Transform> focused_objects = new List<Transform>();

    public void AddFocusedObject(GameObject _obj)
    {
        if (!focused_objects.Contains(_obj.transform))
            focused_objects.Add(_obj.transform);
    }


    void Awake()
    {
        RespawnManager.listener_module.AddListener(this.gameObject);
    }


    void OnDisable()
    {
        focused_objects.Clear();
    }


	void Update()
    {
        UpdateFocusedObjects();
        CalculateAveragePosition();
        CalculateZoom();
	}


    void UpdateFocusedObjects()
    {
        // Garbage collect focused objects.
        focused_objects.RemoveAll(item => item == null);
    }


    void CalculateAveragePosition()
    {
        Vector3 average_pos = Vector3.zero;
        int count = 0;

        foreach (Transform t in focused_objects)
        {
            if (!t.gameObject.activeSelf)
                continue;

            ++count;

            Vector3 tPos = t.position;
            average_pos += tPos;
        }

        average_pos /= count;
        //camera_manager.SetLookTarget(average_pos);
    }


    void CalculateZoom()
    {
        int objects_in_view = 0;
        int objects_in_outer_view = 0;
        int objects_in_inner_view = 0;

        Plane[] bounds_planes;
        Plane[] planes_outer;
        Plane[] planes_inner;

        float size_change_value = CameraManager.cam.orthographicSize / 4;

        CameraManager.cam.orthographicSize -= size_change_value / 2;
        bounds_planes = GeometryUtility.CalculateFrustumPlanes(CameraManager.cam);

        CameraManager.cam.orthographicSize -= size_change_value;
        planes_outer = GeometryUtility.CalculateFrustumPlanes(CameraManager.cam);

        CameraManager.cam.orthographicSize -= size_change_value / 2;
        planes_inner = GeometryUtility.CalculateFrustumPlanes(CameraManager.cam);

        CameraManager.cam.orthographicSize += size_change_value * 2;

        // check if objects are within base or narrower field of view
        foreach (Transform t in focused_objects)
        {
            if (!t.gameObject.activeSelf)
                continue;

            ++objects_in_view;

            Bounds object_bounds = t.GetComponent<Collider>().bounds;

            if (GeometryUtility.TestPlanesAABB(planes_inner, object_bounds))
            {
                ++objects_in_inner_view;
                ++objects_in_outer_view;
            }
            else if (GeometryUtility.TestPlanesAABB(planes_outer, object_bounds))
            {
                ++objects_in_outer_view;
            }
        }

        if (objects_in_view == objects_in_inner_view && CameraManager.cam.orthographicSize > min_zoom_in)
        {
            Debug.Log("Zooming in");
            CameraManager.SetTargetSize(CameraManager.cam.orthographicSize - 1);
        }
        else if (objects_in_view != objects_in_outer_view && CameraManager.cam.orthographicSize < max_zoom_out)
        {
            Debug.Log("Zooming out");
            CameraManager.SetTargetSize(CameraManager.cam.orthographicSize + 1);
        }
    }

}
