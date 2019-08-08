using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerScript : MonoBehaviour
{
    private Rigidbody rb;
    private Collider mycollider;
    private Animator anim;
    private float dtog = 0f;

    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float runSpeed = 2f;
    [SerializeField] private float backpaddle = 0.5f;
    [SerializeField] private float rotSpeed = 0f;
    [SerializeField] private float jumpForce = 0f;
    [SerializeField] private float fallmult = 1.5f;
    [SerializeField] private float lowmult = 1f;
    [SerializeField] private float jumpDiagonal = 0f;

    bool isGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up, -transform.up, 1.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mycollider = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        //dtog = mycollider.bounds.extents.y;
        //Debug.Log(dtog);
    }

    // Update is called once per frame
    void Update()
    {
        #region Jumping
        if (isGrounded() && Input.GetKeyDown("space"))
        {
            anim.SetBool("inAir", !isGrounded());

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //djump = isGrounded() ? 0 : djump + 1;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * fallmult * Time.deltaTime;

        }
        else if (rb.velocity.y > 0 && !Input.GetKey("space"))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * lowmult * Time.deltaTime;
        }
        #endregion

        #region Animations
        anim.SetFloat("inputH", Input.GetAxis("Horizontal"));
        anim.SetFloat("inputV", Input.GetAxis("Vertical"));
        anim.SetFloat("absV", Mathf.Abs(Input.GetAxis("Vertical")));
        anim.SetFloat("velMagn", Vector3.Magnitude(rb.velocity));

        if (Vector3.Magnitude(new Vector3(rb.velocity.x, 0, rb.velocity.z)) > 1.5f)
            anim.SetBool("running", true);
        else anim.SetBool("running", false);

        anim.SetBool("inAir", !isGrounded());
        #endregion

    }


    private void FixedUpdate()
    {
        //! Note that moving the character resets current velocity
        #region Movement
        if (Input.GetAxis("Vertical") > 0)
        {
            //rb.AddForce(transform.forward * Input.GetAxis("Vertical") * walkSpeed);              //starts too slow
            Vector3 newVel = transform.forward * Input.GetAxis("Vertical") * walkSpeed;
            Vector3 oldVelY = new Vector3(0f, rb.velocity.y, 0f); 
            rb.velocity = Input.GetKey("r") ? newVel * runSpeed + oldVelY : newVel + oldVelY;
        }


        if (Input.GetAxis("Vertical") < 0)
        {
            Vector3 oldVelY = new Vector3(0f, rb.velocity.y, 0f);
            rb.velocity = transform.forward * Input.GetAxis("Vertical") * walkSpeed * backpaddle + oldVelY;
        }

        if (Input.GetButton("Horizontal"))
        {
            //rb.AddForce(Input.GetAxis("Horizontal") * walkSpeed, 0f, 0f);
            //rb.velocity = Vector3.right * Input.GetAxis("Horizontal") * walkSpeed;
            rb.rotation = Quaternion.Lerp(rb.rotation, rb.rotation * Quaternion.AngleAxis(Input.GetAxis("Horizontal") * rotSpeed *
                Vector3.Magnitude(rb.velocity), Vector3.up), 0.5f);
        } 
        #endregion

    }
}
