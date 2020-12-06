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

    int nSlots = 5;
    MeshRenderer mr;
    public bool isHighlighted = false;
    string defaultText = "Empty Slot";

    private Queue<string> peopleQueue;

    // Start is called before the first frame update
    void Start()
    {
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
        peopleQueue.Dequeue();
        peopleQueue.Enqueue(defaultText);
        UpdateTextDisplay();
    }

    void UpdateTextDisplay()
    { 
        int i = 0;
        foreach (string itemText in peopleQueue)
        {
            m_TextSlots[i].GetComponent<RealtimeTransform>().RequestOwnership();
            m_TextSlots[i].text = itemText;
            i++;
        }
    }
}
