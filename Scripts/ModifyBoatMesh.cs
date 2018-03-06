using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BoatTutorial
{
    public class ModifyBoatMesh{

        private Transform boatTrans;

        Vector3[] boatVertices;

        int[] boatTriangles;

        public Vector3[] boatVerticesGlobal;

        float[] allDistancesToWater;

        public List<TriangleData> underWaterTriangleData = new List<TriangleData>();

        public ModifyBoatMesh(GameObject boatObj)
        {
            boatTrans = boatObj.transform;

            boatVertices = boatObj.GetComponent<MeshFilter>().mesh.vertices;
            boatTriangles = boatObj.GetComponent<MeshFilter>().mesh.triangles; //bt[i]标记第i个坐标所属的三角形

            boatVerticesGlobal = new Vector3[boatVertices.Length];

            allDistancesToWater = new float[boatVertices.Length];
        }

        public void GenerateUnderWaterMesh()
        {
            underWaterTriangleData.Clear();
            
            for(int i = 0; i < boatVertices.Length; i++)
            {
                Vector3 globalPos = boatTrans.TransformPoint(boatVertices[i]);
                boatVerticesGlobal[i] = globalPos;
                float nowtime = Time.time;
                allDistancesToWater[i] = WaterController.current.DistanceToWater(globalPos, nowtime);
            }
            AddTriangles();
        }

        private void AddTriangles()
        {
            List<VertexData> vertexData = new List<VertexData>();

            vertexData.Add(new VertexData());
            vertexData.Add(new VertexData());
            vertexData.Add(new VertexData());

            int i = 0;
            while(i < boatTriangles.Length)
            {
                for(int x = 0; x < 3; ++x)
                {
                    vertexData[x].distance = allDistancesToWater[boatTriangles[i]];
                    vertexData[x].index = x;
                    vertexData[x].globalVertexPos = boatVerticesGlobal[boatTriangles[i]];
                    i++;
                }

                if(vertexData[0].distance > 0f && vertexData[1].distance > 0f && vertexData[2].distance > 0f)
                {
                    continue;
                }

                if(vertexData[0].distance < 0f && vertexData[1].distance < 0f && vertexData[2].distance < 0f)
                {
                    Vector3 p1 = vertexData[0].globalVertexPos;
                    Vector3 p2 = vertexData[1].globalVertexPos;
                    Vector3 p3 = vertexData[2].globalVertexPos;

                    underWaterTriangleData.Add(new TriangleData(p1, p2, p3));
                }
                else
                {
                    vertexData.Sort((x, y) => x.distance.CompareTo(y.distance));
                    vertexData.Reverse();

                    if (vertexData[0].distance > 0f && vertexData[1].distance < 0f && vertexData[2].distance < 0f)
                    {
                        AddTrianglesOneAboveWater(vertexData);
                    }
                    //Two vertices are above the water, the other is below
                    else if (vertexData[0].distance > 0f && vertexData[1].distance > 0f && vertexData[2].distance < 0f)
                    {
                        AddTrianglesTwoAboveWater(vertexData);
                    }
                }
            }
        }

        private void AddTrianglesOneAboveWater(List<VertexData> vertexData)
        {
            Vector3 H = vertexData[0].globalVertexPos;

            int M_index = vertexData[0].index - 1;
            if (M_index < 0)
            {
                M_index = 2;
            }

            float h_H = vertexData[0].distance;
            float h_M = 0f;
            float h_L = 0f;

            Vector3 M = Vector3.zero;
            Vector3 L = Vector3.zero;

            if(vertexData[1].index == M_index)
            {
                M = vertexData[1].globalVertexPos;
                L = vertexData[2].globalVertexPos;

                h_M = vertexData[1].distance;
                h_L = vertexData[2].distance;
            }
            else
            {
                M = vertexData[2].globalVertexPos;
                L = vertexData[1].globalVertexPos;

                h_M = vertexData[2].distance;
                h_L = vertexData[1].distance;
            }

            Vector3 MH = H - M;
            float t_M = -h_M / (h_H - h_M);
            Vector3 MI_M = t_M * MH;
            Vector3 I_M = MI_M + M;

            Vector3 LH = H - L;
            float t_L = -h_L / (h_H - h_L);
            Vector3 LI_L = t_L * LH;
            Vector3 I_L = LI_L + L;

            underWaterTriangleData.Add(new TriangleData(M, I_M, I_L));
            underWaterTriangleData.Add(new TriangleData(M, I_L, L));
        }

        private void AddTrianglesTwoAboveWater(List<VertexData> vertexData)
        {
            //H and M are above the water
            //H is after the vertice that's below water, which is L
            //So we know which one is L because it is last in the sorted list
            Vector3 L = vertexData[2].globalVertexPos;

            //Find the index of H
            int H_index = vertexData[2].index + 1;
            if (H_index > 2)
            {
                H_index = 0;
            }


            //We also need the heights to water
            float h_L = vertexData[2].distance;
            float h_H = 0f;
            float h_M = 0f;

            Vector3 H = Vector3.zero;
            Vector3 M = Vector3.zero;

            //This means that H is at position 1 in the list
            if (vertexData[1].index == H_index)
            {
                H = vertexData[1].globalVertexPos;
                M = vertexData[0].globalVertexPos;

                h_H = vertexData[1].distance;
                h_M = vertexData[0].distance;
            }
            else
            {
                H = vertexData[0].globalVertexPos;
                M = vertexData[1].globalVertexPos;

                h_H = vertexData[0].distance;
                h_M = vertexData[1].distance;
            }


            //Now we can find where to cut the triangle

            //Point J_M
            Vector3 LM = M - L;

            float t_M = -h_L / (h_M - h_L);

            Vector3 LJ_M = t_M * LM;

            Vector3 J_M = LJ_M + L;


            //Point J_H
            Vector3 LH = H - L;

            float t_H = -h_L / (h_H - h_L);

            Vector3 LJ_H = t_H * LH;

            Vector3 J_H = LJ_H + L;


            //Save the data, such as normal, area, etc
            //1 triangle below the water
            underWaterTriangleData.Add(new TriangleData(L, J_H, J_M));
        }

        private class VertexData
        {
            public float distance;

            public int index;

            public Vector3 globalVertexPos;
        }

	    public void DisplayMesh(Mesh mesh, string name, List<TriangleData> triangleData)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            for(int i = 0; i < triangleData.Count; i++)
            {
                Vector3 p1 = boatTrans.InverseTransformPoint(triangleData[i].p1);
                Vector3 p2 = boatTrans.InverseTransformPoint(triangleData[i].p2);
                Vector3 p3 = boatTrans.InverseTransformPoint(triangleData[i].p3);

                vertices.Add(p1);
                triangles.Add(vertices.Count - 1);

                vertices.Add(p2);
                triangles.Add(vertices.Count - 1);

                vertices.Add(p3);
                triangles.Add(vertices.Count - 1);
            }

            mesh.Clear();

            mesh.name = name;

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateBounds();
        }
    }
}

