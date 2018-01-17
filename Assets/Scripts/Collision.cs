using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour {

    public Material def;
    public Material green;
    public Material red;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay (Collider collider)
    {
        //Debug.Log("Collision");
        //this.gameObject.SetActive(false);
        GetComponent<Renderer>().material = red;
    }

    void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("Collision");
        //this.gameObject.SetActive(false);
        GetComponent<Renderer>().material = red;
    }

    void OnTriggerExit(Collider collider)
    {
        //Debug.Log("Collision");
        //this.gameObject.SetActive(false);
        GetComponent<Renderer>().material = green;
    }
}
