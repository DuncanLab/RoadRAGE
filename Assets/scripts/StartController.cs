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

    public bool isGameLoaded;

    private void Awake()
    {
        //DontDestroyOnLoad(transform.gameObject);
    }

    // Use this for initialization
    void Start()
    {
        isGameLoaded = false;
        configPath = Application.dataPath.ToString() + "/StreamingAssets/config/RoadRage.json";
        Debug.Log(configPath);

        inputTextPlaceHold.text = configPath;
    }


    // Update is called once per frame
    void Update()
    {
        bool spacePressed = CrossPlatformInputManager.GetButtonDown("Jump");
        if (!isGameLoaded && spacePressed)
        {
            LoadMainGame();
        }
    }

    public void ChangePath()

    {
        // Here we're using: https://github.com/gkngkc/UnityStandaloneFileBrowser because it was easier than rolling our own
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Choose configuration file", "./StreamingAssets/Config", "json", false);
        if (paths.Length > 0)
        {
            configPath = paths[0];
        }
        inputTextPlaceHold.text = configPath;
    }

    public void LoadMainGame()
    {
        // Load in the data from config file
        var file = System.IO.File.ReadAllText(configPath);

        var temp = JsonUtility.FromJson<GameData>(file);
        Toolbox.Instance.data = temp;
        data = Toolbox.Instance.data;
        isGameLoaded = true;
        Debug.Log("Loading main game");

        // Retrieve the first trial to determine whether instructions are required
        var firstBlockIndex = data.BlockOrder[0] - 1;
        var firstBlock = data.BlockList[firstBlockIndex];
        var firstTrialIndex = data.BlockList[firstBlockIndex].TrialOrder[0] - 1;
        var firstTrial = data.TrialList[firstTrialIndex];

        // A file location is specified so we assume it's instructions
        if (firstTrial.FileLocation != null)
        {
            SceneManager.LoadScene("Instructions");
        } else
        {
            SceneManager.LoadScene("Main");
        }

    }
}
