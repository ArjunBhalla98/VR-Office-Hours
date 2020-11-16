using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class WhiteboardEdge : MonoBehaviour
{
    [SerializeField]
    GameObject m_WhiteboardSurface;

    [SerializeField]
    Material m_HighlightMaterial;

    GameObject m_RightHand;
    Material originalMaterial;
    MeshRenderer meshRenderer;

    RealtimeTransform _realtimeTransform;
    RealtimeTransform _parentRealtimeTransform;

    bool isHovered = false;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;
        m_RightHand = GameObject.Find("RightControllerAnchor");
        
        // Normcore stuff
        _realtimeTransform = GetComponent<RealtimeTransform>();
        _parentRealtimeTransform = m_WhiteboardSurface.GetComponent<RealtimeTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_RightHand.transform.position,  m_RightHand.transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject == gameObject)
            {
                isHovered = true;
                meshRenderer.material = m_HighlightMaterial;

                if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
                {
                    _realtimeTransform.RequestOwnership();
                    _parentRealtimeTransform.RequestOwnership();
                    float beforePositionY = transform.position.y;
                    float afterPositionY = hit.point.y;
                    float scaleUpRatio = (afterPositionY - beforePositionY);
                    m_WhiteboardSurface.transform.localScale += new Vector3(scaleUpRatio, scaleUpRatio, 0);
			    }
			}
            else
            {
                isHovered = false;
                meshRenderer.material = originalMaterial;
	        }
        }
        else if (isHovered)
        {
            isHovered = false;
            meshRenderer.material = originalMaterial;
	    }
    }
}
