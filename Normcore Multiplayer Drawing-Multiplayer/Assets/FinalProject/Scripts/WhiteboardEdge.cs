using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteboardEdge : MonoBehaviour
{
    [SerializeField]
    GameObject m_WhiteboardSurface;

    [SerializeField]
    GameObject m_RightHand;

    [SerializeField]
    Material m_HighlightMaterial;

    Material originalMaterial;
    MeshRenderer meshRenderer;

    bool isHovered = false;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;
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
                    // Simple, motion agnostic scale
                    //Vector3 whiteboardSurfaceScale = m_WhiteboardSurface.transform.localScale;
                    m_WhiteboardSurface.transform.localScale *= 1.2f;
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y * 1.2f, gameObject.transform.position.z);
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
