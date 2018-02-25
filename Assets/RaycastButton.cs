using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RaycastButton : MonoBehaviour, IRaycastFocusEvent
{

    public UnityEvent OnClick;
    public UnityEvent OnFocusEntered;
    public UnityEvent OnFocusLost;

    public BlockingType blockingType;

    public BlockingType BlockingType
    {
        get
        {
            return blockingType;
        }
        set
        {

        }
    }
    public void Activate()
    {
        OnFocusEntered.Invoke();
    }

    public void Click()
    {
        OnClick.Invoke();
    }

    public void Deactivate()
    {
        OnFocusLost.Invoke();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
