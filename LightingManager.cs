// Created by: Liam Gilmore
// Lighting Manager controls the rotation of the directional light relative to the time of day to simulate realistic shadows. 
//  Manages all lighting related effects.       Living List{ Shadow Direction, Lighting Preset Gradients, Grass Color, Bush Color, Tree Color, Skybox Color, Active Time Mode }

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    [Tooltip("Deactivate to stop Time Of Day effects during runtime"), SerializeField]
    public bool progressTime;
    [SerializeField]
    public Light directionalLight;
    [SerializeField]
    public LightingPreset preset;
    [SerializeField, Range(0, 24)]
    public float timeOnChange;
    private float halfHour = 0.5f;

    [Tooltip("Set to 25 for 20 minute cycle"), SerializeField]
    public float timeUntilChange = 25;
    [SerializeField]
    public int lengthOfChange = 15;
    [SerializeField]
    public float hoursInDay = 24;
    [SerializeField]
    public Material skybox;

    [HideInInspector]
    public float timeOfDay;

    private bool tweening = false;
    [SerializeField]
    private WeatherManager _weather;
    private WeatherManager Weather
    {
        get
        {
            if(_weather) return _weather;
            if(!LevelController.Instance) return null;
            _weather = LevelController.Instance.GetComponent<WeatherManager>();
            return _weather;
        }
    }

    [SerializeField]
    List<Material> treeTypes = new List<Material>();
    [SerializeField]
    List<GameObject> treeTypeObj = new List<GameObject>();
    [SerializeField]
    public List<GameObject> bushMeshes = new List<GameObject>();
    [SerializeField]
    List<GameObject> grassMeshes = new List<GameObject>();
    [SerializeField]
    public List<GameObject> treeMeshes = new List<GameObject>();
    
    [HideInInspector]
    private bool tweeningLightRotation = false;

    [HideInInspector]
    public enum TimeMode { morning, afternoon, evening, night };
    [SerializeField]
    public TimeMode activeMode;
    [HideInInspector] 
    private TimeMode previousActiveMode;
    [SerializeField]
    private bool stepping = true;
    [SerializeField]
    private float starsMinIntensity = 5000f;
    [SerializeField]
    private float starsMaxIntensity = 150f;

    [SerializeField]
    private MaterialPropertyBlock _grassPropertyBlock;
    [SerializeField]
    private MaterialPropertyBlock _treesPropertyBlock;
    [SerializeField]
    private Renderer grassRender;
    [SerializeField]
    private Renderer treesRender;

    //These values are the preset times for each TimeMode
    public Dictionary<TimeMode, (float start, float end)> timeOfDayData = new()
    {
        [TimeMode.morning] = new(5, 11.99f),
        [TimeMode.afternoon] = new(12, 16.99f),
        [TimeMode.evening] = new(17, 21.99f),
        [TimeMode.night] = new(22, 4.99f)
    };

    public float TimeOfDay
    {
        get
        {
            return timeOfDay;
        }

        set
        {
            timeOfDay = value;
            timeOfDay %= hoursInDay;

            //stepping activates every other frame to save resources
            if(stepping)
            {
                onChangeEvent?.Invoke(timeOfDay / hoursInDay);
            }
            stepping = !stepping;
        }
    }

    public delegate void OnChangeDelegate(float timeOfDay);
    public static event OnChangeDelegate onChangeEvent;

    private void OnEnable()
    {
        onChangeEvent += Updatelighting;

    }
    private void OnDisable()
    {
        onChangeEvent -= Updatelighting;
    }

    //Start begins infinite cycle for day/night cycle
    private void Start()
    {
        _grassPropertyBlock = new MaterialPropertyBlock();
        _treesPropertyBlock = new MaterialPropertyBlock();

        timeOnChange = TimeOfDay;
        StartCoroutine(InGameTimer());
        TimeOfDay = TimeOfDay;
        ChangeTime();
        SetSceneColors(timeOfDay / hoursInDay);
    }

    //This timer has 48 ticks in a 24 hour day. Setting timeUntilChanges to 25 will result a 20 min day/night cycle
    private IEnumerator InGameTimer()
    {
        if(progressTime)
        {
            float timer = timeUntilChange;
            while(timer > 0)
            {
                timer -= Time.deltaTime;
                yield return null;
            }
            if(timeOnChange >= hoursInDay)
            {
                timeOnChange = 0;
            }
            else
            {
                //On timer complete, and there is still time left in the day, increase time half an in-game hour
                timeOnChange += halfHour;
            }

            ChangeTime();
            StartCoroutine(InGameTimer());
        }
    }

    //Function call to progress time through TimeOfDay OnChangeEvent
    public void ChangeTime()
    {
        if(!tweening)
        {
            tweening = true;
            DOTween.To(() => TimeOfDay, x => TimeOfDay = x, timeOnChange, lengthOfChange).OnComplete(() => tweening = false);
        }
        
        //TimeOfDay activeMode check
        if(timeOnChange < timeOfDayData[activeMode].start || timeOnChange > timeOfDayData[activeMode].end )
        {              
            activeMode = GetActiveMode();  
            
            if(activeMode != previousActiveMode)
            {
                Weather.LightingTick();
                previousActiveMode = activeMode;
            }
        }
    }

    public TimeMode GetActiveMode()
    {
        foreach(KeyValuePair<TimeMode, (float start, float end)> time in timeOfDayData)
        {
            if(time.Value.start <= timeOnChange && (time.Value.end > timeOnChange)) // || time.Value.end < timeOnChange))
            {                   
                return time.Key;
            }                
        }
        return TimeMode.night;
    }

    //Updates every frame there is a change in TimeOfDay
    private void Updatelighting(float timePercent)
    {
        RenderSettings.ambientLight = preset.ambientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.fogColor.Evaluate(timePercent);
        RenderSettings.subtractiveShadowColor = preset.shadowColor.Evaluate(timePercent);

        //Changing shadow directions with time of day
        if(directionalLight != null)
        {
            directionalLight.color = preset.directionalColor.Evaluate(timePercent);

            //This snippet changes the directional light rotation with Time Of Day. It pans the bottom 225� of the X axis so that shadows are never going from the ground up.
            float lightAngle = (timePercent * 360f) - 90f;
            if(lightAngle >= 225)
            {
                timePercent = 0;
                lightAngle = (timePercent * 360f) - 90f;
                timePercent = -55;
            }

            if(!tweeningLightRotation)
            {
                tweeningLightRotation = true;
                directionalLight.transform.DORotateQuaternion(Quaternion.Euler(new Vector3(lightAngle, 180f, 0)), lengthOfChange).OnComplete(() =>
                {
                    SetSceneColors(timePercent);
                    tweeningLightRotation = false;
                });
            }
        }
    }

    public void SetSceneColors(float time)
    {
        //Grass color change with Time Of Day           
        foreach(GameObject grass in grassMeshes)
        {
            grassRender = grass.GetComponent<MeshRenderer>();
            grassRender.GetPropertyBlock(_grassPropertyBlock);
            if(grass.GetComponent<MeshRenderer>().sharedMaterial.HasProperty("_ColorTop") && grass.GetComponent<MeshRenderer>().sharedMaterial.HasProperty("_ColorBottom") && grass != null)
            {
                _grassPropertyBlock.SetColor("_ColorTop", preset.grassTopColor.Evaluate(time));
                _grassPropertyBlock.SetColor("_ColorBottom", preset.grassBottomColor.Evaluate(time));
            }
            grassRender.SetPropertyBlock(_grassPropertyBlock);
            grass.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_ColorTop", _grassPropertyBlock.GetColor("_ColorTop"));
            grass.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_ColorBottom", _grassPropertyBlock.GetColor("_ColorBottom"));
        }

        //--------------------------------------------------------------------

        //Tree color change with Time Of Day
        Gradient gradient = null;
        foreach(GameObject tree in treeTypeObj) //This had to be a GameObject reference to minimize load times for this function and access the Renderer component allowing single line material instancing
        {
            treesRender = tree.GetComponent<MeshRenderer>();
            treesRender.GetPropertyBlock(_treesPropertyBlock);
            if(tree.GetComponent<MeshRenderer>().sharedMaterial.HasProperty("_BaseColor") && tree != null)
            {
                if(tree.GetComponent<MeshRenderer>().sharedMaterial == treeTypes[0])
                {
                    gradient = preset.treeColor1;
                }
                else if(tree.GetComponent<MeshRenderer>().sharedMaterial == treeTypes[1])
                {
                    gradient = preset.treeColor2;
                }
                else if(tree.GetComponent<MeshRenderer>().sharedMaterial == treeTypes[2])
                {
                    gradient = preset.treeColor3;
                }
                _treesPropertyBlock.SetColor("_BaseColor", gradient.Evaluate(time));
            }
            treesRender.SetPropertyBlock(_treesPropertyBlock);
            tree.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_BaseColor", _treesPropertyBlock.GetColor("_BaseColor"));
        }

        //--------------------------------------------------------------------

        //Set skybox color with Time Of Day
        if(RenderSettings.skybox.HasProperty("_Top") && RenderSettings.skybox.HasProperty("_Bottom") && RenderSettings.skybox.HasProperty("_StarSize"))
        {
            RenderSettings.skybox.SetColor("_Top", preset.skyboxHigh.Evaluate(time));
            RenderSettings.skybox.SetColor("_Bottom", preset.skyboxLow.Evaluate(time));
            if(timeOfDay / hoursInDay <= .25f || timeOfDay / hoursInDay >= .75f && RenderSettings.skybox.GetFloat("_StarSize") >= starsMinIntensity)
            {
                RenderSettings.skybox.SetFloat("_StarSize", starsMaxIntensity);
            }
            else if(timeOfDay / hoursInDay > .25f && timeOfDay / hoursInDay < .75f && RenderSettings.skybox.GetFloat("_StarSize") <= starsMaxIntensity)
            {
                RenderSettings.skybox.SetFloat("_StarSize", starsMinIntensity);
            }
        }
    }

    public void ToggleMainLight(bool enabled)
    {
        directionalLight.enabled = enabled;
    }

    private void OnValidate()
    {
        if(RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach(Light light in lights)
            {
                if(light.type == LightType.Directional)
                {
                    directionalLight = light;
                    return;
                }
            }
        }
    }
}

