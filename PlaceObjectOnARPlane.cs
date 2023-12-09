//Created by: Liam Gilmore
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlaceObjectOnARPlane : MonoBehaviour
{
    [SerializeField]
    LayerMask layerMaskForReticle;

    StateManager stateManager;
    ObjectReference objRef;
    GetRaycastCollision raycastData;
    Vector2 middleOfScreen = new Vector2(Screen.width / 2, Screen.height / 2);
    GameObject loadedObjectForPlacement;

    // Start is called before the first frame update
    void Start()
    {
        stateManager = GetComponent<StateManager>();
        raycastData = GetComponent<GetRaycastCollision>();
        objRef = GetComponent<ObjectReference>();
    }

    public void PreloadModel()
    {
        loadedObjectForPlacement = Instantiate(objRef.ObjectToPlacePrefab);
        loadedObjectForPlacement.transform.position = Vector3.one * -10000;
        Vector3 startingScale = loadedObjectForPlacement.transform.localScale;
        loadedObjectForPlacement.transform.localScale = Vector3.zero;
        loadedObjectForPlacement.transform.DOScale(startingScale, .5f);
        loadedObjectForPlacement.transform.position = new Vector3 (objRef.placementReticle.transform.position.x, objRef.placementReticle.transform.position.y, objRef.placementReticle.transform.position.z);
        loadedObjectForPlacement.transform.SetParent(objRef.placementReticle.transform);
        loadedObjectForPlacement.transform.rotation = Quaternion.identity;
        GetComponent<TransformObjectManager>().LoopHover(loadedObjectForPlacement);
    }

    // Update is called once per frame
    void Update()
    {
        if (stateManager.currentState == States.ObjectPlacement)
        {
            raycastData.GetRaycast(middleOfScreen, layerMaskForReticle);
            objRef.placementReticle.transform.position = raycastData.pointOfRaycastHit;

            //makes reticle face user so objects are placed at proper rotation
            objRef.placementReticle.transform.LookAt(new Vector3(Camera.main.transform.position.x, objRef.placementReticle.transform.position.y, Camera.main.transform.position.z));

            if (loadedObjectForPlacement == null)
                PreloadModel();
        }
    }

    public void PlaceObjectOnFloor()
    {
        print("placing object");
        loadedObjectForPlacement.transform.SetParent(null);
        GetComponent<TransformObjectManager>().StopHover(loadedObjectForPlacement);
        GetComponent<StateManager>().SetState("ObjectViewing");
        loadedObjectForPlacement = null;
    }
}
