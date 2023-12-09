// Created by: Liam Gilmore
// Weather Manager controls weather related effects relative to TimeOfDay in Lighting Manager. 

using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    [SerializeField, Range(0, 100)] 
    public int chanceToRain;
    [SerializeField, Range(0, 100)] 
    public int chanceToSnow;
    [SerializeField]
    public GameObject weatherContainer;
    private List<GameObject> weather = new List<GameObject>();
    private CinemachineBrain camera;

    void Start()
    {
        camera = GameObject.FindObjectOfType<CinemachineBrain>();

        for(int i = 0; i < weatherContainer.transform.childCount; i++)
        {
            weather.Add(weatherContainer.transform.GetChild(i).gameObject);
        }
    }

    public void LightingTick()
    {
        if(chanceToRain > 0)
        {
            WeatherEffect(1);
        }
        else if(chanceToSnow > 0)
        {
            WeatherEffect(0);
        }
    }
    private void WeatherEffect(int weatherType)
    {
        Clear();
        weather[weatherType].transform.parent = camera.gameObject.transform;
        weather[weatherType].transform.position = camera.gameObject.transform.position;
        weather[weatherType].GetComponent<ParticleSystem>().Play();
    }

    private void Clear()
    {
        foreach(GameObject child in weather)
        {
            child.transform.parent = weatherContainer.transform;
            child.transform.position = new Vector3(0, 0, 0);
            child.GetComponent<ParticleSystem>().Stop();
        }
    }
}
