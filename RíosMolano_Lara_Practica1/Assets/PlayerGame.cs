using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerGame : MonoBehaviour {

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
    
    private float TimeCounter = 60.0f;
    private float currentTime;

    public Text m_TimeText;

    private void Awake()
    {
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = m_PitchControllerTransform.localRotation.eulerAngles.x;

        m_CharacterController = GetComponent<CharacterController>();
    }

    // Use this for initialization
    void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentTime = TimeCounter;
    }
	
	// Update is called once per frame
	void Update ()
    {
        currentTime -= Time.deltaTime;
        
        m_TimeText.text = " "+ Mathf.Floor(currentTime);

        if (currentTime <= 0.0f)
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadSceneAsync("Replay");
        }

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
        m_VerticalSpeed += Physics.gravity.y * 3 * Time.deltaTime;
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
    
}
