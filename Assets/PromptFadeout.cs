using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptFadeout : MonoBehaviour {

    private Color color;

    private bool isAnimating;
    private float fadeoutStartTime;

	// Use this for initialization
	void Start () {

	}

    public void StartFadeout(float time)
    {
        fadeoutStartTime = time;
        color = gameObject.GetComponent<TextMesh>().color;
    }

    // Update is called once per frame
    void Update () {
        if(Time.time > fadeoutStartTime && color.a > 0)
        {
            color.a -= Time.deltaTime * 0.8f;
            gameObject.GetComponent<TextMesh>().color = color;
        }
	}
}
