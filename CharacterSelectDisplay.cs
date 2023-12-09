//Created by: Liam Gilmore
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;

public class CharacterSelectDisplay : MonoBehaviour
{
    [SerializeField] string attributeName;
    private int index = 0;

    public float variableTime = 0.03f;

    float pixelsPerFrame = 8f;
    private int i = 0;
    bool foundData = false;
    public bool defaultTesting = false;

    public CharacterSelectDisplayManager animManager;
    public int thisTraitTypeNum = 0;
    Image thisImage;
    Texture2D[] allAvatarSprites;
    public Sprite defaultSprite;
    public Texture2D currentTexture;
    [HideInInspector] public Sprite currentFrame;

    [HideInInspector] public Sprite[] idleFrames = new Sprite[8],
                    walkFrames = new Sprite[4],
                    attackFrames = new Sprite[7],
                    throwFrames = new Sprite[1];

    [HideInInspector] public Sprite[] spriteFrames,
                    currentIdleFrames = new Sprite[8],
                    currentWalkFrames = new Sprite[4],
                    currentAttackFrames = new Sprite[7],
                    currentThrowFrames = new Sprite[1];

    void Start()
    {
        thisImage = GetComponent<Image>();
        SliceSprite();
    }

    public void SliceSprite()
    {
        if (!defaultTesting)
        {
            index = animManager.spriteIndex;
            if (SpritesheetManager.instance.spritesheets.Count == 0)
                return;

            switch (attributeName)
            {
                case ("Headwear"):
                    SliceSpriteSheet(SpritesheetManager.instance.spritesheets[index].headwear, 64, attributeName);
                    break;
                case ("Bodywear"):
                    SliceSpriteSheet(SpritesheetManager.instance.spritesheets[index].bodywear, 64, attributeName);
                    break;
                case ("Face Accessory"):
                    SliceSpriteSheet(SpritesheetManager.instance.spritesheets[index].faceAccessory, 64, attributeName);
                    break;
                case ("Pants"):
                    SliceSpriteSheet(SpritesheetManager.instance.spritesheets[index].pants, 64, attributeName);
                    break;
                case ("Shoes"):
                    SliceSpriteSheet(SpritesheetManager.instance.spritesheets[index].shoes, 64, attributeName);
                    break;
                case ("Weapon"):
                    SliceSpriteSheet(SpritesheetManager.instance.spritesheets[index].weapon, 64, attributeName);
                    break;
                case ("Backwear"):
                    SliceSpriteSheet(SpritesheetManager.instance.spritesheets[index].backwear, 64, attributeName);
                    break;
                case ("Tail"):
                    SliceSpriteSheet(SpritesheetManager.instance.spritesheets[index].tail, 64, attributeName);
                    break;
                case ("Body Tail"):
                    SliceSpriteSheet(SpritesheetManager.instance.spritesheets[index].bodyTail, 64, attributeName);
                    break;
                case ("Body"):
                    SliceSpriteSheet(SpritesheetManager.instance.spritesheets[index].body, 64, attributeName);
                    break;
                case ("Eyes"):
                    SliceSpriteSheet(SpritesheetManager.instance.spritesheets[index].eyes, 64, attributeName);
                    break;
                case ("Outerwear"):
                    SliceSpriteSheet(SpritesheetManager.instance.spritesheets[index].outerwear, 64, attributeName);
                    break;
                case ("Special"):
                    SliceSpriteSheet(SpritesheetManager.instance.spritesheets[index].special, 64, attributeName);
                    break;
                default:
                    break;
            }
        }
        else
            DefaultSpriteSplice();

    }

    
    public void DefaultSpriteSplice()
    {
        if (defaultSprite != null)
            SliceSpriteSheet(defaultSprite.texture, 64, "default");
    }

    /// <summary>
    /// Takes the provided 2d textures and slices it up to be used in in the sprite animations.
    /// </summary>
    /// <param name="texture">Sprite sheet to slice up.</param>
    private void SliceSpriteSheet(Texture2D texture, int size, string name)
    {
        if (texture == null)
        {
            thisImage.enabled = false;
            return;
        }
        thisImage.enabled = true;
        Sprite tempSlicedAvatarSprite;
        spriteFrames = new Sprite[32];

        for (int r = 0; r < 4; r++)
        {
            string rowName = "";

            switch (r)
            {
                case 0:
                    rowName = "Idle";
                    break;

                case 1:
                    rowName = "Walk";
                    break;

                case 2:
                    rowName = "Attack";
                    break;
                case 3:
                    rowName = "Throw";
                    break;
            }

            for (int c = 0; c < 8; c++)
            {
                tempSlicedAvatarSprite = Sprite.Create(texture, new Rect(c * size, r * size, size, size), new Vector2(0.5f, 0.5f), pixelsPerFrame);
                tempSlicedAvatarSprite.name = name + rowName + c;
                spriteFrames[r * 8 + c] = tempSlicedAvatarSprite;

                switch (r)
                {
                    case 0:
                        if (c < 1)
                            currentThrowFrames[c] = tempSlicedAvatarSprite;
                        break;

                    case 1:
                        if (c < 7)
                            currentAttackFrames[c] = tempSlicedAvatarSprite;

                        break;

                    case 2:
                        if (c < 4)
                            currentWalkFrames[c] = tempSlicedAvatarSprite;

                        break;

                    case 3:
                        if (c < 8)
                            currentIdleFrames[c] = tempSlicedAvatarSprite;

                        break;
                }
            }
        }

        thisImage.sprite = currentIdleFrames[0];
    }

}