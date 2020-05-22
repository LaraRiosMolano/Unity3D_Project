using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObstacles : MonoBehaviour {
    public GameObject cubo;

	// Use this for initialization
	void Start () {
		for(int i=-5; i<5; i++)
        {
            for(int j=-5; j < 5; j++)
            {
                if(Random.Range(0.0f, 1.0f)> 0.8f)
                {
                    GameObject c = Instantiate(cubo, transform);
                    c.transform.localPosition = new Vector3(i, 0.5f, j);
                }
            }
        }
        //NavMesh.
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
