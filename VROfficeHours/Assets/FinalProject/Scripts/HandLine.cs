﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Normal.Realtime;

public class HandLine : MonoBehaviour
{
    [SerializeField]
    string m_PlayerName;

    [SerializeField]
    QueueBoard QueueBoard; 

    bool isQueuePressed = false;
    const float maxResetTime = 1f;
    float currentResetTime = maxResetTime;
    LineRenderer lr;
    float lineDistance = 10;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    void MakeNameTag()
    {
        GameObject[] allNameTags = GameObject.FindGameObjectsWithTag("NamePlate");
        GameObject closestNameTag = allNameTags[0];
        float minDistance = 9999999999;
	    foreach (GameObject obj in allNameTags)
	    {
			float dist = Vector3.Distance(transform.position, obj.transform.position);
			if (dist < minDistance)
			{
				    minDistance = dist;
				    closestNameTag = obj;
			}
        }

        closestNameTag.GetComponent<RealtimeTransform>().RequestOwnership();
        closestNameTag.GetComponent<Text>().text = m_PlayerName;
    }

    // Update is called once per frame
    void Update()
    {
        MakeNameTag();
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
            }
            else if (obj.CompareTag("OHQ"))
            {
                obj.GetComponent<RealtimeTransform>().RequestOwnership();
                QueueBoard = obj.GetComponent<QueueBoard>();
                QueueBoard.isHighlighted = true;

                if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && !isQueuePressed)
                {
                    QueueBoard.AddStudent(m_PlayerName);
                    isQueuePressed = true;
                    
                }
                else if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && !isQueuePressed)
                {
                    QueueBoard.RemoveStudent();
                    isQueuePressed = true;
		        }
            }
			else
			{
			    QueueBoard.isHighlighted = false;
			}
		}

        if (isQueuePressed)
        {
            currentResetTime -= Time.deltaTime;
            if (currentResetTime < 0)
            {
                isQueuePressed = false;
                currentResetTime = maxResetTime;
	        }
	    }

    }
}
