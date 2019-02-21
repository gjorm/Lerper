using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class GameSaver : MonoBehaviour {
    public static GameSaver gSaver;

    public int charIndexActive;
    public bool[] charBought;
    public int[] charLevel;
    public int numCoins;
    public bool firstTimePlaying;
    public float scrollViewPosition;
    public DateTime lastCoinDay;

    [System.NonSerialized]
    public bool fileReadSuccess;

    private string filePath;

	// Use this for initialization
	void Awake () {
        // fuck it, i cant get this script to call PlayerController.pScript in anything except Start and by then its useless
        fileReadSuccess = false;
        firstTimePlaying = true;
        charBought = new bool[58];
        charLevel = new int[58];
        charBought[0] = true;
        
        //default all char levels to 1 before reading file
        for(int i = 0; i < 58; i++)
        {
            charLevel[i] = 1;
        }

        if (gSaver == null)
        {
            DontDestroyOnLoad(gameObject);
            gSaver = this;

            fileReadSuccess = false;

            filePath = "" + Application.persistentDataPath + "/SavedGameData.boop";

            charIndexActive = 0;
            numCoins = 0;
            firstTimePlaying = true;
            scrollViewPosition = 0f;
            lastCoinDay = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc).Date;
        }
        else if(gSaver != this)
        {
            Destroy(gameObject);
        }

        // read file data
        if (File.Exists(filePath))
        {
            fileReadSuccess = true;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file;
            PlayerData data;

            try
            {
                file = File.OpenRead(filePath);
            }
            catch (IOException ex)
            {
                Debug.Log("Lerper Couldnt Open saved game data for reading: " + ex.Message);
                fileReadSuccess = false;
                file = null;
            }

            if (file != null)
            {
                Debug.Log("Saved Game Data File Exists.");
                data = (PlayerData)bf.Deserialize(file);

                charIndexActive = data.charIndexActive;
                charBought[0] = true;
                for (int i = 1; i < data.charBought.Length; i++)
                    charBought[i] = data.charBought[i];
                for (int i = 0; i < data.charLevel.Length; i++)
                    charLevel[i] = data.charLevel[i];

                numCoins = data.numCoins;
                firstTimePlaying = data.firstTimePlaying;
                scrollViewPosition = data.scrollViewPosition;
                lastCoinDay = data.lastCoinDay;

            }
            else //if the file read was unsuccessful
            {
                fileReadSuccess = false;
                charIndexActive = 0;
                charBought[0] = true; // first character is cactus and is always bought
                for (int i = 1; i < charBought.Length; i++)
                    charBought[i] = false;
                for (int i = 0; i < charLevel.Length; i++)
                    charLevel[i] = 1;
                numCoins = 0;
                firstTimePlaying = true; // if there is no file (aka first game instantiation) or file couldnt be read, default to true
                scrollViewPosition = 0f;
                lastCoinDay = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc).Date;
            }
        }
        else // the file did not exist
        {
            Debug.Log("Saved Game Data file does not exist. Running with defaults.");
            fileReadSuccess = true;
            charIndexActive = 0;
            charBought[0] = true; // first character is cactus and is always bought
            for (int i = 1; i < charBought.Length; i++)
                charBought[i] = false;
            for (int i = 0; i < charLevel.Length; i++)
                charLevel[i] = 1;
            numCoins = 0;
            firstTimePlaying = true; // if there is no file (aka first game instantiation) or file couldnt be read, default to true
            scrollViewPosition = 0f;
            lastCoinDay = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc).Date;
        }

        if (fileReadSuccess == true)
            Debug.Log("Saved Game Data successfully read.");
        else
            Debug.Log("Issues reading saved game data.");
    }

    // Have to use OnApplicationPause for mobile!!!
    public void OnApplicationPause()
    {
        SaveData();
    }

    public void OnApplicationQuit()
    {
        SaveData();
    }

    public void SaveData()
    {
        Debug.Log("Attempting to save game data.");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        PlayerData data = new PlayerData();

        if (!File.Exists(filePath))
        {
            try
            {
                file = File.Create(filePath);
            }
            catch (IOException ex)
            {
                Debug.Log("Couldnt Create Saved Game Data File for writing:" + ex.Message);
                file = null;
            }
        }
        else
        {
            try
            {
                file = File.OpenWrite(filePath);
            }
            catch (IOException ex)
            {
                Debug.Log("Couldnt Open existing Saved Game Data file for writing:" + ex.Message);
                file = null;
            }
        }

        if (file != null)
        {
            data.charIndexActive = charIndexActive;
            data.charBought = new bool[charBought.Length];
            for (int i = 0; i < charBought.Length; i++)
                data.charBought[i] = charBought[i];
            for (int i = 0; i < charLevel.Length; i++)
                data.charLevel[i] = charLevel[i];

            data.numCoins = numCoins;
            data.firstTimePlaying = false;
            data.scrollViewPosition = scrollViewPosition;
            data.lastCoinDay = DateTime.Now.Date;

            bf.Serialize(file, data);
            file.Close();

            Debug.Log("Saved File Data written.");
        }
    }
}

[System.Serializable]
class PlayerData : System.Object
{
    public int charIndexActive;
    public bool[] charBought;
    public int[] charLevel;
    public int numCoins;
    public bool firstTimePlaying;
    public float scrollViewPosition;
    public DateTime lastCoinDay;

    public PlayerData()
    {
        charIndexActive = 0;
        charBought = new bool[58];
        charLevel = new int[58];
        numCoins = 0;
        firstTimePlaying = true;
        scrollViewPosition = 0.0f;
        lastCoinDay = DateTime.Now.Date;

        for(int i = 0; i < 58; i++)
        {
            charBought[i] = false;
            charLevel[i] = 1;
        }
        charBought[0] = true;
    }
}
