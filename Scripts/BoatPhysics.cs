using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace BoatTutorial
{
    public class BoatPhysics : MonoBehaviour {
        public GameObject underWaterObj;

        private ModifyBoatMesh modifyBoatMesh;

        private Mesh underWaterMesh;

        private Rigidbody boatRB;

        public float rhoWater = 1027f;
	    // Use this for initialization
	    void Start () {
            boatRB = gameObject.GetComponent<Rigidbody>();

            modifyBoatMesh = new ModifyBoatMesh(gameObject);

            underWaterMesh = underWaterObj.GetComponent<MeshFilter>().mesh;
	    }
	
	    // Update is called once per frame
	    void Update () {
            modifyBoatMesh.GenerateUnderWaterMesh();
            modifyBoatMesh.DisplayMesh(underWaterMesh, "UnderWaterMesh", modifyBoatMesh.underWaterTriangleData);
            
	    }

        void FixedUpdate()
        {
            if(modifyBoatMesh.underWaterTriangleData.Count > 0)
            {
                AddUnderWaterForces();
            }
        }

        void AddUnderWaterForces()
        {
            List<TriangleData> underWaterTriangleData = modifyBoatMesh.underWaterTriangleData;
            for(int i = 0; i < underWaterTriangleData.Count; i++)
            {
                TriangleData triangleData = underWaterTriangleData[i];

                Vector3 buoyancyForce = CalcBuoyancyForce(rhoWater, triangleData);

                boatRB.AddForceAtPosition(buoyancyForce, triangleData.center);
            }
        }

        private Vector3 CalcBuoyancyForce(float rho, TriangleData triangleData)
        {
            Vector3 buoyancyForce = rho * Physics.gravity.y * triangleData.distanceToSurface * triangleData.area * triangleData.normal; //new Vector3(0f, -0.5f, 0f); 避免沉船,方便调试

            buoyancyForce.x = 0f;
            buoyancyForce.z = 0f;

            return buoyancyForce;
        }       
    }
}
