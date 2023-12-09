//Created by: Liam Gilmore
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotationFunctions : MonoBehaviour
{
    public float passiveSpeed = 0.1f;
    public float min = -0.5f;
    public float max = 0.5f;

    private float maxIncrement = 0;
    private float minIncrement = 0;
    private bool animating = false;

    [SerializeField]
    private Slider _slider;


    void Start()
    {
        _slider.onValueChanged.AddListener((slideValue) =>
        {
            passiveSpeed = (float)slideValue;
        });
    }

    public void FixedUpdate()
    {
        if (!animating && passiveSpeed < 0.095f && passiveSpeed > -0.095f)
        {
            passiveSpeed = 0;
        }

        _slider.value = passiveSpeed;
        gameObject.transform.Rotate(0, passiveSpeed, 0);
    }
    public void StartIncreasePassiveSpeed()
    {
        animating = true;
        StartCoroutine(IncreasePassiveSpeed());
    }
    public void StartDecreasePassiveSpeed()
    {
        animating = true;
        StartCoroutine(DecreasePassiveSpeed());
    }

    public IEnumerator IncreasePassiveSpeed()
    {
        if (maxIncrement == 0 && passiveSpeed < max)
            maxIncrement = passiveSpeed + max / 5;

        if (maxIncrement != 0)    
            if (passiveSpeed >= maxIncrement)
            {
                if (passiveSpeed > max)
                {
                    passiveSpeed = max;
                    _slider.value = passiveSpeed;
                }

                maxIncrement = 0;
                animating = false;
                yield return null;
            }
            else
            {
                passiveSpeed = passiveSpeed + 0.01f;
                _slider.value = passiveSpeed;

                yield return new WaitForSeconds(0.01f);
                StartCoroutine(IncreasePassiveSpeed());
            }
    }

    public IEnumerator DecreasePassiveSpeed()
    {
        if (minIncrement == 0 && passiveSpeed > min)
            minIncrement = passiveSpeed - -min / 5;

        if (minIncrement != 0)
            if (passiveSpeed <= minIncrement)
            {
                if(passiveSpeed < min)
                {
                    passiveSpeed = min;
                    _slider.value = passiveSpeed;
                }

                minIncrement = 0;
                animating = false;
                yield return null;
            }
            else
            {   
                passiveSpeed = passiveSpeed - 0.01f;
                _slider.value = passiveSpeed;

                yield return new WaitForSeconds(0.01f);
                StartCoroutine(DecreasePassiveSpeed());
            }
    }
}
