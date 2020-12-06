using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.XR;

public class Pen : MonoBehaviour
{
    [SerializeField]
    GameObject m_PenTip;


    // Prefab to instantiate when we draw a new brush stroke
    //[SerializeField] private GameObject _brushStrokePrefab = null;

    // Which hand should this brush instance track?
    private enum Hand { LeftHand, RightHand };
    [SerializeField] private Hand _hand = Hand.RightHand;
    [SerializeField]
    GameObject rightHand;

    [SerializeField]
    Material PenGreen;

    [SerializeField]
    Material PenBlue;

    [SerializeField]
    Material PenYellow;

    [SerializeField]
    Material PenRed;

    [SerializeField]
    Material PenEraser;

    GameObject penSnapOrientation;

    const float hapticFeedbackLength = 0.15f;
    float hapticFeedbackTimeLeft = hapticFeedbackLength;
    // Used to keep track of the current brush tip position and the actively drawing brush stroke
    private Vector3 _handPosition;
    private Quaternion _handRotation;
    float lastDepthPosition;

    // board writing on
    public Whiteboard _whiteboard;
    private RaycastHit _touch;
    RaycastHit _stillInRange;
    private bool _isTouching; // Is the pentip is in contact with the whiteboard
    float raycastLength = 0.11f; 

    // For making sure the pen doesn't go through the board
    OVRGrabbable m_GrabState;

    // Define Colours to be used
    Color32 greenPenColour = new Color32(0, 190, 0, 255);
    Color32 bluePenColour = new Color32(0, 0, 255, 255);
    Color32 redPenColour = new Color32(255, 0, 0, 255);
    Color32 yellowPenColour = new Color32(255, 255, 0, 255);
    Color32[] penColours;
    Material[] penMaterials;
    bool eraseMode = false;

    private const int nColours = 4;
    private int counter = 0;
    private Color32 currentPenColour;
    private Material currentPenMaterial;
    private MeshRenderer penTipRenderer;

    private const float eraseCooldownTime = 0.26f;
    private float eraseTimer = eraseCooldownTime;
    private bool eraseTriggered = false;

    private Color32 _boardColor = new Color32(255, 255, 255, 255);
    private Color32 _prevPenColor;
    private Material _prevPenMaterial;

    // Start is called before the first frame update
    void Start()
    {
        _whiteboard = GameObject.Find("Whiteboard").GetComponent<Whiteboard>();
        m_GrabState = GetComponent<OVRGrabbable>();
        _isTouching = false;
        penSnapOrientation = GameObject.Find("PenSnap");
        m_GrabState.snapOffset = penSnapOrientation.transform;
        penColours = new Color32[nColours] { greenPenColour, bluePenColour, redPenColour, yellowPenColour };
        penMaterials = new Material[nColours] { PenGreen, PenBlue, PenRed, PenYellow };
        penTipRenderer = m_PenTip.GetComponent<MeshRenderer>();
        currentPenColour = greenPenColour;
        _prevPenColor = currentPenColour;
        currentPenMaterial = PenGreen;
        _prevPenMaterial = currentPenMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        // returns true if the “Y” button was released this frame.
        if(OVRInput.GetUp(OVRInput.RawButton.Y))
        {
            Switch();
        }
        
        // Makes it stop rapid switching between eraser and pen
	    if (eraseTriggered)
	    {
			eraseTimer -= Time.deltaTime;

			if (eraseTimer < 0)
			{
			    eraseTimer = eraseCooldownTime;
			    eraseTriggered = false;
			}
		}

        if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger) && !eraseTriggered && m_GrabState.isGrabbed)
        {
            eraseTriggered = true;

            if (eraseMode)
            {
                currentPenColour = _prevPenColor;
                currentPenMaterial =_prevPenMaterial;
                penTipRenderer.material = currentPenMaterial;
                eraseMode = false;
            }
            else
            {

                _prevPenColor = new Color32(currentPenColour.r, currentPenColour.g, currentPenColour.b, currentPenColour.a);
                _prevPenMaterial = currentPenMaterial;
			    currentPenColour = _boardColor;
			    currentPenMaterial = PenEraser;
			    penTipRenderer.material = currentPenMaterial;
                eraseMode = true;
		    
	        }
        }

        if (Physics.Raycast(transform.position, transform.forward, out _touch, raycastLength))
        {
            if (!(_touch.collider.gameObject.CompareTag("Whiteboard")))
                return;


            _whiteboard = _touch.collider.gameObject.GetComponent<Whiteboard>();
            RealtimeTransform whiteboardTransform = _whiteboard.GetComponent<RealtimeTransform>();
            whiteboardTransform.RequestOwnership();


            if (!_isTouching)
            {
			    // Haptic feedback for writing, vibrates when the controller
			    // touches the Whiteboard for the first time: per Prof. Haraldsson feedback
			    OVRInput.SetControllerVibration(1f, 0.5f, OVRInput.Controller.RTouch);
                hapticFeedbackTimeLeft = hapticFeedbackLength;
                lastDepthPosition = _whiteboard.xAxisSnap ? transform.position.x : transform.position.z;
                _whiteboard.previousTexture = (Texture2D) _touch.collider.gameObject.GetComponent<MeshRenderer>().material.mainTexture;
                _whiteboard.previousWrittenPixels = new List<List<int>>();
            }

			_whiteboard.SetColor(currentPenColour);

            if (hapticFeedbackTimeLeft < 0)
            {
                // Haptic feedback for writing, stop vibration after initial touch 
                OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
            }
            else
            {
                hapticFeedbackTimeLeft -= Time.deltaTime;
	        }

            if (_whiteboard.xAxisSnap)
            {
                transform.position = new Vector3(lastDepthPosition, transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, lastDepthPosition);
	        }

            _isTouching = true;

			_whiteboard.SetTouchPositon(_touch.textureCoord.x, _touch.textureCoord.y);

            //Current pen color
            _whiteboard.IsDrawing = true;
        }
        else
        {
			OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
		    _isTouching = false;
            _whiteboard.IsDrawing = false;
        }
    }

    void Switch()
    {
        if (m_GrabState.isGrabbed)
        { 
			currentPenColour = penColours[++counter % nColours];
            _prevPenColor = currentPenColour;
			currentPenMaterial = penMaterials[counter % nColours];
			penTipRenderer.material = currentPenMaterial;
	    }
    }
}
