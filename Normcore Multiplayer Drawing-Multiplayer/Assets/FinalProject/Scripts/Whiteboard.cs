using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Whiteboard : MonoBehaviour
{
    Transform m_GrabOffset;

    Rigidbody rb;
    OVRGrabbable m_GrabState;
    bool isGrabbed = false;

    /// <summary>
    /// Using texture mapping on the whiteboard
    /// </summary>
    private int _textureSize = 2048;
    private int _penSize = 10;
    private Texture2D _texture;
    private Color[] _color;

    private bool _touching, _touchingLast;
    private float _posX, _posY;
    private float _lastX, lastY;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        m_GrabState = GetComponent<OVRGrabbable>();
        m_GrabOffset = GameObject.Find("WhiteboardGrabOffset").transform;
        m_GrabState.snapOffset = m_GrabOffset;

        Renderer renderer = GetComponent<Renderer>();
        this._texture = new Texture2D(_textureSize, _textureSize);
        renderer.material.mainTexture = this._texture;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_GrabState.isGrabbed && !isGrabbed)
        {
            isGrabbed = true;
            rb.constraints = RigidbodyConstraints.None;
        }
        else if (!m_GrabState.isGrabbed && isGrabbed)
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


    ///////// Texture mapping for writing

    public void ToggleTouch(bool touching)
    {
        this._touching = touching;
    }

    public void SetTouchPosition(float x, float y)
    {
        this._posX = x;
        this._posY = y;
    }

    public void SetColor(Color color)
    {
        this._color = Enumerable.Repeat<Color>(color, _penSize * _penSize).ToArray<Color>();
    }
}
