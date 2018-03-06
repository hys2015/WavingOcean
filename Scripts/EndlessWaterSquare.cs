using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

public class EndlessWaterSquare : MonoBehaviour {

    public GameObject boatObj;

    public GameObject waterSqrObj;

    public float squareWidth = 800f;
    public float innerSquareResolution = 100f;
    public float outerSquareResolution = 200f;

    List<WaterSquare> waterSquares = new List<WaterSquare>();

    float secondsSinceStart;

    Vector3 boatPos;
    Vector3 oceanPos;

    bool hasThreadUpdatedWater;

	// Use this for initialization
	void Start () {
        CreateEndlessSea();

        secondsSinceStart = Time.time;

        ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWaterWithThreadPooling));

        StartCoroutine(UpdatedWater());
	}

    // Update is called once per frame
    void Update()
    {
        secondsSinceStart = Time.time;

        boatPos = boatObj.transform.position;
    }

    //Update the water with no thread to compare
    void UpdateWaterNoThread()
    {
        boatPos = boatObj.transform.position;

        MoveWaterToBoat();

        transform.position = oceanPos;

        for(int i = 0; i < waterSquares.Count; ++i)
        {
            waterSquares[i].MoveSea(oceanPos, Time.time);
        }
    }

    private IEnumerator UpdatedWater()
    {
        while(true)
        {
            if(hasThreadUpdatedWater)
            {
                transform.position = oceanPos;

                for(int i = 0; i <waterSquares.Count; i++)
                {
                    waterSquares[i].terrainMeshFilter.mesh.vertices = waterSquares[i].vertices;
                    waterSquares[i].terrainMeshFilter.mesh.RecalculateNormals();
                }

                hasThreadUpdatedWater = false;
                ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateWaterWithThreadPooling));
            }

            yield return new WaitForSeconds(Time.deltaTime * 3f);
        }
    }

    private void UpdateWaterWithThreadPooling(object state)
    {
        //print("Does Coroutine Loop?");
        MoveWaterToBoat();

        for(int i = 0; i < waterSquares.Count; i++)
        {
            Vector3 centerPos = waterSquares[i].centerPos;
            Vector3[] vertices = waterSquares[i].vertices;

            for(int j = 0; j < vertices.Length; j++)
            {
                Vector3 vertexPos = vertices[j];

                Vector3 vertexPosGlobal = vertexPos + centerPos + oceanPos;

                vertexPos.y = WaterController.current.GetWaveYPos(vertexPosGlobal, secondsSinceStart);

                vertices[j] = vertexPos;
            }
        }

        hasThreadUpdatedWater = true;
    }

    private void MoveWaterToBoat()
    {
        float x = innerSquareResolution * (int)Mathf.Round(boatPos.x / innerSquareResolution);
        float z = innerSquareResolution * (int)Mathf.Round(boatPos.z / innerSquareResolution);

        //print(x.ToString() + " " + z.ToString() + " "+ oceanPos.ToString());

        if(oceanPos.x != x || oceanPos.z != z)
        {
            oceanPos = new Vector3(x, oceanPos.y, z);
        }
    }

    private void CreateEndlessSea()
    {
        AddWaterPlane(0f, 0f, 0f, squareWidth, innerSquareResolution);

        for(int x = -1; x <= 1; x += 1)
        {
            for(int z = -1; z <= 1; z += 1)
            {
                if(x == 0 && z == 0)
                {
                    continue;
                }

                float yPos = -0.1f;
                AddWaterPlane(x * squareWidth, z * squareWidth, yPos, squareWidth, outerSquareResolution);
            }
        }
    }

    private void AddWaterPlane(float xCoord, float zCoord, float yPos, float squareWidth, float spacing)
    {
        GameObject waterPlane = Instantiate(waterSqrObj, transform.position, transform.rotation) as GameObject;
        waterPlane.SetActive(true);

        Vector3 centerPos = transform.position;

        centerPos.x += xCoord;
        centerPos.y = yPos;
        centerPos.z += zCoord;

        waterPlane.transform.position = centerPos;

        waterPlane.transform.parent = transform;

        WaterSquare newWaterSquare = new WaterSquare(waterPlane, squareWidth, spacing);

        waterSquares.Add(newWaterSquare);
    }
}
