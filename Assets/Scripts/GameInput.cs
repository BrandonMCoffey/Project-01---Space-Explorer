using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts {
public class GameInput : MonoBehaviour {
    [SerializeField] private bool _lockMouseInGame = true;
    [SerializeField] private KeyCode _exitGame = KeyCode.Escape;
    [SerializeField] private KeyCode _reloadLevelKey = KeyCode.Backspace;
    [SerializeField] private CameraController _cameraController = null;
    [SerializeField] private bool _zoomWithMouseWheel = false;
    [SerializeField] private KeyCode _zoomInKey = KeyCode.Equals;
    [SerializeField] private KeyCode _zoomOutKey = KeyCode.Minus;
    [SerializeField] private KeyCode _switchCameraKey = KeyCode.Z;

    private void Start()
    {
        if (_lockMouseInGame) {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (_cameraController == null) {
            Debug.Log("[GameInput] No Camera Controller connected to \"" + gameObject.name + "\"");
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
        if (SceneManager.sceneCount > activeSceneIndex + 1) {
            SceneManager.LoadScene(activeSceneIndex + 1);
        }
    }

    public void ReloadLevel()
    {
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(activeSceneIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
}