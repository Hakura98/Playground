using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance
    {
        get { return s_Instance; }
    }

    protected static PlayerInput s_Instance;

    //Initiate...
    protected Vector2 m_Movement;
    protected Vector2 m_Camera;
    protected bool m_sprinting;
    protected bool m_jumping;
    protected bool m_forceShield;

    private void Awake()
    {
        if (s_Instance == null)
            s_Instance = this;
        else if (s_Instance != this)
            throw new UnityException("There can only be one PlayerInput Script.");
    }


    void Update()
    {
        m_Movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        m_Camera.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        m_jumping = Input.GetKey(KeyCode.Space);
        m_sprinting = (Input.GetKey(KeyCode.LeftShift));
        m_forceShield = (Input.GetKey("r"));
    }

    public Vector2 MovementInput
    {
        get { return m_Movement; }
    }

    public Vector2 CameraInput
    {
        get { return m_Camera; }
    }

    public bool IsSprinting
    {
        get { return m_sprinting; }
    }

    public bool JumpInput
    {
        get { return m_jumping; }
    }

    public bool ForceShield
    {
        get { return m_forceShield; }
    }
}
