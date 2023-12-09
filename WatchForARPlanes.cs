//Created by: Liam Gilmore
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class WatchForARPlanes : MonoBehaviour
{
    public ARPlaneManager planeManager;
    StateManager stateManager;

    void Start()
    {
        stateManager = GetComponent<StateManager>();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (stateManager.currentState == States.PlaneSeeking)
            stateManager.SetState("PlaneFound");
        if (stateManager.currentState == States.PlaneFound)
            stateManager.SetState("ObjectPlacement");
#elif UNITY_IOS
        if (stateManager.currentState == States.PlaneSeeking && planeManager.trackables.count > 0)
            stateManager.SetState("PlaneFound");
        if (stateManager.currentState == States.PlaneFound)
            stateManager.SetState("ObjectPlacement");
#endif
    }
}
