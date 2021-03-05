using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts {
    public class GameInput : MonoBehaviour {
        [SerializeField] private bool _lockMouseInGame = true;
        [SerializeField] private KeyCode _exitGame = KeyCode.Escape;
        [SerializeField] private KeyCode _reloadLevelKey = KeyCode.Backspace;
        [SerializeField] private KeyCode _fireWeapon = KeyCode.Space;
        [SerializeField] private KeyCode _zoomInKey = KeyCode.Equals;
        [SerializeField] private KeyCode _zoomOutKey = KeyCode.Minus;
        [SerializeField] private bool _zoomWithMouseWheel = false;
        [SerializeField] private KeyCode _switchCameraKey = KeyCode.Z;

        private void Start()
        {
            GameController._instance._cameraController._zoomWithMouseWheel = _zoomWithMouseWheel;
            if (_lockMouseInGame) {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(_exitGame)) {
                GameController._instance.ExitGame();
            }
            if (Input.GetKeyDown(_reloadLevelKey)) {
                GameController._instance.ReloadLevel();
                if (_lockMouseInGame) {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }

            if (Input.GetKeyDown(_fireWeapon)) {
                GameController._instance._playerShip.FireWeapon();
            }

            if (Input.GetKey(_zoomInKey)) {
                GameController._instance._cameraController.Zoom(-0.005f);
            }
            if (Input.GetKey(_zoomOutKey)) {
                GameController._instance._cameraController.Zoom(0.005f);
            }
            if (Input.GetKeyDown(_switchCameraKey)) {
                GameController._instance._cameraController.RotateThroughCameras();
            }
        }
    }
}