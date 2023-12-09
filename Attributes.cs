//Created by: Liam Gilmore
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attributes", menuName = "ScriptableObjects/Attributes", order = 1)]
public class Attributes : ScriptableObject
{
    [Header("Default Values")]
    public int lives = 2;
    public int maxHP = 100;
    //public int f2pHPMod = -20;
    public float invincibleTime = 0.1f;
    public float enemyInvincibleTime = 0.5f;
    public float bulletCooldown = 0.2f;
    public float respawnTime = 2f;

    [Header("Basic Movement Attributes")]
    public float moveMagnitude = 0.2f;
    public float moveMagnitudeCap = 0.5f;
    public float moveDeceleration = 0.2f;
    public float airMoveMagnitude = 0.2f;
    public float airMoveMagnitudeCap = 0.5f;
    public float airMoveDeceleration = 0.2f;
    public float jumpMagnitude = 5f;
    public float jumpHoldMaxDuration = 0.5f;
    public float jumpReleaseBump = 5f;
    public float coyoteTime = 0.2f;
    public float dashSpeed = 5f;
    public int airJumps = 1;
    public int airDashes = 1;
    public float dashCooldown = 1f;
    public float dashSnapVerticalDistance = 0.1f;

    [Header("Basic Attack Attributes")]
    public int stompDamage = 10;
    public float stompBounceMagnitude = 0.2f;
    public float maxAmmo = 4;
    //public int f2pAmmoMod = -1;
    public float ammoRechargeAmount = 0.2f;

    [Header("Physics Attributes")]
    public float terminalVelocity = 5f;
    public float gravity = 3f;
    public float bumpDecaySpeed = 2f;
    public float floatThreshold = 0.5f;
    public float floatMagnitude = 0.025f;

    [Header("Visuals")]
    public float flashSpeed = 0.02f;

    [Header("Prefabs")]
    public GameObject playerPrefab = null;

    [Header("NFT Attributes")]
    public float healthRegenAmount = 0;
    public float addedProjectiles = 0;
    public bool snowBallProjectiles = false;
    public bool slowBeingAttacked = false;
    public bool slowOnAttack = false;
    public bool thorns = false;
    public bool doubleMelee = false;
    public bool doubleProjectile = false;
    public bool doubleDash = false;
    public bool lightningDash = false;
    public bool healthRegen = false;
    public bool healthRegenToHalf = false;
    public bool reflect = false;
    public bool triShot = false;
    public bool bubbleShield = false;
    public bool invisible = false;
    public bool oneBomb = false;
    public bool TwoBomb = false;
    public bool eyeLasers = false;
    public bool chestLaser = false;
    public bool mouthLaser = false;
    public bool throwItem = false;
    public bool throwItemDagger = false;
    public bool throwItemSombrero = false;
    public bool throwItemSpikeBall = false;
    public float luck = 0;
}
