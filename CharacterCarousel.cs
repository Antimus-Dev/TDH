//Created by Liam Gilmore, Andrew Sylvester
// Script to generate characters to select from the provided attributes objects. Provides logic for moving the carousel and selecting a character.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SoundSystem;
using UnityEngine.InputSystem;
using System;
using TMPro;

public class CharacterCarousel : MonoBehaviour
{
    [SerializeField] private Attributes[] characters;
    [SerializeField] private Attributes nftCharAtt;
    public bool[] charactersUnlocked; 
    [SerializeField] private GameObject characterImagePrefab;
    [SerializeField] private GameObject nftCharacterImagePrefab;
    [SerializeField] private P_PlayerPrefs playerPrefs;
    [SerializeField] private MusicEvent levelMusic;
    public SoundEvent moveSFX;
    public SoundEvent ConfirmSFX;
    [SerializeField] private JavascriptHook hook;
    public GameObject selectedCharacter;
    public GameObject selectedCharacterDisplay;
    [SerializeField] private GameObject buttonTextObj;
    [SerializeField] private GameObject waitingTextObj;
    [SerializeField] private Button confirmButton;

    private int index = 0;
    private float lastDirection;
    private Vector2 inputValue;
    private bool loadedNFTCharacters = false;
    private Attributes NFTMod;

    private void Start()
    {
        #if !UNITY_EDITOR
            confirmButton.interactable = false;
            waitingTextObj.SetActive(true);
        #endif

        SpawnCharacterImages();
        levelMusic.Play(1);
        NFTMod = playerPrefs.NFTModifier;
    }

    private void Update()
    {
        #if !UNITY_EDITOR
            if (SecurityHandler.instance.hasSessionID()) 
            {
                confirmButton.interactable = true;
                waitingTextObj.SetActive(false);
            }
        #endif

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Fire1"))
        {
            ConfirmCharacter();
        }

        if(SpritesheetManager.instance.IsDoneLoadingSprites() && !loadedNFTCharacters)
        {
            loadedNFTCharacters = true;
            //RefreshCharacters();
            SpawnCharacterImagesNFTs();
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        inputValue = ctx.ReadValue<Vector2>();
    }

    /// <summary>
    /// Populates character carousel with images pulled from the player prefabs on the Attributes.
    /// </summary>
   // public GameObject defaultRow;
  // private bool lastRow;
    public GameObject content;
    private void SpawnCharacterImages()
    {
        List<GameObject> charImage = new List<GameObject>();

        for (int i = 0; i < characters.Length; i++)
        {
            Attributes charAtt = characters[i];
                charImage.Add(Instantiate(characterImagePrefab, content.transform));

            Sprite sprite = charAtt.playerPrefab.GetComponent<Player>().spriteObject.GetComponent<SpriteRenderer>().sprite;
            charImage[i].GetComponent<CharacterSelectImage>().SetUp(playerPrefs.charactersUnlocked[i], sprite);
        }
        index = 0;
        content.transform.GetChild(0).GetComponent<CharacterSelectImage>().selectionOverlay.SetActive(true);
    }

    /// <summary>
    /// Creates NFT character selects
    /// </summary>
    private void SpawnCharacterImagesNFTs()
    {
        for (int k = 0; k < selectedCharacterDisplay.GetComponent<CharacterSelectDisplayManager>().accessories.Length; k++)
        {
            selectedCharacterDisplay.GetComponent<CharacterSelectDisplayManager>().accessories[k].GetComponent<CharacterSelectDisplay>().enabled = true;
            selectedCharacterDisplay.GetComponent<CharacterSelectDisplayManager>().accessories[k].GetComponent<Image>().enabled = true;
        }

        for (int i = SpritesheetManager.instance.spritesheets.Count - 1; i >= 0; i--)
        {
            GameObject charImage = Instantiate(nftCharacterImagePrefab, content.transform);
            charImage.transform.SetAsFirstSibling();
            charImage.GetComponent<CharacterSelectDisplayManager>().SliceNewSprites(i);
        }
        index = 0;
        selectedCharacter = content.transform.GetChild(0).gameObject;

        NFTCharacterSelectOnPointer pointer = selectedCharacter.GetComponent<NFTCharacterSelectOnPointer>();

        if (pointer.NFTImageList.Count >= 1)
            for (int i = 0; i < pointer.NFTImageList.Count; i++)
            {
                for (int k = 0; k < pointer.NFTImageList[i].avatarAnim.Length; k++)
                {

                    pointer.NFTImageList[i].avatarAnim[k].GetComponent<Image>().enabled = true;
                }

                if (pointer.NFTImageList[i].selectionOverlay != null && !pointer.NFTImageList[i].displayObject)
                {
                    pointer.NFTImageList[i].selectionOverlay.SetActive(false);
                    pointer.NFTImageList[i].selected = false;

                }
            }

        for (int i = 0; i < pointer.imageList.Count; i++)
        {
            pointer.imageList[i].selectionOverlay.SetActive(false);
            pointer.imageList[i].selected = false;
            pointer.imageList[i].enabled = false;
        }

        selectedCharacter.GetComponent<CharacterSelectDisplayManager>().selectionOverlay.SetActive(true);
        pointer.selectionOverlay.SetActive(true);
        selectedCharacterDisplay.GetComponent<Image>().sprite = selectedCharacterDisplay.transform.GetChild(0).GetComponent<Image>().sprite;
    }
    /// <summary>
    /// Locks in the selected character and loads the game scene or lobby.
    /// </summary>
    public void ConfirmCharacter()
    {
        ConfirmSFX.PlayOneShot(0);
        if (SecurityHandler.instance.hasSessionID())
        {
            index = selectedCharacter.transform.GetSiblingIndex();
            int adjustedIndex = index - SpritesheetManager.instance.spritesheets.Count;
            if (index < SpritesheetManager.instance.spritesheets.Count)
            {
                playerPrefs.nftIndex = index;
                playerPrefs.attributesModifier = nftCharAtt;
                playerPrefs.NFTModifier = NFTMod;
                hook.AddAttributes(playerPrefs.NFTModifier, index);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);                    
            }
            else if (playerPrefs.charactersUnlocked[adjustedIndex])
            {
                playerPrefs.attributesModifier = characters[adjustedIndex];
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }            
        }
    }

    public void RefreshCharacters()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        SpawnCharacterImages();
    }
}
