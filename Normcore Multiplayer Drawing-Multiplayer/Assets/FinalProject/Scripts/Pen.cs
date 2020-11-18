using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.XR;

public class Pen : MonoBehaviour
{
    [SerializeField]
    GameObject m_PenTip;

    ///////////////// Start Writing ////////////////

    /// <summary>
    /// Using Normcore brush mesh
    /// </summary>
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

    // board writing on
    public Whiteboard _whiteboard;
    private RaycastHit _touch;
    private bool _isTouching; // Is the pentip is in contact with the whiteboard
    float tipHeight = 0.3f; // TODO: fine tune the distance to make writing smoother

    // color of the brush
    public Color32 _penColor;
    ///////////////// End Writing ////////////////

    // LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        //lr = GetComponent<LineRenderer>();

        ///////////////// Start Writing ////////////////
        this._whiteboard = GameObject.Find("Whiteboard").GetComponent<Whiteboard>();

        _isTouching = false;
        ///////////////// END Writing ////////////////
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

        ///////////////// Start Writing ////////////////
        Vector3 tip = m_PenTip.transform.position;

        if (Physics.Raycast(tip, transform.up, out _touch, tipHeight)) {
            if (!(_touch.collider.tag == "Whiteboard"))
                return;

            this._whiteboard = _touch.collider.GetComponent<Whiteboard>();

            _isTouching = true;

            // Haptic feedback for writing, vibrates when the controller
            // touches the Whiteboard
            OVRInput.SetControllerVibration(1f, 0.1f, OVRInput.Controller.RTouch);

            //this._whiteboard.SetColor(Color.blue);
            //this._whiteboard.SetTouchPosition(_touch.textureCoord.x, _touch.textureCoord.y);
            //this._whiteboard.ToggleTouch(true);
            _whiteboard.SetTouchPositon(_touch.textureCoord.x, _touch.textureCoord.y);
            //Current pen color
            _whiteboard.SetColor(new Color32(0, 255, 0, 255));
            _whiteboard.IsDrawing = true;
        } else
        {
            _isTouching = false;

            // Haptic feedback for writing, stop vibration when controller
            // leaves the Whiteboard
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);

            this._whiteboard.ToggleTouch(false);
        }

        //UpdateDrawBrush(); // Using normcore ribben-like brush texture


        ///////////////// END Writing ////////////////
    }


    /// <summary>
    /// Modification of Normcore drawing by enabling drawing from the pentip
    /// using BrushStroke
    /// </summary>
//    void UpdateDrawBrush()
//    {
//        if (!_realtime.connected)
//            return;

//        // Start by figuring out which hand we're tracking
//        XRNode node = _hand == Hand.LeftHand ? XRNode.LeftHand : XRNode.RightHand;
//        string trigger = _hand == Hand.LeftHand ? "Left Trigger" : "Right Trigger";

//        // Get the position & rotation of the hand
//        bool handIsTracking = UpdatePose(node, ref _handPosition, ref _handRotation);

//        // Figure out if the trigger is pressed or not
//        bool triggerPressed = Input.GetAxisRaw(trigger) > 0.1f;

//        // If we lose tracking, stop drawing
//        if (!handIsTracking)
//            triggerPressed = false;

//        // If the trigger is pressed and PenTip touches the board,
//        // and we haven't created a new brush stroke to draw, create one!
//        if (triggerPressed && _isTouching && _activeBrushStroke == null)
//        {
//            // Instantiate a copy of the Brush Stroke prefab, set it to be owned by us.
//            GameObject brushStrokeGameObject = Realtime.Instantiate(_brushStrokePrefab.name, ownedByClient: true, useInstance: _realtime);

//            // Grab the BrushStroke component from it
//            _activeBrushStroke = brushStrokeGameObject.GetComponent<BrushStroke>();

//            // Tell the BrushStroke to begin drawing at the current brush position
//            //_activeBrushStroke.BeginBrushStrokeWithBrushTipPoint(_handPosition, _handRotation);
//            _activeBrushStroke.BeginBrushStrokeWithBrushTipPoint(m_PenTip.transform.position, m_PenTip.transform.rotation);
//        }

//        // If the trigger is pressed and PenTip touches the board,
//        // and we have a brush stroke, move the brush stroke to the new brush tip position
//        if (triggerPressed && _isTouching)
//            //_activeBrushStroke.MoveBrushTipToPoint(_handPosition, _handRotation);
//            _activeBrushStroke.MoveBrushTipToPoint(m_PenTip.transform.position, m_PenTip.transform.rotation);

//        // If the trigger is no longer pressed and the PenTip touches the board,
//        // and we still have an active brush stroke, mark it as finished and clear it.
//        if (!triggerPressed && _isTouching && _activeBrushStroke != null)
//        {
//            //_activeBrushStroke.EndBrushStrokeWithBrushTipPoint(_handPosition, _handRotation);
//            _activeBrushStroke.EndBrushStrokeWithBrushTipPoint(m_PenTip.transform.position, m_PenTip.transform.rotation);
//            _activeBrushStroke = null;
//        }
//    }

//    //// Utility

//    // Given an XRNode, get the current position & rotation. If it's not tracking, don't modify the position & rotation.
//    private static bool UpdatePose(XRNode node, ref Vector3 position, ref Quaternion rotation)
//    {
//        List<XRNodeState> nodeStates = new List<XRNodeState>();
//        InputTracking.GetNodeStates(nodeStates);

//        foreach (XRNodeState nodeState in nodeStates)
//        {
//            if (nodeState.nodeType == node)
//            {
//                Vector3 nodePosition;
//                Quaternion nodeRotation;
//                bool gotPosition = nodeState.TryGetPosition(out nodePosition);
//                bool gotRotation = nodeState.TryGetRotation(out nodeRotation);

//                if (gotPosition)
//                    position = nodePosition;
//                if (gotRotation)
//                    rotation = nodeRotation;

//                return gotPosition;
//            }
//        }

//        return false;
//    }
}
