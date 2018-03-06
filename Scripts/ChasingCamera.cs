using UnityEngine;
using System.Collections;
using System;

public class ChasingCamera : MonoBehaviour {

    public GameObject target;

    private Vector3 offset;
	// Use this for initialization
	void Start () {
        UpdateCameraPosition();
	}
    // Update is called once per frame
    void Update () {
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        offset = -target.transform.forward * 10f;
        offset.y = target.transform.position.y + 5f;
        transform.position = target.transform.position + offset;
        transform.forward = target.transform.position - transform.position;
    }

}
