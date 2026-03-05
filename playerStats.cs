using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class playerStats : MonoBehaviour
{
    public static playerStats instance;
    [SerializeField] private float playerLevel = 1f;
    [SerializeField] private float playerXp = 0f;
    [SerializeField] private int playerMoney = 99999999;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        playerData data = LoadData();
        if (data != null)
        {
            playerLevel = data.level;
            playerXp = data.xp;
            playerMoney = data.money;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name);
    }


    public float playerLevel_()
    {
        return playerLevel;
    }

    public float playerXp_()
    {
        return playerXp;
    }

    public int playerMoney_()
    {
        return playerMoney;
    }

    public void transaction(int amount)
    {
        playerMoney += amount;
        SaveData();
    }

    void SaveData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + "PlayerData" + ".helix";
        FileStream stream = new FileStream(path, FileMode.Create);
        Debug.Log("Stream Opened!");

        playerData data = new playerData(this);

        formatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("Stream Close!");
    }

    playerData LoadData()
    {
        string path = Application.persistentDataPath + "/" + "PlayerData" + ".helix";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            playerData data = formatter.Deserialize(stream) as playerData;
            stream.Close();
            return data;

        }
        else
        {
            Debug.LogError("Save File not found in " + path);
            return null;
        }
    }
}