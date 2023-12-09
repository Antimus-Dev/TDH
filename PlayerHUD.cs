//Created by: Liam Gilmore
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public int playerNumber = 0;
    public int heartOffset;
    public int offset = 0;

    bool firstMax = true;
    bool livesSpawned;

    public Animator HPFill;
    public Animator shimmer;

    public RuntimeAnimatorController HPFull;
    public RuntimeAnimatorController HPMid;

    public GameObject lives;
    public GameObject lifeObject;
    public GameObject crack1;
    public GameObject crack2;
    public GameObject crack3;

    public string playerName;
    public Text playerDisplayText;
    public Image playerHPFill;

    public IEnumerator InstantiateLife(int count, bool offsetActive)
    {
        if(offsetActive)
            for (int i = 0; i < count; i++)
                Instantiate(lifeObject, new Vector2(lives.transform.position.x + offset, lives.transform.position.y), Quaternion.identity, lives.transform);
        else
            for (int i = 0; i < count; i++)
            {
                Instantiate(lifeObject, new Vector2(lives.transform.position.x + offset, lives.transform.position.y), Quaternion.identity, lives.transform);
                offset += heartOffset;
            }

        yield return new WaitForSeconds(0);
    }

    private void Update()
    {
        if (GameController.instance.playerData.Count > 0 && !livesSpawned) 
        {
            if (GameController.instance.playerData[playerNumber].lives > 1 && GameController.instance.debug.livesAdded) 
            {
                int startingLives = GameController.instance.playerData[playerNumber].lives;

                for (int i = 0; i < startingLives + 1; i++)
                {
                    Instantiate(lifeObject, new Vector2(lives.transform.position.x + offset, lives.transform.position.y), Quaternion.identity, lives.transform);
                    offset += heartOffset;
                }

                livesSpawned = true;
            }
        }

        if(GameController.instance.playerData[playerNumber].NFTPlayer != null && GameController.instance.playerData[playerNumber].controller.Player.NFTCharacter)
            playerHPFill.fillAmount = (float)GameController.instance.playerData[playerNumber].hp / (float)GameController.instance.playerData[playerNumber].GetMaxHP(GameController.instance.playerData[playerNumber].NFTPlayer);
        else
            playerHPFill.fillAmount = (float)GameController.instance.playerData[playerNumber].hp / (float)GameController.instance.playerData[playerNumber].GetMaxHP();

        playerDisplayText.text = playerName; //+ "\nLIVES: " + GameController.instance.playerData[playerNumber].lives.ToString();
        
        

        if(playerHPFill.fillAmount == 1)
        {
            HPFill.runtimeAnimatorController = HPFull;
            crack3.SetActive(false);
            crack2.SetActive(false);
            crack1.SetActive(false);

            if (firstMax)
                StartCoroutine(Shimmer());
        }
        else if (playerHPFill.fillAmount < 1 && playerHPFill.fillAmount > 0)
        {
            HPFill.runtimeAnimatorController = HPMid;
            firstMax = true;

            if(playerHPFill.fillAmount <= .5f)
                crack1.SetActive(true);
            else
                crack1.SetActive(false);

            if (playerHPFill.fillAmount <= .25f)
            {
                crack1.SetActive(true);
                crack2.SetActive(true);
            }                
            else
                crack2.SetActive(false);                             
        }
        else
        {
            crack1.SetActive(true);
            crack2.SetActive(true);
            crack3.SetActive(true);
        }
            
    }

    IEnumerator Shimmer()
    {
        shimmer.gameObject.SetActive(true);
        yield return new WaitForSeconds(.2f);
        shimmer.gameObject.SetActive(false);
        firstMax = false;
    }
}
