using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class HandLine : MonoBehaviour
{
    LineRenderer lr;
    float lineDistance = 10;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();        
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + lineDistance * transform.forward);

        RaycastHit _out;
        if (Physics.Raycast(transform.position, transform.forward, out _out, Mathf.Infinity))
        {
            GameObject obj = _out.collider.gameObject;

            if (obj.CompareTag("Whiteboard"))
            {
                if (OVRInput.Get(OVRInput.Button.Two))
                {
                    Realtime.Destroy(obj);
                }
                else if (OVRInput.Get(OVRInput.Button.Four))
                {
                    //Texture2D oldTexture = obj.GetComponent<Whiteboard>().previousTexture;
                    //obj.GetComponent<MeshRenderer>().material.mainTexture = oldTexture;
                    //oldTexture.Apply();
                    Whiteboard wb = obj.GetComponent<Whiteboard>();
                    List<List<int>> prevPixels = wb.previousWrittenPixels;
                    MeshRenderer whiteboardMr = obj.GetComponent<MeshRenderer>();

		            foreach (List<int> posList in prevPixels)
                    {
                        wb._texture.SetPixels32(posList[0], posList[1], wb._painterTipsWidth + 2, wb._painterTipsHeight + 2, wb._colorWhite);
		            }

                    wb._texture.Apply();
		        }
	        }
	    }
    }
}
