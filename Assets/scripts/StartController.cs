using SFB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class StartController : MonoBehaviour
{

    public Text inputTextPlaceHold;
    public string configPath;

    public GameData data;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Use this for initialization
    void Start()
    {
        configPath = Application.dataPath.ToString() + "/config/RoadRage.json";
        Debug.Log(configPath);

        inputTextPlaceHold.text = configPath;
    }


    // Update is called once per frame
    void Update()
    {
        float h = CrossPlatformInputManager.GetAxis("Jump");
        if (h > 0)
        {
            LoadMainGame();
        }
    }

    public void ChangePath()

    {
        // Here we're using: https://github.com/gkngkc/UnityStandaloneFileBrowser because it was easier than rolling our own
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Choose configuration file", "./../config", "json", false);
        configPath = paths[0];
        inputTextPlaceHold.text = configPath;
    }

    public void LoadMainGame()
    {
        // Load in the data from config file
        var file = System.IO.File.ReadAllText(configPath);

        var temp = JsonUtility.FromJson<GameData>(file);
        data = temp;
        SceneManager.LoadScene("Main");
    }
}
