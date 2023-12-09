//Created by: Liam Gilmore
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum States { PreStart, Start, MainMenu, PlaneSeeking, PlaneFound, ObjectPlacement, ObjectTransform, ObjectViewing, Invalid };

public class StateManager : MonoBehaviour
{
    public States currentState;
    public ObjectReference objectRef;
    int currentPositionInArray;
    int enumLength;

    void Start()
    {
        enumLength = States.GetValues(typeof(States)).Length;
        SetState("Start");

        #if UNITY_EDITOR
            objectRef.debugEditorObjects.SetActive(true);
            objectRef.ARCameraRig.SetActive(false);

        #elif UNITY_IOS || UNITY_ANDROID
            objectRef.debugEditorObjects.SetActive(false);
            objectRef.ARCameraRig.SetActive(true);

        #endif
    }

    private void Update()
    {
        DebugStateSwitch();
    }

    public void SetState(string newStateName)
    {
        States stateToPassToEnum;

        switch (newStateName)
        {
            case "PreStart":
                stateToPassToEnum = States.PreStart;
                break;
            case "Start":
                stateToPassToEnum = States.Start;
                break;
            case "MainMenu":
                stateToPassToEnum = States.MainMenu;
                break;
            case "PlaneSeeking":
                stateToPassToEnum = States.PlaneSeeking;
                break;
            case "PlaneFound":
                stateToPassToEnum = States.PlaneFound;
                break;
            case "ObjectPlacement":
                stateToPassToEnum = States.ObjectPlacement;
                break;
            case "ObjectTransform":
                stateToPassToEnum = States.ObjectTransform;
                break;
            case "ObjectViewing":
                stateToPassToEnum = States.ObjectViewing;
                break;
            default:
                // debug.log("incorrect state name given: " + newStateName);
                stateToPassToEnum = States.Invalid;
                break;
        }
        objectRef.stateDebugText.text = newStateName;
        StartCoroutine("UpdateState", stateToPassToEnum);
    }

    void UpdateState(States newState)
    {
        switch (newState)
        {
            case States.PreStart:
                break;
            case States.Start:
                SetObjectVisibility(true, false, false, false, false, false);
                break;

            case States.MainMenu:
                SetObjectVisibility(true, false, false, true, false, false);
                break;

            case States.PlaneSeeking:
                SetObjectVisibility(false, true, false, true, false, false);
                break;

            case States.PlaneFound:
                SetObjectVisibility(false, false, false, true, false, true);
                break;

            case States.ObjectPlacement:
                SetObjectVisibility(false, false, true, true, true, true);
                break;

            case States.ObjectTransform:
                SetObjectVisibility(false, false, false, true, true, true);
                break;

            case States.ObjectViewing:
                SetObjectVisibility(false, false, false, true, false, false);
                break;

            case States.Invalid:
                Debug.LogError("Invalid State");
                break;
        }

        currentState = newState;
    }

    void SetObjectVisibility(bool mainMenu, bool locateAPlaneDialogUI, bool placeObjectUI, bool objectSelectionView, bool objectTransformCanvas, bool placementReticle)
    {
        objectRef.mainMenuCanvas.SetActive(mainMenu);
        objectRef.findAPlaneCanvas.SetActive(locateAPlaneDialogUI);
        objectRef.placeObjectCanvas.SetActive(placeObjectUI);
        objectRef.objectViewCanvas.SetActive(objectSelectionView);
        objectRef.objectTransformCanvas.SetActive(objectTransformCanvas);
        objectRef.placementReticle.SetActive(placementReticle);
    }

    void DebugStateSwitch()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentPositionInArray = (int)currentState;

            if (currentPositionInArray < enumLength - 1)
                currentPositionInArray++;

            UpdateState((States)currentPositionInArray);

        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentPositionInArray > 0)
                currentPositionInArray--;

            UpdateState((States)currentPositionInArray);

        }
    }
}
