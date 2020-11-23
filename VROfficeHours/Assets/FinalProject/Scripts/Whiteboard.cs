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

    // When the brush moves very fast, in order to avoid intermittent points,
    // it is necessary to interpolate between two points,
    // and LERP is the interpolation coefficient
    [Range(0, 1)]
    public float lerp = 0.001f;

    GameObject playerAnchor;
    public bool xAxisSnap = false;
    // Texture for writing on the board
    private Texture2D _texture;
    //The position of the brush is mapped to the UV coordinates of the board texture
    private Vector2 _paintPos;
    private bool _isTouching = false;

    //Where the brush is when you leave 
    private int _lastX, _lastY;

    //The size of the color block represented by the brush
    private int _painterTipsWidth = 5;
    private int _painterTipsHeight = 5;

    //The size of the background picture of the current palette
    private Vector3 _localScale;
    private float _localScaleX, _localScaleY;
    private int _textureWidth;
    private int _textureHeight;
    private bool isWhitboradReshaped = false;

    //The color of the brush
    private Color32[] _color;


    // Start is called before the first frame update
    void Start()
    {
        // Grabbing
        rb = GetComponent<Rigidbody>();
        m_GrabState = GetComponent<OVRGrabbable>();
        m_GrabOffset = GameObject.Find("WhiteboardGrabOffset").transform;
        m_GrabState.snapOffset = m_GrabOffset;
        playerAnchor = GameObject.Find("CenterEyeAnchor");
        // TODO: attempt to get the texture from the meshrenderer,
        // right now it's non-existant and the dimension of the texture is hardcoded
        // i.e. our board is 1.98 x 1.08 -> 1980x1080 right now

        //Get the size of the original texture of the board
        //Texture2D originTexture = (Texture2D)this.GetComponent<MeshRenderer>().material.mainTexture;

        //TODO: could change the size of the board
        _localScale = transform.localScale;
        _textureWidth = (int)(_localScale.x * 1000); 
        _textureHeight = (int)(_localScale.y * 1000); 

        //Set current texture
        _texture = new Texture2D(_textureWidth, _textureHeight, TextureFormat.RGBA32, false, true);
        //_texture.SetPixels32(originTexture.GetPixels32());
        _texture.Apply();

        //Assign to whiteboard
        GetComponent<MeshRenderer>().material.mainTexture = _texture;

        //Initialize brush color
        _color = Enumerable.Repeat<Color32>(new Color32(255, 0, 0, 255), _painterTipsWidth * _painterTipsHeight).ToArray<Color32>();
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
            // Snap to axis calculations
            transform.LookAt(playerAnchor.transform);
            Vector3 currentRotationEuler = transform.eulerAngles;
            float xStraightSnapAngle = Mathf.Abs(currentRotationEuler.y - 90) % 360;
            float xReverseSnapAngle = Mathf.Abs(currentRotationEuler.y + 90) % 360;
            float zStraightSnapAngle = Mathf.Abs(currentRotationEuler.y) % 360;
            float zReverseSnapAngle = Mathf.Abs(currentRotationEuler.y + 180) % 360;
            float minAngle = Mathf.Min(xStraightSnapAngle, xReverseSnapAngle, zStraightSnapAngle, zReverseSnapAngle);

            if (minAngle == xStraightSnapAngle)
            {
                transform.rotation = Quaternion.Euler(0, -90, 0);
                xAxisSnap = true;
            }
            else if (minAngle == xReverseSnapAngle)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
                xAxisSnap = true;
            }
            else if (minAngle == zStraightSnapAngle)
            {
                transform.rotation = Quaternion.Euler(0, -180, 0);
                xAxisSnap = false;
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                xAxisSnap = false;
	        }
            isGrabbed = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        // If whiteboard is resized
        if (transform.localScale != _localScale)
        {
            _localScale = transform.localScale;
        }

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


    ///////// Texture mapping for writing

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
    ///Set the UV position of the current brush
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetTouchPositon(float x, float y)
    {
        _paintPos.Set(x, y);
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


//// Utility


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
