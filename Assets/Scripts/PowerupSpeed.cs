using System.Collections;
using UnityEngine;

namespace Assets.Scripts {
[RequireComponent(typeof(Collider))]
public class PowerupSpeed : MonoBehaviour {
    [Header("Powerup Settings")] [SerializeField]
    private float _speedIncreaseAmount = 20;

    [SerializeField] private float _powerupDuration = 5;

    [Header("Setup")] [SerializeField] private GameObject _visualsToDeactivate = null;

    private Collider _colliderToDeactivate;
    private bool _poweredUp = false;

    private void Awake()
    {
        _colliderToDeactivate = GetComponent<Collider>();
        _colliderToDeactivate.isTrigger = true;

        EnableObject();

        // REMOVE
        if (_poweredUp) {
            Debug.Log("Powered up");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerShip playerShip = other.GetComponent<PlayerShip>();
        if (playerShip == null) return;
        StartCoroutine(PowerUpSequence(playerShip));
    }

    private IEnumerator PowerUpSequence(PlayerShip playerShip)
    {
        _poweredUp = true;
        ActivatePowerup(playerShip);
        DisableObject();
        yield return new WaitForSeconds(_powerupDuration);
        _poweredUp = false;
        DeactivatePowerup(playerShip);
        EnableObject();
    }

    private void ActivatePowerup(PlayerShip playerShip)
    {
        if (playerShip == null) return;
        playerShip.SetSpeed(_speedIncreaseAmount);
        playerShip.SetBoosters(true);
    }

    private void DeactivatePowerup(PlayerShip playerShip)
    {
        if (playerShip == null) return;
        playerShip.SetSpeed(-_speedIncreaseAmount);
        playerShip.SetBoosters(false);
    }

    private void DisableObject()
    {
        _colliderToDeactivate.enabled = false;
        if (_visualsToDeactivate != null) {
            _visualsToDeactivate.SetActive(false);
        }
    }

    private void EnableObject()
    {
        _colliderToDeactivate.enabled = true;
        if (_visualsToDeactivate != null) {
            _visualsToDeactivate.SetActive(true);
        }
    }
}
}