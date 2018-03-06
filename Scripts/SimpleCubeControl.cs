using UnityEngine;
using System.Collections;

public class SimpleCubeControl : MonoBehaviour {


	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKey(KeyCode.H))
        {
            transform.Translate(transform.InverseTransformVector(Vector3.right));
        }
        if (Input.GetKey(KeyCode.B))
        {
            transform.Translate(transform.InverseTransformVector(-Vector3.right));
        }
    }
}
