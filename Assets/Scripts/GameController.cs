using Assets.Scripts.Camera;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts {
    public class GameController : MonoBehaviour {
        [SerializeField] private bool _reloadEntireScene = false;

        public PlayerShip _playerShip;
        public CamControl _cameraController;
        public CanvasController _canvasController;
        public EnemyController _enemyController;
        public PowerupController _powerupController;

        #region Singleton

        public static GameController _instance;

        private void Awake()
        {
            _instance = this;

            if (_playerShip == null) {
                Debug.Log("[GameController] Error: No Player Ship Connected");
            }
            if (_cameraController == null) {
                Debug.Log("[GameController] Error: No Camera Controller Connected");
            }
            if (_canvasController == null) {
                Debug.Log("[GameController] Error: No Canvas Controller Connected");
            }
            if (_enemyController == null) {
                Debug.Log("[GameController] Warning: No Enemy Controller Connected");
            }
            if (_powerupController == null) {
                Debug.Log("[GameController] Warning: No Powerup Controller Connected");
            }
        }

        #endregion

        private void Start()
        {
            QualitySettings.vSyncCount = 1;
#if UNITY_EDITOR
            Application.targetFrameRate = 60;
#endif
        }

        public void ReloadLevel()
        {
            // Reload entire scene
            if (_reloadEntireScene) {
                int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(activeSceneIndex);
            } else {
                _playerShip.Reload();
                _cameraController.Reload();
                if (_enemyController != null) _enemyController.Reload();
                if (_powerupController != null) _powerupController.Reload();
            }
        }

        public void NextLevel()
        {
            int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(activeSceneIndex + 1);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}