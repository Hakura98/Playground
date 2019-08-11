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
    public float jumpSpeed = 10f;
    public float idleTimout = 5f;

    #region Membervariable
    protected PlayerInput m_Input;
    protected CharacterController m_CharCtrl;
    protected Animator m_Animator;
    protected float m_DesiredSpeed;
    protected float m_ForwardSpeed;
    protected float m_VerticalSpeed;
    protected bool m_Grounded = true;
    protected Quaternion m_Rotation;
    protected bool m_readyJump = false;
    protected bool m_forceShield = false;
    protected AnimatorStateInfo m_CurrAnimatorStateInfo;
    protected AnimatorStateInfo m_NextAnimatorStateInfo;
    protected bool m_CurrIsTransitioning;
    protected float m_idleTimer;
    #endregion

    #region Hash
    readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
    readonly int m_HashGrounded = Animator.StringToHash("Grounded");
    readonly int m_HashVerticalSpeed = Animator.StringToHash("AirVerticalSpeed");
    readonly int m_HashRoar = Animator.StringToHash("Roar");
    readonly int m_HashIdleTimeOut = Animator.StringToHash("IdleTimeOut");
    readonly int m_HashInputDetected = Animator.StringToHash("InputDetected");
    readonly int m_HashBlockInput = Animator.StringToHash("BlockInput");
    #endregion

    #region Constants
    const float k_acceleration = 20f;
    const float k_stickyGravitation = 0.3f;
    const float k_distanceToGroundRay = 1.0f;
    const float k_fastFall = 10f;
    #endregion

    protected bool IsMove
    {
        get { return !Mathf.Approximately(m_Input.MovementInput.sqrMagnitude, 0f); }
    }

    private void Awake()
    {
        m_Input = GetComponent<PlayerInput>();
        m_CharCtrl = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();

        s_Instance = this;
    }

    private void FixedUpdate()
    {
        CacheAnimState();
        UpdateBlockInput();

        CalculateForwardSpeed();
        CalculateVerticalSpeed();
        SetRotation();
        UpdateRotation();

        SetForceShield();

        TimeOutIdle();
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

        movement += Vector3.up * m_VerticalSpeed * Time.deltaTime;

        m_CharCtrl.Move(movement);

        m_Grounded = m_CharCtrl.isGrounded;

        if (!m_Grounded)
            m_Animator.SetFloat(m_HashVerticalSpeed, m_VerticalSpeed);

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
        if (m_Grounded && !m_Input.JumpInput)
            m_readyJump = true;

        if (m_Grounded)
        {
            m_VerticalSpeed = -gravity * k_stickyGravitation;

            if(m_readyJump && m_Input.JumpInput)
            {
                m_VerticalSpeed = jumpSpeed;
                m_Grounded = false;
                m_readyJump = false;

            }
        }
        else
        {
            if (!m_Input.JumpInput && m_VerticalSpeed > 0.0f)
                m_VerticalSpeed -= k_fastFall * Time.deltaTime;

            if (Mathf.Approximately(m_VerticalSpeed, 0f))
                m_VerticalSpeed = 0f;

            m_VerticalSpeed -= gravity * Time.deltaTime;
        }
    }

    private void CacheAnimState()
    {
        m_CurrAnimatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
        m_NextAnimatorStateInfo = m_Animator.GetNextAnimatorStateInfo(0);
        m_CurrIsTransitioning = m_Animator.IsInTransition(0);
    }

    private void UpdateBlockInput()
    {
        bool blockinput = m_CurrAnimatorStateInfo.tagHash == m_HashBlockInput && !m_CurrIsTransitioning;
        m_Input.blockInput = blockinput;
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

    private void SetForceShield()
    {
        MeshRenderer ShieldRenderer = GetComponentInChildren<MeshRenderer>();
        SphereCollider ShieldCollider = GetComponentInChildren<SphereCollider>();
        if (m_forceShield)
        {
            m_Animator.SetBool(m_HashRoar, true);
            ShieldRenderer.enabled = true;
            ShieldCollider.enabled = true;
        }
        else
        {
            m_Animator.SetBool(m_HashRoar, false);
            ShieldRenderer.enabled = false;
            ShieldCollider.enabled = false;
        }

        m_forceShield = m_Input.ForceShield;
    }

    private void TimeOutIdle()
    {
        bool inputdetected = IsMove || m_forceShield || m_Input.JumpInput || m_Input.IsSprinting;

        if(!inputdetected && m_Grounded)
        {
            m_idleTimer += Time.deltaTime;

            if(m_idleTimer >= idleTimout)
            {
                m_idleTimer = 0f;
                m_Animator.SetTrigger(m_HashIdleTimeOut);
            }
        }
        else
        {
            m_idleTimer = 0f;
            m_Animator.ResetTrigger(m_HashIdleTimeOut);
        }

        m_Animator.SetBool(m_HashInputDetected, inputdetected);

    }

}
