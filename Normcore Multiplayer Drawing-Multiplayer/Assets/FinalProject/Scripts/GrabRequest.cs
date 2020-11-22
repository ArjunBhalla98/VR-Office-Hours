using UnityEngine;
using Normal.Realtime;

public class GrabRequest : MonoBehaviour
{
    private Realtime _realtime;
    private RealtimeView _realtimeView;
    private RealtimeTransform _realtimeTransform;

    private string _sharpiePrefabName;
    private GameObject _currentSharpie;
    private Vector3 _originalSharpiePosition;
    private Quaternion _originalSharpieRotation;

    private void Start()
    {
        _realtime = GetComponent<Realtime>();
        _realtimeView = GetComponent<RealtimeView>();
        _realtimeTransform = GetComponent<RealtimeTransform>();

        _sharpiePrefabName = "sharpie Variant";
        _originalSharpiePosition = new Vector3(gameObject.transform.position.x,
                                               gameObject.transform.position.y,
                                               gameObject.transform.position.z);
        _originalSharpieRotation = gameObject.transform.rotation;
        _currentSharpie = this.gameObject;
    }

    private void Update()
    {
        if (gameObject.GetComponent<OVRGrabbable>().isGrabbed)
        {
            _realtimeTransform.RequestOwnership();
        }
        

        GameObject instantiatedSharpie = Realtime.Instantiate(_sharpiePrefabName,
            position: _originalSharpiePosition + Vector3.forward,
            rotation: _originalSharpieRotation,
            ownedByClient: false,
            preventOwnershipTakeover: false,
            useInstance: _realtime);
    }
}
