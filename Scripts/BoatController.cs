using UnityEngine;
using System.Collections;
using System;

public class BoatController : MonoBehaviour {

    private float currentSpeed;
    private Vector3 lastPosition;
    private Rigidbody boatRb;

    //public float speedFactor = 50;
    //public float maxFactor = 100;
    //public float maxSpeed = 10;

    private Vector3 rotation;

    void start()
    {
        boatRb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
        rotation = transform.rotation.eulerAngles;
    }

	void FixedUpdate()
    {
        CalculateSpeed();
        BoatStable();
    }

    private void BoatStable()
    {
        if(GetComponent<Rigidbody>().mass > 300f && Time.frameCount % 10 == 0)
        {
            rotation = transform.rotation.eulerAngles;
            rotation.z = AngleClamp(rotation.z);
            rotation.x = AngleClamp(rotation.x);

            transform.rotation = Quaternion.Euler(rotation);
        }
    }
    
    private float AngleClamp(float angle)
    {
        float ret = 0.0f;
        if (angle < 180f)
        {
            ret = Mathf.Clamp(angle, 0f, 60f);
        }
        else
        {
            ret = Mathf.Clamp(angle, 300f, 360f);
        }
        return ret;
    }

    private void CalculateSpeed()
    {
        //if (Input.GetKey(KeyCode.I))
        //{
        //    if(speedFactor < maxFactor && currentSpeed < maxSpeed)
        //    {
        //        speedFactor = 1000;
        //        Vector3 force = transform.forward * speedFactor;
        //        boatRb = GetComponent<Rigidbody>();
        //        boatRb.AddForce(force);
        //    }
        //    else
        //    {
        //        speedFactor = 0;
        //    }
        //}
        
        if (Input.GetKey(KeyCode.J))
        {
            transform.Rotate(transform.up, -5f);
        }
        else if (Input.GetKey(KeyCode.L))
        {
            transform.Rotate(transform.up, +5f);
        }
        currentSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;
    }

    public float CurrentSpeed
    {
        get
        {
            return this.currentSpeed;
        }
    }
}
