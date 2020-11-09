using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    Rigidbody rb;
    Vector3 restingPosition;
    Quaternion restingRotation;

    [SerializeField]
    Collider m_OuterCollider;

    [SerializeField]
    Collider m_InnerCollider;

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        //restingPosition = gameObject.transform.position;
        //restingRotation = gameObject.transform.rotation;
        Physics.IgnoreCollision(m_OuterCollider, m_InnerCollider, true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
	    //rb.velocity = Vector3.zero;
	    //rb.angularVelocity = Vector3.zero;
     //   rb.position = restingPosition;
     //   rb.rotation = restingRotation;
    }

    void OnCollisionEnter(Collision other)
    {
        //rb.AddForce(-other.rigidbody.velocity);
        //rb.Sleep();
        //   gameObject.transform.position = restingPosition;
        //   gameObject.transform.rotation = restingRotation;
        //rb.velocity = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;
        //rb.constraints = RigidbodyConstraints.FreezePosition;
    }
}
