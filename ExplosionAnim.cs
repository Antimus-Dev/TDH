//Created by; Liam Gilmore
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAnim : MonoBehaviour
{
    public AudioSource SFX;
    public RectTransform rectTransform;
    float t = 0;
    float size = 0;
    public int damage = 50;

    private void OnEnable()
    {
        SFX.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController enemy;
        if (collision.GetComponent<EnemyController>())
        {
            enemy = collision.gameObject.GetComponent<EnemyController>();
            enemy.GetHurt();
            enemy.Enemy.CurrentHP -= damage;
        }
    }
}
