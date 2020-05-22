using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour {
    public float time;
    private float currentTime;
    bool show = true;

	// Use this for initialization
	void Start () {
        currentTime = time;
	}
	
	// Update is called once per frame
	void Update () {
        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = time;
            if (show) { show = false; }
            else { show = true; }
        }

        if (!show)
        {
            transform.localScale = new Vector3(0,0,0);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
	}
}
