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
    }

    public Vector2 MovementInput
    {
        get { return m_Movement; }
    }

    public Vector2 CameraInput
    {
        get { return m_Camera; }
    }

}
