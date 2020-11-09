using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pen : MonoBehaviour
{
    [SerializeField]
    GameObject m_PenTip;

    LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>(); 
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, m_PenTip.transform.position);
        lr.SetPosition(1, m_PenTip.transform.position + m_PenTip.transform.forward);

        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward))
        { 
	        
	    }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
