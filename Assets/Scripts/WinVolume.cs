using UnityEngine;

namespace Assets.Scripts {
    public class WinVolume : MonoBehaviour {
        private void OnTriggerEnter(Collider other)
        {
            PlayerShip playerShip = other.GetComponent<PlayerShip>();
            if (playerShip == null) return;

            playerShip.Win();
        }
    }
}