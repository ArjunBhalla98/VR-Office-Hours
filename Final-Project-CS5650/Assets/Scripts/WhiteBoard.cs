using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WhiteBoard : MonoBehaviour
{
    private int textureSize = 2048;
    private int penSize = 10;
    private Texture2D texture;
    private Color[] colors;

    bool touching, touchingLast;
    private float posX, posY;
    float lastX, lastY;

    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        this.texture = new Texture2D(textureSize, textureSize);
        renderer.material.mainTexture = texture;
    }

    // Update is called once per frame
    void Update()
    {
        int x = (int)posX * textureSize - (penSize / 2);
        int y = (int)posY * textureSize - (penSize / 2);

        //if (touchingLast)
        //{
            //texture.SetPixels(x, y, penSize, penSize, colors);
            //Debug.Log("Position of X and Y: " + x + y);
            //SetIntermediatePixels(new Vector2(lastX, lastY), new Vector2(x, y));
            //texture.Apply();
	    //}

        lastX = (float)x;
        lastY = (float)y;
        touchingLast = touching;
    }

    private void SetIntermediatePixels(Vector2 startPos, Vector2 endPos)
    {
        for (float t = 0.01f; t < 1f; t += 0.01f)
        {
            Vector2 newPos = Vector2.Lerp(startPos, endPos, t);
            texture.SetPixels((int)newPos.x, (int)newPos.y, penSize, penSize, colors);
	    }
    }

    public void SetTouch(bool touching)
    {
        this.touching = touching;
    }

    public void SetTouchPosition(float x, float y)
    {
        posX = x;
        posY = y;
    }

    public void SetColor(Color color)
    {
        this.colors = Enumerable.Repeat<Color>(color, penSize * penSize).ToArray<Color>();
    }
}