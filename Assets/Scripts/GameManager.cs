using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
    public string playerName;
    public static GameManager instance;
    private InputField input;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }


        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }



    void Update()
    {

    }

    public void RestartLevel(float delay)
    {
        StartCoroutine(RestartLevelDelay(delay));
    }

    private IEnumerator RestartLevelDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Level 1");
    }

    public List<PlayerTimeEntry> LoadPreviousTimes()
    {
        //use try catch to attempt to load saved time entries with a combontion of the players name and the app persistent data path function
        try
        {
            var scoreFile = Application.persistentDataPath + "/" + playerName + "_times.dat";
            using (var stream = File.Open(scoreFile, FileMode.Open))
            {
                var bin = new BinaryFormatter();
                var times = (List<PlayerTimeEntry>)bin.Deserialize(stream);
                return times;
            }
        }
        //if the try fails the catch will find the error in deserialization and display the error message 
        catch (IOException ex)
        {
            Debug.Log("could not load previous time for: " + playerName + ".Exception: " + ex.Message);
            return new List<PlayerTimeEntry>();
        }

    }

    public void SaveTime(decimal time)
    {
        //when savinf times load the previous time first
        var times = LoadPreviousTimes();
        //create a new p-layer time object 
        var newTime = new PlayerTimeEntry();

        newTime.entryDate = DateTime.Now;
        newTime.time = time;

        //create a binary formatter to serialize the player time data
        var bFormatter = new BinaryFormatter();
        var filePath = Application.persistentDataPath + "/" + playerName + "_times.dat";

        using (var file = File.Open(filePath, FileMode.Create))
        {
            times.Add(newTime);
            bFormatter.Serialize(file, times);
        }
    }

    public void DisplayPreviousTimes()
    {
        // collects existing times
        var times = LoadPreviousTimes();
        var topThree = times.OrderBy(time => time.time).Take(3);

        // find previous times
        var timesLabel = GameObject.Find("Text")
        .GetComponent<Text>();

        // changes to show each time found
        timesLabel.text = "BEST TIMES \n";
        foreach (var time in topThree)
        {
            timesLabel.text += time.entryDate.ToShortDateString() + ": " + time.time + "\n";
        }

    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadsceneMode)
    {
        if (scene.name == "Level 1")
        {
            DisplayPreviousTimes();
        }
    }
}
