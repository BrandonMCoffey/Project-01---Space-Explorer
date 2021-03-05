using TMPro;
using UnityEngine;

namespace Assets.Scripts {
    public class CanvasController : MonoBehaviour {
        [SerializeField] private ScreenFlash _screenFlash = null;
        [SerializeField] private TextMeshProUGUI _speedViewText = null;
        [SerializeField] private TextMeshProUGUI _respawnViewText = null;
        [SerializeField] private GameObject _winPanel = null;
        public TextMeshProUGUI _powerupTextTemplate = null;

        private void Awake()
        {
            if (_screenFlash == null) {
                Debug.Log("[Canvas] No \"Screen Flash\" image found");
            }
            if (_speedViewText == null) {
                Debug.Log("[Canvas] No \"Speed View Text\" game object found");
            }
            if (_respawnViewText == null) {
                Debug.Log("[Canvas] No \"Respawn View Text\" game object found");
            }
            if (_winPanel == null) {
                Debug.Log("[Canvas] No \"Win Panel\" game object found");
            }
            if (_powerupTextTemplate == null) {
                Debug.Log("[Canvas] No \"Powerup Text Template\" found");
            }
        }

        public void SetSpeedViewText(string text)
        {
            if (_speedViewText == null) return;
            _speedViewText.text = text;
        }

        public void SetRespawnViewText(string text)
        {
            if (_respawnViewText == null) return;
            _respawnViewText.text = text;
        }

        public void EnableRespawnViewText(bool active = true)
        {
            if (_respawnViewText == null) return;
            _respawnViewText.gameObject.SetActive(active);
        }

        public void EnableWinPanel(bool active = true)
        {
            if (_winPanel == null) return;
            _winPanel.gameObject.SetActive(active);
        }

        public void FlashScreen(int flashCount = 1)
        {
            if (_screenFlash == null) return;
            _screenFlash.Flash(flashCount);
        }

        public void NextLevel()
        {
            GameController._instance.NextLevel();
        }

        public void RetryLevel()
        {
            GameController._instance.ReloadLevel();
        }

        public void ExitGame()
        {
            GameController._instance.ExitGame();
        }
    }
}