using System.Collections.Generic;
using UnityEngine;

// This class controls the basic "infinite scroll" effect that is 
// is used for the simulation.
public class TerrainGenerator : MonoBehaviour {

    const float RIGHT_LANE_POS_X = 256.4f;
    const float RIGHT_LANE_POS_Y = 0.003f;
    const float RIGHT_LANE_POS_Z = 375f;

    const float LEFT_LANE_POS_X = 243.6f;
    const float LEFT_LANE_POS_Y = 0.003f;
    const float LEFT_LANE_POS_Z = 375f;

    const float BILLBOARD_RIGHT_POS_X = 261.56f;
    const float BILLBOARD_RIGHT_POS_Y = 0.0f;
    const float BILLBOARD_RIGHT_POS_Z = 218.87f;


    // This is the standard (prefab) chunk we want to load to simulate
    // an infinite roadway.
    public GameObject road1;

    // Prefabs for loading
    public GameObject billboard;
    public GameObject LaneRight;
    public GameObject LaneLeft;

    // Reference pointers
    public GameObject currTerrain;
    public GameObject currTerrainChunk;
    public GameObject prevTerrainChunk;

    public List<GameObject> _createdGameObjects;
   
	// Use this for initialization.
	void Start () {
        // Simulation starts with only one chunk.
        currTerrainChunk = GameObject.Find("road1Example");
        prevTerrainChunk = currTerrainChunk;

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
       // _createdGameObjects.Add(prevTerrainChunk);
    }
	
	// Update is called once per frame.
	void Update () {
        var relativePos = transform.position.z - currTerrainChunk.transform.position.z;

        // We have left the previous terrain chunk, never to return - so destroy all of it.
        if (transform.position.z - prevTerrainChunk.transform.position.z > 1005)
        {
            DestroyPreviousChunk();
            prevTerrainChunk = currTerrainChunk;
        }

        // Generate the next terrain chunk if the car is close enough.
        if (relativePos > 10)
        {
            _createdGameObjects.Add(prevTerrainChunk);
            currTerrainChunk = Instantiate(road1, new Vector3(0, 0, currTerrainChunk.transform.position.z + 1000f), Quaternion.identity);   

            // TODO : replace this with serial lane/billboard loading
            List<string> _objectsToLoad = new List<string>
        {
            "billboard",
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
                _createdGameObjects.Add(Instantiate(LaneRight, new Vector3(RIGHT_LANE_POS_X, RIGHT_LANE_POS_Y, RIGHT_LANE_POS_Z + currTerrainChunk.transform.position.z), Quaternion.Euler(new Vector3(0f, 180f, 0f))));
            }
            else if ("LaneLeft".Equals(objectsToCreate[i]))
            {
                _createdGameObjects.Add(Instantiate(LaneLeft, new Vector3(LEFT_LANE_POS_X, LEFT_LANE_POS_Y, LEFT_LANE_POS_Z + currTerrainChunk.transform.position.z), Quaternion.identity));
            }
        }
    }
}
