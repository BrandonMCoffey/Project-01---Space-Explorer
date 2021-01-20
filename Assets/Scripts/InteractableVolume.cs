using UnityEngine;

namespace Assets.Scripts {
internal enum Interaction {
    Kill,
    Win
}

[RequireComponent(typeof(Collider))]
public class InteractableVolume : MonoBehaviour {
    [SerializeField] private Interaction _interaction;

    private void OnTriggerEnter(Collider other)
    {
        PlayerShip playerShip = other.GetComponent<PlayerShip>();
        if (playerShip == null) return;

        switch (_interaction) {
            case Interaction.Kill:
                playerShip.Kill();
                break;
            case Interaction.Win:
                playerShip.Win();
                break;
        }
    }
}
}