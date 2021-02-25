using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Camera {
    public class CamControl : MonoBehaviour {
        [SerializeField] private CamShake _cameraShake = null;
        [SerializeField] private float _cameraSmoothing = 0.1f;
        [SerializeField] private List<CamFollow> _cameras = new List<CamFollow>();

        private Transform _cameraToFollow;
        private int _cameraToFollowIndex;

        private Vector3 _desiredPosition;
        private Vector3 _smoothedPosition;
        private Quaternion _desiredRotation;
        private Quaternion _smoothedRotation;

        private void Awake()
        {
            if (_cameras != null) {
                _cameraToFollow = _cameras[0].GetTransform();
                if (_cameraShake != null) {
                    _cameraShake.SetMagnitude(_cameras[0]._cameraShakeOverride);
                }
            } else {
                Debug.Log("CamControl: No Attached Cameras on \"" + gameObject.name + "\"");
            }
        }

        private void FixedUpdate()
        {
            if (_cameraToFollow == null) return;

            _desiredPosition = _cameraToFollow.position;
            _smoothedPosition = Vector3.Lerp(transform.position, _desiredPosition, _cameraSmoothing);

            _desiredRotation = _cameraToFollow.rotation;
            _smoothedRotation = Quaternion.Lerp(transform.rotation, _desiredRotation, _cameraSmoothing);
        }

        // LateUpdate happens after the Update (Camera should always move last)
        private void LateUpdate()
        {
            transform.position = _smoothedPosition;
            transform.rotation = _smoothedRotation;
        }

        public void Zoom(float amount)
        {
            if (_cameras == null || _cameras.Count < 1) return;
            _cameras[_cameraToFollowIndex].Zoom(amount);
        }

        public void RotateThroughCameras(bool reverse = false)
        {
            if (_cameras == null || _cameras.Count < 1) return;
            // Iterate the current camera
            _cameraToFollowIndex += reverse ? -1 : 1;
            // If the end of the list is reached, reset to start / end of list
            if (_cameraToFollowIndex >= _cameras.Count || _cameraToFollowIndex < 0) {
                _cameraToFollowIndex = reverse ? _cameras.Count - 1 : 0;
            }

            // Activate the new active camera
            _cameraToFollow = _cameras[_cameraToFollowIndex].GetTransform();
            if (_cameraShake != null) {
                _cameraShake.SetMagnitude(_cameras[_cameraToFollowIndex]._cameraShakeOverride);
            }
        }

        public void Reload()
        {
            _desiredPosition = _cameras[_cameraToFollowIndex].GetRespawnPosition();
            _smoothedPosition = _desiredPosition;
            transform.position = _desiredPosition;

            _desiredRotation = Quaternion.Euler(_cameras[_cameraToFollowIndex].GetTransform().rotation.eulerAngles.x, 0, 0);
            _smoothedRotation = _desiredRotation;
            transform.rotation = _desiredRotation;
        }
    }
}