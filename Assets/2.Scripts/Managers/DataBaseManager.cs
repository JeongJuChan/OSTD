using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataBaseManager : MonoBehaviourSingleton<DataBaseManager>
{
    private HashSet<string> keys;
    private Dictionary<string, object> initDatas;

    public void Init()
    {
        LoadAllDatas();
    }

    // When it Called that load settings are all ready
    public void LateInit()
    {
        initDatas.Clear();
        initDatas = null;
    }

    #region Save & Load
    private void LoadAllDatas()
    {
        ES3.CacheFile();
        ES3.settings = new ES3Settings(ES3.Location.Cache);

        initDatas = new Dictionary<string, object>();

        keys = ES3.Load(Consts.DATA_BASE_KEY_HASH, new HashSet<string>(), ES3.settings);
        foreach (string str in keys)
        {
            initDatas.Add(str, ES3.Load(str, ES3.settings));
        }
    }

    public string Load(string key, string defaultVlaue)
    {
        if (initDatas.ContainsKey(key))
        {
            return initDatas[key].ToString();
        }

        return defaultVlaue;
    }

    public T Load<T>(string key, T defaultVlaue)
    {
        if (initDatas.ContainsKey(key))
        {
            return (T)initDatas[key];
        }
        else if (typeof(T) == typeof(string))
        {
            return default;
        }

        return defaultVlaue;
    }

    public void Save(string key, object obj)
    {
        if (!initDatas.ContainsKey(key))
        {
            initDatas.Add(key, obj);
        }
        else
        {
            initDatas[key] = obj;
        }

        keys.Add(key);
        ES3.Save(key, obj, ES3.settings);
        ES3.Save(Consts.DATA_BASE_KEY_HASH, keys, ES3.settings);
        ES3.StoreCachedFile();
    }

    public bool ContainsKey(string key)
    {
        return keys.Contains(key);
    }

    public void Delete(string key)
    {
        if (ContainsKey(key))
        {
            ES3.DeleteKey(key);
            keys.Remove(key);
        }
    }
    #endregion
}
