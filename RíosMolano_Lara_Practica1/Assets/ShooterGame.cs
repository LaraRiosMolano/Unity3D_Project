using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShooterGame : MonoBehaviour {

    public int m_MouseShootButton = 0;
    int m_CurrentPointsCount;
    int Points = 0;
    public LayerMask m_ShootLayerMask;
    public GameObject m_ShootHitParticles;
    public GameObject m_WeaponDummy;
    public GameObject m_WeaponParticlesEffect;
    public Animation weaponAnimation;
    public AudioClip shotSound;
    private AudioSource source;
    public Transform m_DestroyObjects;



    public Text m_PointsText;

    void Shoot()
    {
        Ray l_CameraRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_CameraRay, out l_RaycastHit, 200.0f, m_ShootLayerMask.value))
            CreateShootHitParticles(l_RaycastHit.point, l_RaycastHit.normal);
        GameObject raycasted = l_RaycastHit.transform.gameObject;

        if (raycasted.CompareTag("Destroyable"))
        {
            m_CurrentPointsCount += 100;
        }
        if (raycasted.CompareTag("Movil"))
        {
            m_CurrentPointsCount += 1000;
        }

        CreateShootWeaponParticles(m_WeaponDummy.transform.position);
    }

    void CreateShootWeaponParticles(Vector3 position)
    {
        Instantiate(m_ShootHitParticles, position, Quaternion.identity, m_WeaponDummy.transform);
    }

    void CreateShootHitParticles(Vector3 Position, Vector3 Normal)
    {
        GameObject.Instantiate(m_ShootHitParticles, Position, Quaternion.identity, m_DestroyObjects);
    }

    void Start ()
    {
        m_CurrentPointsCount = Points;
        weaponAnimation.CrossFade("Idle");
        source = GetComponent<AudioSource>();
    }
	
	
	void Update ()
    {
        m_PointsText.text = "POINTS: " + m_CurrentPointsCount;

        if (Input.GetMouseButtonDown(m_MouseShootButton))
        {
            Shoot();
            source.PlayOneShot(shotSound);
            weaponAnimation.CrossFade("Shoot");
            weaponAnimation.CrossFadeQueued("Idle");
            
        }
    }
}
