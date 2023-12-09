//Created by: Liam Gilmore
using Devhouse.Tools.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFunctions : Singleton<ColorFunctions>
{
    public Material mat;
    public float time = 0.02f; // 0.02 for 50 FPS color animation
    public int colorRange = 120;

    private float aTarget;
    private float bTarget;

    private bool aTrigger = false;
    private bool bTrigger = false;

    public void Start()
    {
        mat.SetFloat("_MaskOneStrength", 1);
        mat.SetFloat("_MaskTwoStrength", 1);
        mat.SetFloat("_MaskThreeStrength", 1);
        mat.SetFloat("_MaskFourStrength", 1);
        mat.SetFloat("_MaskFiveStrength", 1);
        mat.SetFloat("_MaskSixStrength", 1);

        mat.SetFloat("_MaskOneHueAdjust", 0);
        mat.SetFloat("_MaskTwoHueAdjust", 0);
        mat.SetFloat("_MaskThreeHueAdjust", 0);
        mat.SetFloat("_MaskFourHueAdjust", 0);
        mat.SetFloat("_MaskFiveHueAdjust", 0);
        mat.SetFloat("_MaskSixHueAdjust", 0);
    }

    public void HueShift(string aString, string strength1, string glow1, int range = 120, string bString = null, string strength2 = null, string glow2 = null) //HueShift(PropertyName, PropertyName)
    {
        // Debug.Log("Activated");
        if(bString != null)
        {
            float aValue = mat.GetFloat(aString);
            float bValue = mat.GetFloat(bString);
            aTarget = aValue + range;
            bTarget = bValue + range;
            StartCoroutine(Hue(aValue, aString, strength1, glow1, bValue, bString, strength2, glow2));
        }
        else
        {
            float aValue = mat.GetFloat(aString);
            aTarget = aValue + range;
            StartCoroutine(Hue(aValue, aString, strength1, glow1)); 
        }
    }

    private IEnumerator Hue(float aValue, string aString, string aStrength, string aGlow, float bValue = -1, string bString = null, string bStrength = null, string bGlow = null)
    {
        if (bString != null)
        {
            if (!bTrigger)
            {
                mat.SetFloat(bStrength, 0);
                bTrigger = true;
            }
            if (!aTrigger)
            {
                mat.SetFloat(aStrength, 0);
                aTrigger = true;
            }

            mat.SetFloat(aString, aValue + 1);
            mat.SetFloat(bString, bValue + 1);
            aValue = mat.GetFloat(aString);
            bValue = mat.GetFloat(bString);
            if (bValue >= 360)
            {
                aValue = 0;
                bValue = 0;
                if (bTarget >= 360)
                {
                    aTarget -= 360;
                    bTarget -= 360;
                }
                yield return new WaitForSeconds(time);
                StartCoroutine(Hue(aValue, aString, aStrength, aGlow, bValue, bString, bStrength, bGlow));
            }
            if (mat.GetFloat(bString) < bTarget)
            {
                yield return new WaitForSeconds(time);
                StartCoroutine(Hue(aValue, aString, aStrength, aGlow, bValue, bString, bStrength, bGlow));
            }
            else
            {
                aTrigger = false;
                bTrigger = false;
                yield return null;
            }
                
        }
        else
        {
            if (!aTrigger)
            {
                mat.SetFloat(aStrength, 0);
                aTrigger = true;
            }
            mat.SetFloat(aString, aValue + 1);
            aValue = mat.GetFloat(aString);
            if (aValue >= 360)
            {
                aValue = 0;
                if (aTarget >= 360)
                    aTarget -= 360;

                yield return new WaitForSeconds(time);
                StartCoroutine(Hue(aValue, aString, aStrength, aGlow));
            }
            if (mat.GetFloat(aString) < aTarget)
            {
                yield return new WaitForSeconds(time);
                StartCoroutine(Hue(aValue, aString, aStrength, aGlow));
            }
            else
            {
                aTrigger = false;
                yield return null;
            }
        }             
    }
}
