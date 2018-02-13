using HoloToolkit.Examples.SpatialUnderstandingFeatureOverview;
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour {

    //public void StartScene(int i)
    //{
    //    DontDestroyOnLoad(GameObject.Find("Managers"));
    //    AppState.Instance.DebugDisplay.text = "";
    //    SceneManager.LoadScene(i);
    //}

    public void SetUI(int i)
    {

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
