//Created by: Liam Gilmore
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;

public class AvatarAnimation : MonoBehaviour
{
    [SerializeField] string attributeName;
    private int index = 0;

    public float variableTime = 0.03f;

    float pixelsPerFrame = 8f;
    private int i = 0;
    bool foundData = false;
    public bool defaultTesting = false;

    public AvatarAnimationManager animManager;
    public int thisTraitTypeNum = 0;
    SpriteRenderer thisSpriteRenderer;
    Texture2D[] allAvatarSprites;
    public Sprite defaultSprite;
    public Texture2D currentTexture;
    public List<AvatarData> allAvatarData = new List<AvatarData>();
    AvatarData tempAData = new AvatarData();
    [HideInInspector] public AvatarData currentAvatar = new AvatarData();
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

    public class AvatarData
    {
        [JsonProperty("Name")] public string name { get; set; }
        [JsonProperty("Symbol")] public string symbol { get; set; }
        [JsonProperty("Description")] public string description { get; set; }
        [JsonProperty("Seller Fee Basis Points")] public int seller_Fee_Basis_Points { get; set; }
        [JsonProperty("Image")] public string image { get; set; }
        [JsonProperty("External URL")] public string external_URL { get; set; }
        [JsonProperty("Edition")] public int edition { get; set; }
        [JsonProperty("Attributes")] public Firebase.Attributes[] attributes { get; set; }

        public AvatarData()
        {
            name = "Revanent";
            symbol = "RRDC";
            description = "default";
            seller_Fee_Basis_Points = 0;
            image = "default";
            external_URL = "";
            edition = 4;
            attributes = null;
        }
    }

    public class AvatarDic
    {
        [JsonProperty("avatar_data")]
        public Dictionary<string, AvatarData> avatar_data { get; set; }
    }

    void Start()
    {
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
        SliceSprite();
    }

    public void SliceSprite()
    {
        if (!defaultTesting)
        {
            index = animManager.spriteIndex;
            if (SpritesheetManager.instance.spritesheets.Count == 0)
            {
                //print("No spritesheets");
                return;
            }

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
                case ("Body"):
                    SliceSpriteSheet(SpritesheetManager.instance.spritesheets[index].body, 64, attributeName);
                    break;
                case ("Body Tail"):
                    SliceSpriteSheet(SpritesheetManager.instance.spritesheets[index].bodyTail, 64, attributeName);
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
            thisSpriteRenderer.enabled = false;
            return;
        }
        thisSpriteRenderer.enabled = true;
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
        SetUpAvatarForAnimation();
    }

    public void SetUpAvatarForAnimation()
    {
        idleFrames = currentIdleFrames;
        walkFrames = currentWalkFrames;
        attackFrames = currentAttackFrames;
        throwFrames = currentThrowFrames;
    }
    IEnumerator SetAvatarWalkFrame(int f)
    {
        if (walkFrames != null && walkFrames.Length != 0)
            if (animManager.thisAnimator.GetBool("isWalk") == true)
            {
                currentFrame = walkFrames[f];
                yield return new WaitForSeconds(variableTime);                    
            }
            else
                Debug.Log("SetAvatarWalkFrame: no frames set");
        yield return new WaitForSeconds(variableTime);

        if (currentFrame != null)
            thisSpriteRenderer.sprite = currentFrame;
        if (animManager.thisAnimator.GetBool("isGround") == false)
            AvatarJumpCycle();
        else
            AvatarWalkCycle();
    }

    IEnumerator SetAvatarIdleFrame(int f)
    {
        if (idleFrames != null && idleFrames.Length != 0)
            if (animManager.thisAnimator.GetBool("isWalk") == false)
            {
                currentFrame = idleFrames[f];
                yield return new WaitForSeconds(variableTime);
            }
            else
                Debug.Log("SetAvatarIdleFrame: no frames set");
        yield return new WaitForSeconds(variableTime);

        if (currentFrame != null)
            thisSpriteRenderer.sprite = currentFrame;

            AvatarIdleCycle();
    }

    IEnumerator SetAvatarAttackFrame(int f)
    {
        if (attackFrames != null && attackFrames.Length != 0)
            if (animManager.thisAnimator.GetBool("Attack") == true)
            {
                currentFrame = attackFrames[f];
                yield return new WaitForSeconds(variableTime);
            }
            else
                Debug.Log("SetAvatarAttackFrame: no frames set");
        yield return new WaitForSeconds(variableTime);

        if (currentFrame != null)
            thisSpriteRenderer.sprite = currentFrame;
        AvatarAttackCycle();
    }

    IEnumerator SetAvatarThrowFrame(int f)
    {
        if (throwFrames != null && throwFrames.Length != 0)
            if (animManager.thisAnimator.GetBool("Throw") == true)
            {
                currentFrame = throwFrames[f];
                yield return new WaitForSeconds(variableTime);    
            }
            else
                Debug.Log("SetAvatarIdleFrame: no frames set");
        yield return new WaitForSeconds(variableTime);

        if (currentFrame != null)
            thisSpriteRenderer.sprite = currentFrame;
        AvatarThrowCycle();
    }

    public void AvatarWalkCycle()
    {
        
        if (animManager.thisAnimator.GetBool("isWalk") == false || animManager.thisAnimator.GetBool("isGround") == false)
        {
            i = 0;
            animManager.animationBranched = false;
            return;
        }
        
        if (i >= walkFrames.Length)
            i = 0;
        StartCoroutine(SetAvatarWalkFrame(i));
        i++;
    }
    public void AvatarIdleCycle()
    {
        if (animManager.thisAnimator.GetBool("isWalk") == true || animManager.thisAnimator.GetBool("isGround") == false)
        {
            i = 0;
            animManager.animationBranched = false;
            return;
        }
        
        if (i >= (idleFrames.Length))
            i = 0;
        StartCoroutine(SetAvatarIdleFrame(i));
        i++;
    }
    public void AvatarAttackCycle()
    {
        if (animManager.thisAnimator.GetBool("Attack") == false)
        {
            i = 0;
            animManager.animationBranched = false;
            return;
        }
        
        if (i >= attackFrames.Length)
            i = 0;
        StartCoroutine(SetAvatarAttackFrame(i));
        i++;
    }
    public void AvatarThrowCycle()
    {
        if (animManager.thisAnimator.GetBool("Throw") == false)
        {
            i = 0;
            animManager.animationBranched = false;
            return;
        }
        StartCoroutine(SetAvatarThrowFrame(0));
    }

    public void AvatarJumpCycle()
    {
        if (animManager.thisAnimator.GetBool("isGround") == true)
        {
            i = 0;
            animManager.animationBranched = false;
            return;
        }
        StartCoroutine(SetAvatarWalkFrame(1));
    }
}