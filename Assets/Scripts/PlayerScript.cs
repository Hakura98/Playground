using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerScript : MonoBehaviour
{
    private Transform mytrans;
    private Rigidbody rb;
    private Collider mycollider;
    private Animator anim;
    private float dtog = 0f;
    private int djump = 0;

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float backpaddle = 0.5f;
    [SerializeField] private float jumpForce = 0f;
    [SerializeField] private float fallmult = 1.5f;
    [SerializeField] private float lowmult = 1f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mytrans = GetComponent<Transform>();
        mycollider = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        dtog = mycollider.bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        if ((isGrounded()|| djump < 1) && Input.GetKeyDown("space"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            djump = isGrounded() ? 0 : djump + 1;
        }

        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * fallmult * Time.deltaTime;

        }
        else if(rb.velocity.y > 0 && !Input.GetKey("space"))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * lowmult * Time.deltaTime;
        }

        anim.SetFloat("inputH", Input.GetAxis("Horizontal"));
        anim.SetFloat("inputV", Input.GetAxis("Vertical"));
    }

    bool isGrounded()
    {
        return Physics.Raycast(mytrans.position, -Vector3.up, dtog + 0.1f);
    }

    private void FixedUpdate()
    {

        if (Input.GetAxis("Vertical") > 0)
        {
            //rb.AddForce(0f, 0f, Input.GetAxis("Vertical") * moveForce);              //starts too slow
            rb.velocity = Vector3.forward * Input.GetAxis("Vertical") * moveSpeed;
            Debug.Log(rb.velocity);
        }

        if (Input.GetAxis("Vertical") < 0)
        {
            rb.velocity = Vector3.forward * Input.GetAxis("Vertical") * moveSpeed * backpaddle;
        }

            if (Input.GetButton("Horizontal"))
        {
            //rb.AddForce(Input.GetAxis("Horizontal") * moveSpeed, 0f, 0f);
            rb.velocity = Vector3.right * Input.GetAxis("Horizontal") * moveSpeed;
        }

    }
}
