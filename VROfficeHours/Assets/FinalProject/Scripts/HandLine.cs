using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class HandLine : MonoBehaviour
{
    LineRenderer lr;
    float lineDistance = 10;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();        
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + lineDistance * transform.forward);

        RaycastHit _out;
        if (Physics.Raycast(transform.position, transform.forward, out _out, Mathf.Infinity))
        {
            GameObject obj = _out.collider.gameObject;

            if (obj.CompareTag("Whiteboard"))
            {
                if (OVRInput.Get(OVRInput.Button.Two))
                {
                    Realtime.Destroy(obj);
                }
	        }
	    }
    }
}
