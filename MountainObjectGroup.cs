//Created by: Liam Gilmore
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainObjectGroup : MonoBehaviour
{
    [field: SerializeField]
    public List<GameObject> ObjectGroup { get; private set; }
    public List<GameObject> Object1Group = new List<GameObject>();
    public List<GameObject> Object2Group = new List<GameObject>();
    public List<GameObject> Object3Group = new List<GameObject>();
    public List<Vector3> ObjectsOriginalScale { get; private set; }
    public List<Vector3> Objects1OriginalScale = new List<Vector3>();
    public List<Vector3> Objects2OriginalScale = new List<Vector3>();
    public List<Vector3> Objects3OriginalScale = new List<Vector3>();

    public bool object1 = false;
    public bool object2 = false;
    public bool object3 = false;
    void Awake()
    {
        foreach (GameObject objectToScale in Object1Group)
            Objects1OriginalScale.Add(objectToScale.transform.localScale);
        foreach (GameObject objectToScale in Object2Group)
            Objects2OriginalScale.Add(objectToScale.transform.localScale);
        foreach (GameObject objectToScale in Object3Group)
            Objects3OriginalScale.Add(objectToScale.transform.localScale);

        ObjectsOriginalScale = new();
        if (ObjectGroup != null)
        {
            foreach (GameObject objectToScale in ObjectGroup)
            {
                ObjectsOriginalScale.Add(objectToScale.transform.localScale);
                objectToScale.transform.localScale = Vector3.zero;
            }
        }
    }
}
