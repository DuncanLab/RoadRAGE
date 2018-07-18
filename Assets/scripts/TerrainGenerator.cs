using System;
using System.Collections.Generic;
using UnityEngine;

// This class controls the basic "infinite scroll" effect that is 
// is used for the simulation.
public class TerrainGenerator : MonoBehaviour
{

    const float RIGHT_LANE_POS_X = 254.2f;
    const float RIGHT_LANE_POS_Y = 0.006f;
    const float RIGHT_LANE_POS_Z = 50f;

    const float LEFT_LANE_POS_X = 245.8f;
    const float LEFT_LANE_POS_Y = 0.006f;
    const float LEFT_LANE_POS_Z = 50f;

    const float BILLBOARD_RIGHT_POS_X = 261.56f;
    const float BILLBOARD_RIGHT_POS_Y = 0.0f;
    const float BILLBOARD_RIGHT_POS_Z = 218.87f;


    // This is the standard (prefab) chunk we want to load to simulate
    // an infinite roadway.
    public GameObject road_normal_100m;

    // Prefabs for loading
    public GameObject billboard;
    public GameObject LaneRight_100m;
    public GameObject LaneLeft_100m;

    // Reference pointers
    public GameObject currTerrain;
    public GameObject currTerrainChunk;
    public GameObject prevTerrainChunk;

    public int CurrentRoadPrefab;

    public List<GameObject> _createdGameObjects;

    public GameData data;

    // Use this for initialization.
    void Awake()
    {
        // Carry over data.
        StartController startController = (StartController)GameObject.Find("StartScriptHolder").GetComponent("StartController");
        data = startController.data;

        // Simulation starts with only one chunk.
        GameObject RoadPrefab = Resources.Load<GameObject>("prefabs/" + data.currTrial.Roads[0].PrefabName);
        currTerrainChunk = Instantiate(RoadPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //currTerrainChunk = GameObject.Find("road1Example");
        prevTerrainChunk = currTerrainChunk;
        CurrentRoadPrefab = 0;

        // Setup objects to load for the next chunk, only load the 
        // items that should be in the first/second chunk to begin with
        // TODO : read this in from a config file
        var billboard1 = "billboard";
        var LaneRight1 = "LaneRight";
        var LaneLeft1 = "LaneLeft";

        List<string> _objectsToLoad = new List<string>
        {
            billboard1,
            LaneRight1,
            LaneLeft1
        };

        CreatePrefabsInChunk(_objectsToLoad);
        _createdGameObjects.Add(prevTerrainChunk);
    }

    // Update is called once per frame.
    void Update()
    {
        var relativePos = transform.position.z - currTerrainChunk.transform.position.z;

        // We have left the previous terrain chunk, never to return - so destroy all of it.
        if (relativePos > 105)
        {
            DestroyPreviousChunk();
            prevTerrainChunk = currTerrainChunk;
        }

        // Generate the required chunks in the current prefab spec.
        if (CurrentRoadPrefab < data.currTrial.Roads.Count)
        {
            GameObject RoadPrefab = (GameObject)Resources.Load("prefabs/" + data.currTrial.Roads[0].PrefabName);

            // Here we want to instantiate enough chunks to last the time specified in the config
            var ChunksRequired = CalculateNumberOfChunksRequired(data.currTrial.Roads[0].TimeToExist, data.GlobalData.MovementSpeed);
            for (int i = 0; i < ChunksRequired; i++)
            {

                currTerrainChunk = Instantiate(RoadPrefab, new Vector3(0, 0, currTerrainChunk.transform.position.z + 100f), Quaternion.identity);
                _createdGameObjects.Add(currTerrainChunk);
            }

            CurrentRoadPrefab++;
            // TODO : replace this with serial lane/billboard loading
            List<string> _objectsToLoad = new List<string>
        {
            "LaneRight",
            "LaneLeft",
        };
            CreatePrefabsInChunk(_objectsToLoad);

        }
    }

    private void DestroyPreviousChunk()
    {
        for (int i = 0; i < _createdGameObjects.Count; i++)
        {
            Destroy(_createdGameObjects[i]);
        }
    }

    private void CreatePrefabsInChunk(List<string> objectsToCreate)
    {
        for (int i = 0; i < objectsToCreate.Count; i++)
        {
            if ("billboard".Equals(objectsToCreate[i]))
            {
                _createdGameObjects.Add(Instantiate(billboard, new Vector3(BILLBOARD_RIGHT_POS_X, BILLBOARD_RIGHT_POS_Y, BILLBOARD_RIGHT_POS_Z + currTerrainChunk.transform.position.z), Quaternion.Euler(new Vector3(0f, 0f, 90f))));
            }
            else if ("LaneRight".Equals(objectsToCreate[i]))
            {
                _createdGameObjects.Add(Instantiate(LaneRight_100m, new Vector3(RIGHT_LANE_POS_X, RIGHT_LANE_POS_Y, RIGHT_LANE_POS_Z + currTerrainChunk.transform.position.z), Quaternion.Euler(new Vector3(0f, 180f, 0f))));
            }
            else if ("LaneLeft".Equals(objectsToCreate[i]))
            {
                _createdGameObjects.Add(Instantiate(LaneLeft_100m, new Vector3(LEFT_LANE_POS_X, LEFT_LANE_POS_Y, LEFT_LANE_POS_Z + currTerrainChunk.transform.position.z), Quaternion.identity));
            }
        }
    }

    private int CalculateNumberOfChunksRequired(float MillesecondsOfChunkExistance, float MovementSpeed)
    {
        float SecondsOfExistence = MillesecondsOfChunkExistance / 1000;

        float MetersOfRoadRequired = MovementSpeed * SecondsOfExistence;

        int ChunksRequired = (int)Math.Ceiling(MetersOfRoadRequired / 100);

        // We are on the first road of the trial, so we need one less chunk.
        if (CurrentRoadPrefab == 0)
        {
            ChunksRequired--;
        }

        return ChunksRequired;
    }
}
