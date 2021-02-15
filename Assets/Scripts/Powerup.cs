using System.Collections;
using UnityEngine;

namespace Assets.Scripts {
public enum PowerupType
{
    Blue,
    Orange
}

[RequireComponent(typeof(Collider))]
public class Powerup : MonoBehaviour {
    [SerializeField] private PowerupType _powerup = PowerupType.Blue;
    [SerializeField] private float _powerupRespawnTime = 8f;
    [SerializeField] private GameObject _visualsToDeactivate = null;

    private Collider _colliderToDeactivate;

    private void Awake()
    {
        _colliderToDeactivate = GetComponent<Collider>();
        _colliderToDeactivate.isTrigger = true;

        EnableObject();
    }

    private void OnTriggerEnter(Collider other)
    {
        PowerupEffects effects = other.GetComponent<PowerupEffects>();
        if (effects == null) return;
        StartCoroutine(PowerUpSequence(effects));
    }

    private IEnumerator PowerUpSequence(PowerupEffects effects)
    {
        switch(_powerup) {
            case PowerupType.Blue:
                effects.ActivateBluePowerup();
                break;
            case PowerupType.Orange:
                effects.ActivateOrangePowerup();
                break;
        }
        
        DisableObject();
        yield return new WaitForSeconds(_powerupRespawnTime);
        EnableObject();
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

    public void Reload()
    {
        StopAllCoroutines();
        EnableObject();
    }
}
}