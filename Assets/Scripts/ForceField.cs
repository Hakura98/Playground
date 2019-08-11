using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    public float minPower = 100;
    public float maxPower = 150f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Vector3 ClosestPoint = other.ClosestPointOnBounds(transform.position);
            Vector3 ForceDirection = ClosestPoint - transform.position;
            float magnitude = Vector3.Distance(transform.position, ClosestPoint);

            float radius = GetComponent<SphereCollider>().bounds.extents.x;

            float powerToBeApplied = Mathf.Lerp(maxPower, minPower, magnitude / radius);

            other.GetComponent<Rigidbody>().AddForce(ForceDirection.normalized *
                powerToBeApplied, ForceMode.Impulse);
        }
    }

}
