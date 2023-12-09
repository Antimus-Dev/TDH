//Created by: Liam Gilmore
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GetRaycastCollision : MonoBehaviour
{
    [SerializeField]
    ARRaycastManager m_RaycastManager;

    public GameObject objectHit;
    public Vector3 pointOfRaycastHit;
    public LayerMask castableLayers;

    StateManager stateManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Start()
    {
        stateManager = GetComponent<StateManager>();
    }

    public void GetRaycast(Vector3 originOnRaycast, LayerMask layerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(originOnRaycast);

        #if UNITY_EDITOR
        UnityEditorRaycast(ray, layerMask);

        #elif UNITY_IOS || UNITY_ANDROID
        ARRaycast(ray, originOnRaycast, layerMask);

        #endif
    }

    void UnityEditorRaycast(Ray ray, LayerMask layerMask)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10000, layerMask))
        {
            objectHit = hit.transform.gameObject;
            pointOfRaycastHit = hit.point;
        }
        else
        {
            objectHit = null;
            pointOfRaycastHit = Vector3.zero;
        }
    }

    void ARRaycast(Ray ray, Vector3 originOnRaycast, LayerMask layerMask)
    {
        RaycastHit hit;
        Debug.Log("AR Raycasting");
        if (stateManager.currentState == States.ObjectPlacement)
        {
            if (m_RaycastManager.Raycast(originOnRaycast, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                pointOfRaycastHit = hitPose.position;
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hit, 10000, layerMask))
            {
                objectHit = hit.transform.gameObject;
                pointOfRaycastHit = hit.point;
            }
            else
            {
                objectHit = null;
                pointOfRaycastHit = Vector3.zero;
            }
        }
    }
}
