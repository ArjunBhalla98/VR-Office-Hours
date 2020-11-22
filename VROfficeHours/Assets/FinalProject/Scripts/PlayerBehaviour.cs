using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    Whiteboard m_WhiteboardPrefab;

    [SerializeField]
    Transform m_PlayerBase;

    public float m_WhiteboardSpawnOffset;
    public float m_penSpawnOffset;

    bool isSpawned = false;
    bool isPenSpawned = false;
    const float k_respawnTime = 1f;
    float respawnTimer;
    float penRespawnTimer;

    const string m_PrefabName = "WhiteboardPrefab";
    const string m_PenPrefabName = "sharpiePrefab";

    Realtime _realtime;

    // Start is called before the first frame update
    void Start()
    {
        respawnTimer = k_respawnTime;
        penRespawnTimer = k_respawnTime;
        _realtime = GetComponent<Realtime>();
        _realtime.didConnectToRoom += _realtime_didConnectToRoom;
    }

    private void _realtime_didConnectToRoom(Realtime realtime)
    {
        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One) && !isSpawned)
        {
            GameObject instantiatedWhiteboard = Realtime.Instantiate(m_PrefabName,
                position: m_PlayerBase.position + m_WhiteboardSpawnOffset * m_PlayerBase.forward,
                rotation: Quaternion.identity,
                ownedByClient: false,
                preventOwnershipTakeover: false,
                useInstance: _realtime);

            instantiatedWhiteboard.transform.LookAt(m_PlayerBase);
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

        if (OVRInput.Get(OVRInput.Button.Three) && !isPenSpawned)
        {
            GameObject instantiatedPen = Realtime.Instantiate(m_PenPrefabName,
                position: m_PlayerBase.position + m_WhiteboardSpawnOffset * m_PlayerBase.forward,
                rotation: Quaternion.identity,
                ownedByClient: false,
                preventOwnershipTakeover: false,
                useInstance: _realtime);

            instantiatedPen.transform.LookAt(m_PlayerBase);
            isPenSpawned = true;
        }
        else if (isPenSpawned)
        {
            penRespawnTimer -= Time.deltaTime;
            if (penRespawnTimer <= 0)
            {
                isPenSpawned = false;
                penRespawnTimer = k_respawnTime;
            }
        }
    }
}

