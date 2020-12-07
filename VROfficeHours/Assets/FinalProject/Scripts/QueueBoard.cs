using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Normal.Realtime;

public class QueueBoard : MonoBehaviour
{
    [SerializeField]
    Material m_HighlightMaterial;

    [SerializeField]
    Material defaultMaterial;

    [SerializeField]
    Text[] m_TextSlots;

    [SerializeField]
    Canvas canvas;

    int nSlots = 5;
    MeshRenderer mr;
    public bool isHighlighted = false;
    string defaultText = "Empty Slot";

    private Queue<string> peopleQueue;
    private RealtimeTransform localRealtimeTransform;
    private RealtimeTransform canvasRealtimeTransform;

    // Start is called before the first frame update
    void Start()
    {
        localRealtimeTransform = GetComponent<RealtimeTransform>();
        canvasRealtimeTransform = canvas.GetComponent<RealtimeTransform>();
        mr = GetComponent<MeshRenderer>();
        defaultMaterial = mr.material;
        peopleQueue = new Queue<string>();
        foreach (Text text in m_TextSlots)
        {
            peopleQueue.Enqueue(text.text);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isHighlighted)
        {
            mr.material = m_HighlightMaterial;
        }
        else
        {
            mr.material = defaultMaterial;
        }
    }

    public void AddStudent(string student)
    {
        localRealtimeTransform.RequestOwnership();
        canvasRealtimeTransform.RequestOwnership();
        bool hasSpace = false;
        foreach (string name in peopleQueue)
        {
            if (name.Equals(defaultText))
            {
                hasSpace = true;
	        }
	    }

        if (!hasSpace)
        {
            return;
	    }

        peopleQueue.Dequeue();
        peopleQueue.Enqueue(student);
        UpdateTextDisplay();
    }

    public void RemoveStudent()
    {
        localRealtimeTransform.RequestOwnership();
        canvasRealtimeTransform.RequestOwnership();
        peopleQueue.Dequeue();
        peopleQueue.Enqueue(defaultText);
        UpdateTextDisplay();
    }

    void UpdateTextDisplay()
    { 
        localRealtimeTransform.RequestOwnership();
        canvasRealtimeTransform.RequestOwnership();
        int i = 0;
        foreach (string itemText in peopleQueue)
        {
            m_TextSlots[i].GetComponent<RealtimeTransform>().RequestOwnership();
            m_TextSlots[i].text = itemText;
            i++;
        }
    }
}
