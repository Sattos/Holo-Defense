using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodUI : MonoBehaviour {

    public GameObject[] Left;
    public GameObject[] Right;

    public bool isLeft;
    public bool isRight;

    public static GoodUI Instance;

    public void Awake()
    {
        Instance = this;
    }

    public void ActivateLeft()
    {
        if (isLeft)
            return;
        foreach (GameObject obj in Left)
        {
            obj.SetActive(true);
            isLeft = true;
        }
    }

    public void ActivateRight()
    {
        if (isRight)
            return;
        foreach (GameObject obj in Right)
        {
            obj.SetActive(true);
            isRight = true;
        }
    }

    public void DeactivateLeft()
    {
        if (!isLeft)
            return;
        foreach (GameObject obj in Left)
        {
            obj.SetActive(false);
            isLeft = false;
        }
    }

    public void DeactivateRight()
    {
        if (!isRight)
            return;
        foreach (GameObject obj in Right)
        {
            obj.SetActive(false);
            isRight = false;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 lookDirTarget = CameraCache.Main.transform.position - transform.position;
        lookDirTarget = (new Vector3(lookDirTarget.x, 0.0f, lookDirTarget.z)).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-lookDirTarget), Time.deltaTime * 10.0f);
    }
}
