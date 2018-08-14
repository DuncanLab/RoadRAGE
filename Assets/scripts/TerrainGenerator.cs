using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public enum LaneSide { left, right };

    // Use this for initialization.
    void Start()
    {
        Debug.Log("terrain gen start");
        // Carry over data.
        data = Toolbox.Instance.data;

        // Simulation starts with only one chunk.
        GameObject RoadPrefab = Resources.Load<GameObject>("prefabs/" + data.currTrial.Roads[0].PrefabName);
        currTerrainChunk = Instantiate(RoadPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //currTerrainChunk = GameObject.Find("road1Example");
        prevTerrainChunk = null;
        _createdGameObjects.Add(prevTerrainChunk);
        CurrentRoadPrefab = 1;

        // pre-load all the lanes here
        //LoadLanes();
    }

    // Update is called once per frame.
    void Update()
    {
        Terrain tr = currTerrainChunk.GetComponentInChildren<Terrain>();
        Vector3 terrainSize = tr.terrainData.size;
        var relativePos = transform.position.z - currTerrainChunk.transform.position.z;
        float terrainPercentageForNewChunk = terrainSize.z / 4;

        // Safety incase the user sets the total time allotted for longer
        // than the amount of road prefabs they've specified, we artificially
        // set the TimeAllotted to 1sec so the trial will end.
        if (CurrentRoadPrefab == data.currTrial.Roads.Count)
        {
            if (relativePos > terrainSize.z - 20)
            {
                Debug.LogError("Time allotted is too short for the amount of road prefabs specified in the config!");
                data.currTrial.TimeAllotted = 1000;
            }
        }


        // We have left the previous terrain chunk, never to return - so destroy all of it.
        //if (relativePos > terrainPercentageForNewChunk)
        //{
        //    Destroy(prevTerrainChunk);
        //    prevTerrainChunk = currTerrainChunk;
        //}

        // Generate the required chunks in the current prefab spec.
        if (CurrentRoadPrefab < data.currTrial.Roads.Count && relativePos > terrainPercentageForNewChunk)
        {
            GameObject RoadPrefab = (GameObject)Resources.Load("prefabs/" + data.currTrial.Roads[CurrentRoadPrefab].PrefabName);

            // Here we want to instantiate enough chunks to last the time specified in the config
            //var ChunksRequired = CalculateNumberOfChunksRequired(data.currTrial.Roads[CurrentRoadPrefab].TimeToExist, data.GlobalData.MovementSpeed, (int) terrainSize.z);
            //for (int i = 0; i < ChunksRequired; i++)
            //{
            float xOffset = DetermineXAxisOffset();
            prevTerrainChunk = currTerrainChunk;
            currTerrainChunk = Instantiate(RoadPrefab, new Vector3(xOffset, 0, currTerrainChunk.transform.position.z + terrainSize.z), Quaternion.identity);
            _createdGameObjects.Add(currTerrainChunk);
            //}

            CurrentRoadPrefab++;
        }
    }

    private float DetermineXAxisOffset()
    {
        if (transform.position.x - currTerrainChunk.transform.position.x < 230)
        {
            return currTerrainChunk.transform.position.x - 189.4f;
        }
        else if (transform.position.x - currTerrainChunk.transform.position.x > 270)
        {
            return currTerrainChunk.transform.position.x + 194.5f;
        }
        else
        {
            return currTerrainChunk.transform.position.x;
        }
    }

    // TODO : refactor.
    private void LoadLanes()
    {
        float ChunkCalculationTime;

        for (int i = 0; i < data.currTrial.Events.Count; i++)
        {
            GameData.Event CurrEvent = data.currTrial.Events[i];
            if (CurrEvent.EventType.ToLower().Equals("lane"))
            {
                if (CurrEvent.DespawnTime == 0f)
                {
                    ChunkCalculationTime = data.currTrial.TimeAllotted;
                }
                else
                {
                    ChunkCalculationTime = CurrEvent.DespawnTime - CurrEvent.SpawnTime;
                }


                if (CurrEvent.SpawnSide.ToLower().Equals("r"))
                {
                    var ChunksRequired = CalculateNumberOfChunksRequired(ChunkCalculationTime, data.GlobalData.MovementSpeed, 100);
                    for (int j = 0; j <= ChunksRequired; j++)
                    {
                        float SpawnDistance = CalculateLaneSpawnDistance(CurrEvent.SpawnTime, data.GlobalData.MovementSpeed);
                        GameObject LaneInstance = Instantiate(Resources.Load<GameObject>("prefabs/LaneRight_100m_WP"), new Vector3(RIGHT_LANE_POS_X, RIGHT_LANE_POS_Y, SpawnDistance + (100 * j)), Quaternion.Euler(0, 180, 0));
                        _createdGameObjects.Add(LaneInstance);
                    }
                }
                else
                {
                    var ChunksRequired = CalculateNumberOfChunksRequired(ChunkCalculationTime, data.GlobalData.MovementSpeed, 100);
                    for (int j = 0; j <= ChunksRequired; j++)
                    {
                        float SpawnDistance = CalculateLaneSpawnDistance(CurrEvent.SpawnTime, data.GlobalData.MovementSpeed);
                        GameObject LaneInstance = Instantiate(Resources.Load<GameObject>("prefabs/LaneLeft_100m_WP"), new Vector3(LEFT_LANE_POS_X, LEFT_LANE_POS_Y, SpawnDistance + (100 * j)), Quaternion.identity);
                        _createdGameObjects.Add(LaneInstance);
                    }
                }
            }


        }
    }

    private int CalculateNumberOfChunksRequired(float MillesecondsOfChunkExistance, float MovementSpeed, int PrefabRoadLength)
    {
        float SecondsOfExistence = MillesecondsOfChunkExistance / 1000;

        float MetersOfRoadRequired = MovementSpeed * SecondsOfExistence;

        int ChunksRequired = (int)Math.Ceiling(MetersOfRoadRequired / PrefabRoadLength);

        // We are on the first road of the trial, so we need one less chunk (it will be loaded on start).
        if (CurrentRoadPrefab == 0)
        {
            ChunksRequired--;
        }

        return ChunksRequired;
    }

    private float CalculateLaneSpawnDistance(float MillesecondsToWaitForSpawn, float MovementSpeed)
    {
        float SecondsToWait = MillesecondsToWaitForSpawn / 1000;

        // We add 50 here because lane origin is dead center.
        return (MovementSpeed * SecondsToWait) + 50;
    }
}
