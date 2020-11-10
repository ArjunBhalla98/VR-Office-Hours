using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    Whiteboard m_WhiteboardPrefab;

    [SerializeField]
    Transform m_PlayerBase;

    public float m_WhiteboardSpawnOffset;
    

    bool isSpawned = false;
    const float k_respawnTime = 1f;
    float respawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        respawnTimer = k_respawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One) && !isSpawned)
        {
            Instantiate(m_WhiteboardPrefab, m_PlayerBase.position + m_WhiteboardSpawnOffset * m_PlayerBase.forward, Quaternion.identity);
            isSpawned = true;
        }
        else if (isSpawned)
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0)
            {
                isSpawned = false;
                respawnTimer = k_respawnTime; 
	        }
	    }
    }
}
