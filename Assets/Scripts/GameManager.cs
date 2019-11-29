using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
    public string playerName;
    public static GameManager instance;
    private InputField input;
    public GameObject buttonPrefab;
    private string selectedLevel;

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
        DiscoverLevels();
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }

    private void SetLevelName(string levelFilePath)
    {
        selectedLevel = levelFilePath;
        SceneManager.LoadScene("Game");
    }

    private void DiscoverLevels()
    {
        var levelFiles = Directory.GetFiles(Application.dataPath,"*.json");

        var yOffset = 0f;
        //loop to determain button position
        for (var i = 0; i < levelFiles.Length; i++)
        {
            if (i == 0)
            {
                yOffset = -30f;
            }
            else
            {
                yOffset -= 65f;
            }
            var levelFile = levelFiles[i];
            

            var levelName = Path.GetFileName(levelFile);
            //makes copy of button prefab
            var levelButtonObj = (GameObject)Instantiate(buttonPrefab, Vector2.zero, Quaternion.identity);
            //makes transform a child of buittons prefab
            var levelButtonRectTransform = levelButtonObj.GetComponent<RectTransform>();

            levelButtonRectTransform.SetParent(GameObject.Find("LevelItemsPanel").GetComponent<RectTransform>(), true);
            //poisitions the button
            levelButtonRectTransform.anchoredPosition = new Vector2(212.5f, yOffset);
            //sets the buttons text to the levels name
            var levelButtonText = levelButtonObj.transform.GetChild(0).GetComponent<Text>();

            levelButtonText.text = levelName;

            var levelButton = levelButtonObj.GetComponent<Button>();
            levelButton.onClick.AddListener(
            delegate { SetLevelName(levelFile); });
            GameObject.Find("LevelItemsPanel").GetComponent<RectTransform>().sizeDelta =
            new Vector2(GameObject.Find("LevelItemsPanel").GetComponent<RectTransform>().sizeDelta.x, 60f * i);
        }
        GameObject.Find("LevelItemsPanel").GetComponent<RectTransform>().offsetMax = new Vector2(GameObject.Find("LevelItemsPanel").GetComponent<RectTransform>().offsetMax.x, 0f);
    }


    private void LoadLevelContent()
    {
        var existingLevelRoot = GameObject.Find("Level");
        Destroy(existingLevelRoot);
        var levelRoot = new GameObject("Level");

        // Reads the JSON file content of the selected level — selectedLevel is the path where
        //the level resides after the player clicks the corresponding button.
        var levelFileJsonContent = File.ReadAllText(selectedLevel);
        var levelData = JsonUtility.FromJson<LevelDataRepresentation>(levelFileJsonContent);

        // Makes levelData.levelItems into a fully populated array of
        //LevelItemRepresentation instances.
        foreach (var li in levelData.levelItems)
        {
            // looped through in the array, the script locates correct prefab
            //and loads it from the prefabs
            var pieceResource = Resources.Load("Prefab/" + li.prefabName);
            if (pieceResource == null)
            {
                Debug.LogError("Cannot find resource: " + li.prefabName);
            }
            // Instantiates a clone of this prefab.
            var piece = (GameObject)Instantiate(pieceResource,li.position, Quaternion.identity);
            var pieceSprite = piece.GetComponent<SpriteRenderer>();
            if (pieceSprite != null)
            {
                pieceSprite.sortingOrder = li.spriteOrder;
                pieceSprite.sortingLayerName = li.spriteLayer;
                pieceSprite.color = li.spriteColor;
            }
            // Makes the object a child of the Level GameObject then sets and its position
            piece.transform.parent = levelRoot.transform;
            piece.transform.position = li.position;
            piece.transform.rotation = Quaternion.Euler(li.rotation.x, li.rotation.y, li.rotation.z);
            piece.transform.localScale = li.scale;
        }
        //locates SoyBoy and places him at the playerStartPosition location saved in the JSON file
        var SoyBoi = GameObject.Find("SoyBoi");
        SoyBoi.transform.position = levelData.playerStartPosition;
        Camera.main.transform.position = new Vector3(SoyBoi.transform.position.x, SoyBoi.transform.position.y,Camera.main.transform.position.z);
        
        //locates the camera lerp to transform script
        var camSettings = FindObjectOfType<CameraLerpToTransform>();

        //Checks that the Smooth Follow script was found, and if so, it populates settings
        if (camSettings != null)
        {
            camSettings.cameraZDepth = levelData.cameraSettings.cameraZDepth;
            camSettings.camTarget = GameObject.Find(levelData.cameraSettings.cameraTrackTarget).transform;
            camSettings.maxX = levelData.cameraSettings.maxX;
            camSettings.maxY = levelData.cameraSettings.maxY;
            camSettings.minX = levelData.cameraSettings.minX;
            camSettings.minY = levelData.cameraSettings.minY;
            camSettings.trackingSpeed = levelData.cameraSettings.trackingSpeed;
        }

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }
    }

    public void RestartLevel(float delay)
    {
        StartCoroutine(RestartLevelDelay(delay));
    }

    private IEnumerator RestartLevelDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Game");
    }

    public List<PlayerTimeEntry> LoadPreviousTimes()
    {
        //use try catch to attempt to load saved time entries with a combontion of the players name and the app persistent data path function
        try
        {
            var levelName = Path.GetFileName(selectedLevel);
            var scoreFile = Application.persistentDataPath + "/" + playerName + "_" + levelName + "_times.dat";
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
        var levelName = Path.GetFileName(selectedLevel);
        var filePath = Application.persistentDataPath +  "/" + playerName + "_" + levelName + "_times.dat";

        using (var file = File.Open(filePath, FileMode.Create))
        {
            times.Add(newTime);
            bFormatter.Serialize(file, times);
        }
    }

    public void DisplayPreviousTimes()
    {
        var times = LoadPreviousTimes();
        var levelName = Path.GetFileName(selectedLevel);
        if (levelName != null)
        {
            levelName = levelName.Replace(".json", "");
        }
        var topThree = times.OrderBy(time => time.time).Take(3);
        var timesLabel = GameObject.Find("PreviousTimes")
        .GetComponent<Text>();
        timesLabel.text = levelName + "\n";
        timesLabel.text += "BEST TIMES \n";
        foreach (var time in topThree)
        {
            timesLabel.text += time.entryDate.ToShortDateString()
            + ": " + time.time + "\n";
        }

    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadsceneMode)
    {
        if (!string.IsNullOrEmpty(selectedLevel)&& scene.name == "Game")
        {
            Debug.Log("Loading level content for: " + selectedLevel);
            LoadLevelContent();
            DisplayPreviousTimes();
        }
        if (scene.name == "Menu")
        {
            DiscoverLevels();
        }
    }
}
