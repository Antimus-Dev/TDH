//Created by: Liam Gilmore, Jonathan Lao, Andrew Sylvester
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class JavascriptHook : MonoBehaviour
{
    [SerializeField] CharacterCarousel characterCarousel;
    [SerializeField] TMP_Text testText;
    [SerializeField] P_PlayerPrefs playerPrefs;
    [SerializeField] Attributes defaultStats;
    [SerializeField] Attack meleeStats;
    [SerializeField] Attack projectileStats;
    [SerializeField] int defaultMeleeDmg;
    int defaultProjectileDmg;

    public static JavascriptHook instance = null;
    public bool levelProgressHandledFromWebsite = false;

    public static List<AttributesJson> nftAttributes = new List<AttributesJson>();
    [SerializeField] string testAttributeJsonString;

    [Header("Initial NFT Attributes")]
    [SerializeField] int moveSpeed = 100;
    [SerializeField] float ammoRechargeSpeed = 0.05f;    // This is by percentage

    public bool fire = false;
    public bool holy = false;
    public bool acid = false;
    public bool bleed = false;

    public int statusDamage;
    public int statusCount;

    public GameObject brownBottles;

    private void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(gameObject);
        }
        else if(instance == null)
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        defaultMeleeDmg = meleeStats.damage;
        defaultProjectileDmg = projectileStats.damage;
        //ReceiveAttributeJson(testAttributeJsonString);
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            RecieveWalletJson("{ \"hasDopeCat\":true, \"hasPixelBand\":true}");
        }*/
    }

    /// <summary>
    /// To be called from website with web3 data. Json of what nft collections is parsed here.
    /// </summary>
    /// <param name="json"></param>
    public void RecieveWalletJson(string json)
    {
        JsonObject jsonObject = JsonUtility.FromJson<JsonObject>(json);
        playerPrefs.charactersUnlocked[1] = jsonObject.hasDopeCat || jsonObject.hasSovanaEgg;
        playerPrefs.charactersUnlocked[4] = jsonObject.hasPixelBand || jsonObject.hasSovanaEgg;
        playerPrefs.charactersUnlocked[7] = jsonObject.hasHippo || jsonObject.hasSovanaEgg;
        playerPrefs.charactersUnlocked[2] = jsonObject.hasSovanaEgg;
        playerPrefs.charactersUnlocked[5] = jsonObject.hasSovanaEgg;
        playerPrefs.charactersUnlocked[8] = jsonObject.hasSovanaEgg;
        playerPrefs.charactersUnlocked[10] = jsonObject.hasSovanaEgg || jsonObject.hasCyberSamurai;
        playerPrefs.charactersUnlocked[11] = jsonObject.hasSovanaEgg;
        playerPrefs.charactersUnlocked[13] = jsonObject.hasSovanaEgg || jsonObject.hasPenguin;
        playerPrefs.charactersUnlocked[14] = jsonObject.hasSovanaEgg;
        playerPrefs.charactersUnlocked[16] = jsonObject.hasSovanaEgg;
        playerPrefs.charactersUnlocked[17] = jsonObject.hasSovanaEgg;

        characterCarousel.RefreshCharacters();
    }

    /// <summary>
    /// To be called from website with web3 data. The metadata of rr special nfts will be sent here to be parsed and the sprite sheets pulled
    /// </summary>
    /// <param name="json">NFT metadata</param>
    public void ReceiveAttributeJson(string json)
    {
        AttributesJson nftAttribute = new AttributesJson();
        metadata metadata = JsonUtility.FromJson<metadata>(json);
        nftAttribute.metadata = metadata;

        foreach(Attribute attribute in metadata.attributes)
        {
            attribute.value = attribute.value.Trim();
            switch (attribute.trait_type)
            {
                case ("Headwear"):
                    nftAttribute.headwear = attribute;
                    break;
                case ("Bodywear"):
                    nftAttribute.bodywear = attribute;
                    break;
                //case ("eyewear"): //Not used
                   // nftAttribute.eyewear = attribute;
                    //break;
                case ("Face Accessory"):
                    nftAttribute.faceAccessory = attribute;
                    break;
               // case ("handAccessory"): //Not Used
                   //// nftAttribute.handAccessory = attribute;
                    //break;
                case ("Pants"):
                    nftAttribute.pants = attribute;
                    break;
                case ("Shoes"):
                    nftAttribute.shoes = attribute;
                    break;
                case ("Weapon"):
                    nftAttribute.weapon = attribute;
                    break;
                //case ("rangedWeapon"): //Not used
                   // nftAttribute.rangedWeapon = attribute;
                   // break;
                case ("Backwear"):
                    nftAttribute.backwear = attribute;
                    break;
                case ("Tail"):
                    nftAttribute.tail = attribute;
                    break;
                case ("Background"):
                    nftAttribute.background = attribute;
                    break;
                case ("Body"):
                    nftAttribute.bodyType = attribute;
                    break;
                case ("Body Tail"):
                    nftAttribute.bodyTail = attribute;
                    break;
                case ("Eyes"):
                    nftAttribute.eyes = attribute;
                    break;
                case ("Outerwear"):
                    nftAttribute.outerWear = attribute;
                    break;
                case ("Special"):
                    nftAttribute.special = attribute;
                    break;
                default:
                    break;
            }
        }

        nftAttributes.Add(nftAttribute);
        //PrintNFTAttributes();
        if (nftAttributes.Count - 1 <= 9)
        {
            SpritesheetManager.instance.PullSpritesheets(nftAttributes.Count - 1);

        }

        //AddAttributes(playerPrefs.NFTModifier, nftAttributes.Count - 1);
    }
    public void PrintNFTAttributes()
    {
        print(nftAttributes[0].metadata.symbol + ", " + nftAttributes[0].metadata.name + ", " + nftAttributes[0].metadata.edition + ", " + nftAttributes[0].metadata.attributes[0].trait_type);
        print(nftAttributes[0].metadata.symbol + ", " + nftAttributes[0].backwear.value + ", " + nftAttributes[0].bodywear.value + ", " + nftAttributes[0].faceAccessory.value);
    }


    #region Dope Cats Attributes
    public void AddAttributes(Attributes NFTAttributes, int index)
    {
        float healthModAmount = 0;
        float meleeModAmount = 0;
        float rangedModAmount = 0;
        float projectileCountModAmount = 0;
        bool snowballs = false;
        // Headwear  
        NFTAttributes = HeadWearAttributes(NFTAttributes, healthModAmount, meleeModAmount, projectileCountModAmount, index);
        if (NFTAttributes.snowBallProjectiles)
            snowballs = true;
        //Bodywear
        NFTAttributes = BodyWearAttributes(NFTAttributes, healthModAmount, meleeModAmount, projectileCountModAmount, index);
        if (NFTAttributes.snowBallProjectiles)
            snowballs = true;
        //Eyewear
        //NFTAttributes = EyewearAttributes(NFTAttributes, healthModAmount, meleeModAmount, projectileCountModAmount, index);

        //Mouth Accessory
        // NFTAttributes = MouthAccessoryAttributes(NFTAttributes, healthModAmount, meleeModAmount, projectileCountModAmount, index);

        //Hand Accessory
        //NFTAttributes = HandAccessoryAttributes(NFTAttributes, healthModAmount, meleeModAmount, projectileCountModAmount, index);

        //Pants
        NFTAttributes = PantsAttributes(NFTAttributes, healthModAmount, meleeModAmount, projectileCountModAmount, index);
        if (NFTAttributes.snowBallProjectiles)
            snowballs = true;
        //Shoes
        NFTAttributes = ShoesAttributes(NFTAttributes, healthModAmount, meleeModAmount, projectileCountModAmount, index);
        if (NFTAttributes.snowBallProjectiles)
            snowballs = true;
        //Melee Weapon
        // NFTAttributes = MeleeWeaponAttributes(NFTAttributes, healthModAmount, meleeModAmount, projectileCountModAmount, index);

        //Ranged Weapon
        // NFTAttributes = RangedWeaponAttributes(NFTAttributes, healthModAmount, meleeModAmount, rangedModAmount, projectileCountModAmount, index);

        //Back Wear
        NFTAttributes = BackWearAttributes(NFTAttributes, healthModAmount, meleeModAmount, projectileCountModAmount, index);
        if (NFTAttributes.snowBallProjectiles)
            snowballs = true;
        //Tail
        NFTAttributes = TailAttributes(NFTAttributes, healthModAmount, meleeModAmount, projectileCountModAmount, index);
        if (NFTAttributes.snowBallProjectiles)
            snowballs = true;
        //Background
        NFTAttributes = BackgroundAttributes(NFTAttributes, healthModAmount, meleeModAmount, projectileCountModAmount, index);
        if (NFTAttributes.snowBallProjectiles)
            snowballs = true;
        //BodyType
        NFTAttributes = BodyTypeAttributes(NFTAttributes, healthModAmount, meleeModAmount, projectileCountModAmount, index);
        if (NFTAttributes.snowBallProjectiles)
            snowballs = true;
        //Eyes
        NFTAttributes = EyesAttributes(NFTAttributes, healthModAmount, meleeModAmount, projectileCountModAmount, index);
        if (NFTAttributes.snowBallProjectiles)
            snowballs = true;
        //FaceAccessories
        NFTAttributes = FaceAccessoryAttributes(NFTAttributes, healthModAmount, meleeModAmount, projectileCountModAmount, index);
        if (NFTAttributes.snowBallProjectiles)
            snowballs = true;
        //Outer Wear
        NFTAttributes = OuterWearAttributes(NFTAttributes, healthModAmount, meleeModAmount, projectileCountModAmount, index);
        if (NFTAttributes.snowBallProjectiles)
            snowballs = true;
        //Weapon
        NFTAttributes = WeaponAttributes(NFTAttributes, healthModAmount, meleeModAmount, rangedModAmount, projectileCountModAmount, index);
        if (NFTAttributes.snowBallProjectiles)
            snowballs = true;
        //Special
        NFTAttributes = SpecialAttributes(NFTAttributes, healthModAmount, index);
        if (NFTAttributes.snowBallProjectiles)
            snowballs = true;
        //if (NFTAttributes.throwItem)
        //{
        //    if (NFTAttributes.throwItemDagger)
        //        if (NFTAttributes.throwItemSombrero)
        //            if (NFTAttributes.throwItemSpikeBall)
        //}
        if (snowballs)
            NFTAttributes.snowBallProjectiles = true;

    }
    Attributes SpecialAttributes(Attributes specialAttributes, float healthModAmount, int index)
    {
        if (nftAttributes[index].special == null)
            return specialAttributes;
        switch (nftAttributes[index].special.value)
        {
            case ("Duck"):          //DONE
                // Add health 1
                healthModAmount = 1;

                // .5 extra projectile
                specialAttributes.maxAmmo += .5f;

                // +1 life
                specialAttributes.lives += 1;

                // +1 health regen
                specialAttributes.healthRegen = true;
                specialAttributes.healthRegenAmount += .005f;

                //+1 Luck
                specialAttributes.luck += 1;
                break;
        }
        specialAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
       // specialAttributes.maxAmmo += MaxAmmoModifier(specialAttributes.maxAmmo, defaultStats.maxAmmo);

        return specialAttributes;
    }

    Attributes HeadWearAttributes(Attributes headWearAttributes, float healthModAmount, float meleeModAmount, float projectileCountModAmount, int index)
    {
        if (nftAttributes[index].headwear == null)
            return headWearAttributes;
        switch (nftAttributes[index].headwear.value)
        {
            case ("Top Hat"):
                // Add health 0.25f
                healthModAmount = 0.25f;
                break;

            case ("Leather Helmet"):
                // Add health 0.25f
                healthModAmount = 0.25f;
                break;

            case ("Iron Helmet"):
                // Add 0.5f health
                healthModAmount = 0.5f;
                break;

            case ("Bronze Helmet"):
                // Add 0.5f health
                healthModAmount = 0.5f;
                break;

            case ("Chrome Helmet"):
                // Add 0.75f health
                healthModAmount = 0.75f;
                break;

            case ("Rubber Helmet"):          //DONE                     
                // Add 0.75 Health
                healthModAmount = 0.75f;

                // 1 Bubble shield/ level
                headWearAttributes.bubbleShield = true;
                break;

            case ("Chrono Helmet"):         //DONE
                // Add 1 Health
                healthModAmount = 1.0f;

                // Fast movement
                headWearAttributes.moveMagnitude = moveSpeed;
                break;

            case ("Winged Helmet"):         //DONE
                // Add 1 health
                healthModAmount = 1.0f;

                // Double Jump
                headWearAttributes.airJumps += 1;
                break;

            case ("Rainbow Helmet"):            //DONE
                // Add 1.25 health
                healthModAmount = 1.25f;

                // Fast Movement
                headWearAttributes.moveMagnitude = moveSpeed;

                // 1 extra projectile
                headWearAttributes.maxAmmo += 1;

                // +1 projectile regen
                headWearAttributes.ammoRechargeAmount += ammoRechargeSpeed;

                // +1 life
                headWearAttributes.lives += 1;
                break;

            case ("Flaming Helmet"):     //DONE                       
                // + 0.75 health
                healthModAmount = 0.75f;

                // return .25 ticks of damage when hit
                headWearAttributes.thorns = true;
                break;

            case ("Ice Helmet"):      //DONE                                 
                // + 0.75 health
                healthModAmount = 0.75f;

                // enemies are slowed after damaging player
                headWearAttributes.slowBeingAttacked = true;
                break;

            case ("Gyro Cap"):       //DONE                         
                // +0.75 health
                healthModAmount = 0.75f;

                // Double Jump
                headWearAttributes.airJumps +=1;

                // Glide
                headWearAttributes.airMoveMagnitudeCap = 5;
                break;

            case ("Sombrero"):       //DONE                           
                // +0.75f health
                healthModAmount = 0.75f;

                // Throw Sombrero when projectiles are empty
                headWearAttributes.throwItem = true;
                headWearAttributes.throwItemSombrero = true;
                break;

            case ("Pumpkin Head"):             //DONE                 
                // +0.75f Health
                healthModAmount = 0.75f;

                // fire twice (beams of  light, one from each eye) when projectiles are empty"
                headWearAttributes.eyeLasers = true;
                break;

            case ("Santa Hat"):       //DONE                         
                // +1 Health
                healthModAmount = 1;

                // +1 Projectile
                headWearAttributes.maxAmmo += 1;

                // Projectiles are snowballs
                headWearAttributes.snowBallProjectiles = true;
                break;

            case ("Bunny Ears"):        //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                // Double Jump
                headWearAttributes.airJumps += 1;
                break;

            case ("Anon Mask"):    //DONE                               
                // +1 health
                healthModAmount = 1;

                // Can Become Invisible (enemies can't track)
                headWearAttributes.invisible = true;
                break;

            case ("Halo"):          //DONE
                // +1.25 health
                healthModAmount = 1.25f;

                // +1 Life
                headWearAttributes.lives += 1;
                break;

            case ("Beret"):          //DONE                     
                // +1 Health
                healthModAmount = 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            //==================================================================================== LE Items Below

            case ("Angel Halo Aqua"):         //DONE                                                                          
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 life
                headWearAttributes.lives += 1;

                // +1 luck
                headWearAttributes.luck += 1;

                // +.5 projectile count
                headWearAttributes.maxAmmo += .5f;
                break;

            case ("Angel Halo"):      //DONE                                                      
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 life
                headWearAttributes.lives += 1;

                // +1 luck
                headWearAttributes.luck += 1;

                // +.5 projectile count
                headWearAttributes.maxAmmo += .5f;
                break;

            case ("Armor Helmet"):          //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Baseball Cap"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Bowler Hat"):            //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Crown"):          //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 life
                headWearAttributes.lives += 1;

                // +1 luck
                headWearAttributes.luck += 1;
                break;

            case ("Detective Hat"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Brown Detective Hat"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Dino Hat"):         //DONE
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .00125f;

                // +1 luck
                headWearAttributes.luck += 1;
                break;

            case ("Diving Goggles"):            //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Dragon Horn"):              //DONE  
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .00125f;

                // +1 luck
                headWearAttributes.luck += 1;
                break;

            case ("Headband Black"):            //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Headband Blue"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Headband Red"):          //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Hoodie Hat"):        //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Hoodie With Earphone"):      //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Horn"):          //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Darker Horn"):           //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Leather Aviator Cap"):           //DONE    
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .00125f;

                // +1 luck
                headWearAttributes.luck += 1;
                break;

            case ("Nightcap"):          //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // enemies are slowed after taking damage
                headWearAttributes.slowOnAttack = true;
                break;

            case ("Racing Helmet"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // enemies are slowed after taking damage
                headWearAttributes.slowOnAttack = true;
                break;

            case ("Rain Coat Hat"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // enemies are slowed after taking damage
                headWearAttributes.slowOnAttack = true;
                break;

            case ("Skull Mask"):            //DONE
                // fire twice (beams of light, one from each eye) when projectiles are empty
                headWearAttributes.eyeLasers = true;

                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Space Hat"):            //DONE  
                // fire twice (beams of light, one from each eye) when projectiles are empty
                headWearAttributes.eyeLasers = true;

                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Straw Hat"):             //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Gold Piercing"):       //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("Deco Piercing"):                 //DONE 
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;
                break;

            case ("VR The Eye"):            //DONE
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount = 0.00125f;

                // +0.25 projectile count
                headWearAttributes.maxAmmo += .25f;

                // fire twice (beams of light, one from each eye) when projectiles are empty
                headWearAttributes.eyeLasers = true;
                break;

            case ("VR"):         //DONE
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount = 0.00125f;

                // +0.25 projectile count
                headWearAttributes.maxAmmo += .25f;

                // fire twice (beams of light, one from each eye) when projectiles are empty
                headWearAttributes.eyeLasers = true;
                break;

            case ("Baby Hat"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;

            case ("Star Hood"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;

            case ("Cbass Head"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;

            case ("Farmer Hat"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;

            case ("Devil Horns"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;

            case ("Dunzzo Head"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;

            case ("Luca Helmet"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;

            case ("Christmas Hat"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;

            case ("DJ Magic Horn"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;

            case ("Hologram Head"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;

            case ("Maniqueen Tiara"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;

            case ("Mecha Fast Head"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;

            case ("Tough Salor Hat"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;

            case ("Lion Dragon Crown"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;

            case ("Glowing Gamer Ears"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;

            case ("Magical Star and Moon"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;

            case ("Spacesuit Hat MARK II"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                headWearAttributes.healthRegen = true;
                headWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                headWearAttributes.maxAmmo += 1;

                // +1 Life
                headWearAttributes.lives += 1;

                // Character can fly
                headWearAttributes.airJumps += 50;
                break;


            default:
                healthModAmount = 0;
                meleeModAmount = 0;
                projectileCountModAmount = 0;
                break;
        }
        headWearAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
        meleeStats.damage += MeleeDamageModifier(meleeModAmount);
        //headWearAttributes.addedProjectiles += projectileCountModAmount;
        //headWearAttributes.maxAmmo += MaxAmmoModifier(headWearAttributes.maxAmmo, defaultStats.maxAmmo);

        return headWearAttributes;
    }
    Attributes BodyWearAttributes(Attributes bodyWearAttributes, float healthModAmount, float meleeModAmount, float projectileCountModAmount, int index)
    {
        if (nftAttributes[index].bodywear == null)
            return bodyWearAttributes;
        switch (nftAttributes[index].bodywear.value)
        {
            case ("T-Shirt"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Hoodie"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Tank Top"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Tuxedo"):
                // +0.50 health
                healthModAmount = 0.5f;
                break;

            case ("Leather Armor"):
                // +0.50 health
                healthModAmount = 0.5f;
                break;

            case ("Plate Armor"):
                // +0.75 health  
                healthModAmount = 0.75f;
                break;

            case ("Gold Armor"):           //DONE                             
                // +0.75 health
                healthModAmount = 0.75f;

                // 1 bubble shield  level
                bodyWearAttributes.bubbleShield = true;
                break;

            case ("Wrestling Singlet"):     //DONE
                // +1 Health
                healthModAmount = 1;

                // Fast Movement
                bodyWearAttributes.moveMagnitude = moveSpeed;
                break;

            case ("Jet Pack"):      //DONE
                // +1 Health
                healthModAmount = 1;

                // Double Jump
                bodyWearAttributes.airJumps += 1;
                break;

            case ("Angel Wings"):       //DONE
                // +1 Health
                healthModAmount = 1;

                // Double Jump
                bodyWearAttributes.airJumps += 1;
                break;

            case ("Thorn Armor"):           //DONE                               
                // +0.75 Health
                healthModAmount = 0.75f;

                // return .25 ticks of damage when hit
                bodyWearAttributes.thorns = true;
                break;

            case ("Ice Armor"):               //DONE      
                // +0.75 health
                healthModAmount = 0.75f;

                // enemies are slowed after damaging player
                bodyWearAttributes.slowBeingAttacked = true;
                break;

            case ("Mirror Armor"):             //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                // reflects 1 projectile every 10 seconds
                bodyWearAttributes.reflect = true;
                break;

            case ("Bandolier"):         //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                // +1 projectile regen
                bodyWearAttributes.ammoRechargeAmount += ammoRechargeSpeed;

                break;

            case ("Robo Armor"):                   //DONE                 
                // +0.75 health
                healthModAmount = 0.75f;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyWearAttributes.chestLaser = true;
                break;

            case ("Santa Suit"):      //DONE                            
                // +1 health
                healthModAmount = 1;

                // +1 projectile
                bodyWearAttributes.maxAmmo += 1;

                // projectiles are snowballs
                bodyWearAttributes.snowBallProjectiles = true;

                break;

            case ("Furry Armor"):           //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                // double jump
                bodyWearAttributes.airJumps += 1;
                break;

            case ("Ninja Armor"):          //DONE                         
                // +1 health
                healthModAmount = 1;

                // can become invisible (enemies can't track)
                bodyWearAttributes.invisible = true;

                break;

            case ("Diamond Armor"):         //DONE
                // +1.25 health
                healthModAmount = 1.25f;

                // +1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Electric/Lighting Armor"):           //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                // fast movement
                bodyWearAttributes.moveMagnitude = moveSpeed;
                break;

            //=================================================================================== LE Items below
            case ("Baseball Jacket"):           //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Black Sleeve Shirt"):            //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Brown Shirt"):           //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Coconut Top"):           //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Gradient Shirt"):            //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Jacket"):            //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Lattice Shirt Red"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Lattice Shirt White"):           //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Meme Tshirt"):     //DONE                                                
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount = 0.00125f;

                // +0.25 projectile count
                bodyWearAttributes.maxAmmo += .25f;

                // 1 bubble shield  level
                bodyWearAttributes.bubbleShield = true;
                break;

            case ("Off Shoulder Top"):          //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Pajamas Shirt"):            //DONE                                        
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount = 0.00125f;

                // +0.25 projectile count
                bodyWearAttributes.maxAmmo += .25f;

                // 1 bubble shield  level
                bodyWearAttributes.bubbleShield = true;
                break;

            case ("Polo Tshirt"):           //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Purple Sleeve Shirt"):           //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Sleeveless Shirt Orange"):           //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Sleeveless Shirt White"):            //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Revenants Shirt White"):            //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Revenants Shirt Colored"):            //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Stripe Shirt Blue"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Stripe Shirt Green"):            //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Suit Coat"):                                            
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount = 0.00125f;

                // +0.25 projectile count
                bodyWearAttributes.maxAmmo += .25f;

                // 1 bubble shield  level
                bodyWearAttributes.bubbleShield = true;
                break;

            case ("Swim Suit Shirt"):           //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("White Shirt Sleeve"):            //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("White Shirt"):           //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("White Sleeve Shirt"):            //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("White T Shirt"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;
                break;

            case ("Cbass Body"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Dunzzo Tee"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Hodl Blouse"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Baby Clothes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Carter Shirt"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Farmer Shirt"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Fastbot Body"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Devil's Shirt"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Hologram Body"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Luca Spacesuit"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Peak Martial Gi"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Heavenly Clothes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Artist Turtleneck"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Spacesuit MARK II"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("DJ Magic Rugged Tee"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Maniqueen Dress Top"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Star's Inner Clothes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("Tough Sailor Clothes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            case ("T$ Christmas Hoodie"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen
                bodyWearAttributes.healthRegen = true;
                bodyWearAttributes.healthRegenAmount += .005f;

                // +1 projectile count
                bodyWearAttributes.maxAmmo += 1;

                // + 1 life
                bodyWearAttributes.lives += 1;
                break;

            default:
                healthModAmount = 0;
                meleeModAmount = 0;
                projectileCountModAmount = 0;

                break;
        }
        bodyWearAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
        meleeStats.damage += MeleeDamageModifier(meleeModAmount);
        //bodyWearAttributes.addedProjectiles += projectileCountModAmount;
       // bodyWearAttributes.maxAmmo += MaxAmmoModifier(bodyWearAttributes.maxAmmo, defaultStats.maxAmmo);
        return bodyWearAttributes;
    }
    //Attributes EyewearAttributes(Attributes eyewearAttributes, float healthModAmount, float meleeModAmount, float projectileCountModAmount, int index)
    //{
    //    if (nftAttributes[index].eyewear == null)
    //        return eyewearAttributes;
    //    switch (nftAttributes[index].eyewear.value)
    //    {
    //        case ("Red Sunglasses"):
    //            // +0.25 health
    //            healthModAmount = 0.25f;
    //            break;

    //        case ("Blue Sunglasses"):
    //            // +0.25 health
    //            healthModAmount = 0.25f;
    //            break;

    //        case ("Pink Sunglasses"):
    //            // +0.25 health
    //            healthModAmount = 0.25f;
    //            break;

    //        case ("Green Sunglasses"):
    //            // +0.5 health
    //            healthModAmount = .5f;
    //            break;

    //        case ("Orange Sunglasses"):
    //            // +0.5 health
    //            healthModAmount = .5f;
    //            break;

    //        case ("White Sunglasses"):
    //            // +0.75 health
    //            healthModAmount = .75f;
    //            break;

    //        case ("Lab Goggles"):                 //DONE          
    //            // +0.75 health
    //            healthModAmount = .75f;

    //            // 1 Bubble Shield/level
    //            eyewearAttributes.bubbleShield = true;
    //            break;

    //        case ("Visor Shades"):          //DONE
    //            // +1 health
    //            healthModAmount = 1;

    //            // Fast Movement
    //            eyewearAttributes.moveMagnitude = moveSpeed;
    //            break;

    //        case ("Flight Goggles"):        //DONE
    //            // +1 Health
    //            healthModAmount = 1;

    //            // Double Jump
    //            eyewearAttributes.airJumps = 1;
    //            break;

    //        case ("Monocle"):           //DONE
    //            // +1.25 health
    //            healthModAmount = 1.25f;

    //            // Fast Movement
    //            eyewearAttributes.moveMagnitude = moveSpeed;

    //            // +1 Projectile
    //            eyewearAttributes.maxAmmo += 1;

    //            // + 0.01 Ammo Regen
    //            eyewearAttributes.ammoRechargeAmount += ammoRechargeSpeed;

    //            // + 1 life
    //            eyewearAttributes.maxHP += 1;
    //            break;

    //        case ("Shutter Shades (Kanye West)"):               //DONE            
    //            // +0.75 health
    //            healthModAmount = 0.75f;

    //            // return .25 ticks of damage when hit
    //            eyewearAttributes.thorns = true;
    //            break;

    //        case ("Scuba Mask"):             //DONE                           
    //            // +0.75f health
    //            healthModAmount = 0.75f;

    //            //enemies are slowed after damaging player
    //            eyewearAttributes.slowBeingAttacked = true;
    //            break;

    //        case ("Yellow Tinted Glasses"):       //DONE                  
    //            // +0.75f health 
    //            healthModAmount = 0.75f;

    //            // double jump
    //            eyewearAttributes.airJumps = 1;

    //            // glide
    //            eyewearAttributes.airMoveMagnitudeCap = 5;
    //            break;

    //        case ("Chinese Circle Glasses"):                            // @TODO ADD Double Dash
    //            // +0.75f health
    //            healthModAmount = 0.75f;

    //            // double dash
    //            break;

    //        case ("Steampunk Glasses"):              //DONE                       
    //            //+0.75f health
    //            healthModAmount = 0.75f;

    //            // Fire Twice (beams of light, one from each eye) when projectile are empty)
    //            eyewearAttributes.eyeLasers = true;
    //            break;

    //        case ("Aviators"):            //DONE                            
    //            // +1f health
    //            healthModAmount = 1;

    //            // +1 extra ammo
    //            eyewearAttributes.maxAmmo += 1;

    //            // Projectiles are snowballs
    //            eyewearAttributes.snowBallProjectiles = true;
    //            break;

    //        case ("Heart Shaped Glasses"):      //DONE                                      
    //            // +.75f health
    //            healthModAmount = 0.75f;

    //            // +1 health regen;
    //            eyewearAttributes.healthRegen = true;
    //            eyewearAttributes.healthRegenAmount = 0.1f;
    //            break;

    //        case ("Pirate Eye Patch"):          //DONE
    //            //+1 Health
    //            healthModAmount = 1;

    //            //Can become invisible (enemies can't track)
    //            eyewearAttributes.invisible = true;

    //            break;

    //        case ("Star Shaped Glasses"):       //DONE
    //            // +1.25 health
    //            healthModAmount = 1.25f;

    //            // +1 life
    //            eyewearAttributes.maxHP += 1;
    //            break;

    //        case ("Solana Colored Balaclava"):          //DONE
    //            //+0.75 health 
    //            healthModAmount = 0.75f;

    //            //Fast Movement
    //            eyewearAttributes.moveMagnitude = moveSpeed;
    //            break;

    //        default:
    //            healthModAmount = 0;
    //            meleeModAmount = 0;
    //            projectileCountModAmount = 0;
    //            break;
    //    }

    //    eyewearAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
    //    meleeStats.damage += MeleeDamageModifier(meleeModAmount);
    //    eyewearAttributes.addedProjectiles += projectileCountModAmount;

    //    return eyewearAttributes;
    //}
    //Attributes MouthAccessoryAttributes(Attributes mouthAccessoryAttributes, float healthModAmount, float meleeModAmount, float projectileCountModAmount, int index)
    //{
    //    if (nftAttributes[index].faceAccessory == null)
    //        return mouthAccessoryAttributes;
    //    switch (nftAttributes[index].faceAccessory.value)
    //    {
    //        case ("Cigarette"):                         
    //            //+0.75 health
    //            healthModAmount = 0.75f;

    //            //Bubble Shield/ level
    //            mouthAccessoryAttributes.bubbleShield = true;
    //            break;

    //        case ("Vape Pen"):
    //            //+1 health
    //            healthModAmount = 1;

    //            //Fast Movement
    //            mouthAccessoryAttributes.moveMagnitude = moveSpeed;
    //            break;

    //        case ("Toothpick"):
    //            //+1 health
    //            healthModAmount = 1;

    //            //Double Jump
    //            mouthAccessoryAttributes.airJumps = 1;
    //            break;

    //        case ("Scuba Regulator"):
    //            // +1.25 health
    //            healthModAmount = 1.25f;

    //            // Fast Movement
    //            mouthAccessoryAttributes.moveMagnitude = moveSpeed;

    //            // +1 Ammo
    //            mouthAccessoryAttributes.maxAmmo += 1;

    //            // +0.1 ammo Regen
    //            mouthAccessoryAttributes.ammoRechargeAmount += ammoRechargeSpeed;

    //            // +1 life
    //            mouthAccessoryAttributes.lives = 1;
    //            break;

    //        case ("Red Pacifier"):                                     
    //            // +0.75 health
    //            healthModAmount = 0.75f;

    //            // return .25ticks of damage when hit;
    //            mouthAccessoryAttributes.thorns = true;
    //            break;

    //        case ("Blue Pacifier"):                                    
    //            // +0.75 health
    //            healthModAmount = 0.75f;

    //            // Enemies are slowed after damaging player
    //            mouthAccessoryAttributes.slowBeingAttacked = true;
    //            break;

    //        case ("Pink Pacifier"):                                   
    //            // +0.75 health
    //            healthModAmount = 0.75f;

    //            // double jump
    //            mouthAccessoryAttributes.airJumps = 1;

    //            // glide
    //            mouthAccessoryAttributes.airMoveMagnitudeCap = 5;
    //            break;

    //        case ("Green Pacifier"):                                    
    //            // +0.75 health
    //            healthModAmount = 0.75f;

    //            // shoot mouth laser when projectiles are empty
    //            mouthAccessoryAttributes.mouthLaser = true;
    //            break;

    //        case ("Orange Pacifier"):                                 
    //            // +0.75 health
    //            healthModAmount = 0.75f;

    //            // fire twice (beams of light, one from each eye) when projectiles are empty
    //            mouthAccessoryAttributes.eyeLasers = true;
    //            break;

    //        case ("White Pacifier"):                                    
    //            // +1 health
    //            healthModAmount = 1;

    //            // +1 ammo
    //            mouthAccessoryAttributes.maxAmmo += 1;

    //            // projectiles are snowballs
    //            mouthAccessoryAttributes.snowBallProjectiles = true;
    //            break;
    //        default:
    //            healthModAmount = 0;
    //            meleeModAmount = 0;
    //            projectileCountModAmount = 0;
    //            break;
    //    }

    //    mouthAccessoryAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
    //    meleeStats.damage += MeleeDamageModifier(meleeModAmount);
    //    mouthAccessoryAttributes.addedProjectiles += projectileCountModAmount;

    //    return mouthAccessoryAttributes;
    //}
    //Attributes HandAccessoryAttributes(Attributes handAccessoryAttributes, float healthModAmount, float meleeModAmount, float projectileCountModAmount, int index)
    //{
    //    if (nftAttributes[index].handAccessory == null)
    //        return handAccessoryAttributes;

    //    switch (nftAttributes[index].handAccessory.value)
    //    {
    //        case ("Red  Mittens"):
    //            // +0.75 health
    //            healthModAmount = 0.75f;

    //            // Double Jump
    //            handAccessoryAttributes.airJumps = 1;
    //            break;

    //        case ("Blue Mittens"):                                    
    //            // +1 health
    //            healthModAmount = 1;

    //            // Can Become Invisible (enemies can't track)
    //            handAccessoryAttributes.invisible = true;
    //            break;

    //        case ("Pink Mittens"):
    //            // +1.25 health
    //            healthModAmount = 1.25f;

    //            // +1 life
    //            handAccessoryAttributes.lives += 1;
    //            break;

    //        case ("Green Mittens"):
    //            // +0.75 health
    //            healthModAmount = 0.75f;

    //            // Fast Movement
    //            handAccessoryAttributes.moveMagnitude = moveSpeed;
    //            break;

    //        case ("Orange Mittens"):                                   
    //            // +0.75 health
    //            healthModAmount = 0.75f;

    //            // Fire Twice (beams of light, one from each eye) when projectiles are empty
    //            handAccessoryAttributes.eyeLasers = true;
    //            break;

    //        case ("White Mittens"):                                     
    //            // +1 health
    //            healthModAmount = 1;

    //            // +1 ammo
    //            handAccessoryAttributes.maxAmmo += 1;

    //            // Projectiles are snowballs
    //            handAccessoryAttributes.snowBallProjectiles = true;
    //            break;

    //        case ("Pink Boxing Gloves"):
    //            // +0.75 health
    //            healthModAmount = 0.75f;

    //            // Double Jump
    //            handAccessoryAttributes.airJumps = 1;
    //            break;

    //        case ("Green Boxing Gloves"):                              
    //            // +1 health
    //            healthModAmount = 1;

    //            // Can become invisible (enemies can't track)
    //            handAccessoryAttributes.invisible = true;
    //            break;

    //        case ("Orange Boxing Gloves"):
    //            // +1.25 health
    //            healthModAmount = 1.25f;

    //            // +1 life
    //            handAccessoryAttributes.lives += 1;
    //            break;

    //        case ("White Boxing Gloves"):
    //            // +0.75 health
    //            healthModAmount = 0.75f;

    //            // Fast Movement
    //            handAccessoryAttributes.moveMagnitude = moveSpeed;
    //            break;
    //        default:
    //            healthModAmount = 0;
    //            meleeModAmount = 0;
    //            projectileCountModAmount = 0;
    //            break;
    //    }

    //    handAccessoryAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
    //    meleeStats.damage += MeleeDamageModifier(meleeModAmount);
    //    handAccessoryAttributes.addedProjectiles += projectileCountModAmount;

    //    return handAccessoryAttributes;
    //}
    Attributes PantsAttributes(Attributes pantsAttributes, float healthModAmount, float meleeModAmount, float projectileCountModAmount, int index)
    {
        if (nftAttributes[index].pants == null)
            return pantsAttributes;
        switch (nftAttributes[index].pants.value)
        {
            //case ("Red Pants"):
                // +.25 health
                //healthModAmount = 0.25f;
               // break;

           // case ("Blue Pants"):
                // +.25 health
                //healthModAmount = 0.25f;
               // break;

            case ("Pink Pants"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Green Pants"):
                // +0.5 health;
                healthModAmount = 0.5f;
                break;

            case ("Orange Pants"):
                // +0.5 health
                healthModAmount = 0.5f;
                break;

            case ("White Pants"):
                // +.75 health
                healthModAmount = 0.75f;
                break;

            case ("Chrome Shorts"):           //DONE                      
                // +.75 health
                healthModAmount = 0.75f;

                // 1 Bubble Shield / level
                pantsAttributes.bubbleShield = true;
                break;

            case ("Red Genie Pants"):           //DONE
                // +1 health
                healthModAmount = 1;

                // Fast Movement
                pantsAttributes.moveMagnitude = moveSpeed;
                break;

            case ("Pink Genie Pants"):          //DONE
                // +1 health
                healthModAmount = 1;

                // Double Jump
                pantsAttributes.airJumps += 1;
                break;

            case ("Kilt"):          //DONE
                // +1.25 health
                healthModAmount = 1.25f;

                // Fast Movement
                pantsAttributes.moveMagnitude = moveSpeed;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +0.01 ammo Regen
                pantsAttributes.ammoRechargeAmount += ammoRechargeSpeed;

                // +1 life
                pantsAttributes.lives += 1;
                break;

            case ("Orange Shorts"):       //DONE                                      
                // +.75 health
                healthModAmount = 0.75f;

                // return .25 ticks of damage when hit
                pantsAttributes.thorns = true;
                break;

            case ("White Shorts"):              //DONE                                       
                // +0.75 health
                healthModAmount = 0.75f;

                // enemies are slowed after damaging player
                pantsAttributes.slowBeingAttacked = true;
                break;

            case ("Hula Skrit"):                  //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                // double jump
                pantsAttributes.airJumps += 1;

                // glide
                pantsAttributes.airMoveMagnitudeCap = 5;
                break;

            case ("Green Shorts"):            //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                // heal 1 tick of health when below 50%
                pantsAttributes.healthRegenToHalf = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Cyborg Legs"):                                             
                // +0.75 health
                healthModAmount = 0.75f;

                // drop two bombs on the ground when projectiles are empty
                pantsAttributes.TwoBomb = true;
                break;

            case ("Santa Pants"):                //DONE                              
                // +1 health
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // projectiles are snowballs
                pantsAttributes.snowBallProjectiles = true;
                break;

            case ("Blue Genie Pants"):          //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                // double jump
                pantsAttributes.airJumps += 1;
                break;

            case ("White Genie Pants"):     //DONE                                  
                // +1 health
                healthModAmount = 1;

                // Can become Invisible (enemies can't track)
                pantsAttributes.invisible = true;
                break;

            case ("Rainbow Genie Pants"):           //DONE
                // +1.25 health
                healthModAmount = 1.25f;

                // +1 life
                pantsAttributes.lives += 1;
                break;

            case ("Green Genie Pants"):         //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                // fast movement
                pantsAttributes.moveMagnitude = moveSpeed;
                break;

            //================================================================================================ LE Items Below
            case ("Armor Pants"):           //DONE
                // +3 health                 
                healthModAmount = 3;

                // +2 ammo
                pantsAttributes.maxAmmo += 2;

                // +2 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = 0.01f;
                break;

            case ("Aubum"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Black Pants with Jacket"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;

                // can become invisible (enemies can't track)
                pantsAttributes.invisible = true;
                break;

            case ("Black"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;

                // can become invisible (enemies can't track)
                pantsAttributes.invisible = true;
                break;

            case ("Blue Jeans"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Blue"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Camo"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;

                // can become invisible (enemies can't track)
                pantsAttributes.invisible = true;
                break;

            case ("Cargo"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Grass"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Grey"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Jeans Blue"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Jeans Brown"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Jeans"):         //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Modern"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("One"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Pajamas"):         //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;

                // can become invisible (enemies can't track)
                pantsAttributes.invisible = true;
                break;

            case ("Patched"):         //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Racing"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Red"):         //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Rickroll"):                 //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 Luck
                pantsAttributes.luck += 1;
                break;

            case ("Short Sports Pants"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Short"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Sport Pants"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Stripe Short Pants"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Suit Pants"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Swimming Pants"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Uniform Pants"):         //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Zipper Pants"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Luca"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Star"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Farmer"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Tough"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Devil's"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("T$ Pants"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Baby Pants"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Cbass Pants"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Dunzzo Pants"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Fastbot Pants"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Designer Pants"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Heavenly Pants"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Magical Girl Skirt"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Peak Martial Pants"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Hologram Transmitter"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Maniqueen Dress Bottom"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            case ("Spacesuit Pants MARK II"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 ammo
                pantsAttributes.maxAmmo += 1;

                // +1 health regen 
                pantsAttributes.healthRegen = true;
                pantsAttributes.healthRegenAmount = .005f;
                break;

            default:
                healthModAmount = 0;
                meleeModAmount = 0;
                projectileCountModAmount = 0;
                break;
        }
        pantsAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
        meleeStats.damage += MeleeDamageModifier(meleeModAmount);
       // pantsAttributes.addedProjectiles += projectileCountModAmount;
       // pantsAttributes.maxAmmo += MaxAmmoModifier(pantsAttributes.maxAmmo, defaultStats.maxAmmo);
        return pantsAttributes;
    }
    Attributes ShoesAttributes(Attributes shoeAttributes, float healthModAmount, float meleeModAmount, float projectileCountModAmount, int index)
    {
        if (nftAttributes[index].shoes == null)
            return shoeAttributes;
        switch (nftAttributes[index].shoes.value)
        {
            case ("Red Tennis Shoes"):
                //  +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Blue Tennis Shoes"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Pink Tennis Shoes"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Green Tennis Shoes"):
                // +0.5 health
                healthModAmount = 0.5f;
                break;

            case ("Orange Tennis Shoes"):
                // +0.5 health
                healthModAmount = 0.5f;
                break;

            case ("White Shoes"):
                // +0.75 health
                healthModAmount = 0.75f;
                break;

            case ("Blue Crocs"):       //DONE                                 
                // +0.75 health
                healthModAmount = 0.75f;

                // 1 bubble shield/ level
                shoeAttributes.bubbleShield = true;
                break;

            case ("Red Roller Skates"):         //DONE
                // +1 health
                healthModAmount = 1;

                // Fast movement
                shoeAttributes.moveMagnitude = moveSpeed;
                break;

            case ("Rainbow Roller Skates"):         //DONE
                // +1 health
                healthModAmount = 1;

                // double jump
                shoeAttributes.airJumps += 1;
                break;

            case ("Rainbow Cowboy Boots"):          //DONE
                // +1.25 health
                healthModAmount = 1.25f;

                // fast movement
                shoeAttributes.moveMagnitude = moveSpeed;

                // +1 ammo
                shoeAttributes.maxAmmo += 1;

                // +0.01 projectile regen
                shoeAttributes.ammoRechargeAmount += ammoRechargeSpeed;

                // +1 life
                shoeAttributes.lives += 1;
                break;

            case ("Red Cowboy Boots"):          //DONE                                          
                // +0.75 health
                healthModAmount = 0.75f;

                // return .25 ticks of damage when hit
                shoeAttributes.thorns = true;
                break;

            case ("Black Cowboy Boots"):            //DONE                                      
                // +0.75 health
                healthModAmount = 0.75f;

                // enemies are slowed after damaging player
                shoeAttributes.slowBeingAttacked = true;
                break;

            case ("Hermes Shoes"):         //DONE                       
                // +0.75 health
                healthModAmount = 0.75f;

                // double jump 
                shoeAttributes.airJumps += 1;

                // glide
                shoeAttributes.airMoveMagnitudeCap = 5;
                break;

            case ("Robot Boots"):                   //DONE                                  
                // +0.75 health
                healthModAmount = 0.75f;

                //  drop bomb when projectiles are empty
                shoeAttributes.oneBomb = true;
                break;

            case ("Rubber Boots"):                      //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                // Dashing causes nearby enemies to take lightning damage
                shoeAttributes.lightningDash = true;
                break;

            case ("Santa Shoes"):          //DONE                                       
                // +1 health
                healthModAmount = 1;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;

                // projectiles are snowballs
                shoeAttributes.snowBallProjectiles = true;
                break;

            case ("Furry Boots"):           //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                // double jump
                shoeAttributes.airJumps += 1;
                break;

            case ("White Cowboy Boots"):                //DONE                                 
                // +1 health
                healthModAmount = 1;

                // can become invisible (enemies can't track)
                shoeAttributes.invisible = true;
                break;

            case ("Diving Flippers"):           //DONE
                // +1.25 health
                healthModAmount = 1.25f;

                // +1 life
                shoeAttributes.lives += 1;
                break;

            case ("Pink Roller Skates"):            //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                // fast movement
                shoeAttributes.moveMagnitude = moveSpeed;
                break;

            //=================================================================================== LE Items Below

            case ("Armor"):           //DONE
                // +3 health                 
                healthModAmount = 3;

                // +2 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = 0.01f;

                // +2 Ammo
                shoeAttributes.maxAmmo += 2;
                break;

            case ("Beach"):         //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Dino"):                  //DONE
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = 0.00125f;

                // +1 Luck
                shoeAttributes.luck += 1;
                break;

            case ("Hiking"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Hiking Shoes Black"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Modern"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Pajamas Shoes"):                          //DONE
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = 0.00125f;

                // +1 Luck
                shoeAttributes.luck += 1;
                break;

            case ("Racing"):                   //DONE
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = 0.00125f;

                // +1 Luck
                shoeAttributes.luck += 1;
                break;

            case ("Rainboots"):         //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Slippers"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Sneaker"):         //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Sneaker Black"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Sneaker Blue"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Sneaker Brown"):             //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Sneaker Green"):         //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Sneaker Purple"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Sports Shoes Orange"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Sports"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Suit"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("White"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Luca"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Dunzzo"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("T$ Shoes"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Hodl Heels"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Star Shoes"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Cbass Shoes"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Farmer Shoes"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Angel's Shoes"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Fastbot Shoes"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Kung Fu Shoes"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Designer Shoes"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Maniqueen Heels"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Transmitter Legs"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Tough Sailor Shoes"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("DJ Magic Rugged Shoes"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            case ("Spacesuit Shoes MARK II"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen
                shoeAttributes.healthRegen = true;
                shoeAttributes.healthRegenAmount = .005f;

                // +1 Ammo
                shoeAttributes.maxAmmo += 1;
                break;

            default:
                healthModAmount = 0;
                meleeModAmount = 0;
                projectileCountModAmount = 0;
                break;
        }
        shoeAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
        meleeStats.damage += MeleeDamageModifier(meleeModAmount);
        shoeAttributes.addedProjectiles += projectileCountModAmount;
       // shoeAttributes.maxAmmo += MaxAmmoModifier(shoeAttributes.maxAmmo, defaultStats.maxAmmo);
        return shoeAttributes;
    }
    //Attributes MeleeWeaponAttributes(Attributes meleeWeaponAttributes, float healthModAmount, float meleeModAmount, float projectileCountModAmount, int index)
    //{
    //    if (nftAttributes[index].weapon == null)
    //        return meleeWeaponAttributes;
    //    switch (nftAttributes[index].weapon.value)
    //    {
    //        case ("Iron Sword"):
    //            // +0.25 melee damage
    //            meleeModAmount = 0.25f;
    //            break;

    //        case ("Bronze Sword"):
    //            // +0.25 Melee damage
    //            meleeModAmount = 0.25f;
    //            break;

    //        case ("Bronze Axe"):
    //            // +0.25 melee damage
    //            meleeModAmount = 0.25f;
    //            break;

    //        case ("Iron Scythe"):
    //            // +0.25 melee damage
    //            meleeModAmount = 0.25f;
    //            break;
    //        case ("Bronze Scythe"):
    //            // +0.25 melee damage
    //            meleeModAmount = 0.25f;
    //            break;

    //        case ("Iron Axe"):
    //            // +0.25 melee damage
    //            meleeModAmount = 0.25f;
    //            break;

    //        case ("Gold Sword"):                                                //@TODO Add 3 ticks of bleeding damage over 3 seconds
    //            // +0.75 melee damage
    //            meleeModAmount = 0.75f;

    //            // Adds 3 ticks of bleeding damage over 3 secounds. 
    //            break;

    //        case ("Diamond Sword"):                                             //@TODO ADD fire damage to melee attacks
    //            // +1 melee damage
    //            meleeModAmount = 1f;

    //            // +1 bubble shield/level
    //            meleeWeaponAttributes.bubbleShield = true;

    //            // adds fire damage to melee attacks(3 ticks of damage over 3 seconds)
    //            break;

    //        case ("Diamond Axe"):                                             
    //            // +1 Melee damage
    //            meleeModAmount = 1;

    //            //Double Melee strike;
    //            meleeWeaponAttributes.doubleMelee = true;
                 
    //            break;

    //        case ("Diamond Scythe"):                                            //@TODO ADD fire damage to melee attacks
    //            // +1.25 Melee damage
    //            meleeModAmount = 1.25f;

    //            // fast movement
    //            meleeWeaponAttributes.moveMagnitude = moveSpeed;

    //            // Adds fire damage to melee attacks (3 ticks of damage over 3 seconds

    //            // +1 health regen
    //            meleeWeaponAttributes.healthRegen = true;
    //            meleeWeaponAttributes.healthRegenAmount = 0.1f;

    //            // +1 life
    //            meleeWeaponAttributes.lives += 1;
    //            break;

    //        case ("BaseBall Bat"):                                              //@TODO ADD 3 ticks of damage over 3 seconds
    //            // +0.75 melee damage
    //            meleeModAmount = 0.75f;

    //            // Add 3 ticks of damage over 3 seconds
    //            break;

    //        case ("Tire Iron"):
    //            // +0.75 melee damage
    //            meleeModAmount = 0.75f;

    //            // enemies are slowed after taking damage
    //            meleeWeaponAttributes.slowOnAttack = true;
    //            break;

    //        case ("Poisoned Scythe"):                                           //@TODO ADD acid damage to melee attacks
    //            // +0.75 melee damage
    //            meleeModAmount = 0.75f;

    //            // Adds acid damage to melee attacks (3 ticks of damage over 3 seconds)
    //            break;

    //        case ("Dagger"):                                                  
    //            // +0.75 melee damage
    //            meleeModAmount = 0.75f;

    //            // throw dagger when projectiles are empty
    //            meleeWeaponAttributes.throwItem = true;
    //            meleeWeaponAttributes.throwItemIndex = 0;
    //            break;

    //        case ("Twin Daggers"):                                             
    //            // +0.75 melee damage 
    //            meleeModAmount = 0.75f;

    //            // Double Melee strike
    //            meleeWeaponAttributes.doubleMelee = true;

    //            break;

    //        case ("Rainbow Sword"):                                             //@TODO ADD holy damage 
    //            // +1 Melee Damage
    //            meleeModAmount = 1;

    //            // Double Melee strike
    //            meleeWeaponAttributes.doubleMelee = true;


    //            // Adds Holy Damage (3 ticks of damage over 3 seconds)
    //            break;

    //        case ("Raipier"):
    //            // +0.75 melee damage
    //            meleeModAmount = 0.75f;

    //            // double jump
    //            meleeWeaponAttributes.airJumps = 1;
    //            break;

    //        case ("Whip"):                                                      //@TODO  ADD holy damage to melee attacks
    //            // +1 melee damage
    //            meleeModAmount = 1;

    //            // Can become invisible(enemies can't track)
    //            meleeWeaponAttributes.invisible = true;

    //            // Adds holy damage to melee attacks (3 ticks of damage over 3 seconds)
    //            break;

    //        case ("Golden Staff"):                                              //@TODO ADD holy damage
    //            // +1.25 melee damage
    //            meleeModAmount = 1.25f;

    //            // +1 extra life
    //            meleeWeaponAttributes.lives += 1;

    //            // Adds holy damage to melee attacks(3 ticks of damage over 3 seconds)
    //            break;

    //        case ("Rainbow Staff"):                                             
    //            // +0.75 melee damage
    //            meleeModAmount = 0.75f;

    //            // fast movement
    //            meleeWeaponAttributes.moveMagnitude = moveSpeed;

    //            // double melee strike
    //            meleeWeaponAttributes.doubleMelee = true;

    //            break;

    //        default:
    //            healthModAmount = 0;
    //            meleeModAmount = 0;
    //            projectileCountModAmount = 0;
    //            break;
    //    }

    //    meleeWeaponAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
    //    meleeStats.damage += MeleeDamageModifier(meleeModAmount);
    //    meleeWeaponAttributes.addedProjectiles += projectileCountModAmount;
    //    return meleeWeaponAttributes;
    //}
    //Attributes RangedWeaponAttributes (Attributes rangedWeaponAttributes, float healthModAmount, float meleeModAmount, float rangedModAmount, float projectileCountModAmount, int index)
    //{
    //    if (nftAttributes[index].rangedWeapon == null)
    //        return rangedWeaponAttributes;
    //    switch (nftAttributes[index].rangedWeapon.value)
    //    {

    //        case ("Squirt Gun"):
    //            // +0.25 projectile Damage
    //            rangedModAmount = 0.25f;
    //            break;

    //        case ("Spray Bottle"):
    //            // +0.25 projectile Damage
    //            rangedModAmount = 0.25f;
    //            break;

    //        case ("Pool Noodle"):
    //            // +0.25 projectile Damage
    //            rangedModAmount = 0.25f;
    //            break;

    //        case ("Rubber hose"):
    //            // +0.25 projectile Damage
    //            rangedModAmount = 0.25f;
    //            break;

    //        case ("Pop Gun"):
    //            // +0.25 projectile Damage
    //            rangedModAmount = 0.25f;
    //            break;

    //        case ("Pistol"):
    //            // +0.25 projectile Damage
    //            rangedModAmount = 0.25f;
    //            break;

    //        case ("Lazer Gun"):
    //            // +0.75 projectile Damage
    //            rangedModAmount = 0.75f;

    //            //+1 projectile recharge
    //            rangedWeaponAttributes.ammoRechargeAmount += ammoRechargeSpeed;
    //            break;

    //        case ("Flamethrower"):                                                              //@TODO ADD fire damage to the projectile
    //            // +1 projectile Damage
    //            rangedModAmount = 1;

    //            //+1 projectile recharge
    //            rangedWeaponAttributes.ammoRechargeAmount += ammoRechargeSpeed;

    //            //adds fire damage to projectile attacks (3 ticks of damage over 3 seconds)
    //            break;

    //        case ("Double Barrelled Lazer Gun"):                                                //@TODO ADD Double Projectile Fire
    //            //+1 Projectile Damage
    //            rangedModAmount = 1;

    //            // Double Projectile Fire
    //            break;

    //        case ("Golden Gun"):
    //            // +1.25 projectile dmg
    //            rangedModAmount = 1.25f;

    //            // fast movement
    //            rangedWeaponAttributes.moveMagnitude = moveSpeed;

    //            // +1 extra projectile
    //            rangedWeaponAttributes.maxAmmo += 1;

    //            // +1 projectile regen
    //            rangedWeaponAttributes.ammoRechargeAmount += ammoRechargeSpeed;

    //            // +1 life
    //            rangedWeaponAttributes.lives += 1;
    //            break;

    //        case ("Shotgun"):                                                               //@TODO adds 3 ticks of damage over 3 seconds
    //            // +0.75 projectile damage
    //            rangedModAmount = 0.75f;

    //            // adds 3 ticks of damage over 3 seconds
    //            break;

    //        case ("Golden Bow and Arrow"):
    //            // +0.75 projectile damage
    //            rangedModAmount = 0.75f;

    //            // enemies are slowed after taking damage;
    //            rangedWeaponAttributes.slowOnAttack = true;
    //            break;

    //        case ("Acid Sprayer"):                                                                  //@TODO ADD acid damage
    //            // +0.75 projectile damage
    //            rangedModAmount = 0.75f;

    //            // adds acid damage to projectile attacks (4 ticks of damage over 4 seconds)
    //            break;

    //        case ("Michale Jackson hat over cannon"):                                               
    //            // +0.75 projectile damage
    //            rangedModAmount = 0.75f;

    //            // Throw Sombrero when projectiles are empty
    //            rangedWeaponAttributes.throwItem = true;
    //            rangedWeaponAttributes.throwItemIndex = 1;
    //            break;

    //        case ("Cyborg Eyes"):                                                                  
    //            // +0.75 projectile damage
    //            rangedModAmount = 0.75f;

    //            // Fire twice (Beams of light one from each eye) when projectiles are empty
    //            rangedWeaponAttributes.eyeLasers = true;
    //            break;

    //        case ("Snow Shovel"):                                                                  
    //            // +1 projectile damage
    //            rangedModAmount = 1;

    //            // +1 extra projectile
    //            rangedWeaponAttributes.maxAmmo += 1;

    //            // projectiles are snowballs
    //            rangedWeaponAttributes.snowBallProjectiles = true;
    //            break;

    //        case ("Bow n Arrow"):
    //            // +0.75 projectile damage
    //            rangedModAmount = 0.75f;

    //            // double jump
    //            rangedWeaponAttributes.airJumps = 1;
    //            break;

    //        case ("Golden Lasso"):                                                             
    //            //+1 projectile damage
    //            rangedModAmount = 1;

    //            // can become invisible (enemies can't track)
    //            rangedWeaponAttributes.invisible = true;
    //            break;

    //        case ("Spell Book"):                                                                    
    //            // +1.25 projectile damage
    //            rangedModAmount = 1.25f;

    //            // +1 life
    //            rangedWeaponAttributes.lives += 1;

    //            // fire 3 projectiles when shooting
    //            rangedWeaponAttributes.triShot = true;
    //            break;

    //        case ("Tommy Gun"):                                                                      
    //            // +0.75f projectile damage
    //            rangedModAmount = .75f;

    //            // fire 3 projectiles when shooting
    //            rangedWeaponAttributes.triShot = true;
    //            break;

    //        default:
    //            healthModAmount = 0;
    //            meleeModAmount = 0;
    //            rangedModAmount = 0;
    //            projectileCountModAmount = 0;
    //            break;
    //    }
    //    rangedWeaponAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
    //    meleeStats.damage += MeleeDamageModifier(meleeModAmount);
    //    projectileStats.damage += ProjectileDamageModifier(rangedModAmount);
    //    rangedWeaponAttributes.addedProjectiles += projectileCountModAmount;
    //    return rangedWeaponAttributes;
    //}
    Attributes BackWearAttributes(Attributes backWearAttributes, float healthModAmount, float meleeModAmount, float projectileCountModAmount, int index)
    {
        if (nftAttributes[index].backwear == null)
            return backWearAttributes;
        switch (nftAttributes[index].backwear.value)
        {
            case ("Red Backbpack"): //<--- Check to see if that is actually the name
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Blue Backpack"):
                //+0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Green Backpack"):
                //+0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Orange Backpack"):
                //+0.5 health
                healthModAmount = 0.5f;
                break;

            case ("Purple Backpack"):
                //+0.5 health
                healthModAmount = 0.5f;
                break;

            case ("White Backpack"):
                //+0.75 health
                healthModAmount = 0.75f;
                break;

            case ("Rainbow Backpack"):          //DONE                                 
                //+0.75 health
                healthModAmount = 0.75f;

                // +1 bubble shield / level
                backWearAttributes.bubbleShield = true;
                break;

            case ("Bat Wings"):         //DONE
                //+1 health
                healthModAmount = 1;

                // fast movement
                backWearAttributes.moveMagnitude = moveSpeed;
                break;

           // case ("Angel Wings"):            //DONE
                // +1 health 
               // healthModAmount = 1;

                // double jump
               // backWearAttributes.airJumps = 1;
               // break;

            case ("Jet Pack"):                  //DONE                                                   
                // +1.25 health
                healthModAmount = 1.25f;

                // fast movement
                backWearAttributes.moveMagnitude = moveSpeed;

                // +1 extra projectile
                backWearAttributes.maxAmmo += 1;

                // +1 life
                backWearAttributes.lives += 1;

                // +1 projectile regen
                backWearAttributes.ammoRechargeAmount = ammoRechargeSpeed;


                break;

            case ("Electric Wings"):            //DONE
                // +0.75f health
                healthModAmount = 0.75f;

                // thorns mechanic
                backWearAttributes.thorns = true;
                break;

            case ("Ice Wings"):         //DONE
                // +0.75 health 
                healthModAmount = 0.75f;

                // enemies are slowed after damaging player
                backWearAttributes.slowBeingAttacked = true;
                break;

            case ("Battery Pack"):                  //DONE                                                     
                // +0.75 health
                healthModAmount = 0.75f;

                //1 shield at the start of every level;
                backWearAttributes.bubbleShield = true;
                break;

            case ("Brown Sack"):                   //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                // projectiles are brown bottles

                GameController.instance.playerData[0].controller.Player.shooter.attackObject[0] = brownBottles;
                break;

            case ("Cyborg Arms with Lazers"):           //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                // Fire twice (Beams of light one from each eye) when projectiles are empty
                   backWearAttributes.eyeLasers = true;
                break;

            case ("Santa Sack"):            //DONE                                                       
                // +1 health
                healthModAmount = 1;

                // +1 extra projectile
                backWearAttributes.maxAmmo += 1;

                // projectiles are snowballs
                backWearAttributes.snowBallProjectiles = true;
                break;


            case ("Octopus Arms"):          //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                //double jump
                backWearAttributes.airJumps += 1;
                break;

            case ("Silver Wings"):              //DONE                   
                // +1 health
                healthModAmount = 1;

                // Can become invisible (enemies can't track)
                backWearAttributes.invisible = true;
                break;

            case ("Golden Wings"):          //DONE
                // +1.25 health
                healthModAmount = 1.25f;

                // +1 life
                backWearAttributes.lives += 1;
                break;

            case ("Rainbow Wings"):         //DONE
                //+0.75 health
                healthModAmount = 0.75f;

                // fast movement
                backWearAttributes.moveMagnitude = moveSpeed;
                break;

            //==============================================================LE Items
            case ("Airplane Wing"):         //DONE                                   
                // +0.5 Health
                healthModAmount = 0.5f;

                // +1 projectileCount
                backWearAttributes.maxAmmo += 1;

                // +0.5 Luck
                backWearAttributes.luck += .5f;

                // Character can fly
                backWearAttributes.airJumps += 50;
                break;
            case ("Bag"):                //DONE
                // +0.25 health
                healthModAmount = 0.25f;

                // +1 projectile
                backWearAttributes.maxAmmo += 1;

                //+.25 Luck
                backWearAttributes.luck += .25f;
                break;

            case ("Balloon"):                  //DONE
                // +0.5 health
                healthModAmount = 0.5f;

                // +1 projectile count
                backWearAttributes.maxAmmo += 1;

                // +1 Luck
                backWearAttributes.luck += 1;

                // Character can double jump
                backWearAttributes.airJumps += 1;
                break;

            case ("Baseball Bat"):
                // +1 health
                healthModAmount = 1;

                // +1 projectile count
                backWearAttributes.maxAmmo += 1;

                // Throw spiked ball when projectiles are empty
                backWearAttributes.throwItem = true;
                backWearAttributes.throwItemSpikeBall = true;
                break;

            case ("Bat Wing"):      //DONE
                // +0.5 health
                healthModAmount = 0.5f;

                // +0.5 projectile count 
                backWearAttributes.maxAmmo += .5f;

                // +0.5 luck
                backWearAttributes.luck += .5f;

                // Character can fly
                backWearAttributes.airJumps += 50;
                break;

            case ("Cloak"):         //DONE                                                          
                // +1 health
                healthModAmount = 1f;

                // Can become invisible (enemies can't track)
                backWearAttributes.invisible = true;
                break;

            case ("Crystal Wing"):                //DONE
                // +1 Health
                healthModAmount = 1;

                // +1 life
                backWearAttributes.lives += 1;

                // +1 projectile count
                backWearAttributes.maxAmmo += 1;

                // +1 Luck
                backWearAttributes.luck += 1;

                // +1 health regen
                backWearAttributes.healthRegen = true;
                backWearAttributes.healthRegenAmount = .005f;

                // Character can fly
                backWearAttributes.airJumps += 50;
                break;

            case ("Dragon Wing"):            //DONE
                // +1 Health
                healthModAmount = 1;

                // +1 life
                backWearAttributes.lives += 1;

                // +1 projectile count
                backWearAttributes.maxAmmo += 1;

                // +1 luck
                backWearAttributes.luck += 1;

                // +1 Health Regen
                backWearAttributes.healthRegen = true;
                backWearAttributes.healthRegenAmount = .005f;

                // character can fly
                backWearAttributes.airJumps += 50;
                break;

            case ("Flag"):         //DONE
                // +1 health
                healthModAmount = 1f;

                // Can become invisible (enemies can't track)
                backWearAttributes.invisible = true;
                break;

            case ("G pen"):         //DONE
                // +1 health
                healthModAmount = 1;

                // +1 projectile count
                backWearAttributes.maxAmmo += 1;

                // +1 projectile regen
                backWearAttributes.ammoRechargeAmount = ammoRechargeSpeed;
                break;

            case ("Gladius"):           //DONE
                // +1 health
                healthModAmount = 1;

                // +1 projectile count
                backWearAttributes.maxAmmo += 1;

                // Throw spiked ball when projectiles are empty
                backWearAttributes.throwItem = true;
                backWearAttributes.throwItemSpikeBall = true;
                break;


            case ("Green Tentacle"):            //DONE                                  
                // +1 health
                healthModAmount = 1;

                // +1 luck
                backWearAttributes.luck += 1;

                // enemies are slowed after taking damage
                backWearAttributes.slowOnAttack = true;
                break;

            case ("Black Wing"):            //DONE
                // +1 Health
                healthModAmount = 1;

                // +1 luck
                backWearAttributes.luck += 1;

                // +1 projectile count
                backWearAttributes.maxAmmo += 1;

                // +1 health regen
                backWearAttributes.healthRegen = true;
                backWearAttributes.healthRegenAmount = .005f;

                // Character can fly
                backWearAttributes.airJumps += 50;

                // +1 life
                backWearAttributes.lives += 1;
                break;

            case ("Ice Cream"):         //DONE                                                   
                // +1 Health
                healthModAmount = 1;

                // +1 luck
                backWearAttributes.luck += 1;

                // enemies are slowed after taking damage
                backWearAttributes.slowOnAttack = true;
                break;

            case ("Leaf"):                 //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 luck
                backWearAttributes.luck += 1;

                // character can double jump
                backWearAttributes.airJumps += 1;
                break;

            case ("Meat"):        //DONE                                               
                // +1 health
                healthModAmount = 1;

                // +1 luck
                backWearAttributes.luck += 1;
                break;

            case ("Red Tentacle"):         //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 luck
                backWearAttributes.luck += 1;

                //enemies take 2 ticks of damage after taking damage
                //bleed = true;
                //statusDamage = 2;
                //statusCount = 1;
                //for (int i = 0; i < GameController.instance.playerData[0].controller.Player.shooter.AttackObjCount; i++)
                //    GameController.instance.playerData[0].controller.Player.shooter.attackObject[i].GetComponent<A_Hitbox>().dealStatus = true;
                break;

            //case ("Skateboard"):            //DONE
                // +1 health
                //healthModAmount = 1;

                // +1 projectile count
                //backWearAttributes.maxAmmo += 1;

                // Throw spiked ball when projectiles are empty
                //backWearAttributes.throwItem = true;
               // backWearAttributes.throwItemIndex = 2;
                //break;


            case ("Skeleton"):          //DONE                                              
                // +1 health
                healthModAmount = 1;

                // +1 projectile count
                backWearAttributes.maxAmmo = 1;

                // +1 luck
                backWearAttributes.luck += 1;

                // +1 health regen
                backWearAttributes.healthRegen = true;
                backWearAttributes.healthRegenAmount = .005f;

                // Character can fly
                backWearAttributes.airJumps += 50;
                break;

            case ("Anchor"):            //DONE
                // +1 health
                healthModAmount = 1;

                // +1 projectile count
                backWearAttributes.maxAmmo = 1;

                // +1 life
                backWearAttributes.lives += 1;

                // +1 health regen
                backWearAttributes.healthRegen = true;
                backWearAttributes.healthRegenAmount = .005f;
                break;

            case ("Boombox"):            //DONE
                // +1 health
                healthModAmount = 1;

                // +1 projectile count
                backWearAttributes.maxAmmo = 1;

                // +1 life
                backWearAttributes.lives += 1;

                // +1 health regen
                backWearAttributes.healthRegen = true;
                backWearAttributes.healthRegenAmount = .005f;
                break;

            case ("Baguette"):            //DONE
                // +1 health
                healthModAmount = 1;

                // +1 projectile count
                backWearAttributes.maxAmmo = 1;

                // +1 life
                backWearAttributes.lives += 1;

                // +1 health regen
                backWearAttributes.healthRegen = true;
                backWearAttributes.healthRegenAmount = .005f;
                break;

            case ("Heart Ally"):            //DONE
                // +1 health
                healthModAmount = 1;

                // +1 projectile count
                backWearAttributes.maxAmmo = 1;

                // +1 life
                backWearAttributes.lives += 1;

                // +1 health regen
                backWearAttributes.healthRegen = true;
                backWearAttributes.healthRegenAmount = .005f;
                break;

            case ("Skateboard"):            //DONE       
                // +1 health
                healthModAmount = 1;

                // +1 projectile count
                backWearAttributes.maxAmmo = 1;

                // +1 life
                backWearAttributes.lives += 1;

                // +1 health regen
                backWearAttributes.healthRegen = true;
                backWearAttributes.healthRegenAmount = .005f;
                break;

            case ("Angel Wings"):            //DONE   
                // +1 health
                healthModAmount = 1;

                // +1 projectile count
                backWearAttributes.maxAmmo = 1;

                // +1 life
                backWearAttributes.lives += 1;

                // +1 health regen
                backWearAttributes.healthRegen = true;
                backWearAttributes.healthRegenAmount = .005f;
                break;

            case ("Devil Wings"):            //DONE
                // +1 health
                healthModAmount = 1;

                // +1 projectile count
                backWearAttributes.maxAmmo = 1;

                // +1 life
                backWearAttributes.lives += 1;

                // +1 health regen
                backWearAttributes.healthRegen = true;
                backWearAttributes.healthRegenAmount = .005f;
                break;

            case ("Farmer Rake"):            //DONE
                // +1 health
                healthModAmount = 1;

                // +1 projectile count
                backWearAttributes.maxAmmo = 1;

                // +1 life
                backWearAttributes.lives += 1;

                // +1 health regen
                backWearAttributes.healthRegen = true;
                backWearAttributes.healthRegenAmount = .005f;
                break;

            case ("Gaming Chair"):            //DONE
                // +1 health
                healthModAmount = 1;

                // +1 projectile count
                backWearAttributes.maxAmmo = 1;

                // +1 life
                backWearAttributes.lives += 1;

                // +1 health regen
                backWearAttributes.healthRegen = true;
                backWearAttributes.healthRegenAmount = .005f;
                break;

            case ("Hacker's Back"):            //DONE
                // +1 health
                healthModAmount = 1;

                // +1 projectile count
                backWearAttributes.maxAmmo = 1;

                // +1 life
                backWearAttributes.lives += 1;

                // +1 health regen
                backWearAttributes.healthRegen = true;
                backWearAttributes.healthRegenAmount = .005f;
                break;

            default:
                healthModAmount = 0;
                meleeModAmount = 0;
                projectileCountModAmount = 0;
                break;
        }
        backWearAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
        meleeStats.damage += MeleeDamageModifier(meleeModAmount);
       // backWearAttributes.addedProjectiles += projectileCountModAmount;
       //backWearAttributes.maxAmmo += MaxAmmoModifier(backWearAttributes.maxAmmo, defaultStats.maxAmmo);
        return backWearAttributes;
    } 
    Attributes TailAttributes(Attributes tailAttributes, float healthModAmount, float meleeModAmount, float projectileCountModAmount, int index) 
    {
        if (nftAttributes[index].tail == null)
            return tailAttributes;
        switch (nftAttributes[index].tail.value)
        {
            case ("Brown Tail"):
                //+0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Red Tail"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Pink Tail"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Orange Tail"):
                // +0.5 health
                healthModAmount = 0.5f;
                break;

            case ("Yellow Tail"):
                // +0.5 health
                healthModAmount = 0.5f;
                break;

            case ("Green Tail"):
                // +0.75 health
                healthModAmount = 0.75f;
                break;
                        
            case ("Armored Tail"):                      //DONE                                      
                // +0.75 health
                healthModAmount = 0.75f;

                // 1 bubble shield/level
                tailAttributes.bubbleShield = true;
                break;

            case ("Fox Tail"):          //DONE
                // +1 health 
                healthModAmount = 1;

                //fast movement
                tailAttributes.moveMagnitude = moveSpeed;
                break;

            case ("Skunk Tail"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // double jump
                tailAttributes.airJumps += 1;
                break;

            case ("Golden Tail"):
                // +1.25 health
                healthModAmount = 1.25f;

                // fast movement
                tailAttributes.moveMagnitude = moveSpeed;

                // +1 extra projectile
                tailAttributes.maxAmmo += 1;

                // +1 projectile regen
                tailAttributes.ammoRechargeAmount = ammoRechargeSpeed;

                // +1 life
                tailAttributes.lives += 1;
                break;

            case ("Flaming Tail"):      //DONE
                // +0.75f health
                healthModAmount = 0.75f;

                // Thorns
                tailAttributes.thorns = true;
                break;

            case ("Ice Tail"):          //DONE
                // +0.75f health
                healthModAmount = .75f;

                // enemies are slowed after damaging player
                tailAttributes.slowBeingAttacked = true;
                break;

            case ("Bird Tail"):             //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                // double jump
                tailAttributes.airJumps += 1;

                //glide
                tailAttributes.airMoveMagnitudeCap = 5;
                break;

            case ("Spiked Ball"):                                               
                // +0.75 health 
                healthModAmount = 0.75f;

                // Throw spiked ball when projectiles are empty
                tailAttributes.throwItem = true;
                tailAttributes.throwItemSpikeBall = true;
                break;

            case ("Cyborg Tail with Lazer Eye"):                //DONE                           
                // +0.75 health
                healthModAmount = 0.75f;

                // fire twice (beams of light, one from each eye) when projectiles are empty
                tailAttributes.eyeLasers = true;
                break;

            case ("Candy Cane Tail"):                   //DONE                              
                // +1 health
                healthModAmount = 1;

                // +1 projectile
                tailAttributes.maxAmmo += 1;

                //projectiles are snowballs
                tailAttributes.snowBallProjectiles = true;
                break;

            case ("Parachute Tail"):        //DONE
                // +0.75 health
                healthModAmount = 0.75f;

                // double jump
                tailAttributes.airJumps += 1;
                break;

            case ("Star Dust"):                 //DONE      
                //+1 health
                healthModAmount = 1;

                // Can become invisible (enemies can't track)
                tailAttributes.invisible = true;
                break;

            case ("Rainbow"):           //DONE
                //+1.25 health
                healthModAmount = 1.25f;

                // +1 life
                tailAttributes.lives += 1;
                break;

            case ("Gun Tail"):          //DONE
                //+0.75 health
                healthModAmount = 0.75f;

                // fast movement
                tailAttributes.moveMagnitude = moveSpeed;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;
               
            //================================================================================ LE Items Below
            case ("Armor"):         //DONE
                //+3 health
                healthModAmount = 3;

                // +2 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = 0.01f;

                // +2 projectile
                tailAttributes.maxAmmo += 2;
                break;

            case ("Bandage"):           //DONE
                //+3 health
                healthModAmount = 3;

                // +2 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = 0.01f;

                // +2 projectile
                tailAttributes.maxAmmo += 2;
                break;

            case ("Bell"):         //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Black Dragon"):                //DONE
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = 0.00125f;

                //+1 luck
                tailAttributes.luck += 1;
                break;

            case ("Book"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                tailAttributes.chestLaser = true;
                break;

            case ("Chain"):         //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Demon"):                                                 //DONE
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 life
                tailAttributes.lives += 1;

                // +1 Luck
                tailAttributes.luck += 1;

                // +0.5 projectile count
                tailAttributes.maxAmmo += .5f;
                break;

            case ("Dino"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Dragon"):                       //DONE
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 life
                tailAttributes.lives += 1;

                // +1 Luck
                tailAttributes.luck += 1;
                    
                // +0.5 projectile count
                tailAttributes.maxAmmo += 1;
                break;

            case ("Fire"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                tailAttributes.chestLaser = true;
                break;

            case ("Joystick"):                   //DONE
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 life
                tailAttributes.lives += 1;

                // +1 Luck
                tailAttributes.luck += 1;

                // +0.5 projectile count
                tailAttributes.maxAmmo += .5f;
                break;

            case ("Meteor Hammer"):         //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Plug"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                tailAttributes.chestLaser = true;
                break;

            case ("Scorpion"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                tailAttributes.chestLaser = true;
                break;

            case ("Shovel"):            //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                tailAttributes.chestLaser = true;
                break;

            case ("Sword"):         //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                tailAttributes.chestLaser = true;
                break;

            case ("Umbrella"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                tailAttributes.chestLaser = true;
                break;

            case ("Watergun"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                tailAttributes.chestLaser = true;

                // + 1 life
                tailAttributes.lives += 1;
                break;

            case ("Hook"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Tube"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Hodl Tail"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Mecha Tail"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("More Corn"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Tail Crown"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Dunzzo Tail"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Fast's Tail"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Grinch Tail"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Angel's Tail"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("DJ Magic Mic"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Devil's Tail"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("MPOFFSHORE TAIL"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Tail with Brush"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Baby Rattle Tail"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Maniqueen Tail Ring"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            case ("Gamestation Controller"):          //DONE
                //+1 health
                healthModAmount = 1;

                // +1 health regen
                tailAttributes.healthRegen = true;
                tailAttributes.healthRegenAmount = .005f;

                // +1 projectile
                tailAttributes.maxAmmo += 1;
                break;

            default:
                healthModAmount = 0;
                meleeModAmount = 0;
                projectileCountModAmount = 0;
                break;
        }
        tailAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
        meleeStats.damage += MeleeDamageModifier(meleeModAmount);
        //tailAttributes.addedProjectiles += projectileCountModAmount;
       // tailAttributes.maxAmmo += MaxAmmoModifier(tailAttributes.maxAmmo, defaultStats.maxAmmo);
        return tailAttributes;
    }              
    Attributes BackgroundAttributes(Attributes backgroundAttributes, float healthModAmount, float meleeModAmount, float projectileCountModAmount, int index)        // <- This has LE items only
    {
        if (nftAttributes[index].background == null)
            return backgroundAttributes;
        switch (nftAttributes[index].background.value)
        {
            case ("Back Alley"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;
                break;

            case ("Basement"):          //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;
                break;

            case ("Beach"):             //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;
                break;

            case ("Concert"):           //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;
                break;

            case ("Desert"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;
                break;

            case ("Dream"):        //DONE                                    
                // +0.25 health 
                healthModAmount = 0.25f;

                // +0.25 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = 0.00125f;
                break;

            case ("Dungeon"):           //DONE                                         
                // +0.25 health 
                healthModAmount = 0.25f;

                // +0.25 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = 0.00125f;
                break;

            case ("Forest"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;
                break;

            case ("Graffitti Wall"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;
                break;

            case ("Lab Hallway"):           //DONE
                // +0.25 health 
                healthModAmount = 0.25f;

                // +0.25 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = 0.00125f;

                // +0.25 projectile count
                backgroundAttributes.maxAmmo += .25f;
                break;
            
            case ("Lab"):           //DONE
                // +0.25 health
                healthModAmount = 0.25f;

                // +0.25 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = 0.00125f;

                // +0.25 projectile count
                backgroundAttributes.maxAmmo += .25f;
                break;

            case ("Library"):           //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;
                break;

            case ("Meadow"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Mysterious room"):           //DONE                                               
                // +0.25 health
                healthModAmount = 0.25f;

                // +0.25 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = 0.00125f;

                // +0.25 projectile count
                backgroundAttributes.maxAmmo += .25f;
                break;

            case ("Racing Track"):          //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;
                break;

            case ("Raining City"):          //DONE
                // +0.25 health
                healthModAmount = 0.25f;

                // +0.25 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = 0.00125f;

                // +0.25 projectile count
                backgroundAttributes.maxAmmo += .25f;
                break;

            case ("Room"):          //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;
                break;

            case ("Sky"):           //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;
                break;

            case ("Sunset"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;
                break;

            case ("Throne"):                //DONE
                // +0.25 health
                healthModAmount = 0.25f;

                // +0.25 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = 0.00125f;

                // +0.25 projectile count
                backgroundAttributes.maxAmmo += .25f;
                break;

            case ("Heck"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            case ("VOID"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            case ("Ocean"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            case ("Space"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            case ("Museum"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            case ("Pagodas"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            case ("Anime Sky"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            case ("Baby Crib"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            case ("North Pole"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            case ("Binary Room"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            case ("Lots a Corn"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            case ("Baguette Land"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            case ("Gamer Hideout"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            case ("Command Center"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            case ("Heaven's Light"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            case ("Mecha Launchpad"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            case ("DJ Magic Rugged Room"):            //DONE
                // +1 health 
                healthModAmount = 1;

                // +1 health regen
                backgroundAttributes.healthRegen = true;
                backgroundAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                backgroundAttributes.maxAmmo += 1;

                // +1 life
                backgroundAttributes.lives += 1;
                break;

            default:
                healthModAmount = 0;
                meleeModAmount = 0;
                projectileCountModAmount = 0;
                break;
        }
        backgroundAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
        meleeStats.damage += MeleeDamageModifier(meleeModAmount);
        //backgroundAttributes.addedProjectiles += projectileCountModAmount;
        //backgroundAttributes.maxAmmo += MaxAmmoModifier(backgroundAttributes.maxAmmo, defaultStats.maxAmmo);
        return backgroundAttributes;
    }   
    Attributes BodyTypeAttributes(Attributes bodyTypeAttributes, float healthModAmount, float meleeModAmount, float projectileCountModAmount, int index)            // <- This has LE items Only
    {
        if (nftAttributes[index].bodyType == null)
            return bodyTypeAttributes;
        switch (nftAttributes[index].bodyType.value)
        {
            case ("Black Body Tattoo"):             //DONE                               
                // +0.25 health
                healthModAmount = 0.25f;

                // +0.25 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = 0.00125f;

                // +0.25 projectile count
                bodyTypeAttributes.maxAmmo += .25f;
                break;

            case ("Black Body"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Burgundy Body"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Green Body"):            //DONE
                // +0.25 health
                healthModAmount = 0.25f;

                // +0.25 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = 0.00125f;

                // +0.25 projectile count
                bodyTypeAttributes.maxAmmo += .25f;
                break;

            case ("Grey Body Army Tattoo"):      //DONE                      
                // +0.25 health
                healthModAmount = 0.25f;

                // +0.25 health regen 
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = 0.00125f;

                // +0.25 projectile count
                bodyTypeAttributes.maxAmmo += .25f;
                break;

            case ("Grey Body Tattoo"):      //DONE                           
                // +0.25 health
                healthModAmount = 0.25f;

                // +0.25 health regen 
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = 0.00125f;

                // +0.25 projectile count
                bodyTypeAttributes.maxAmmo += .25f;

                // Fast Movement
                bodyTypeAttributes.moveMagnitude = moveSpeed;
                break;

            case ("Grey Body"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Light Blue Body"):           //DONE
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;
                break;

            case ("Navy Blue Body"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Orange Body Tattoo"):            //DONE                            
                // +0.25 health
                healthModAmount = 0.25f;

                // +0.25 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = 0.00125f;

                // +0.25 projectile count
                bodyTypeAttributes.maxAmmo += .25f;

                // Fast Movement
                bodyTypeAttributes.moveMagnitude = moveSpeed;
                break;

            case ("Orange Body"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Pink Body"):         //DONE
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;
                break;

            case ("Sky Blue Body"):         //DONE
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;
                break;

            case ("Yellow Body"):           //DONE
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;
                break;

            case ("Alien Body"):            //DONE                           
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // return .25 ticks of damage when hit
                bodyTypeAttributes.thorns = true;

                //Deal .25 dmg on valid hit on enemy
                //bleed = true;
                //statusDamage = 1;
                //statusCount = 1;
                //for (int i = 0; i < GameController.instance.playerData[0].controller.Player.shooter.AttackObjCount; i++)
                //    GameController.instance.playerData[0].controller.Player.shooter.attackObject[i].GetComponent<A_Hitbox>().dealStatus = true;
                break;

            case ("Gold Body"):        //DONE                           
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                //+1 luck
                bodyTypeAttributes.luck += 1;
                break;

            case ("Cbass"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("Jking"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("Dunzzo"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("Capnjah"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("T$ Body"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("CapNJack"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("Wes Body"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("Fast Body"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("Luca Body"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("Maniqueen"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("Star Body"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("Angel Body"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("Devil Body"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("MPOFFSHORE"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("Sailor Hodl"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("DJ Magic Rugged"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("Skelly Baby Body"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("Skelly Carter Body"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            case ("Skelly Daniel Body"):        //DONE 
                // +1 health
                healthModAmount = 1;

                // +1 health regen
                bodyTypeAttributes.healthRegen = true;
                bodyTypeAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                bodyTypeAttributes.maxAmmo += 1;

                // + 1 life
                bodyTypeAttributes.lives += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                bodyTypeAttributes.chestLaser = true;
                break;

            default:
                healthModAmount = 0;
                meleeModAmount = 0;
                projectileCountModAmount = 0;
                break;
        }
        bodyTypeAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
        meleeStats.damage += MeleeDamageModifier(meleeModAmount);
        //bodyTypeAttributes.addedProjectiles += projectileCountModAmount;
        //bodyTypeAttributes.maxAmmo += MaxAmmoModifier(bodyTypeAttributes.maxAmmo, defaultStats.maxAmmo);
        return bodyTypeAttributes;
    }       
    Attributes EyesAttributes(Attributes eyesAttributes, float healthModAmount, float meleeModAmount, float projectileCountModAmount, int index)                    //<- This has LE Items only
    {
        if (nftAttributes[index].eyes == null)
            return eyesAttributes;
        switch (nftAttributes[index].eyes.value)
        {
            case ("Black"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;
                break;

            case ("Sclera Aqua"):       //DONE
                // +0.25 health                 
                healthModAmount = 0.25f;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;
                break;

            case ("Sclera Black"):          //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;
                break;

            case ("Sclera Orange"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;
                break;

            case ("T$ Eyes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            case ("Star Eye"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            case ("Wes Eyes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            case ("Baby Eyes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            case ("Luca Eyes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            case ("Cbass Eyes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            case ("Jking Eyes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            case ("Daniel Eyes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            case ("Dunzzo Eyes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            case ("Kawaii Eyes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            case ("Angel's Eyes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            case ("Devil's Eyes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            case ("Hologram Eyes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            case ("Maniqueen Eyes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            case ("MPOFFSHORE EYES"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            case ("CapNJack Tough Eyes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            case ("DJ Magic Rugged Eyes"):         //DONE
                // +1 health                 
                healthModAmount = 1f;

                // +1 health regen;
                eyesAttributes.healthRegen = true;
                eyesAttributes.healthRegenAmount = .005f;

                // +1 Projectile
                eyesAttributes.maxAmmo += 1;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                eyesAttributes.eyeLasers = true;

                // + 1 life
                eyesAttributes.lives += 1;
                break;

            default:
                healthModAmount = 0;
                meleeModAmount = 0;
                projectileCountModAmount = 0;
                break;
        }
        eyesAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
        meleeStats.damage += MeleeDamageModifier(meleeModAmount);
        //eyesAttributes.addedProjectiles += projectileCountModAmount;
        //eyesAttributes.maxAmmo += MaxAmmoModifier(eyesAttributes.maxAmmo, defaultStats.maxAmmo);
        return eyesAttributes;
    }              
    Attributes FaceAccessoryAttributes(Attributes faceAccessoryAttributes, float healthModAmount, float meleeModAmount, float projectileCountModAmount, int index)  //<- This has LE items only
    {
        if (nftAttributes[index].faceAccessory == null)
            return faceAccessoryAttributes;
        switch (nftAttributes[index].faceAccessory.value)
        {
            //Eyewears
            case ("Red Sunglasses"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Blue Sunglasses"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Pink Sunglasses"):
                // +0.25 health
                healthModAmount = 0.25f;
                break;

            case ("Green Sunglasses"):
                // +0.5 health
                healthModAmount = .5f;
                break;

            case ("Orange Sunglasses"):
                // +0.5 health
                healthModAmount = .5f;
                break;

            case ("White Sunglasses"):
                // +0.75 health
                healthModAmount = .75f;
                break;

            case ("Lab Goggles"):                 //DONE          
                // +0.75 health
                healthModAmount = .75f;

                // 1 Bubble Shield/level
                faceAccessoryAttributes.bubbleShield = true;
                break;

            case ("Visor Shades"):          //DONE
                // +1 health
                healthModAmount = 1;

                // Fast Movement
                faceAccessoryAttributes.moveMagnitude = moveSpeed;
                break;

            case ("Flight Goggles"):        //DONE
                // +1 Health
                healthModAmount = 1;

                // Double Jump
                faceAccessoryAttributes.airJumps += 1;
                break;

            case ("Monocle"):           //DONE
                // +1.25 health
                healthModAmount = 1.25f;

                // Fast Movement
                faceAccessoryAttributes.moveMagnitude = moveSpeed;

                // +1 Projectile
                faceAccessoryAttributes.maxAmmo += 1;

                // + 0.01 Ammo Regen
                faceAccessoryAttributes.ammoRechargeAmount += ammoRechargeSpeed;

                // + 1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("Shutter Shades (Kanye West)"):               //DONE            
                // +0.75 health
                healthModAmount = 0.75f;

                // return .25 ticks of damage when hit
                faceAccessoryAttributes.thorns = true;
                break;

            case ("Scuba Mask"):             //DONE                           
                // +0.75f health
                healthModAmount = 0.75f;

                //enemies are slowed after damaging player
                faceAccessoryAttributes.slowBeingAttacked = true;
                break;

            case ("Yellow Tinted Glasses"):       //DONE                  
                // +0.75f health 
                healthModAmount = 0.75f;

                // double jump
                faceAccessoryAttributes.airJumps += 1;

                // glide
                faceAccessoryAttributes.airMoveMagnitudeCap = 5;
                break;

            case ("Chinese Circle Glasses"):        //DONE
                // +0.75f health
                healthModAmount = 0.75f;

                // double dash
                faceAccessoryAttributes.doubleDash = true;
                break;

            case ("Steampunk Glasses"):              //DONE                       
                //+0.75f health
                healthModAmount = 0.75f;

                // Fire Twice (beams of light, one from each eye) when projectile are empty)
                faceAccessoryAttributes.eyeLasers = true;
                break;

            case ("Aviators"):            //DONE                            
                // +1f health
                healthModAmount = 1;

                // +1 extra ammo
                faceAccessoryAttributes.maxAmmo += 1;

                // Projectiles are snowballs
                faceAccessoryAttributes.snowBallProjectiles = true;
                break;

            case ("Heart Shaped Glasses"):      //DONE                                      
                // +.75f health
                healthModAmount = 0.75f;

                // +1 health regen;
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;
                break;

            case ("Pirate Eye Patch"):          //DONE
                //+1 Health
                healthModAmount = 1;

                //Can become invisible (enemies can't track)
                faceAccessoryAttributes.invisible = true;

                //+1 luck
                faceAccessoryAttributes.luck += 1;

                break;

            case ("Star Shaped Glasses"):       //DONE
                // +1.25 health
                healthModAmount = 1.25f;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("Solana Colored Balaclava"):          //DONE
                //+0.75 health 
                healthModAmount = 0.75f;

                //Fast Movement
                faceAccessoryAttributes.moveMagnitude = moveSpeed;
                break;

            case ("3D Spectacles"):         //DONE                              
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = 0.00125f;

                // +0.25 projectile count
                faceAccessoryAttributes.maxAmmo += .25f;
                break;

            case ("BubbleGum"):              //DONE                          
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = 0.0125f;

                // +0.25 projectile count
                faceAccessoryAttributes.maxAmmo += .25f;
                break;

            case ("Cigarettes"):            //DONE                        
                // +0.25 projectile regen
                faceAccessoryAttributes.ammoRechargeAmount = ammoRechargeSpeed;

                // +0.25 projectile count
                faceAccessoryAttributes.maxAmmo += .25f;
                break;

            case ("Collar"):
                // +0.25 health                 
                healthModAmount = 0.25f;
                break;

            case ("Donut"):             //DONE                                      
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = 0.00125f;
                break;

            case ("Gas Mask"):
                // +0.25 health                 
                healthModAmount = 0.25f;
                break;

            case ("Gold"):
                // +0.25 health                 
                healthModAmount = 0.25f;
                break;

            case ("Ladder Glasses"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // fire twice (beams of  light, one from each eye) when projectiles are empty"
                faceAccessoryAttributes.eyeLasers = true;
                break;

            case ("Leaf Mouth"):                 //DONE
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = 0.00125f;

                // +1 luck
                faceAccessoryAttributes.luck += 1;
                break;

            case ("Monocle Glasses"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // fire twice (beams of  light, one from each eye) when projectiles are empty"
                faceAccessoryAttributes.eyeLasers = true;
                break;

            case ("Mustache"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;
                break;

            case ("Nerd Glasses"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // fire twice (beams of  light, one from each eye) when projectiles are empty"
                faceAccessoryAttributes.eyeLasers = true;
                break;

            case ("Pipe"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;
                break;

            case ("Plaster Face"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // fire twice (beams of  light, one from each eye) when projectiles are empty"
                faceAccessoryAttributes.eyeLasers = true;
                break;

            case ("Plaster Nose"):              //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // fire twice (beams of  light, one from each eye) when projectiles are empty"
                faceAccessoryAttributes.eyeLasers = true;
                break;

            case ("Ribbon"):
                // +0.25 health                 
                healthModAmount = 0.25f;
                break;

            case ("Round Glasses"):         //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;
                break;

            case ("Scar Eyes"):         //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;
                break;

            case ("Scar Face"):         //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;
                break;

            case ("Sleep Mask"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // enemies are slowed after taking damage
                faceAccessoryAttributes.slowOnAttack = true;
                break;

            case ("Sunglasses"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // fire twice (beams of  light, one from each eye) when projectiles are empty"
                faceAccessoryAttributes.eyeLasers = true;
                break;

            case ("Tongue"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;
                break;

            case ("Pacifier"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("Starscreen"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("Cbass Visor"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("Dunzzo Face"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("Devil's Fang"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("Fastbot Face"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("Luca Glasses"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("Winter Tears"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("Face Diamonds"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("Grinch Makeup"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("Hologram Face"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("MPOFFSHORE Face"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("Dragon Accessory"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("Gamer Headphones"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("Artistic Moustache"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("DJ Magic Headphones"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            case ("Tough Guy Eye Patch"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                faceAccessoryAttributes.healthRegen = true;
                faceAccessoryAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                faceAccessoryAttributes.maxAmmo += 1;

                // +1 life
                faceAccessoryAttributes.lives += 1;
                break;

            default:
                healthModAmount = 0;
                meleeModAmount = 0;
                projectileCountModAmount = 0;
                break;
        }

        faceAccessoryAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
        meleeStats.damage += MeleeDamageModifier(meleeModAmount);
       // faceAccessoryAttributes.addedProjectiles += projectileCountModAmount;
        //faceAccessoryAttributes.maxAmmo += MaxAmmoModifier(faceAccessoryAttributes.maxAmmo, defaultStats.maxAmmo);
        return faceAccessoryAttributes;
    }  
    Attributes OuterWearAttributes(Attributes outerWearAttributes, float healthModAmount, float meleeModAmount, float projectileCountModAmount, int index)          //<- This has LE items only
    {
        if (nftAttributes[index].outerWear == null)
            return outerWearAttributes;
        switch (nftAttributes[index].outerWear.value)
        {
            case ("Aqua Blue Jacket"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;
                break;

            case ("Armor Body"):            //DONE
                // +3 health                 
                healthModAmount = 3;

                // +2 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = 0.01f;

                // +2 projectile count
                outerWearAttributes.maxAmmo += 2;
                break;

            case ("Beige Jacket"):         //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;
                break;

            case ("Bell Hoodie"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;
                break;

            case ("Black Cloak"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;

                // Can Become Invisible (enemies can't track)
                outerWearAttributes.invisible = true;
                break;

            case ("Black Jacket"):          //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;

                // Can Become Invisible (enemies can't track)
                outerWearAttributes.invisible = true;
                break;

            case ("Blue Hoodie"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;

                // Can Become Invisible (enemies can't track)
                outerWearAttributes.invisible = true;
                break;

            case ("Doctor Coat"):              //DONE
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = 0.00125f;

                // +1 Luck
                outerWearAttributes.luck += 1;
                break;

            case ("Flower Necklace"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;

                // Can Become Invisible (enemies can't track)
                outerWearAttributes.invisible = true;
                break;

            case ("Green Hoodie"):         //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;
                break;

            case ("Jungle Hoodie"):                //DONE
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = 0.00125f;

                // +1 Luck
                outerWearAttributes.luck += 1;
                break;

            case ("Leather Jacket"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;
                break;

            case ("Raincoat"):                             //DONE
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = 0.00125f;

                // +1 Luck
                outerWearAttributes.luck += 1;
                break;

            case ("Red Hoodie"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;
                break;

            case ("Scarf Blue"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;
                break;

            case ("Scarf Red"):            //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;
                break;

            case ("Sleeveless Hoodie"):       //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;
                break;

            case ("Uniform"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;
                break;

            case ("Spiked Jacket"):                                //DONE
                // +0.25 health                 
                healthModAmount = 0.25f;

                // +0.25 health regen
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = 0.00125f;

                // +1 Luck
                outerWearAttributes.luck += 1;
                break;

            case ("Yellow Jacket"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;
                break;

            case ("Royal Cloak"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 Luck 
                outerWearAttributes.luck += 1;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;

                // Double Jump
                outerWearAttributes.airJumps += 1;
                break;

            case ("Baby Bib"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;

                // +1 life
                outerWearAttributes.lives += 1;

                // Character can fly
                outerWearAttributes.airJumps += 50;
                break;

            case ("Cbass Jacket"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;

                // +1 life
                outerWearAttributes.lives += 1;

                // Character can fly
                outerWearAttributes.airJumps += 50;
                break;

            case ("Carter Jacket"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;

                // +1 life
                outerWearAttributes.lives += 1;

                // Character can fly
                outerWearAttributes.airJumps += 50;
                break;

            case ("Dunzzo Jacket"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;

                // +1 life
                outerWearAttributes.lives += 1;

                // Character can fly
                outerWearAttributes.airJumps += 50;
                break;

            case ("Farmer Overall"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;

                // +1 life
                outerWearAttributes.lives += 1;

                // Character can fly
                outerWearAttributes.airJumps += 50;
                break;

            case ("Maniqueen Shawl"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;

                // +1 life
                outerWearAttributes.lives += 1;

                // Character can fly
                outerWearAttributes.airJumps += 50;
                break;

            case ("Super Hacker Hoodie"):           //DONE
                // +1 health                 
                healthModAmount = 1;

                // +1 health regen 
                outerWearAttributes.healthRegen = true;
                outerWearAttributes.healthRegenAmount = .005f;

                // +1 projectile count
                outerWearAttributes.maxAmmo += 1;

                // +1 life
                outerWearAttributes.lives += 1;

                // Character can fly
                outerWearAttributes.airJumps += 50;
                break;

            default:
                healthModAmount = 0;
                meleeModAmount = 0;
                projectileCountModAmount = 0;
                break;
        }
        outerWearAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
        meleeStats.damage += MeleeDamageModifier(meleeModAmount);
       // outerWearAttributes.addedProjectiles += projectileCountModAmount;
       // outerWearAttributes.maxAmmo += MaxAmmoModifier(outerWearAttributes.maxAmmo, defaultStats.maxAmmo);
        return outerWearAttributes;
    }     
    
    // This is different from the melee and ranged weapon category it is more general and specifically LE 
    Attributes WeaponAttributes(Attributes weaponAttributes, float healthModAmount, float meleeModAmount, float rangedModAmount, float projectileCountModAmount, int index)
    {
        if (nftAttributes[index].weapon == null)
            return weaponAttributes;
        switch (nftAttributes[index].weapon.value)
        {
            case ("Black"):          //DONE                   
                // +0.25 health
                healthModAmount = 0.25f;

                // 0.25 projectile count
                weaponAttributes.maxAmmo += .25f;

                // 0.25 projectile count
                weaponAttributes.maxAmmo += .25f;

                // +0.5 luck
                weaponAttributes.luck += .5f;
                break;

            case ("Blade"):                //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                weaponAttributes.chestLaser = true;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Bottle Of Stars"):               //DONE                
                // +1 health
                healthModAmount = 1f;

                // 0.25 projectile count
                weaponAttributes.maxAmmo += .25f;

                // +1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 luck
                weaponAttributes.luck += 1;

                // projectiles are snowballs
                weaponAttributes.snowBallProjectiles = true;
                break;

            case ("Bow"):        //DONE                    
                // +0.25 health
                healthModAmount = 0.25f;

                // 0.25 projectile count
                weaponAttributes.maxAmmo += .25f;

                // 0.25 projectile count
                weaponAttributes.maxAmmo += .25f;

                // +0.5 luck
                weaponAttributes.luck += .5f;
                break;

            case ("Claw"):          //DONE                   
                // +0.25 health
                healthModAmount = 0.25f;

                // 0.25 projectile count
                weaponAttributes.maxAmmo += .25f;

                // 0.25 projectile count
                weaponAttributes.maxAmmo += .25f;

                // +0.5 luck
                weaponAttributes.luck += .5f;
                break;

            case ("Dakimakura"):                //DONE            
                // +0.25 health
                healthModAmount = 0.25f;

                // 0.25 projectile count
                weaponAttributes.maxAmmo += .25f;

                // 0.25 projectile count
                weaponAttributes.maxAmmo += .25f;

                // +0.5 luck
                weaponAttributes.luck += .5f;
                break;

            case ("Fish"):              //DONE                
                // +1 health
                healthModAmount = 1;

                // 0.25 projectile count
                weaponAttributes.maxAmmo += 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 luck
                weaponAttributes.luck += 1;
                break;

            case ("Hand"):                  //DONE                                
                // +0.25 health
                healthModAmount = 0.25f;

                // 0.25 projectile count
                weaponAttributes.maxAmmo += .25f;

                // 0.25 projectile count
                weaponAttributes.maxAmmo += .25f;

                // +0.5 luck
                weaponAttributes.luck += .5f;
                break;

            case ("Lolipop"):               //DONE                          
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                weaponAttributes.chestLaser = true;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Mic"):               //DONE                      
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                weaponAttributes.chestLaser = true;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Net"):           //DONE                        
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                weaponAttributes.chestLaser = true;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Paint"):                //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // fire twice (beams of light from the chest) when Projectiles are empty
                weaponAttributes.chestLaser = true;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Paper"):                 //DONE     
                // +0.25 health
                healthModAmount = 0.25f;

                // 0.25 projectile count
                weaponAttributes.maxAmmo += .25f;

                // 0.25 projectile count
                weaponAttributes.maxAmmo += .25f;

                // +0.5 luck
                weaponAttributes.luck += .5f;
                break;

            case ("Pumpkin"):               //DONE                                 
                // +0.25 health
                healthModAmount = 0.25f;

                // 0.25 projectile count
                weaponAttributes.maxAmmo += .25f;

                // 0.25 projectile count
                weaponAttributes.maxAmmo += .25f;

                // +0.5 luck
                weaponAttributes.luck += .5f;
                break;

            case ("Corn Sword"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Fast Claws"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Luca Laser"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Star Sword"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Angel Sword"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Baby Rattle"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("DJ Magic CD"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Paint Brush"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Hologram Sword"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Devil's Trident"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Tough Guy Fists"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Christmas Cookie"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Cbass' Lightsaber"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Crescent Moon Staff"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Skateboard The Weapon"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Gamestation Controller"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("MPOFFSHORE Light Saber"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Maniqueen Diamond Sword"):            //DONE
                // +1 health
                healthModAmount = 1;

                // 1 projectile count
                weaponAttributes.maxAmmo += 1;

                // +1 life
                weaponAttributes.lives += 1;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;
                break;

            case ("Iron Sword"):
                // +0.25 melee damage
                meleeModAmount = 0.25f;
                break;

            case ("Bronze Sword"):
                // +0.25 Melee damage
                meleeModAmount = 0.25f;
                break;

            case ("Bronze Axe"):
                // +0.25 melee damage
                meleeModAmount = 0.25f;
                break;

            case ("Iron Scythe"):
                // +0.25 melee damage
                meleeModAmount = 0.25f;
                break;
            case ("Bronze Scythe"):
                // +0.25 melee damage
                meleeModAmount = 0.25f;
                break;

            case ("Iron Axe"):
                // +0.25 melee damage
                meleeModAmount = 0.25f;
                break;

            case ("Gold Sword"):              //DONE
                // +0.75 melee damage
                meleeModAmount = 0.75f;

                // Adds 3 ticks of bleeding damage over 3 seconds.
                //bleed = true;
                //statusDamage = 3;
                //statusCount = 3;
                //GameController.instance.playerData[0].controller.Player.shooter.attackObject[1].GetComponent<A_Hitbox>().dealStatus = true;
                break;

            case ("Diamond Sword"):           //DONE
                // +1 melee damage
                meleeModAmount = 1f;

                // +1 bubble shield/level
                weaponAttributes.bubbleShield = true;

                // adds fire damage to melee attacks(3 ticks of damage over 3 seconds)
                //fire = true;
                //statusDamage = 3;
                //statusCount = 3;
                //GameController.instance.playerData[0].controller.Player.shooter.attackObject[1].GetComponent<A_Hitbox>().dealStatus = true;
                break;

            case ("Diamond Axe"):           //DONE
                // +1 Melee damage
                meleeModAmount = 1;

                //Double Melee strike;
                weaponAttributes.doubleMelee = true;

                break;

            case ("Diamond Scythe"):                  //DONE
                // +1.25 Melee damage
                meleeModAmount = 1.25f;

                // fast movement
                weaponAttributes.moveMagnitude = moveSpeed;

                // Adds fire damage to melee attacks (3 ticks of damage over 3 seconds
                //fire = true;
                //statusDamage = 3;
                //statusCount = 3;
                //GameController.instance.playerData[0].controller.Player.shooter.attackObject[1].GetComponent<A_Hitbox>().dealStatus = true;

                // +1 health regen
                weaponAttributes.healthRegen = true;
                weaponAttributes.healthRegenAmount = .005f;

                // +1 life
                weaponAttributes.lives += 1;
                break;

            case ("BaseBall Bat"):         //DONE
                // +0.75 melee damage
                meleeModAmount = 0.75f;

                // Add 3 ticks of damage over 3 seconds
                //bleed = true;
                //statusDamage = 3;
                //statusCount = 3;
                //GameController.instance.playerData[0].controller.Player.shooter.attackObject[1].GetComponent<A_Hitbox>().dealStatus = true;
                break;

            case ("Tire Iron"):         //DONE
                // +0.75 melee damage
                meleeModAmount = 0.75f;

                // enemies are slowed after taking damage
                weaponAttributes.slowOnAttack = true;
                break;

            case ("Poisoned Scythe"):           //DONE
                // +0.75 melee damage
                meleeModAmount = 0.75f;

                // Adds acid damage to melee attacks (3 ticks of damage over 3 seconds)
                //acid = true;
                //statusDamage = 3;
                //statusCount = 3;
                //GameController.instance.playerData[0].controller.Player.shooter.attackObject[1].GetComponent<A_Hitbox>().dealStatus = true;                
                break;

            case ("Dagger"):            //DONE
                // +0.75 melee damage
                meleeModAmount = 0.75f;

                // throw dagger when projectiles are empty
                weaponAttributes.throwItem = true;
                weaponAttributes.throwItemDagger = true;
                break;

            case ("Twin Daggers"):          //DONE
                // +0.75 melee damage 
                meleeModAmount = 0.75f;

                // Double Melee strike
                weaponAttributes.doubleMelee = true;

                break;

            case ("Rainbow Sword"):                //DONE
                // +1 Melee Damage
                meleeModAmount = 1;

                // Double Melee strike
                weaponAttributes.doubleMelee = true;


                // Adds Holy Damage (3 ticks of damage over 3 seconds)
                //holy = true;
                //statusDamage = 3;
                //statusCount = 3;
                //GameController.instance.playerData[0].controller.Player.shooter.attackObject[1].GetComponent<A_Hitbox>().dealStatus = true;
                break;

            case ("Raipier"):           //DONE
                // +0.75 melee damage
                meleeModAmount = 0.75f;

                // double jump
                weaponAttributes.airJumps += 1;
                break;

            case ("Whip"):                //DONE
                // +1 melee damage
                meleeModAmount = 1;

                // Can become invisible(enemies can't track)
                weaponAttributes.invisible = true;

                // Adds holy damage to melee attacks (3 ticks of damage over 3 seconds)
                //holy = true;
                //statusDamage = 3;
                //statusCount = 3;
                //GameController.instance.playerData[0].controller.Player.shooter.attackObject[1].GetComponent<A_Hitbox>().dealStatus = true;
                break;

            case ("Golden Staff"):            //DONE
                // +1.25 melee damage
                meleeModAmount = 1.25f;

                // +1 extra life
                weaponAttributes.lives += 1;

                // Adds holy damage to melee attacks(3 ticks of damage over 3 seconds)
                //holy = true;
                //statusDamage = 3;
                //statusCount = 3;
                //GameController.instance.playerData[0].controller.Player.shooter.attackObject[1].GetComponent<A_Hitbox>().dealStatus = true;
                break;

            case ("Rainbow Staff"):         //DONE
                // +0.75 melee damage
                meleeModAmount = 0.75f;

                // fast movement
                weaponAttributes.moveMagnitude = moveSpeed;

                // double melee strike
                weaponAttributes.doubleMelee = true;

                break;

            case ("Squirt Gun"):
                // +0.25 projectile Damage
                rangedModAmount = 0.25f;
                break;

            case ("Spray Bottle"):
                // +0.25 projectile Damage
                rangedModAmount = 0.25f;
                break;

            case ("Pool Noodle"):
                // +0.25 projectile Damage
                rangedModAmount = 0.25f;
                break;

            case ("Rubber hose"):
                // +0.25 projectile Damage
                rangedModAmount = 0.25f;
                break;

            case ("Pop Gun"):
                // +0.25 projectile Damage
                rangedModAmount = 0.25f;
                break;

            case ("Pistol"):
                // +0.25 projectile Damage
                rangedModAmount = 0.25f;
                break;

            case ("Lazer Gun"):         //DONE
                // +0.75 projectile Damage
                rangedModAmount = 0.75f;

                //+1 projectile recharge
                weaponAttributes.ammoRechargeAmount += ammoRechargeSpeed;
                break;

            case ("Flamethrower"):            //DONE
                // +1 projectile Damage
                rangedModAmount = 1;

                //+1 projectile recharge
                weaponAttributes.ammoRechargeAmount += ammoRechargeSpeed;

                //adds fire damage to projectile attacks (3 ticks of damage over 3 seconds)
                //fire = true;
                //statusDamage = 3;
                //statusCount = 3;
                //GameController.instance.playerData[0].controller.Player.shooter.attackObject[0].GetComponent<A_Hitbox>().dealStatus = true;
                break;

            case ("Double Barrelled Lazer Gun"):                      //DONE
                //+1 Projectile Damage
                rangedModAmount = 1;

                // Double Projectile Fire
                weaponAttributes.doubleProjectile = true;
                break;

            case ("Golden Gun"):            //DONE
                // +1.25 projectile dmg
                rangedModAmount = 1.25f;

                // fast movement
                weaponAttributes.moveMagnitude = moveSpeed;

                // +1 extra projectile
                weaponAttributes.maxAmmo += 1;

                // +1 projectile regen
                weaponAttributes.ammoRechargeAmount += ammoRechargeSpeed;

                // +1 life
                weaponAttributes.lives += 1;
                break;

            case ("Shotgun"):                    //DONE
                // +0.75 projectile damage
                rangedModAmount = 0.75f;

                // adds 3 ticks of damage over 3 seconds
                //bleed = true;
                //statusDamage = 3;
                //statusCount = 3;
                //GameController.instance.playerData[0].controller.Player.shooter.attackObject[0].GetComponent<A_Hitbox>().dealStatus = true;
                break;

            case ("Golden Bow and Arrow"):          //DONE
                // +0.75 projectile damage
                rangedModAmount = 0.75f;

                // enemies are slowed after taking damage;
                weaponAttributes.slowOnAttack = true;
                break;

            case ("Acid Sprayer"):            //DONE
                // +0.75 projectile damage
                rangedModAmount = 0.75f;

                // adds acid damage to projectile attacks (4 ticks of damage over 4 seconds)
                //acid = true;
                //statusDamage = 4;
                //statusCount = 4;
                //GameController.instance.playerData[0].controller.Player.shooter.attackObject[0].GetComponent<A_Hitbox>().dealStatus = true;


                break;

            case ("Michale Jackson hat over cannon"):           //DONE
                // +0.75 projectile damage
                rangedModAmount = 0.75f;

                // Throw Sombrero when projectiles are empty
                weaponAttributes.throwItem = true;
                weaponAttributes.throwItemSombrero = true;
                break;

            case ("Cyborg Eyes"):           //DONE
                // +0.75 projectile damage
                rangedModAmount = 0.75f;

                // Fire twice (Beams of light one from each eye) when projectiles are empty
                weaponAttributes.eyeLasers = true;
                break;

            case ("Snow Shovel"):           //DONE
                // +1 projectile damage
                rangedModAmount = 1;

                // +1 extra projectile
                weaponAttributes.maxAmmo += 1;

                // projectiles are snowballs
                weaponAttributes.snowBallProjectiles = true;
                break;

            case ("Bow n Arrow"):           //DONE
                // +0.75 projectile damage
                rangedModAmount = 0.75f;

                // double jump
                weaponAttributes.airJumps += 1;
                break;

            case ("Golden Lasso"):          //DONE
                //+1 projectile damage
                rangedModAmount = 1;

                // can become invisible (enemies can't track)
                weaponAttributes.invisible = true;
                break;

            case ("Spell Book"):            //DONE
                // +1.25 projectile damage
                rangedModAmount = 1.25f;

                // +1 life
                weaponAttributes.lives += 1;

                // fire 3 projectiles when shooting
                weaponAttributes.triShot = true;
                break;

            case ("Tommy Gun"):         //DONE
                // +0.75f projectile damage
                rangedModAmount = .75f;

                // fire 3 projectiles when shooting
                weaponAttributes.triShot = true;
                break;

            default:
                healthModAmount = 0;
                meleeModAmount = 0;
                //projectileCountModAmount = 0;
                rangedModAmount = 0;
                break;
        }
        weaponAttributes.maxHP += HealthModifier(healthModAmount, defaultStats.maxHP);
        meleeStats.damage += MeleeDamageModifier(meleeModAmount);
        projectileStats.damage += ProjectileDamageModifier(rangedModAmount);
       // weaponAttributes.addedProjectiles += projectileCountModAmount;
        //weaponAttributes.maxAmmo += MaxAmmoModifier(weaponAttributes.maxAmmo, defaultStats.maxAmmo);
        return weaponAttributes;
    }

    #endregion
    bool firstRunHP = true;
    //bool firstRunAmmo = true;
    bool firstMeleeRun = true;
    bool firstProjectileRun = true;
    public int MaxAmmoModifier(float value, float defaultAmmo)
    {
        //if (firstRunAmmo)
        //{
        //    float ammoMod = defaultAmmo + value;
        //    firstRunAmmo = false;
        //    return (int)ammoMod;
        //}
        //else
            return(int)value;

    }
    public int ProjectileDamageModifier(float value)
    {
        value = value / 3;//NERF

        if (firstProjectileRun)
        {
            float damageMod = value + defaultProjectileDmg;
            firstProjectileRun = false;
            return (int)damageMod;
        }
        //return 0;
        else
            return (int)value;
    }
    public int MeleeDamageModifier(float value)
    {
        value = value / 3;//Nerfed 66%

        if (firstMeleeRun)
        {
            float damageMod = value + defaultMeleeDmg;
            firstMeleeRun = false;
            return (int)damageMod;
        }
        //return 0;
        else
            return (int)value;
    }

    public int HealthModifier(float value, float maxHealth)
    {
        if (firstRunHP)
        {
            float percentage = value + maxHealth;               
            firstRunHP = false;
            return (int)percentage;
        }
        else
            return (int)value;
        //return 0;
    }

    public class JsonObject
    {
        public bool hasDopeCat;
        public bool hasPixelBand;
        public bool hasHippo;
        public bool hasSovanaEgg;
        public bool hasCyberSamurai;
        public bool hasPenguin;
    }

    [Serializable]
    public class AttributesJson
    {
        public metadata metadata;
        public Attribute headwear;
        public Attribute bodywear;
        public Attribute eyewear;   //Does not exist
        public Attribute handAccessory; //Does not exist
        public Attribute pants;
        public Attribute shoes;
        public Attribute weapon;
        public Attribute rangedWeapon; //Does not exist
        public Attribute backwear;
        public Attribute tail;
        public Attribute background;
        public Attribute bodyType;
        public Attribute bodyTail;
        public Attribute eyes;
        public Attribute faceAccessory;
        public Attribute outerWear;
        public Attribute special;
    }

    [Serializable]
    public class Attribute
    {
        public string trait_type;
        public string value;
    }

    [Serializable]
    public class metadata
    {
        public string name;
        public string symbol;
        public string description;
        public string seller_fee_basis_points;
        public string image;
        public string external_url;
        public string edition;
        public List<Attribute> attributes;
        public string properties;
    }

    public void NextLevel()
    {
        GameController.instance.NextLevel();
    }
    

}
