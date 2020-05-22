using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    float Counter = 0;
    public GameController PlayerController;
    Vector3 l_Destination;
    public int bulletSpeed = 1;
    public GameObject Disparador;

    // Use this for initialization
    void Start () {
        l_Destination = PlayerController.Player.transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        transform.Translate(l_Destination*bulletSpeed*Time.deltaTime);
        Counter += Time.deltaTime;

        if(Counter >= 5.0f)
        {
            Destroy(gameObject);
        }
        
	}
}
