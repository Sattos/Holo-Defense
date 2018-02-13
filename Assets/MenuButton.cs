using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour {

    public void ChangeScene()
    {
        DontDestroyOnLoad(GameObject.Find("Managers"));
        SceneManager.LoadScene(0);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
