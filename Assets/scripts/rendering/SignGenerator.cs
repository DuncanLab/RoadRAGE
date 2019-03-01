using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SignGenerator : MonoBehaviour
{

    public GameData data;

    public GameObject RightSignPanel;
    public GameObject LeftSignPanel;

    // Use this for initialization
    void Start()
    {
        // Carry over data.
        data = Toolbox.Instance.data;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < data.currTrial.Events.Count; i++)
        {
            GameData.Event currEvent = data.currTrial.Events[i];
            if (currEvent.EventType.ToLower().Equals("sign"))
            {
                // Check if time is matching time, if so,
                // pop the UI graphic.
                if (data.currTrial.Timer.ElapsedMilliseconds >= currEvent.SpawnTime && data.currTrial.Timer.ElapsedMilliseconds <= currEvent.DespawnTime)
                {
                    if (currEvent.SpawnSide.ToLower().Equals("r"))
                    {
                        var filedata = File.ReadAllBytes(Application.dataPath + "/StreamingAssets/config/images/Sign1.png");
                        Texture2D tex = new Texture2D(1, 1);
                        tex.LoadImage(filedata);

                        RawImage RightImage = (RawImage)RightSignPanel.GetComponent("RawImage");
                        RightImage.texture = tex;
                        RightImage.enabled = true;
                    }
                    // left side
                    else
                    {
                        var filedata = File.ReadAllBytes(Application.dataPath + "/StreamingAssets/config/images/Sign2.png");
                        Texture2D tex = new Texture2D(1, 1);
                        tex.LoadImage(filedata);

                        RawImage RightImage = (RawImage)LeftSignPanel.GetComponent("RawImage");
                        RightImage.texture = tex;
                        RightImage.enabled = true;
                    }
                }

                else
                {
                    if (currEvent.SpawnSide.ToLower().Equals("r"))
                    {
                        RawImage RightImage = (RawImage)RightSignPanel.GetComponent("RawImage");
                        RightImage.enabled = false;
                    }
                    else
                    {
                        RawImage LeftImage = (RawImage)LeftSignPanel.GetComponent("RawImage");
                        LeftImage.enabled = false;
                    }
                }

            }

        }
    }
}
