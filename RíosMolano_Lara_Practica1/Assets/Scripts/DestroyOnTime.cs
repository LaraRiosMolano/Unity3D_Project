using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
    public float time = 20f;
	void Start ()
    {
        
	}
	
	void Update ()
    {
        time -= Time.deltaTime;
        if (time <= 0f)
        {
            Destroy(gameObject);
        }
	}
}
