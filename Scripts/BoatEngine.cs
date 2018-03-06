using UnityEngine;
using System.Collections;
using System;

public class BoatEngine : MonoBehaviour {

    public Transform waterJetTransform;

    public float powerFactor;

    public float maxPower;

    public float currentJetPower;

    private float thrustFromWaterJet = 0f;

    private Rigidbody boatRB;

    private float WaterJetRotation_Y = 0f;

    BoatController boatController;

	// Use this for initialization
	void Start () {
        boatRB = GetComponent<Rigidbody>();
        boatController = GetComponent<BoatController>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        UserInput();
	}

    void FixedUpdate()
    {
        UPdateWaterJet();
    }

    private void UPdateWaterJet()
    {
        Vector3 forceToAdd = waterJetTransform.forward * currentJetPower;

        float waveYPos = WaterController.current.GetWaveYPos(waterJetTransform.position, Time.time);

        if(waterJetTransform.position.y < waveYPos)
        {
            boatRB.AddForceAtPosition(forceToAdd, waterJetTransform.position);
        }else
        {
            boatRB.AddForceAtPosition(forceToAdd, waterJetTransform.position);
        }
        if(waterJetTransform.localEulerAngles.y != 0)
        {
            if(waterJetTransform.localEulerAngles.y > 300f)
            {
                waterJetTransform.localEulerAngles = new Vector3(0f, waterJetTransform.localEulerAngles.y + 1.0f, 0f);
            }
            if (waterJetTransform.localEulerAngles.y < 60f)
            {
                waterJetTransform.localEulerAngles = new Vector3(0f, waterJetTransform.localEulerAngles.y - 1.0f, 0f);
            }
        }
    }

    private void UserInput()
    {
        if(Input.GetKey(KeyCode.I))
        {
            if(boatController.CurrentSpeed < 50f && currentJetPower < maxPower)
            {
                currentJetPower += 1f * powerFactor;
            }
        }
        else
        {
            currentJetPower = 0f;
        }

        if(Input.GetKey(KeyCode.J))
        {
            WaterJetRotation_Y = waterJetTransform.localEulerAngles.y + 2f;
            if (WaterJetRotation_Y > 30f/* && WaterJetRotation_Y < 270f*/)
            {
                WaterJetRotation_Y = 30f;
            }

            Vector3 newRotation = new Vector3(0f, WaterJetRotation_Y, 0f);

            waterJetTransform.localEulerAngles = newRotation;
        }
        else if(Input.GetKey(KeyCode.L))
        {
            WaterJetRotation_Y = waterJetTransform.localEulerAngles.y - 2f;

            if(WaterJetRotation_Y < 330f /*&& WaterJetRotation_Y > 90f*/)
            {
                WaterJetRotation_Y = 330f;
            }

            Vector3 newRotation = new Vector3(0f, WaterJetRotation_Y, 0f);
            waterJetTransform.localEulerAngles = newRotation;
        }
    }

}
