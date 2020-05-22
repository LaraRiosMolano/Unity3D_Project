using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {
    public GameObject obj;
    public GameObject ala1;
    public GameObject ala2;

    private bool canFade;
    public Color alphaColor = new Color(0,0,0,0);
    private float timeToFade = 1.0f;
    private float timeToDie = 5.0f;

    public GameObject bullet;
    public GameObject bulletTarget;
    public float bulletSpeed = 10f;
    public Transform disparador;
    private AudioSource source;
    public AudioClip shotSound;

    public NavMeshAgent m_NavMeshAgent; //Usado
    public enum TState //Usado
    {
        IDLE,
        PATROL,
        HIT,
        ALERT,
        CHASE,
        ATTACK,
        DIE
    }
    public Transform item1;
    public Transform item2;
    public Transform item3;

    public TState currentState; //Usado
    public List<Transform> m_PatrolPositions; //Usado
    public int EnemyHealth;
    public bool Hit = false;
    public TState lastState;

    float currentTime = 0.0f; //Usado
    int m_CurrentPatrolPositionId = 0; //Usado

    float m_StartAlertRotation = 0.0f; //Usado
    float m_CurrentAlertRotation = 0.0f; //Usado
    float Rotate=0; //Usado

    public GameController PlayerController; //Usado
    public float m_MinDistanceToAlert = 5.0f; //Usado
    public LayerMask m_CollisionLayerMask; //Usado

    public float m_MinDistanceToAttack = 3.0f; //Usado
    public float m_MaxDistanceToAttack = 7.0f; //Usado

    public float m_MaxDistanceToPatrol = 15.0f; //Usado
    public float m_ConeAngle = 60.0f;
    public float m_LerpAttackRotation = 0.6f;
    const float m_MaxLife = 100.0f;
    float m_Life = m_MaxLife;
    public int whichPart;

    [Range(0.0f, 1.0f)]
    public float m_ShootAccuracy = 0.3f;

    float maxTimeIdle = 20.0f;

    float Counter = 0;

    public GameObject ShootParticles;

    /*public GameObject m_WeaponDummy;
    public GameObject player;
    public GameObject m_WeaponParticlesEffect;
    public GameObject m_ShootHitParticles;*/

    void Start ()
    {
        canFade = false;
        source = GetComponent<AudioSource>();
        
        currentState = TState.IDLE;
        m_Life = m_MaxLife;
        
    }
	
	void Update ()
    {
        currentTime += Time.deltaTime;

        switch (currentState)
        {
            case TState.IDLE:
                UpdateIdleState();
                break;
            case TState.PATROL:
                UpdatePatrolState();
                break;
            case TState.HIT:
                UpdateHitState();
                break;
            case TState.ALERT:
                UpdateAlertState();
                break;
            case TState.CHASE:
                UpdateChaseState();
                break;
            case TState.ATTACK:
                UpdateAttackState();
                break;
            case TState.DIE:
                UpdateDieState();
                break;
        }
	}

    //Updates..........................................

    private void UpdateIdleState()
    {
        Debug.Log("Idle");

        if (CurrentTimeMoreThan(maxTimeIdle))
        {
            SetPatrolState();
        }

        lastState = TState.IDLE;
    }

    private void UpdatePatrolState()
    {
        if (!m_NavMeshAgent.hasPath && m_NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            MoveToNextPatrolPosition();
        }

        if (HearsPlayer())
        {
            SetAlertState();
        }

        Debug.Log("Patrol");
        
        lastState = TState.PATROL;
    }
    
    private void UpdateHitState()
    {
        Debug.Log("Hit");

        if (whichPart == 2)
        {
            m_Life -= 100;
        }

        if (whichPart == 3)
        {
            m_Life -= 25;
        }

        if (whichPart == 1)
        {
            m_Life -= 0;
        }

        if (m_Life <= 0)
        {
            SetDieState();
        }
        else
        {
            switch (lastState)
            {
                case TState.PATROL:
                    SetAlertState();
                    break;
                case TState.ALERT:
                    SetAlertState();
                    break;
                case TState.ATTACK:
                    SetAttackState();
                    break;
                case TState.CHASE:
                    SetChaseState();
                    break;
            }
        }
        whichPart = 0;
        
    }

    private void UpdateAlertState()
    {
        m_NavMeshAgent.isStopped = true;
        Debug.Log("Alert");

        Rotate = Time.deltaTime * 50;
        transform.Rotate(0, Rotate, 0);

        Debug.Log(m_CurrentAlertRotation);

        m_CurrentAlertRotation += Rotate;

        if (m_CurrentAlertRotation <= 360.0f)
        {
            if (SeesPlayer())
            {
                SetChaseState();
            }
        }
        else
        {
            SetPatrolState();
        }
        lastState = TState.ALERT;
    }

    private void UpdateChaseState()
    {
        Debug.Log("Chase");
        SetNextChasePosition();
        float Distance = PlayerController.Player.transform.position.magnitude - transform.position.magnitude;
        if (Distance < 0)
        {
            Distance *= -1;
        }
        if (Distance > m_MaxDistanceToPatrol )
        {
            SetPatrolState();
        }
        if(Distance <= m_MinDistanceToAttack )
        {
            SetAttackState();
        }
        lastState = TState.CHASE;
    }

    private void UpdateAttackState()
    {
        Debug.Log("Attack");

        Counter += Time.deltaTime;

        if(Counter >= 2.0f)
        {
            
            source.PlayOneShot(shotSound);
            GameObject instBullet = Instantiate(bullet, disparador.position, Quaternion.identity) as GameObject;
            
            Rigidbody instBulletRigidbody = instBullet.GetComponent<Rigidbody>();
            instBulletRigidbody.AddForce((PlayerController.Player.transform.position - transform.position) * bulletSpeed);
            Counter = 0;
        }

        float Distance = PlayerController.Player.transform.position.magnitude - transform.position.magnitude;
        if(Distance < 0)
        {
            Distance *= -1;
        }
        if (Distance > m_MaxDistanceToAttack )
        {
            SetChaseState();
        }
        lastState = TState.ATTACK;
    }

    private void UpdateDieState()
    {
        timeToDie -= Time.deltaTime;
        canFade = true;
        if (canFade)
        {
            obj.GetComponent<MeshRenderer>().material.color = Color.Lerp(obj.GetComponent<MeshRenderer>().material.color, alphaColor, timeToFade * Time.deltaTime);
            ala1.GetComponent<MeshRenderer>().material.color = Color.Lerp(ala1.GetComponent<MeshRenderer>().material.color, alphaColor, timeToFade * Time.deltaTime);
            ala2.GetComponent<MeshRenderer>().material.color = Color.Lerp(ala2.GetComponent<MeshRenderer>().material.color, alphaColor, timeToFade * Time.deltaTime);
        }

        if (timeToDie <= 0)
        {
            SpawnItem();
            Destroy(gameObject);
            Debug.Log("Die");
        }
        
    }

    //Sets..............................................
    
    private void SetIdleState()
    {
        currentState = TState.IDLE;
        currentTime = 0.0f;
    }

    private void SetPatrolState()
    {
        currentState = TState.PATROL;
        currentTime = 0.0f;
        m_NavMeshAgent.isStopped = false;
        m_NavMeshAgent.SetDestination(m_PatrolPositions[m_CurrentPatrolPositionId].position);
    }

    public void SetHitState(int zone)
    {
        whichPart = zone;
        currentState = TState.HIT;
    }

    private void SetAlertState()
    {
        m_CurrentAlertRotation = m_StartAlertRotation;
        currentState = TState.ALERT;
    }

    private void SetChaseState()
    {
        currentState = TState.CHASE;
    }

    private void SetAttackState()
    {
        currentState = TState.ATTACK;
    }

    private void SetDieState()
    {
        
        currentState = TState.DIE;
    }

    //Otros..................................................

    private void SpawnItem()
    {
        int num = Random.Range(0,3);
        Transform item;
        switch (num)
        {
            case 0:
                item = item1;
                break;
            case 1:
                item = item2;
                break;
            default:
                item = item3;
                break;
        }
        Instantiate(item, transform.position, Quaternion.identity);
    }

    private bool CurrentTimeMoreThan(float maxTimeIdle)
    {
        return maxTimeIdle > currentTime;
    }

    void SetNextChasePosition()
    {
        m_NavMeshAgent.isStopped = false;
        Vector3 l_Destination = PlayerController.Player.transform.position - transform.position;
        float l_Distance = l_Destination.magnitude;
        l_Destination /= l_Distance;
        l_Destination = transform.position + l_Destination * (l_Distance - m_MinDistanceToAttack);
        m_NavMeshAgent.SetDestination(l_Destination);
    }

    void MoveToNextPatrolPosition()
    {
        ++m_CurrentPatrolPositionId;
        if (m_CurrentPatrolPositionId == m_PatrolPositions.Count)
            m_CurrentPatrolPositionId = 0;
        m_NavMeshAgent.SetDestination(m_PatrolPositions[m_CurrentPatrolPositionId].position);
    }

    bool SeesPlayer()
    {
        Debug.Log("SEES PLAYER");
        Vector3 l_Direction = (PlayerController.Player.transform.position + Vector3.up * 0.9f) - transform.position;
        Ray l_Ray = new Ray(transform.position, l_Direction);
        float l_Distance = l_Direction.magnitude;
        l_Direction /= l_Distance;
        bool l_Collides = Physics.Raycast(l_Ray, l_Distance, m_CollisionLayerMask.value);
        float l_DotAngle = Vector3.Dot(l_Direction, transform.forward);

        Debug.DrawRay(transform.position, l_Direction * l_Distance, l_Collides ? Color.red : Color.yellow);
        return !l_Collides && (l_DotAngle > Mathf.Cos(m_ConeAngle * 0.5f * Mathf.Deg2Rad)); //
    }

    bool HearsPlayer()
    {
        return GetSqrDistanceXZToPosition(PlayerController.Player.transform.position) < (m_MinDistanceToAlert * m_MinDistanceToAlert);
    }

    float GetSqrDistanceXZToPosition(Vector3 a)
    {
        float dist = Vector3.Distance(a, transform.position);
        return dist;
    }
    
    

    
    
}
