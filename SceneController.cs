//Created by: Liam Gilmore
using Devhouse.Tools.Utilities;
using NextMind.NeuroTags;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SceneController : Singleton<SceneController>
{
    public GameObject mountain;
    [SerializeField]
    public SerializedDictionary<GameObject, NeuroTagBehaviour> neuroTagContent = new();
    public float scaleWaitTime;
    public PlayableDirector exitSubMenuTimeline;
    public PlayableDirector openSubMenuTimeline;
    public List<GameObject> maskUIList;
    public List<GameObject> rotationUIList;

    private float glowWaitTime = 0.01f;

    private bool[] lerpTriggered = new bool[4];
    private bool[] lerpLock = new bool[4];
    private bool[] undoTriggered = new bool[4];
    private bool[] undoLock = new bool[4];

    private void Awake()
    {
        for (int i = 0; undoLock.Length; i++)
        {
            lerpTriggered[i] = false;
            lerpLock[i] = false;
            undoTriggered[i] = false;
            undoLock[i] = false;
        }

        base.Awake();
        foreach (GameObject maskButton in maskUIList)
        {
            maskButton.GetComponent<MaskSelector>().SetCallbacks();
        }
        rotationUIList[0].GetComponent<NeuroTag>().onTriggered.AddListener(delegate { DecreaseRotationSpeed(rotationUIList[0]); });
        rotationUIList[0].GetComponent<NeuroTag>().onReleased.AddListener(delegate { FocusLost(); });
        rotationUIList[1].GetComponent<NeuroTag>().onTriggered.AddListener(delegate { IncreaseRotationSpeed(rotationUIList[1]); });
        rotationUIList[1].GetComponent<NeuroTag>().onReleased.AddListener(delegate { FocusLost(); });
    }

    public void OpenSubMenu(GameObject neuroTag)
    {
        StartCoroutine(OpenSubMenuTask(neuroTag));
    }

    public IEnumerator OpenSubMenuTask(GameObject neuroTag)
    {
        CameraManger.Instance.ChangeCamera(((MaskSelector)neuroTagContent[neuroTag]).virtualCamera);

        TrackAsset track = ((TimelineAsset)openSubMenuTimeline.playableAsset).GetOutputTrack(0);
        if (neuroTag == maskUIList[0].gameObject)
            openSubMenuTimeline.SetGenericBinding(track, maskUIList[0].transform.GetChild(1).GetComponent<Animator>());
        else if (neuroTag == maskUIList[1].gameObject)
            openSubMenuTimeline.SetGenericBinding(track, maskUIList[1].transform.GetChild(1).GetComponent<Animator>());
        else if (neuroTag == maskUIList[2].gameObject)
            openSubMenuTimeline.SetGenericBinding(track, maskUIList[2].transform.GetChild(1).GetComponent<Animator>());
        else if (neuroTag == maskUIList[3].gameObject)
            openSubMenuTimeline.SetGenericBinding(track, maskUIList[3].transform.GetChild(1).GetComponent<Animator>());

        openSubMenuTimeline.Play();

        for (int i = 0; undoLock.Length; i++)
        {
            lerpTriggered[i] = false;
            lerpLock[i] = false;
            undoTriggered[i] = false;
            undoLock[i] = false;
        }

        if (neuroTag == maskUIList[0].gameObject)
            StartCoroutine(Lerp3());
        else if (neuroTag == maskUIList[1].gameObject)
            StartCoroutine(Lerp1());
        else if (neuroTag == maskUIList[2].gameObject)
            StartCoroutine(Lerp4());
        else if (neuroTag == maskUIList[3].gameObject)
            StartCoroutine(Lerp2());
        yield return new WaitForSeconds((float)openSubMenuTimeline.duration);

        openSubMenuTimeline.Stop();
        //Turn off main mane
        UIMaskListActivated(false);
        //Turn off sub menus
        TurnOffSubMenus();
        //Turn on the correct menu
        MaskSelector maskSelector = (MaskSelector)neuroTagContent[neuroTag];
        maskSelector.OpenSubMenu();
        IconManager.Instance.EnterSubmenu();
    }

    public IEnumerator Lerp1()
    {
        if (!lerpLock[1])
        {
            if (ColorFunctions.Instance.mat.GetFloat("_MaskOneGlow") <= 0.5f && !undoTriggered[1])
                ColorFunctions.Instance.mat.SetFloat("_MaskOneGlow", ColorFunctions.Instance.mat.GetFloat("_MaskOneGlow") + 0.02f);
            if (ColorFunctions.Instance.mat.GetFloat("_MaskTwoGlow") <= 0.5f && !undoTriggered[1])
                ColorFunctions.Instance.mat.SetFloat("_MaskTwoGlow", ColorFunctions.Instance.mat.GetFloat("_MaskTwoGlow") + 0.02f);
            else
                lerp1Triggered = true;

            if (lerp1Triggered && !undoTriggered[1])
            {
                ColorFunctions.Instance.mat.SetFloat("_MaskOneStrength", 0);
                ColorFunctions.Instance.mat.SetFloat("_MaskTwoStrength", 0);
                ColorFunctions.Instance.mat.SetFloat("_MaskOneGlow", 0.5f);
                ColorFunctions.Instance.mat.SetFloat("_MaskTwoGlow", 0.5f);
                yield return new WaitForSeconds(glowWaitTime);                
                lerp1Lock = true;
            }
            yield return new WaitForSeconds(glowWaitTime);
            StartCoroutine(Lerp1());
        }
        else
            StartUndo1();
    }
    public void StartUndo1()
    {
        StartCoroutine(Undo1());
        undoTriggered[1] = false;
    }
    public IEnumerator Undo1()
    {
        if (!undo1Lock)
        {
            if (ColorFunctions.Instance.mat.GetFloat("_MaskOneGlow") >= 0.1f && !undoTriggered[1])
                ColorFunctions.Instance.mat.SetFloat("_MaskOneGlow", ColorFunctions.Instance.mat.GetFloat("_MaskOneGlow") - 0.1f);
            if (ColorFunctions.Instance.mat.GetFloat("_MaskTwoGlow") >= 0.1f && !undoTriggered[1])
                ColorFunctions.Instance.mat.SetFloat("_MaskTwoGlow", ColorFunctions.Instance.mat.GetFloat("_MaskTwoGlow") - 0.1f);
            else
                undoTriggered[1] = true;

            if (undoTriggered[1])
            {
                ColorFunctions.Instance.mat.SetFloat("_MaskOneGlow", 0);
                ColorFunctions.Instance.mat.SetFloat("_MaskTwoGlow", 0);
                undoLock[1] = true;
            }
            yield return new WaitForSeconds(glowWaitTime);
            StartCoroutine(Undo1());
        }
    }
    public IEnumerator Lerp2()
    {
        if (!lerpLock[2])
        {
            if (ColorFunctions.Instance.mat.GetFloat("_MaskThreeGlow") <= 0.5f && !undoTriggered[2])
                ColorFunctions.Instance.mat.SetFloat("_MaskThreeGlow", ColorFunctions.Instance.mat.GetFloat("_MaskThreeGlow") + 0.02f);
            if (ColorFunctions.Instance.mat.GetFloat("_MaskFiveGlow") <= 0.5f && !undoTriggered[2])
                ColorFunctions.Instance.mat.SetFloat("_MaskFiveGlow", ColorFunctions.Instance.mat.GetFloat("_MaskFiveGlow") + 0.02f);
            else
                lerpTriggered[2] = true;

            if (lerpTriggered[2] && !undoTriggered[2])
            {
                ColorFunctions.Instance.mat.SetFloat("_MaskThreeStrength", 0);
                ColorFunctions.Instance.mat.SetFloat("_MaskFiveStrength", 0);
                ColorFunctions.Instance.mat.SetFloat("_MaskThreeGlow", 0.5f);
                ColorFunctions.Instance.mat.SetFloat("_MaskFiveGlow", 0.5f);
                yield return new WaitForSeconds(glowWaitTime);
                lerpLock[2] = true;
            }
            yield return new WaitForSeconds(glowWaitTime);
            StartCoroutine(Lerp2());
        }
        else
            StartUndo2();
    }
    public void StartUndo2()
    {
        StartCoroutine(Undo2());
        undoTriggered[2] = false;
    }
    public IEnumerator Undo2()
    {
        if (!undoLock[2])
        {
            if (ColorFunctions.Instance.mat.GetFloat("_MaskThreeGlow") >= 0.1f && !undoTriggered[2])
                ColorFunctions.Instance.mat.SetFloat("_MaskThreeGlow", ColorFunctions.Instance.mat.GetFloat("_MaskThreeGlow") - 0.1f);
            if (ColorFunctions.Instance.mat.GetFloat("_MaskFiveGlow") >= 0.1f && !undoTriggered[2])
                ColorFunctions.Instance.mat.SetFloat("_MaskFiveGlow", ColorFunctions.Instance.mat.GetFloat("_MaskFiveGlow") - 0.1f);
            else
                undoTriggered[2] = true;

            if (undoTriggered[2])
            {
                ColorFunctions.Instance.mat.SetFloat("_MaskThreeGlow", 0);
                ColorFunctions.Instance.mat.SetFloat("_MaskFiveGlow", 0);
                Debug.Log("Leaving");
                undoLock[2] = true;
            }
            yield return new WaitForSeconds(glowWaitTime);
            StartCoroutine(Undo2());
        }
    }
    public IEnumerator Lerp3()
    {
        if (!lerpLock[3])
        {
            if (ColorFunctions.Instance.mat.GetFloat("_MaskFourGlow") <= 0.5f && !undoTriggered[3])
                ColorFunctions.Instance.mat.SetFloat("_MaskFourGlow", ColorFunctions.Instance.mat.GetFloat("_MaskFourGlow") + 0.02f);
            else
                lerpTriggered[3] = true;

            if (lerpTriggered[3] && !undoTriggered[3])
            {
                ColorFunctions.Instance.mat.SetFloat("_MaskFourStrength", 0);
                ColorFunctions.Instance.mat.SetFloat("_MaskFourGlow", 0.5f);
                yield return new WaitForSeconds(glowWaitTime);
                lerpLock[3] = true;
            }
            yield return new WaitForSeconds(glowWaitTime);
            StartCoroutine(Lerp3());
        }
        else
            StartUndo3();
    }
    public void StartUndo3()
    {
        StartCoroutine(Undo3());
        undoTriggered[3] = false;
    }
    public IEnumerator Undo3()
    {
        if (!undoLock[3])
        {
            if (ColorFunctions.Instance.mat.GetFloat("_MaskFourGlow") >= 0.1f && !undoTriggered[3])
                ColorFunctions.Instance.mat.SetFloat("_MaskFourGlow", ColorFunctions.Instance.mat.GetFloat("_MaskFourGlow") - 0.1f);
            else
                undoTriggered[3] = true;

            if (undoTriggered[3])
            {
                ColorFunctions.Instance.mat.SetFloat("_MasFourGlow", 0);
                Debug.Log("Leaving");
                undoLock[3] = true;
            }
            yield return new WaitForSeconds(glowWaitTime);
            StartCoroutine(Undo3());
        }
    }
    public IEnumerator Lerp4()
    {
        if (!lerpLock[4])
        {
            if (ColorFunctions.Instance.mat.GetFloat("_MaskSixGlow") <= 0.5f && !undoTriggered[4])
                ColorFunctions.Instance.mat.SetFloat("_MaskSixGlow", ColorFunctions.Instance.mat.GetFloat("_MaskSixGlow") + 0.02f);
            else
                lerpTriggered[4] = true;

            if (lerpTriggered[4] && !undoTriggered[4])
            {
                ColorFunctions.Instance.mat.SetFloat("_MaskSixStrength", 0f);
                ColorFunctions.Instance.mat.SetFloat("_MaskSixGlow", 0.5f);
                yield return new WaitForSeconds(glowWaitTime);
                lerpLock[4] = true;
            }
            yield return new WaitForSeconds(glowWaitTime);
            StartCoroutine(Lerp4());
        }
        else
            StartUndo4();
    }
    public void StartUndo4()
    {
        StartCoroutine(Undo4());
        undoTriggered[4] = false;
    }
    public IEnumerator Undo4()
    {
        if (!undoLock[4])
        {
            if (ColorFunctions.Instance.mat.GetFloat("_MaskSixGlow") >= 0.1f && !undoTriggered[4])
                ColorFunctions.Instance.mat.SetFloat("_MaskSixGlow", ColorFunctions.Instance.mat.GetFloat("_MaskSixGlow") - 0.1f);
            else
                undoTriggered[4] = true;

            if (undoTriggered[4])
            {
                ColorFunctions.Instance.mat.SetFloat("_MaskSixGlow", 0);
                Debug.Log("Leaving");
                undoLock[4] = true;
            }
            yield return new WaitForSeconds(0.01f);
            StartCoroutine(Undo4());
        }
    }
    public void CloseSubMenu(GameObject neuroTag)
    {
        StartCoroutine(CloseSubMenuTask(neuroTag));
    }

    public IEnumerator CloseSubMenuTask(GameObject neuroTag)
    {
       // neuroTagContent[neuroTag].GetComponent<MountainSector>().mainAnim.Play("Anim");
        CameraManger.Instance.ReturnToStartCamera();
        //Turn off submenus
        TurnOffSubMenus();
        exitSubMenuTimeline.Play();
        yield return new WaitForSeconds((float)exitSubMenuTimeline.duration);
        exitSubMenuTimeline.Stop();
        //Turn of Main Menu
        UIMaskListActivated(false);
        //Turn on Main menu
        UIMaskListActivated(true);
        LineRendering.Instance.Toggle2DRenderingLinesVisibility(true);
        LineRendering.Instance.ToggleCircleVisibility(true);
        RotationFunctions.Instance.ToggleSliderVisibilty(true);

        IconManager.Instance.ExitSubmenu();
    }

    public void FocusLost()
    {
    }

    public void ChangeColor(GameObject neuroTag)
    {
        MountainSector mountainSector = (MountainSector)neuroTagContent[neuroTag];
        mountainSector.ChangeColor(mountain.GetComponent<ColorFunctions>());
    }
    public void FlipFlopDeers(GameObject neuroTag)
    {
        MountainSector mountainSector = (MountainSector)neuroTagContent[neuroTag];
        mountainSector.FlipFlopDeers(scaleWaitTime);
    }
    public void FlipFlopLodges(GameObject neuroTag)
    {
        MountainSector mountainSector = (MountainSector)neuroTagContent[neuroTag];
        mountainSector.FlipFlopLodges(scaleWaitTime);
    }
    public void FlipFlopTrees(GameObject neuroTag)
    {
        MountainSector mountainSector = (MountainSector)neuroTagContent[neuroTag];
        mountainSector.FlipFlopTrees(scaleWaitTime);
    }
    public void ChangeWeather(GameObject neuroTag)
    {
        MountainSector mountainSector = (MountainSector)neuroTagContent[neuroTag];
        mountainSector.ChangeWeather();
    }
    
    public void IncreaseRotationSpeed(GameObject neuroTag)
    {
        RotationNeuroTag rotationNeuroTag = (RotationNeuroTag)neuroTagContent[neuroTag];
        rotationNeuroTag.IncreaseRotationSpeed(mountain.GetComponent<RotationFunctions>());
    }

    public void DecreaseRotationSpeed(GameObject neuroTag)
    {
        RotationNeuroTag rotationNeuroTag = (RotationNeuroTag)neuroTagContent[neuroTag];
        rotationNeuroTag.DecreaseRotationSpeed(mountain.GetComponent<RotationFunctions>());
    }

    public void UIMaskListActivated(bool value)
    {
            foreach (GameObject neuroTag in maskUIList)
            {
                neuroTag.SetActive(value);
            }
            foreach (GameObject neuroTag in rotationUIList)
            {
                neuroTag.SetActive(value);
            }
    }

    public void TurnOffSubMenus()
    {
        foreach (GameObject neuroTag in maskUIList)
        {
            neuroTag.GetComponent<MaskSelector>().SubmenuUIActive(false);
        }
    }
}
