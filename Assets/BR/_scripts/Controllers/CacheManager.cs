using UnityEngine;
using System.Collections.Generic;
using System;

public class CacheManager : MonoBehaviour {

    #region INITIALIZATION

    private static CacheManager _instance;

    public static CacheManager instance
    {
        get
        {
            if(_instance == null)
            {
                throw new System.Exception("CacheManager object not found");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // Allow only one instance of the manager in scene
        if(_instance != null && _instance != this)
        {
            DestroyImmediate(this);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    #endregion

    #region VARIABLES

    private Dictionary<string, Texture2D> cacheDictionary;

    #endregion

    #region PUBLIC METHODS

    public void AddToDictionary(string url, Texture2D tex)
    {
        if(cacheDictionary == null)
            cacheDictionary = new Dictionary<string, Texture2D>();

        try
        {
            cacheDictionary.Add(url, tex);
        }
        catch (ArgumentException)
        {
            Debug.Log("Already exists");
        }
    }

    public bool RemoveFromDictionary(string url)
    {
        if(cacheDictionary == null)
        {
            cacheDictionary = new Dictionary<string, Texture2D>();
            return false;
        }
        return cacheDictionary.Remove(url);
    }

    public bool GetCachedTexture(string url, out Texture2D tex)
    {
        if(cacheDictionary == null)
        {
            cacheDictionary = new Dictionary<string, Texture2D>();
            tex = null;
            return false;
        }

        return cacheDictionary.TryGetValue(url, out tex);
    }

    #endregion

}
