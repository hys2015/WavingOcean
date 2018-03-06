using UnityEngine;
using System.Collections;

public class WaterController : MonoBehaviour
{
    public static WaterController current;
    public bool isMoving;

    public float scale = 0.1f;
    public float speed = 1.0f;

    public float waveDistance = 1f;

    public float noiseStrength = 1f;
    public float noiseWalk = 1f;

	// Use this for initialization
	void Start () {
        //print("Start WaterContoller");
        current = this;
	}

    public float GetWaveYPos(Vector3 position, float timeSinceStart)
    {
        return WaveTypes.SinXWave(position, speed, scale, waveDistance, noiseStrength, noiseWalk, timeSinceStart);
    }
	
    public float DistanceToWater(Vector3 position, float timeSinceStart)
    {
        float waterHeight = GetWaveYPos(position, timeSinceStart);
        return position.y - waterHeight;
    }

    void Update()
    {
        UpdateTiles(Time.time);
    }

    public void UpdateTiles(float timeSinceStart)
    {
        foreach (Transform wt in transform)
        {
            MeshFilter component = wt.GetComponent<MeshFilter>();
            if (!component) { continue; }
            Mesh mesh = component.mesh;
            
            Vector3[] vertices = mesh.vertices;
            for( int i = 0; i < vertices.Length; ++i)
            {
                Vector3 vertex = wt.TransformPoint(vertices[i]);
                vertex.y = GetWaveYPos(vertex, Time.time);
                vertices[i] = wt.InverseTransformPoint(vertex);
            }
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
        }
    }
}

