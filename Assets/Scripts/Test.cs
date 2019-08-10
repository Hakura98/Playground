using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    protected CharacterController m_characterController;
    

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
    }


    // Update is called once per frame
    void Update()
    {
        Vector3 movement = transform.forward * Time.deltaTime * 1.5f;
        m_characterController.Move(movement);
        Debug.Log(m_characterController.isGrounded);

        Quaternion a = new Quaternion(0, 0, 0, 1);
        Quaternion b = new Quaternion(0, 0, 0, 1);

        Quaternion c = Quaternion.Lerp(a, b, 0.5f);
        Debug.Log(c);
    }
}
