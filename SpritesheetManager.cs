//Created by: Liam Gilmore, Andrew Sylvester
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirebaseWebGL.Scripts.FirebaseBridge;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SpritesheetManager : MonoBehaviour
{
    public static SpritesheetManager instance;
    public List<Spritesheets> spritesheets = new List<Spritesheets>();
    private const string pathStart = "Attribute Sprite Sheets/Base Modifiers/";
    //public CharacterSelectDisplayManager testAvatar;
    private List<string> LoadingList = new List<string>();
    private bool startedLoading = false;

    [Serializable]
    public class Spritesheets
    {
        public Texture2D backwear;
        public Texture2D body;
        public Texture2D bodyTail;
        public Texture2D eyes;
        public Texture2D tail;
        public Texture2D bodywear;
        public Texture2D pants;
        public Texture2D shoes;
        public Texture2D outerwear;
        public Texture2D faceAccessory;
        public Texture2D headwear;
        public Texture2D special;
        public Texture2D weapon;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Pulls sprite sheets based on the passed in json data
    /// </summary>
    /// <param name="index">What index of nftAttributes to use</param>
    public void PullSpritesheets(int index)
    {
        startedLoading = true;
        Spritesheets sheet = new Spritesheets();
        spritesheets.Add(sheet);
        string path;
        //Eww what redundancy, please give me time to refactor this later
        JavascriptHook.AttributesJson attributes = JavascriptHook.nftAttributes[index];
        if (attributes.bodywear.value != "None" && attributes.bodywear.value != "None " && !string.IsNullOrEmpty(attributes.bodywear.value))
        {
            path = pathStart + attributes.bodywear.trait_type + "/Dope Cats " + attributes.bodywear.value + ".png";
            LoadingList.Add(index + attributes.bodywear.trait_type);
            FirebaseStorage.GetDownloadURL(index, path, gameObject.name, "GetBodywearTexture", "OnFailure");
        }
        if (attributes.backwear.value != "None" && attributes.backwear.value != "None " && !string.IsNullOrEmpty(attributes.backwear.value))
        {
            path = pathStart + attributes.backwear.trait_type + "/Dope Cats " + attributes.backwear.value + ".png";
            LoadingList.Add(index + attributes.backwear.trait_type);
            FirebaseStorage.GetDownloadURL(index, path, gameObject.name, "GetBackwearTexture", "OnFailure");
        }
        if (attributes.bodyType.value != "None" && attributes.bodyType.value != "None " && !string.IsNullOrEmpty(attributes.bodyType.value))
        {
            path = pathStart + attributes.bodyType.trait_type + "/Dope Cats " + attributes.bodyType.value + ".png";
            LoadingList.Add(index + attributes.bodyType.trait_type);
            FirebaseStorage.GetDownloadURL(index, path, gameObject.name, "GetBodyTexture", "OnFailure");

            //Body tail match
            attributes.bodyTail.value = attributes.bodyType.value + " Tail";
            attributes.bodyTail.trait_type = "Body Tail";
            path = pathStart + attributes.bodyTail.trait_type + "/Dope Cats " + attributes.bodyTail.value + ".png";
            LoadingList.Add(index + attributes.bodyTail.trait_type);
            FirebaseStorage.GetDownloadURL(index, path, gameObject.name, "GetBodyTailTexture", "OnFailure");
        }
        if (attributes.eyes.value != "None" && attributes.eyes.value != "None " && !string.IsNullOrEmpty(attributes.eyes.value))
        {
            path = pathStart + attributes.eyes.trait_type + "/Dope Cats " + attributes.eyes.value + ".png";
            LoadingList.Add(index + attributes.eyes.trait_type);
            FirebaseStorage.GetDownloadURL(index, path, gameObject.name, "GetEyesTexture", "OnFailure");
        }
        if (attributes.faceAccessory.value != "None" && attributes.faceAccessory.value != "None " && !string.IsNullOrEmpty(attributes.faceAccessory.value))
        {
            path = pathStart + attributes.faceAccessory.trait_type + "/Dope Cats " + attributes.faceAccessory.value + ".png";
            LoadingList.Add(index + attributes.faceAccessory.trait_type);
            FirebaseStorage.GetDownloadURL(index, path, gameObject.name, "GetFaceAccessoryTexture", "OnFailure");
        }
        if (attributes.headwear.value != "None" && attributes.headwear.value != "None " && !string.IsNullOrEmpty(attributes.headwear.value))
        {
            path = pathStart + attributes.headwear.trait_type + "/Dope Cats " + attributes.headwear.value + ".png";
            LoadingList.Add(index + attributes.headwear.trait_type);
            FirebaseStorage.GetDownloadURL(index, path, gameObject.name, "GetHeadwearTexture", "OnFailure");
        }
        if (attributes.outerWear.value != "None" && attributes.outerWear.value != "None " && !string.IsNullOrEmpty(attributes.outerWear.value))
        {
            path = pathStart + attributes.outerWear.trait_type + "/Dope Cats " + attributes.outerWear.value + ".png";
            LoadingList.Add(index + attributes.outerWear.trait_type);
            FirebaseStorage.GetDownloadURL(index, path, gameObject.name, "GetOuterwearTexture", "OnFailure");
        }
        if (attributes.pants.value != "None" && attributes.pants.value != "None " && !string.IsNullOrEmpty(attributes.pants.value))
        {
            path = pathStart + attributes.pants.trait_type + "/Dope Cats " + attributes.pants.value + ".png";
            LoadingList.Add(index + attributes.pants.trait_type);
            FirebaseStorage.GetDownloadURL(index, path, gameObject.name, "GetPantsTexture", "OnFailure");
        }
        if (attributes.shoes.value != "None" && attributes.shoes.value != "None " && !string.IsNullOrEmpty(attributes.shoes.value))
        {
            path = pathStart + attributes.shoes.trait_type + "/Dope Cats " + attributes.shoes.value + ".png";
            LoadingList.Add(index + attributes.shoes.trait_type);
            FirebaseStorage.GetDownloadURL(index, path, gameObject.name, "GetShoesTexture", "OnFailure");
        }
        if (attributes.special.value != "None" && attributes.special.value != "None " && !string.IsNullOrEmpty(attributes.special.value))
        {
            path = pathStart + attributes.special.trait_type + "/Dope Cats " + attributes.special.value + ".png";
            LoadingList.Add(index + attributes.special.trait_type);
            FirebaseStorage.GetDownloadURL(index, path, gameObject.name, "GetSpecialTexture", "OnFailure");
        }
        if (attributes.tail.value != "None" && attributes.tail.value != "None " && !string.IsNullOrEmpty(attributes.tail.value))
        {
            path = pathStart + attributes.tail.trait_type + "/Dope Cats " + attributes.tail.value + ".png";
            LoadingList.Add(index + attributes.tail.trait_type);
            FirebaseStorage.GetDownloadURL(index, path, gameObject.name, "GetTailTexture", "OnFailure");
        }
        if (attributes.weapon.value != "None" && attributes.weapon.value != "None " && !string.IsNullOrEmpty(attributes.weapon.value))
        {
            path = pathStart + attributes.weapon.trait_type + "/Dope Cats " + attributes.weapon.value + ".png";
            LoadingList.Add(index + attributes.weapon.trait_type);
            FirebaseStorage.GetDownloadURL(index, path, gameObject.name, "GetWeaponTexture", "OnFailure");
        }
    }

    //ugh I know this is bad and redundant, but time limits for ya
    /// <summary>
    /// Uses download url to get sprite sheet for specific trait
    /// </summary>
    /// <param name="data">index and download url</param>
    public void GetBackwearTexture(string data)
    {
        //First part of data is the index
        int index = int.Parse(data.Substring(0, 1)); //Will only work up to 9
        //Rest is the url
        string url = data.Substring(1);
        StartCoroutine(GetTexture());

        IEnumerator GetTexture()
        {
            Texture2D texture;
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                while (!request.downloadHandler.isDone)
                    yield return null;

                texture = DownloadHandlerTexture.GetContent(request);
                texture.filterMode = FilterMode.Point;
                spritesheets[index].backwear = texture;
                LoadingList.Remove(index + JavascriptHook.nftAttributes[index].backwear.trait_type);
                //testAvatar.SliceNewSprites();
            }
        }
    }

    public void GetBodywearTexture(string data)
    {
        //First part of data is the index
        int index = int.Parse(data.Substring(0, 1)); //Will only work up to 9
        //Rest is the url
        string url = data.Substring(1);
        StartCoroutine(GetTexture());

        IEnumerator GetTexture()
        {
            Texture2D texture;
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                while (!request.downloadHandler.isDone)
                    yield return null;

                texture = DownloadHandlerTexture.GetContent(request);
                texture.filterMode = FilterMode.Point;
                spritesheets[index].bodywear = texture;
                LoadingList.Remove(index + JavascriptHook.nftAttributes[index].bodywear.trait_type);
                //testAvatar.SliceNewSprites();
            }
        }
    }

    public void GetBodyTailTexture(string data)
    {
        //First part of data is the index
        int index = int.Parse(data.Substring(0, 1)); //Will only work up to 9
        //Rest is the url
        string url = data.Substring(1);
        StartCoroutine(GetTexture());

        IEnumerator GetTexture()
        {
            Texture2D texture;
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                while (!request.downloadHandler.isDone)
                    yield return null;

                texture = DownloadHandlerTexture.GetContent(request);
                texture.filterMode = FilterMode.Point;
                spritesheets[index].bodyTail = texture;
                LoadingList.Remove(index + JavascriptHook.nftAttributes[index].bodyTail.trait_type);
                //testAvatar.SliceNewSprites();
            }
        }
    }

    public void GetBodyTexture(string data)
    {
        //First part of data is the index
        int index = int.Parse(data.Substring(0, 1)); //Will only work up to 9
        //Rest is the url
        string url = data.Substring(1);
        StartCoroutine(GetTexture());

        IEnumerator GetTexture()
        {
            Texture2D texture;
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                while (!request.downloadHandler.isDone)
                    yield return null;

                texture = DownloadHandlerTexture.GetContent(request);
                texture.filterMode = FilterMode.Point;
                spritesheets[index].body = texture;
                LoadingList.Remove(index + JavascriptHook.nftAttributes[index].bodyType.trait_type);
                //testAvatar.SliceNewSprites();
            }
        }
    }

    public void GetFaceAccessoryTexture(string data)
    {
        //First part of data is the index
        int index = int.Parse(data.Substring(0, 1)); //Will only work up to 9
        //Rest is the url
        string url = data.Substring(1);
        StartCoroutine(GetTexture());

        IEnumerator GetTexture()
        {
            Texture2D texture;
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                while (!request.downloadHandler.isDone)
                    yield return null;

                texture = DownloadHandlerTexture.GetContent(request);
                texture.filterMode = FilterMode.Point;
                spritesheets[index].faceAccessory = texture;
                LoadingList.Remove(index + JavascriptHook.nftAttributes[index].faceAccessory.trait_type);
                //testAvatar.SliceNewSprites();
            }
        }
    }

    public void GetHeadwearTexture(string data)
    {
        //First part of data is the index
        int index = int.Parse(data.Substring(0, 1)); //Will only work up to 9
        //Rest is the url
        string url = data.Substring(1);
        StartCoroutine(GetTexture());

        IEnumerator GetTexture()
        {
            Texture2D texture;
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                while (!request.downloadHandler.isDone)
                    yield return null;

                texture = DownloadHandlerTexture.GetContent(request);
                texture.filterMode = FilterMode.Point;
                spritesheets[index].headwear = texture;
                LoadingList.Remove(index + JavascriptHook.nftAttributes[index].headwear.trait_type);
                //testAvatar.SliceNewSprites();
            }
        }
    }

    public void GetOuterwearTexture(string data)
    {
        //First part of data is the index
        int index = int.Parse(data.Substring(0, 1)); //Will only work up to 9
        //Rest is the url
        string url = data.Substring(1);
        StartCoroutine(GetTexture());

        IEnumerator GetTexture()
        {
            Texture2D texture;
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                while (!request.downloadHandler.isDone)
                    yield return null;

                texture = DownloadHandlerTexture.GetContent(request);
                texture.filterMode = FilterMode.Point;
                spritesheets[index].outerwear = texture;
                LoadingList.Remove(index + JavascriptHook.nftAttributes[index].outerWear.trait_type);
                //testAvatar.SliceNewSprites();
            }
        }
    }

    public void GetPantsTexture(string data)
    {
        //First part of data is the index
        int index = int.Parse(data.Substring(0, 1)); //Will only work up to 9
        //Rest is the url
        string url = data.Substring(1);
        StartCoroutine(GetTexture());

        IEnumerator GetTexture()
        {
            Texture2D texture;
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                while (!request.downloadHandler.isDone)
                    yield return null;

                texture = DownloadHandlerTexture.GetContent(request);
                texture.filterMode = FilterMode.Point;
                spritesheets[index].pants = texture;
                LoadingList.Remove(index + JavascriptHook.nftAttributes[index].pants.trait_type);
                //testAvatar.SliceNewSprites();
            }
        }
    }

    public void GetShoesTexture(string data)
    {
        //First part of data is the index
        int index = int.Parse(data.Substring(0, 1)); //Will only work up to 9
        //Rest is the url
        string url = data.Substring(1);
        StartCoroutine(GetTexture());

        IEnumerator GetTexture()
        {
            Texture2D texture;
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                while (!request.downloadHandler.isDone)
                    yield return null;

                texture = DownloadHandlerTexture.GetContent(request);
                texture.filterMode = FilterMode.Point;
                spritesheets[index].shoes = texture;
                LoadingList.Remove(index + JavascriptHook.nftAttributes[index].shoes.trait_type);
                //testAvatar.SliceNewSprites();
            }
        }
    }

    public void GetSpecialTexture(string data)
    {
        //First part of data is the index
        int index = int.Parse(data.Substring(0, 1)); //Will only work up to 9
        //Rest is the url
        string url = data.Substring(1);
        StartCoroutine(GetTexture());

        IEnumerator GetTexture()
        {
            Texture2D texture;
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                while (!request.downloadHandler.isDone)
                    yield return null;

                texture = DownloadHandlerTexture.GetContent(request);
                texture.filterMode = FilterMode.Point;
                spritesheets[index].special = texture;
                LoadingList.Remove(index + JavascriptHook.nftAttributes[index].special.trait_type);
                //testAvatar.SliceNewSprites();
            }
        }
    }

    public void GetTailTexture(string data)
    {
        //First part of data is the index
        int index = int.Parse(data.Substring(0, 1)); //Will only work up to 9
        //Rest is the url
        string url = data.Substring(1);
        StartCoroutine(GetTexture());

        IEnumerator GetTexture()
        {
            Texture2D texture;
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                while (!request.downloadHandler.isDone)
                    yield return null;

                texture = DownloadHandlerTexture.GetContent(request);
                texture.filterMode = FilterMode.Point;
                spritesheets[index].tail = texture;
                LoadingList.Remove(index + JavascriptHook.nftAttributes[index].tail.trait_type);
                //testAvatar.SliceNewSprites();
            }
        }
    }

    public void GetWeaponTexture(string data)
    {
        //First part of data is the index
        int index = int.Parse(data.Substring(0, 1)); //Will only work up to 9
        //Rest is the url
        string url = data.Substring(1);
        StartCoroutine(GetTexture());

        IEnumerator GetTexture()
        {
            Texture2D texture;
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                while (!request.downloadHandler.isDone)
                    yield return null;

                texture = DownloadHandlerTexture.GetContent(request);
                texture.filterMode = FilterMode.Point;
                spritesheets[index].weapon = texture;
                LoadingList.Remove(index + JavascriptHook.nftAttributes[index].weapon.trait_type);
                //testAvatar.SliceNewSprites();
            }
        }
    }


    public void GetEyesTexture(string data)
    {
        //First part of data is the index
        int index = int.Parse(data.Substring(0, 1)); //Will only work up to 9
        //Rest is the url
        string url = data.Substring(1);
        StartCoroutine(GetTexture());

        IEnumerator GetTexture()
        {
            Texture2D texture;
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                while (!request.downloadHandler.isDone)
                    yield return null;

                texture = DownloadHandlerTexture.GetContent(request);
                texture.filterMode = FilterMode.Point;
                spritesheets[index].eyes = texture;
                LoadingList.Remove(index + JavascriptHook.nftAttributes[index].eyes.trait_type);
                //testAvatar.SliceNewSprites();
            }
        }
    }

    /// <summary>
    /// Default failure function of getting download url. Will remove from loading list if missing
    /// </summary>
    /// <param name="errorData">index and url to parse trait type</param>
    public void OnFailure(string errorData)
    {
        //First part of data is the index
        int index = int.Parse(errorData.Substring(0, 1)); //Will only work up to 9
        //Rest is the path
        string errorPath = errorData.Substring(1);
        int endOfString = errorPath.IndexOf("/Dope Cat");
        int len = endOfString - pathStart.Length;
        string traitType = errorPath.Substring(pathStart.Length, len);
        LoadingList.Remove(index + traitType);
        Debug.LogError("Error Getting " + traitType);
    }

    public bool IsDoneLoadingSprites()
    {
        return LoadingList.Count == 0 && startedLoading;
    }
}
