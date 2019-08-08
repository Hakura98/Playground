using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Transform playerTrans;
    private Vector3 offset;


    [SerializeField]
    [Range(0.1f, 1f)]
    private float smoothness = 0.1f;
    [SerializeField]
    private float rotationspeed = 1f;

    void Start()
    {
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - playerTrans.position;
    }


    private void LateUpdate()
    {
        Quaternion rot = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationspeed, Vector3.up);
        offset = rot * offset;

        float zoomFactor = (1 + -Input.GetAxis("Mouse ScrollWheel"));
        offset = zoomFactor*offset;
        
        Vector3 newPos = playerTrans.position + offset;

        transform.position = Vector3.Slerp(transform.position, newPos, smoothness);
        transform.LookAt(playerTrans.position + playerTrans.up);
    }
}
