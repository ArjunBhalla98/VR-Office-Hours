using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WritingControl : OVRGrabbable 
{
    public WhiteBoard whiteboard;
    private RaycastHit touch;
    private bool lastTouch;
    private Quaternion lastPenAngle;

    // Start is called before the first frame update
    void Start()
    {
        whiteboard = GameObject.Find("Whiteboard").GetComponent<WhiteBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        Transform tipTransform = GameObject.Find("Tip").transform;
        float tipHeight = tipTransform.localScale.y;
        Vector3 tip = tipTransform.position; // These might be slow, TODO change here for multi pen

        if (Physics.Raycast(tip, tipTransform.up, out touch, tipHeight))
        {
            Debug.DrawRay(tip, tipTransform.up, Color.red, 200, true);
            Debug.Log("COLLIDER HIT WITH: " + touch.collider.tag);
            if (touch.collider.tag != "Whiteboard")
            {
                return;
            }
            whiteboard.SetColor(Color.blue);
            whiteboard.SetTouchPosition(touch.textureCoord.x, touch.textureCoord.y);
            whiteboard.SetTouch(true);
            Debug.Log("TOUCHING THE WHITEBOARD");

            //if (!lastTouch)
            //{
            //    lastTouch = true;
            //    lastPenAngle = transform.rotation;
            //}
        }
        else
        {
            //whiteboard.SetTouch(false);
            //lastTouch = false;
            //Debug.Log("AWAY AND NO CONTACT WITH WHITEBOARD");
	    }

     //   if (lastTouch)
     //   {
     //       transform.rotation = lastPenAngle;
	    //}
    }
}
