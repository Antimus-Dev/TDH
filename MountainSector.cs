//Created by: Liam Gilmore
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
using Devhouse.Tools.Utilities;

public enum HueAdjustValues
{
    _MaskOneHueAdjust, _MaskTwoHueAdjust, _MaskThreeHueAdjust, _MaskFiveHueAdjust, _MaskFourHueAdjust, _MaskSixHueAdjust, None
}

//Set of objects that have a relation in the scene
//[Serializable]
public class MountainSector : NeuroTagBehaviour
{
    //Scale
    public MountainObjectGroup deersObjects;
    public MountainObjectGroup lodgesObjects;
    public MountainObjectGroup treesObjects;
    public GameObject subMenu;

    //Color
    public HueAdjustValues hueValue1;
    public HueAdjustValues hueValue2;

    private string strengthTarget1;
    private string strengthTarget2;

    private string glowTarget1;
    private string glowTarget2;

    //Animations
    public Animator backAnim;
    public Animator colorAnim;
    public Animator deerAnim;
    public Animator lodgeAnim;
    public Animator treeAnim;
    public Animator weatherAnim;

    //Weather
    public enum WeatherStates{
        RAINY,SNOWY,LEAFY,NORMAL
    }
    public SerializedDictionary<WeatherStates, GameObject> weatherStatesInSector = new();
    private WeatherStates currentState = WeatherStates.NORMAL;

    #region ButtonFunctions
    public void ChangeColor(ColorFunctions colorFunctions)
    {
        colorAnim.Play("Anim");
        string hueValueString1 = hueValue1.ToString();
        string hueValueString2 = hueValue2.ToString();
        if (hueValueString1 == "_MaskOneHueAdjust")
        {
            strengthTarget1 = "_MaskOneStrength";
            glowTarget1 = "_MaskOneGlow";
        }
        else if (hueValueString1 == "_MaskTwoHueAdjust")
        {
            strengthTarget1 = "_MaskTwoStrength";
            glowTarget1 = "_MaskTwoGlow";
        }
        else if (hueValueString1 == "_MaskThreeHueAdjust")
        {
            strengthTarget1 = "_MaskThreeStrength";
            glowTarget1 = "_MaskThreeGlow";
        }
        else if (hueValueString1 == "_MaskFourHueAdjust")
        {
            strengthTarget1 = "_MaskFourStrength";
            glowTarget1 = "_MaskFourGlow";
        }
        else if (hueValueString1 == "_MaskFiveHueAdjust")
        {
            strengthTarget1 = "_MaskFiveStrength";
            glowTarget1 = "_MaskFiveGlow";
        }
        else if (hueValueString1 == "_MaskSixHueAdjust")
        {
            strengthTarget1 = "_MaskSixStrength";
            glowTarget1 = "_MaskSixGlow";
        }

        if (hueValueString2 == "None")
        {
            if (hueValueString2 == "_MaskOneHueAdjust")
            {
                strengthTarget2 = "_MaskOneStrength";
                glowTarget2 = "_MaskOneGlow";
            }
            else if (hueValueString2 == "_MaskTwoHueAdjust")
            {
                strengthTarget2 = "_MaskTwoStrength";
                glowTarget2 = "_MaskTwoGlow";
            }
            else if (hueValueString2 == "_MaskThreeHueAdjust")
            {
                strengthTarget2 = "_MaskThreeStrength";
                glowTarget2 = "_MaskThreeGlow";
            }
            else if (hueValueString2 == "_MaskFourHueAdjust")
            {
                strengthTarget2 = "_MaskFourStrength";
                glowTarget2 = "_MaskFourGlow";
            }
            else if (hueValueString2 == "_MaskFiveHueAdjust")
            {
                strengthTarget2 = "_MaskFiveStrength";
                glowTarget2 = "_MaskFiveGlow";
            }
            else if (hueValueString2 == "_MaskSixHueAdjust")
            {
                strengthTarget2 = "_MaskSixStrength";
                glowTarget2 = "_MaskSixGlow";
            }


            colorFunctions.HueShift(hueValueString1, strengthTarget1, glowTarget1);
            return;
        }
        colorFunctions.HueShift(hueValueString1, strengthTarget1, glowTarget1, 120, hueValueString2, strengthTarget2, glowTarget2);
    }
    public void FlipFlopDeers(float waitTime)
    {
        deerAnim.Play("Anim");
        FlipFlop(deersObjects, waitTime);
    }
    public void FlipFlopLodges(float waitTime)
    {
        lodgeAnim.Play("Anim");
        FlipFlop(lodgesObjects, waitTime);
    }
    public void FlipFlopTrees(float waitTime)
    {
        treeAnim.Play("Anim");
        FlipFlop(treesObjects, waitTime);
    }
    public void ChangeWeather()
    {
        weatherAnim.Play("Anim");
        switch (currentState)
        {
            case WeatherStates.NORMAL:
                currentState = WeatherStates.RAINY;
                break;
            case WeatherStates.RAINY:
                currentState = WeatherStates.LEAFY;
                break;
            case WeatherStates.LEAFY:
                currentState = WeatherStates.SNOWY;
                break;
            case WeatherStates.SNOWY:
                currentState = WeatherStates.NORMAL;
                break;
            default:
                break;
        }
        foreach(GameObject weatherParticles in weatherStatesInSector.Values)
        {
            weatherParticles.SetActive(false);
        }
        weatherStatesInSector[currentState].SetActive(true);
    }

