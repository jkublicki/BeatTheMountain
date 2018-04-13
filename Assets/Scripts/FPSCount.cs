using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCount : MonoBehaviour {

    Text tx;
    float t = 0.0f;
    int i = 0;
    float FPS = -1.0f;

	// Use this for initialization
	void Start () {

        tx = GetComponent<Text>();

	}
	
	// Update is called once per frame
	void Update () {

        t += Time.unscaledDeltaTime;
        i++;

        if(i == 500)
        {
            FPS = 500.0f / t;
        }

        tx.text = Time.timeSinceLevelLoad.ToString("00:00:00.0") + "; frames: " + Time.frameCount.ToString() + "; f/t: "
            + (Time.frameCount / Time.timeSinceLevelLoad).ToString("0.000")
            + "; FPS: " + FPS.ToString("0.000");


    }
}
