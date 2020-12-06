using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Normal.Realtime;
using System.Linq;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    Whiteboard m_WhiteboardPrefab;

    [SerializeField]
    Transform m_PlayerBase;

    [SerializeField]
    Text m_NameTagField;

    public string PlayerName = "Default";
    bool playerNameUpdated = false;

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
        _realtime.didConnectToRoom += DidConnectToRoom;
    }

    private void DidConnectToRoom(Realtime realtime)
    { 
	    GameObject[] allNameTags = GameObject.FindGameObjectsWithTag("NamePlate");
	    GameObject closestNameTag = allNameTags[0];
	    float minDistance = 9999999999;
	    foreach (GameObject obj in allNameTags)
	    {
		float dist = Vector3.Distance(m_PlayerBase.position, obj.transform.position);
		if (dist < minDistance)
		{
		    minDistance = dist;
		    closestNameTag = obj;
			} 
		}

	    closestNameTag.GetComponent<RealtimeTransform>().RequestOwnership();
	    closestNameTag.GetComponent<Text>().text = PlayerName;
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

