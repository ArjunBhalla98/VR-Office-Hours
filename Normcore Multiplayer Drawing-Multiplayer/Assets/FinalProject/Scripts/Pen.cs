using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pen : MonoBehaviour
{
    [SerializeField]
    GameObject m_PenTip;

    // Start Writing 
    public Whiteboard whiteboard;
    private RaycastHit touch;
    // End Writing

    LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();

        // Start Writing
        this.whiteboard = GameObject.Find("Whiteboard").GetComponent<Whiteboard>();
        // End Writing 
    }

    // Update is called once per frame
    void Update()
    {
        //   lr.SetPosition(0, m_PenTip.transform.position);
        //   lr.SetPosition(1, m_PenTip.transform.position + m_PenTip.transform.forward);

        //   if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward))
        //   { 

        //}
        //float tipHeight = m_PenTip.transform.localScale.y;

        // Start Writing
        float tipHeight = 0.1f;
        Vector3 tip = m_PenTip.transform.position;

        if (Physics.Raycast(tip, transform.up, out touch, tipHeight)) {
            if (!(touch.collider.tag == "Whiteboard"))
                return;

            this.whiteboard = touch.collider.GetComponent<Whiteboard>();

            // Haptic feedback for writing, vibrates when the controller touches the Whiteboard
            OVRInput.SetControllerVibration(1f, 0.1f, OVRInput.Controller.RTouch);
        } else
        {
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }
        // End Writing
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
