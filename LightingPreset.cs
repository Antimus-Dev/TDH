//Created by: Liam Gilmore
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Lighting Preset", menuName = "Scriptables/Lighting Preset", order = 1)]
public class LightingPreset : ScriptableObject
{
    public Gradient ambientColor;
    public Gradient directionalColor;
    public Gradient fogColor;
    public Gradient shadowColor;
    public Gradient grassTopColor;
    public Gradient grassBottomColor;
    public Gradient bushColorHigh;
    public Gradient bushColorMed;
    public Gradient bushColorLow;
    public Gradient bushColorResult;
    public Gradient treeColor1;
    public Gradient treeColor2;
    public Gradient treeColor3;
    public Gradient skyboxLow;
    public Gradient skyboxHigh;
}
