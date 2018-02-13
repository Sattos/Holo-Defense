using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour {

    public void StartScene(int i)
    {
        DontDestroyOnLoad(GameObject.Find("Managers"));
        SceneManager.LoadScene(i);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
