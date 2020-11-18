﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Whiteboard : MonoBehaviour
{
    Transform m_GrabOffset;

    Rigidbody rb;
    OVRGrabbable m_GrabState;
    bool isGrabbed = false;

    //When the brush moves very fast, in order to avoid intermittent points,
    // it is necessary to interpolate between two points,
    // and LERP is the interpolation coefficient
    [Range(0, 1)]
    public float lerp = 0.05f;

    // Texture for writing on the board
    private Texture2D _texture;
    //The position of the brush is mapped to the UV coordinates of the board texture
    private Vector2 _paintPos;
    private bool _isTouching = false;

    //Where the brush is when you leave 
    private int _lastX, _lastY;

    //The size of the color block represented by the brush
    private int _painterTipsWidth = 30;
    private int _painterTipsHeight = 15;

    //The size of the background picture of the current palette
    private int _textureWidth;
    private int _textureHeight;

    //The color of the brush
    private Color32[] _color;


    //private int _textureSize = 2048;
    //private int _penSize = 10;
    ////private bool _touching, _touchingLast;
    //private int _posX, _posY;

    // Start is called before the first frame update
    void Start()
    {
        // Grabbing
        rb = GetComponent<Rigidbody>();
        m_GrabState = GetComponent<OVRGrabbable>();
        m_GrabOffset = GameObject.Find("WhiteboardGrabOffset").transform;
        m_GrabState.snapOffset = m_GrabOffset;

        //Renderer renderer = GetComponent<Renderer>();
        //this._texture = new Texture2D(_textureSize, _textureSize);
        //renderer.material.mainTexture = this._texture;

        //this.SetColor(Color.blue);
        //_texture.SetPixels(2000, 2000, _penSize, _penSize, _color);
        //_texture.Apply();

        //Get the size of the original texture of the board
        //Texture2D originTexture = this.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
        //Debug.Log("============");
        //Debug.Log(originTexture);
        _textureWidth = 1920; //1920 
        _textureHeight = 1080; //1080

        //Set current picture
        //_texture = new Texture2D(_textureWidth, _textureHeight, TextureFormat.RGBA32, false, true);
        //_texture.SetPixels32(originTexture.GetPixels32());
        //_texture.Apply();

        //Assign to whiteboard
        GetComponent<MeshRenderer>().material.mainTexture = _texture;

        //Initialize brush color
        _color = Enumerable.Repeat<Color32>(new Color32(255, 0, 0, 255), _painterTipsWidth * _painterTipsHeight).ToArray<Color32>();
    }

    // Update is called once per frame
    void Update()
    {
        // Grabbing
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

        //int x = (int)(_posX * _textureSize - (_penSize / 2));
        //int y = (int)(_posY * _textureSize - (_penSize / 2));

        //if (_touchingLast)
        //{
        //    _texture.SetPixels32(x, y, _penSize, _penSize, _color);
        //    _texture.Apply();
        //}

        //this._lastX = (float)x;
        //this._lastY = (float)y;

        //this._touchingLast = this._touching;
    }

    private void LateUpdate()
    {
        //Calculate the starting point of the color block represented by the current brush
        int texPosX = (int)(_paintPos.x * (float)_textureWidth - (float)(_painterTipsWidth / 2));
        int texPosY = (int)(_paintPos.y * (float)_textureHeight - (float)(_painterTipsHeight / 2));
        if (_isTouching)
        {
            //Change the pixel value of the block where the brush is located
            _texture.SetPixels32(texPosX, texPosY, _painterTipsWidth, _painterTipsHeight, _color);

            //If you move the brush quickly, there will be intermittent phenomenon, so interpolation is needed
            if (_lastX != 0 && _lastY != 0)
            {
                int lerpCount = (int)(1 / lerp);
                for (int i = 0; i <= lerpCount; i++)
                {
                    int x = (int)Mathf.Lerp((float)_lastX, (float)texPosX, lerp);
                    int y = (int)Mathf.Lerp((float)_lastY, (float)texPosY, lerp);
                    _texture.SetPixels32(x, y, _painterTipsWidth, _painterTipsHeight, _color);
                }
            }
            _texture.Apply();
            _lastX = texPosX;
            _lastY = texPosY;
        }
        else
        {
            _lastX = _lastY = 0;
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
        this._isTouching = touching;
    }

    //public void SetColor(Color color)
    //{
    //    this._color = Enumerable.Repeat<Color>(color, _penSize * _penSize).ToArray<Color>();
    //}

    /// <summary>
    ///Set the UV position of the current brush
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetTouchPositon(float x, float y)
    {
        _paintPos.Set(x, y);
    }

    /// <summary>
    ///Is the brush drawing at present
    /// </summary>
    public bool IsDrawing
    {
        get
        {
            return _isTouching;
        }
        set
        {
            _isTouching = value;
        }
    }

    /// <summary>
    ///Use the color of the brush currently on the palette
    /// </summary>
    /// <param name="color"></param>
    public void SetColor(Color32 color)
    {
        if (!_color[0].IsEqual(color))
        {
            for (int i = 0; i < _color.Length; i++)
            {
                _color[i] = color;
            }
        }
    }
}

public static class MethodExtention
{
    /// <summary>
    ///Used to compare two color32 types for the same color
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="compare"></param>
    /// <returns></returns>
    public static bool IsEqual(this Color32 origin, Color32 compare)
    {
        if (origin.g == compare.g && origin.r == compare.r)
        {
            if (origin.a == compare.a && origin.b == compare.b)
            {
                return true;
            }
        }
        return false;
    }
}