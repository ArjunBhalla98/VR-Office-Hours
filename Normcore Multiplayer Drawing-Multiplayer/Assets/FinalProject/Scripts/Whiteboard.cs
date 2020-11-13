using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    Transform m_GrabOffset;

    Rigidbody rb;
    OVRGrabbable m_GrabState;
    bool isGrabbed = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        m_GrabState = GetComponent<OVRGrabbable>();
        m_GrabOffset = GameObject.Find("WhiteboardGrabOffset").transform;
        m_GrabState.snapOffset = m_GrabOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if ((m_GrabState.isGrabbed && !isGrabbed))
        {
            isGrabbed = true;
            rb.constraints = RigidbodyConstraints.None;
        }
        else if ((!m_GrabState.isGrabbed && isGrabbed))
        {
            isGrabbed = false;
            rb.constraints = RigidbodyConstraints.FreezeAll; 
	    }

    }

    private void FixedUpdate()
    {
    }

    void OnCollisionEnter(Collision other)
    {
    }
}
