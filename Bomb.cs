//Created by: Liam Gilmore
//Bomb ability for players
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float igniteTimer = 0;
    public float timerThreshold = 0;
    public float radius = 0;
    public Collider2D[] enemyColliders;
    public GameObject explosionsFX;
    private bool doOnce = true;
    public SpriteRenderer bombSpriteColor;
    public RectTransform bombSpriteRectTransform;
    private Vector3 position;
    private float shakeTimer;
    private float shakeDir = 1;

    void Start()
    {
        position = bombSpriteRectTransform.anchoredPosition;
        shakeTimer = 0.5f;
    }

    void Update()
    {
        igniteTimer += Time.deltaTime;
     
 
        if (igniteTimer >= timerThreshold)
        {
            explosionsFX.SetActive(true);

            if (igniteTimer > timerThreshold + 1)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            shakeTimer += Time.deltaTime * shakeDir * (5 + igniteTimer * 10);
            bombSpriteColor.color = new Vector4(igniteTimer / timerThreshold, 0, 0, 1);
            bombSpriteRectTransform.anchoredPosition = new Vector3(position.x + Mathf.Lerp(-.05f, .05f, shakeTimer), position.y, position.z);
            if (shakeTimer >= 1)
            {
                shakeDir *= -1;
            }
            else if (shakeTimer <= 0)
            {
                shakeDir = 1;
            }
        }
    }
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, radius);
    }
}
