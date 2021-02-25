using Assets.Scripts.Camera;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts {
    public class GameInput : MonoBehaviour {
        [SerializeField] private bool _lockMouseInGame = true;
        [SerializeField] private KeyCode _exitGame = KeyCode.Escape;
        [SerializeField] private KeyCode _reloadLevelKey = KeyCode.Backspace;
        [SerializeField] private bool _reloadEntireScene = true;
        [SerializeField] private PlayerShip _playerShip = null;
        [SerializeField] private CamControl _cameraController = null;
        [SerializeField] private KeyCode _zoomInKey = KeyCode.Equals;
        [SerializeField] private KeyCode _zoomOutKey = KeyCode.Minus;
        [SerializeField] private bool _zoomWithMouseWheel = false;
        [SerializeField] private KeyCode _switchCameraKey = KeyCode.Z;
        [SerializeField] private PowerupController _powerupController = null;

        private void Start()
        {
            if (_lockMouseInGame) {
                Cursor.lockState = CursorLockMode.Locked;
            }
            if (_playerShip == null) {
                Debug.Log("[GameInput] No Player Ship connected");
            }
            if (_cameraController == null) {
                Debug.Log("[GameInput] No Camera Controller connected");
            }
            if (_powerupController == null) {
                Debug.Log("[GameInput] No Powerup Controller connected");
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(_exitGame)) {
                ExitGame();
            }
            if (Input.GetKeyDown(_reloadLevelKey)) {
                ReloadLevel();
            }

            if (_cameraController != null) {
                if (Input.GetKey(_zoomInKey)) {
                    _cameraController.Zoom(-0.005f);
                }
                if (Input.GetKey(_zoomOutKey)) {
                    _cameraController.Zoom(0.005f);
                }
                if (Input.GetKeyDown(_switchCameraKey)) {
                    _cameraController.RotateThroughCameras();
                }
            }
        }

        private void LateUpdate()
        {
            if (_zoomWithMouseWheel && _cameraController != null) {
                _cameraController.Zoom(-Input.GetAxisRaw("Mouse ScrollWheel"));
            }
        }

        public void NextLevel()
        {
            int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(activeSceneIndex + 1);
        }

        public void ReloadLevel()
        {
            // Reload entire scene
            if (_reloadEntireScene) {
                int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(activeSceneIndex);
            } else {
                if (_playerShip != null) {
                    _playerShip.Reload();
                }
                if (_cameraController != null) {
                    _cameraController.Reload();
                }
                if (_powerupController != null) {
                    _powerupController.Reload();
                }
                if (_lockMouseInGame) {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}