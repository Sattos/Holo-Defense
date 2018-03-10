using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementCollision : MonoBehaviour
{
    public bool IsBlocked()
    {
        return count != 0;
    }

    public int count = 0;

    // Use this for initialization
    void Start()
    {

    }

    private void Update()
    {
        if (IsBlocked())
            Debug.Log("COLLISIOn");
    }

    void OnTriggerStay(Collider collider)
    {
        //Debug.Log("Collision");
        //this.gameObject.SetActive(false);
        //GetComponent<Renderer>().material = red;
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "BlockPlacement")
        {
            count++;
        }
        //Debug.Log("Collision");
        //this.gameObject.SetActive(false);
        //GetComponent<Renderer>().material = red;
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "BlockPlacement")
        {
            count--;
        }
        //Debug.Log("Collision");
        //this.gameObject.SetActive(false);
        //GetComponent<Renderer>().material = green;
    }
}
