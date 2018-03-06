using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CameraSwitcher : MonoBehaviour {

    public Camera defaultCamera;
    public Camera chasingCamera;

    public Text infoText;
    public GameObject sea;

    private WaterController waterControl;
    private float factor = 0.01f;
	// Use this for initialization
	void Start () {
        defaultCamera.enabled = true;
        chasingCamera.enabled = false;
        waterControl = sea.GetComponent<WaterController>();
        UpdateInfoText();
	}

    private void UpdateInfoText()
    {
        infoText.text = "Scale: " + waterControl.scale.ToString() + "\n"
             + "Speed: " + waterControl.speed.ToString() + "\n"
             + "WaveDistance: " + waterControl.waveDistance.ToString();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyUp(KeyCode.C))
        {
            defaultCamera.enabled = !defaultCamera.enabled;
            chasingCamera.enabled = !chasingCamera.enabled;
        }
        if(Input.GetKey(KeyCode.Insert))
        {
            waterControl.scale += factor;
        }else if (Input.GetKey(KeyCode.Delete))
        {
            waterControl.scale -= factor;
        }
        if (Input.GetKey(KeyCode.Home))
        {
            waterControl.speed += factor;
        }
        else if (Input.GetKey(KeyCode.End))
        {
            waterControl.speed -= factor;
        }
        if (Input.GetKey(KeyCode.PageUp))
        {
            waterControl.waveDistance += factor;
        }
        else if (Input.GetKey(KeyCode.PageDown))
        {
            waterControl.waveDistance -= factor;
        }
        UpdateInfoText();
    }
}
