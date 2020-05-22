using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSShooter : MonoBehaviour
{
    public EnemyController[] drones;

    public GameObject drone1;
    public GameObject drone2;

    public GameObject ala2l;
    public GameObject ala2r;

    public GameObject ala1l;
    public GameObject ala1r;

    public int m_MouseShootButton = 0;
    int m_CurrentAmmoCount = 0;
    float m_CurrentHealthCount;
    GameController gameController;
    
    public int TotalAmmo = 0;
    public int TotalHealth = 0;

    public LayerMask m_ShootLayerMask;
    public GameObject m_ShootHitParticles;
    public Transform m_DestroyObjects;

    public int m_StartAmmo = 10;
    public float m_StartHealth = 10;

    public GameObject m_WeaponDummy;
    public GameObject m_WeaponParticlesEffect;
    public GameObject bulletHole;

    public float m_DestroyOnTime = 0.0f;

    public Animation weaponAnimation;

    public AudioClip shotSound;
    public AudioClip reloadSound;

    private AudioSource source;

    public Text m_AmmoText;

    void Shoot()
    {
        m_CurrentAmmoCount--;
        Ray l_CameraRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_CameraRay, out l_RaycastHit, 200.0f, m_ShootLayerMask.value))
            CreateShootHitParticles(l_RaycastHit.point, l_RaycastHit.normal);
            CreateDecals(l_RaycastHit.point + new Vector3(0,0,1), l_RaycastHit.normal);
        GameObject raycasted = l_RaycastHit.transform.gameObject;
        //GameObject raycasted = l_RaycastHit.collider.gameObject;

        if (raycasted.CompareTag("Enemy"))
        {
            if(raycasted == drone1)
            {
                drones[0].SetHitState(2);
            }
            if (raycasted == drone2)
            {
                drones[1].SetHitState(2);
            }
        }

        if (raycasted.CompareTag("Ala"))
        {
            if (raycasted == ala1l || raycasted == ala1r)
            {
                drones[0].SetHitState(3);
            }
            if(raycasted == ala2l || raycasted == ala2r)
            {
                drones[1].SetHitState(3);
            }
        }

        if (raycasted.CompareTag("Cuerpo"))
        {
            if (raycasted == drone1)
            {
                drones[0].SetHitState(1);
            }
            if (raycasted == drone2)
            {
                drones[1].SetHitState(1);
            }
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

    void CreateDecals(Vector3 Position, Vector3 Normal)
    {
        GameObject.Instantiate(bulletHole, Position, Quaternion.LookRotation(Normal), m_DestroyObjects);
    }

    void Start ()
    {
        
        m_CurrentAmmoCount = m_StartAmmo;
        m_CurrentHealthCount = m_StartHealth;
        weaponAnimation.CrossFade("Idle");
        source = GetComponent<AudioSource>();
	}
	
	void Update ()
    {
        m_AmmoText.text = ": " + m_CurrentAmmoCount;

        if (Input.GetMouseButtonDown(m_MouseShootButton))
        {
            if (CanShoot())
            {
                Shoot();
                source.PlayOneShot(shotSound);
                weaponAnimation.CrossFade("Shoot");
                weaponAnimation.CrossFadeQueued("Idle");
            }
            else
            {
                weaponAnimation.CrossFade("noAmmo");
                weaponAnimation.CrossFadeQueued("Idle");
            }
        }
	}

    void OnTriggerEnter(Collider _Collider)
    {
        if (_Collider.tag == "Item" && m_CurrentAmmoCount < m_StartAmmo)
        {
            TotalAmmo = 10;
            Reload();
            source.PlayOneShot(reloadSound);
            weaponAnimation.CrossFade("Reload");
            weaponAnimation.CrossFadeQueued("Idle");
            Item l_Item = _Collider.GetComponent<Item>();
            l_Item.TakeItem();
        }

        /*if (_Collider.tag == "HealthItem")
        {
            ReloadHealth();
        }*/
    }

    void Reload()
    {

        int tryReload = 10 - m_CurrentAmmoCount;
        int toReload = Mathf.Min(tryReload, TotalAmmo);
        TotalAmmo -= toReload;
        m_CurrentAmmoCount += toReload;

        /*int toReload = TotalAmmo;
        if (TotalAmmo > 10)
        {
            toReload = 10;
        }
        TotalAmmo -= toReload;
        m_CurrentAmmoCount += toReload;*/
    }

    /*void ReloadHealth()
    {
        if (m_CurrentHealthCount < TotalHealth)
        {
            m_CurrentHealthCount++;
        }
    }*/

    bool CanShoot()
    {
        return m_CurrentAmmoCount > 0;
    }


}
