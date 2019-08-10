using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    protected static PlayerController s_Instance;

    public static PlayerController Instance
    {
        get { return s_Instance; }
    }

    public float maxForwardSpeed = 4f;
    public float walkSpeed = 1f;
    public float gravity = 20f;
    public float maxTurnSpeed = 1200f;
    public float minTurnSpeed = 400f;

    #region Membervariable
    protected PlayerInput m_Input;
    protected CharacterController m_CharCtrl;
    protected Animator m_Animator;
    protected float m_DesiredSpeed;
    protected float m_ForwardSpeed;
    protected float m_VerticalSpeed;
    protected bool m_Grounded = true;
    protected Quaternion m_Rotation;
    #endregion

    #region Hash
    readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
    readonly int m_HashGrounded = Animator.StringToHash("Grounded");
    #endregion

    #region Constants
    const float k_acceleration = 20f;
    const float k_stickyGravitation = 0.3f;
    const float k_distanceToGroundRay = 1.0f;
    #endregion

    private void Awake()
    {
        m_Input = GetComponent<PlayerInput>();
        m_CharCtrl = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();

        s_Instance = this;
    }

    private void Start()
    {
        m_Animator.applyRootMotion = true;
    }

    private void FixedUpdate()
    {
        CalculateForwardSpeed();
        CalculateVerticalSpeed();
        SetRotation();
        UpdateRotation();
    }

    private void OnAnimatorMove()
    {
        Vector3 movement;

        if (m_Grounded)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);

            if (Physics.Raycast(ray, out hit, k_distanceToGroundRay, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                movement = Vector3.ProjectOnPlane(transform.forward * Time.deltaTime * m_ForwardSpeed,
                    hit.normal);
            }
            else
            {
                movement = transform.forward * Time.deltaTime * m_ForwardSpeed;
            }
        }
        else
        {
            movement = transform.forward * Time.deltaTime * m_ForwardSpeed;
        }

        m_CharCtrl.transform.rotation *= m_Animator.deltaRotation;

        movement += Vector3.up * m_VerticalSpeed * Time.deltaTime;

        m_CharCtrl.Move(movement);

        m_Grounded = m_CharCtrl.isGrounded;

        m_Animator.SetBool(m_HashGrounded, m_Grounded);
    }

    private void CalculateForwardSpeed()
    {
        Vector2 moveInput = m_Input.MovementInput;

        if (moveInput.SqrMagnitude() > 1)
            moveInput.Normalize();

        m_DesiredSpeed = m_Input.IsSprinting? moveInput.magnitude * maxForwardSpeed : moveInput.magnitude * walkSpeed;
        m_ForwardSpeed = Mathf.MoveTowards(m_ForwardSpeed, m_DesiredSpeed, Time.deltaTime * k_acceleration);
        m_Animator.SetFloat(m_HashForwardSpeed, m_ForwardSpeed);
    }

    private void CalculateVerticalSpeed()
    {
        if (m_Grounded)
        {
            m_VerticalSpeed = -gravity * k_stickyGravitation;
        }
        else
        {
            m_VerticalSpeed -= gravity * Time.deltaTime;
        }
    }

    private void SetRotation()
    {
        Vector2 Input = m_Input.MovementInput;
        Vector3 Direction = new Vector3(Input.x, 0f, Input.y);
        Quaternion Offset = Quaternion.FromToRotation(Vector3.forward, Direction.normalized);
        Quaternion TargetRotation = Quaternion.LookRotation(Offset * transform.forward);
        m_Rotation = TargetRotation;
    }

    private void UpdateRotation()
    {

        float rotSpeed = m_DesiredSpeed == 0 ? 0 : m_ForwardSpeed / m_DesiredSpeed;
        float groundTurnSpeed = Mathf.Lerp(maxTurnSpeed, minTurnSpeed, rotSpeed);
        m_Rotation = Quaternion.RotateTowards(transform.rotation, m_Rotation, groundTurnSpeed * Time.deltaTime);

        transform.rotation = m_Rotation;
    }
}
