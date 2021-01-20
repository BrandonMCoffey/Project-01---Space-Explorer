using UnityEngine;

namespace Assets.Scripts {
    public class CameraFollow : MonoBehaviour {
        [SerializeField] private Transform _objectToFollow = null;

        private Vector3 _objectOffset;

        private void Awake()
        {
            if (_objectToFollow != null) {
                // Create an offset between this position and the other object's position
                _objectOffset = transform.position - _objectToFollow.position;
            } else {
                // Warn the user that there is no attached object to follow
                Debug.Log("CameraFollow: No Object to Follow attached to \"" + gameObject.name + "\"");
            }
        }

        // LateUpdate happens after the Update (Camera should always move last)
        private void LateUpdate()
        {
            if (_objectToFollow == null) return;
            // Apply the offset every frame, to reposition this object
            transform.position = _objectToFollow.position + _objectOffset;
        }
    }
}