using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Camera {
    public class CamControl : MonoBehaviour {
        public CamShake _cameraShake = null;

        [SerializeField] private List<CamFollow> _cameras = new List<CamFollow>();

        [HideInInspector] public bool _zoomWithMouseWheel;

        private Transform _cameraToFollow;
        private int _cameraToFollowIndex;
        private float _cameraSmoothing;

        private Vector3 _desiredPosition;
        private Vector3 _smoothedPosition;
        private Quaternion _desiredRotation;
        private Quaternion _smoothedRotation;

        private Coroutine _adjustSmoothing;

        private void Awake()
        {
            _cameraToFollowIndex = PlayerPrefs.GetInt("CameraToFollow");
            if (_cameras != null) {
                _cameraToFollow = _cameras[_cameraToFollowIndex].GetTransform();
                if (_cameraShake != null) {
                    _cameraShake.SetMagnitude(_cameras[_cameraToFollowIndex]._cameraShakeOverride);
                }
                _cameraSmoothing = _cameras[_cameraToFollowIndex]._cameraSmoothness;
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

            if (_zoomWithMouseWheel) {
                GameController._instance._cameraController.Zoom(-Input.GetAxisRaw("Mouse ScrollWheel"));
            }
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
            PlayerPrefs.SetInt("CameraToFollow", _cameraToFollowIndex);
            _cameraToFollow = _cameras[_cameraToFollowIndex].GetTransform();
            if (_cameraShake != null) {
                _cameraShake.SetMagnitude(_cameras[_cameraToFollowIndex]._cameraShakeOverride);
            }
            if (_adjustSmoothing != null) {
                StopCoroutine(_adjustSmoothing);
            }
            _adjustSmoothing = StartCoroutine(AdjustSmoothing(_cameras[_cameraToFollowIndex]._cameraSmoothness));
        }

        private IEnumerator AdjustSmoothing(float value)
        {
            if (_cameraSmoothing < value) {
                for (float t = 0f; t <= 0.5f; t += Time.deltaTime) {
                    _cameraSmoothing = Mathf.Lerp(_cameraSmoothing, value, t / 100);
                    yield return null;
                }
                _cameraSmoothing = value;
            } else {
                _cameraSmoothing = value;
                yield return null;
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