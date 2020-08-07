using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGeneration : MonoBehaviour
{
    public GameObject redCell;
    public GameObject blueCell;
    public GameObject greenCell;

    public int chunks = 100;
    public int spawnProbability = 25;
    public int boundary = 20;

    public int redRatio = 25;
    public int blueRatio = 25;
    public int greenRatio = 25;

    private Camera mainCamera = null;

    private void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();
    }

    void Start()
    {
        for(int x = boundary; x < Screen.width - boundary; x += Screen.width/chunks)
        {
            for(int y = boundary; y < Screen.height - boundary; y += Screen.height/chunks)
            {
                //Random probability to spawn or not spawn
                if(Random.Range(0f,100f) < (float)spawnProbability)
                {
                    Vector3 spawnPosition = mainCamera.ScreenToWorldPoint(new Vector3(x,y,0));
                    spawnPosition.z = 0f;

                    GameObject toSpawn = null;
                    float type = Random.Range(0f, (redRatio+blueRatio+greenRatio));
                    if (type <= redRatio) toSpawn = redCell;
                    else if (type <= (redRatio+blueRatio)) toSpawn = blueCell;
                    else toSpawn = greenCell;

                    Instantiate(toSpawn, spawnPosition, Quaternion.identity);
                }
            }
        }

        //Debug.Log("Spawned all cells");
    }
}
