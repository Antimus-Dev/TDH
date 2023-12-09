//Created by: Liam Gilmore
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectDisplayManager : MonoBehaviour
{
    public GameObject[] accessories = new GameObject[13];
    public GameObject selectionOverlay;
    public CharacterSelectDisplay[] avatarAnim = new CharacterSelectDisplay[13];
    public CharacterCarousel carousel;
    public Image emptyImage = null;
    public int spriteIndex = 0;
    public bool selected = false;
    public bool displayObject = false;

    void Start()
    {
        for (int i = 0; i < accessories.Length; i++)
            avatarAnim[i] = accessories[i].GetComponent<CharacterSelectDisplay>();

        if (!displayObject)
        {
            selectionOverlay = gameObject.transform.GetChild(0).gameObject;
            carousel = gameObject.transform.parent.parent.parent.parent.parent.GetChild(2).GetComponent<CharacterCarousel>();
        }
    }

    public void SliceNewSprites(int index = 0)
    {
        spriteIndex = index;
        Debug.Log("Sprite Index: " + spriteIndex);
        Debug.Log("Avatar Anim Length: " + avatarAnim.Length);

        for (int i = 0; i < avatarAnim.Length; i++)
        {
            if (avatarAnim[i] != null)
            {
                Debug.Log("Avatar Anim: " + avatarAnim[i]);

                avatarAnim[i].enabled = true;
                avatarAnim[i].gameObject.GetComponent<Image>().enabled = true;
                avatarAnim[i].SliceSprite();
            }
        }          
    }
}
