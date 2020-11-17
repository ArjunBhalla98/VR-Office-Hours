using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.XR;

public class Pen : MonoBehaviour
{
    [SerializeField]
    GameObject m_PenTip;

    // Start Writing
    // Reference to Realtime to use to instantiate brush strokes
    [SerializeField] private Realtime _realtime = null;

    // Prefab to instantiate when we draw a new brush stroke
    [SerializeField] private GameObject _brushStrokePrefab = null;

    // Which hand should this brush instance track?
    private enum Hand { LeftHand, RightHand };
    [SerializeField] private Hand _hand = Hand.RightHand;

    // Used to keep track of the current brush tip position and the actively drawing brush stroke
    private Vector3 _handPosition;
    private Quaternion _handRotation;
    private BrushStroke _activeBrushStroke;

    public Whiteboard whiteboard;
    private RaycastHit touch;
    private bool isTouching; // Is the pentip is in contact with the whiteboard
    // End Writing

    // LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        //lr = GetComponent<LineRenderer>();

        // Start Writing
        this.whiteboard = GameObject.Find("Whiteboard").GetComponent<Whiteboard>();

        isTouching = false;
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
        float tipHeight = 0.3f;
        Vector3 tip = m_PenTip.transform.position;

        if (Physics.Raycast(tip, transform.up, out touch, tipHeight)) {
            if (!(touch.collider.tag == "Whiteboard"))
                return;

            this.whiteboard = touch.collider.GetComponent<Whiteboard>();

            isTouching = true;

            // Haptic feedback for writing, vibrates when the controller
            // touches the Whiteboard
            OVRInput.SetControllerVibration(1f, 0.1f, OVRInput.Controller.RTouch);
        } else
        {
            isTouching = false;

            // Haptic feedback for writing, stop vibration when controller
            // leaves the Whiteboard
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        }

        UpdateDrawBrush();
        // End Writing
    }


    void UpdateDrawBrush()
    {
        if (!_realtime.connected)
            return;

        // Start by figuring out which hand we're tracking
        XRNode node = _hand == Hand.LeftHand ? XRNode.LeftHand : XRNode.RightHand;
        string trigger = _hand == Hand.LeftHand ? "Left Trigger" : "Right Trigger";

        // Get the position & rotation of the hand
        bool handIsTracking = UpdatePose(node, ref _handPosition, ref _handRotation);

        // Figure out if the trigger is pressed or not
        bool triggerPressed = Input.GetAxisRaw(trigger) > 0.1f;

        // If we lose tracking, stop drawing
        if (!handIsTracking)
            triggerPressed = false;

        // If the trigger is pressed and we are in contact with the whiteboard,
        // and we haven't created a new brush stroke to draw, create one!
        if (triggerPressed && isTouching && _activeBrushStroke == null)
        {
            // Instantiate a copy of the Brush Stroke prefab, set it to be owned by us.
            GameObject brushStrokeGameObject = Realtime.Instantiate(_brushStrokePrefab.name, ownedByClient: true, useInstance: _realtime);

            // Grab the BrushStroke component from it
            _activeBrushStroke = brushStrokeGameObject.GetComponent<BrushStroke>();

            // Tell the BrushStroke to begin drawing at the current brush position
            //_activeBrushStroke.BeginBrushStrokeWithBrushTipPoint(_handPosition, _handRotation);
            _activeBrushStroke.BeginBrushStrokeWithBrushTipPoint(m_PenTip.transform.position, m_PenTip.transform.rotation);
        }

        // If the trigger is pressed, and we have a brush stroke, move the brush stroke to the new brush tip position
        if (triggerPressed && isTouching)
            //_activeBrushStroke.MoveBrushTipToPoint(_handPosition, _handRotation);
            _activeBrushStroke.MoveBrushTipToPoint(m_PenTip.transform.position, m_PenTip.transform.rotation);

        // If the trigger is no longer pressed, and we still have an active brush stroke, mark it as finished and clear it.
        if (!triggerPressed && _activeBrushStroke != null)
        {
            //_activeBrushStroke.EndBrushStrokeWithBrushTipPoint(_handPosition, _handRotation);
            _activeBrushStroke.EndBrushStrokeWithBrushTipPoint(m_PenTip.transform.position, m_PenTip.transform.rotation);
            _activeBrushStroke = null;
        }
    }

    //// Utility

    // Given an XRNode, get the current position & rotation. If it's not tracking, don't modify the position & rotation.
    private static bool UpdatePose(XRNode node, ref Vector3 position, ref Quaternion rotation)
    {
        List<XRNodeState> nodeStates = new List<XRNodeState>();
        InputTracking.GetNodeStates(nodeStates);

        foreach (XRNodeState nodeState in nodeStates)
        {
            if (nodeState.nodeType == node)
            {
                Vector3 nodePosition;
                Quaternion nodeRotation;
                bool gotPosition = nodeState.TryGetPosition(out nodePosition);
                bool gotRotation = nodeState.TryGetRotation(out nodeRotation);

                if (gotPosition)
                    position = nodePosition;
                if (gotRotation)
                    rotation = nodeRotation;

                return gotPosition;
            }
        }

        return false;
    }
}
