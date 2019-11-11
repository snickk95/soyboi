using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName : MonoBehaviour
{

    private InputField input;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputField>();
        input.onValueChanged.AddListener(savePlayerName);

        var savedName = PlayerPrefs.GetString("PlayerName");
        if (!string.IsNullOrEmpty(savedName)) ;
        {
            input.text = savedName;
            GameManager.instance.playerName = savedName;
        }
    }

    

    private void savePlayerName(string playerName)
    {
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
        GameManager.instance.playerName = playerName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
