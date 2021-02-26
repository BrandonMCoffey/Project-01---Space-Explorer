using UnityEngine;

namespace Assets.Scripts {
    public class GameController : MonoBehaviour {
        void Start()
        {
            QualitySettings.vSyncCount = 1;
#if UNITY_EDITOR
            Application.targetFrameRate = 60;
#endif
        }
    }
}