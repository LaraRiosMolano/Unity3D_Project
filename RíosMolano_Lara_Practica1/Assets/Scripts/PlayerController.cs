using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    //Camera and rotation
    float m_Yaw;
    float m_Pitch;
    public float m_YawRotationalSpeed = 360.0f;
    public float m_PitchRotationalSpeed = 180.0f;
    public float m_MinPitch = -80.0f;
    public float m_MaxPitch = 50.0f;
    public Transform m_PitchControllerTransform;
    public bool InvertedYaw;
    public bool InvertedPitch;

    //Movement control
    CharacterController m_CharacterController;
    public float m_Speed = 10.0f;
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_ForwardKeyCode = KeyCode.W;
    public KeyCode m_BackwardsKeyCode = KeyCode.S;
    private bool IsMoving = false;

    //Gravity
    float m_VerticalSpeed = 0.0f;
    bool m_OnGround = false;

    //Jump and run
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public float m_FastSpeedMultiplier = 10.0f;
    public float m_JumpSpeed = 10.0f;

    public float m_Health = 100;
    public float StartHealth = 100;
    public float m_CurrentHealthCount;
    public Text m_HealthText;
    public Text shieldText;

    public float enemyDamage = 100f;
    public bool HasShield = false;
    public float StartShieldHealth = 100;
    public float m_CurrentShieldHealthCount;

    private AudioSource source;
    public AudioClip healthSound;
    public AudioClip shieldSound;

    private void Awake()
    {
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = m_PitchControllerTransform.localRotation.eulerAngles.x;

        m_CharacterController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        shieldText.text = ": " + 0;
        Cursor.lockState = CursorLockMode.Locked;
        m_CurrentHealthCount = m_Health;
        m_CurrentShieldHealthCount = StartShieldHealth;
    }

    void KillPlayer()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_CurrentShieldHealthCount<= 0)
        {
            HasShield = false;
            shieldText.text = ": " + 0;
        }

        if (Input.GetKey("p"))
        {
            SceneManager.LoadSceneAsync("ShootingGame");
        }
        m_HealthText.text = ": "+ m_CurrentHealthCount;

        if (m_CurrentHealthCount <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadSceneAsync("GameOver");
        }

        ///////////////////////////////////////////////////////////////////////////////77777

        //Camera and rotation
        float l_MouseAxisY = Input.GetAxis("Mouse Y");
        m_Pitch += l_MouseAxisY * m_PitchRotationalSpeed * Time.deltaTime;
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);
        m_PitchControllerTransform.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);

        float l_MouseAxisX = Input.GetAxis("Mouse X");
        m_Yaw += l_MouseAxisX * m_YawRotationalSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);


        //Movement control
        float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;
        float l_Yaw90InRadians = (m_Yaw + 90.0f) * Mathf.Deg2Rad;
        Vector3 l_Forward = new Vector3(Mathf.Sin(l_YawInRadians), 0.0f, Mathf.Cos(l_YawInRadians));
        Vector3 l_Right = new Vector3(Mathf.Sin(l_Yaw90InRadians), 0.0f, Mathf.Cos(l_Yaw90InRadians));
        Vector3 l_Movement = Vector3.zero;

        if (Input.GetKey(m_ForwardKeyCode))
        {
            l_Movement = l_Forward;
            
        }
        else if (Input.GetKey(m_BackwardsKeyCode))
        {
            l_Movement = -l_Forward;
            
        }

        if (Input.GetKey(m_RightKeyCode))
        {
            l_Movement += l_Right;
            
        }
        else if (Input.GetKey(m_LeftKeyCode))
        {
            l_Movement -= l_Right;
            
        }

        l_Movement.Normalize();
        l_Movement = l_Movement * Time.deltaTime * m_Speed;
        

        //Jump and run
        float l_SpeedMultiplier = 1.0f;
        if (Input.GetKey(m_RunKeyCode))
        {
            l_SpeedMultiplier = m_FastSpeedMultiplier;
        }
        l_Movement *= l_SpeedMultiplier;
        

        if (m_OnGround && Input.GetKeyDown(m_JumpKeyCode))
        {
            m_VerticalSpeed = m_JumpSpeed;

        }


        //Gravity
        m_VerticalSpeed += Physics.gravity.y*3 * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);

        if ((l_CollisionFlags & CollisionFlags.Below) != 0)
        {
            m_OnGround = true;
            m_VerticalSpeed = 0.0f;
            
        }
        else
        {
            m_OnGround = false;
        }

        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
        {
            m_VerticalSpeed = 0.0f;
        }
    }

    void OnTriggerEnter(Collider _Collider)
    {
        if (_Collider.tag == "HealthItem" && m_CurrentHealthCount < StartHealth)
        {
            source.PlayOneShot(healthSound);
            m_Health = 100;
            ReloadHealth();
            Item l_Item = _Collider.GetComponent<Item>();
            l_Item.TakeItem();
        }

        if (_Collider.tag == "ShieldItem" && HasShield == false)
        {
            source.PlayOneShot(shieldSound);
            HasShield = true;
            shieldText.text = ": " + 1;
            m_CurrentShieldHealthCount = StartShieldHealth;
            Item l_Item = _Collider.GetComponent<Item>();
            l_Item.TakeItem();
        }

        if (_Collider.tag == "DeadZone")
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadSceneAsync("GameOver");
        }

        if (_Collider.tag == "Enemy")
        {
            if (HasShield)
            {
                m_CurrentHealthCount -= enemyDamage * 0.25f;
                Debug.Log(m_CurrentHealthCount);
                m_CurrentShieldHealthCount -= enemyDamage * 0.75f;
                Debug.Log(m_CurrentShieldHealthCount);
            }
            else
            {
                m_CurrentHealthCount -= enemyDamage;
            }
        }
    }

    void ReloadHealth()
    {
        float tryReload = 100 - m_CurrentHealthCount;
        float toReload = Mathf.Min(tryReload, m_Health);
        m_Health -= toReload;
        m_CurrentHealthCount += toReload;
    }
}
