using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Camera {
public class CameraController : MonoBehaviour {
    private List<CameraFollow> _cameras;
    private int _currentCamera;

    private void Awake()
    {
        // Initiate a new list of cameras
        _cameras = new List<CameraFollow>();
        bool activateFirst = true;

        // Search through the children of this transform
        foreach (Transform child in transform) {
            if (!child.gameObject.activeSelf) return;
            // If the transform has a connected camera, add it to the list
            CameraFollow newCamera = child.GetComponent<CameraFollow>();
            if (newCamera == null) continue;
            newCamera.EnableCamera(activateFirst);
            _cameras.Add(newCamera);
            activateFirst = false;
        }

        // Warn the user if no cameras were found
        if (_cameras.Count < 1) {
            Debug.Log("[CameraSwitch] No connected cameras");
        }
    }

    public void Zoom(float amount)
    {
        if (_cameras == null || _cameras.Count < 1) return;
        _cameras[_currentCamera].GetComponent<CameraFollow>().Zoom(amount);
    }

    public void RotateThroughCameras(bool reverse = false)
    {
        if (_cameras == null || _cameras.Count < 1) return;
        // Disable the active camera
        _cameras[_currentCamera].EnableCamera(false);
        // Iterate the current camera
        _currentCamera += reverse ? -1 : 1;
        // If the end of the list is reached, reset to start / end of list
        if (_currentCamera >= _cameras.Count || _currentCamera < 0) {
            _currentCamera = reverse ? _cameras.Count - 1 : 0;
        }

        // Activate the new active camera
        _cameras[_currentCamera].EnableCamera();
    }
}
}