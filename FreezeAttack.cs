//Created by: Liam Gilmore
//Freeze attack for level 6 boss to immobilize the player
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeAttack : MonoBehaviour
{
    public State freezeState;
    public State idleState;

    private Player player;
    public int frozenTime = 3;
    private void Start()
    {
        player = GameController.instance.playerData[0].controller.Player;
    }
    public void FreezeCheck()
    {
        if (player.IsGround)
            StartCoroutine(Freeze());
    }

    IEnumerator Freeze()
    {
        player.CurrentState.SwitchState(player, freezeState);
        SpriteRenderer[] sprites = player.gameObject.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sprite in player.gameObject.GetComponentsInChildren<SpriteRenderer>())
            sprite.color = Color.blue;

        yield return new WaitForSeconds(frozenTime);
        player.CurrentState.SwitchState(player, idleState);

        foreach (SpriteRenderer sprite in player.gameObject.GetComponentsInChildren<SpriteRenderer>())
            sprite.color = Color.white;
    }
}
