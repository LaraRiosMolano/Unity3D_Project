using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pared : MonoBehaviour {
    public GameController PlayerController;
    public float minDist;
    public Animation animation;
    bool HaCaido;

    // Use this for initialization
    void Start () {
        HaCaido = false;
	}
	
	// Update is called once per frame
	void Update () {
		if((PlayerController.Player.transform.position.magnitude - transform.position.magnitude) <= minDist && !HaCaido)
        {
            animation.CrossFade("DeathPared");
            HaCaido = true;
        }
	}
}