    #endregion

    #region ScaleFuntions
    public void FlipFlop(MountainObjectGroup mountainObjectGroup, float waitTime)
    {
        if (!mountainObjectGroup.object1)
        {
            StartScaleUp(mountainObjectGroup, 0, waitTime);
            mountainObjectGroup.object1 = true;
        }
        else if (!mountainObjectGroup.object2)
        {
            StartScaleUp(mountainObjectGroup, 1, waitTime);
            mountainObjectGroup.object2 = true;
        }
        else if (!mountainObjectGroup.object3)
        {
            StartScaleUp(mountainObjectGroup, 2, waitTime);
            mountainObjectGroup.object3 = true;
        }
        else
        {
            mountainObjectGroup.object1 = false;
            mountainObjectGroup.object2 = false;
            mountainObjectGroup.object3 = false;
            StartScaleDown(mountainObjectGroup, waitTime); 
        }
    }

    public void StartScaleUp(MountainObjectGroup objectGroup, int group, float scaleWaitTime)
    {
        List<GameObject> objectsToScale = objectGroup.ObjectGroup;
        List<Vector3> finishedScaleSize = objectGroup.ObjectsOriginalScale;

        if (group == 0 && objectGroup.Object1Group.Count > 0)
        {
            objectsToScale = objectGroup.Object1Group;
            finishedScaleSize = objectGroup.Objects1OriginalScale;
        }
        else if (group == 1 && objectGroup.Object2Group.Count > 0)
        {
            objectsToScale = objectGroup.Object2Group;
            finishedScaleSize = objectGroup.Objects2OriginalScale;
        }
        else if (group == 2 && objectGroup.Object3Group.Count > 0)
        {
            objectsToScale = objectGroup.Object3Group;
            finishedScaleSize = objectGroup.Objects3OriginalScale;
        }

        for (int i = 0; i < objectsToScale.Count; i++)
        {
            print(objectsToScale[i].name);
            if (objectsToScale.Count > i)
                StartCoroutine(ScaleUp(objectsToScale[i], finishedScaleSize[i], scaleWaitTime));
        }
    }

    IEnumerator ScaleUp(GameObject objectToScale, Vector3 newScale, float scaleWaitTime)
    {
        yield return new WaitForSeconds(Random.Range(scaleWaitTime - .5f, scaleWaitTime + .5f));
        objectToScale.transform.DOScale(newScale, Random.Range(0.5f, 1));
    }

    public void StartScaleDown(MountainObjectGroup objectGroup, float scaleWaitTime)
    {
        List<GameObject> objectsToScale = objectGroup.ObjectGroup;
        List<Vector3> finishedScaleSize = objectGroup.ObjectsOriginalScale;
        for (int i = 0; i < objectsToScale.Count; i++)
            if (objectsToScale.Count > i)
                StartCoroutine(ScaleDown(objectsToScale[i],scaleWaitTime));
    }

    IEnumerator ScaleDown(GameObject objectToScale, float scaleWaitTime)
    {
        yield return new WaitForSeconds(Random.Range(scaleWaitTime - .5f, scaleWaitTime + .5f));
        objectToScale.transform.DOScale(Vector3.zero, Random.Range(0.5f, 1));
    }

    #endregion

    #region DataFunctions
    public MountainSegment GetSegmentValues()
    {
        DecoratorGroup deerGroup = new DecoratorGroup(deersObjects.object1, deersObjects.object2, deersObjects.object3);
        DecoratorGroup lodgeGroup = new DecoratorGroup(lodgesObjects.object1, lodgesObjects.object2, lodgesObjects.object3);
        DecoratorGroup treeGroup = new DecoratorGroup(treesObjects.object1, treesObjects.object2, treesObjects.object3);

        //Function values have been changed from boolean "isScaled" values to object lists. 
        return new MountainSegment(deerGroup, lodgeGroup, treeGroup, GetCurrentWeather());
    }

    public string GetCurrentWeather()
    {
        return currentState.ToString();
    }
    #endregion

}
