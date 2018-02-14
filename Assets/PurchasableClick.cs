using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasableClick : MonoBehaviour, IRaycastFocusEvent {

    public GameObject RotatingObject;

    public bool isRotating;

    public void Activate()
    {
        isRotating = true;
    }

    public void Deactivate()
    {
        isRotating = false;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(isRotating)
        {
            RotatingObject.transform.Rotate(0, 1, 0);
        }
	}
}
