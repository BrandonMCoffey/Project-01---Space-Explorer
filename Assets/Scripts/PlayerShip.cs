using System.Collections;
using Assets.Scripts.Camera;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {
[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : MonoBehaviour {
    [Header("Player Options")] [SerializeField]
    private float _moveSpeed = 12f;
    [SerializeField] private float _turnSpeed = 3f;
    [SerializeField] private int _respawnTimer = 3;

    [Header("Player Options")] [SerializeField]
    private PlayerParticles _shipParticles = null;
    [SerializeField] private TrailRenderer _shipTrail = null;

    [Header("Player Options")] [SerializeField]
    private Text _speedViewText = null;
    [SerializeField] private Text _respawnViewText = null;
    [SerializeField] private GameObject _winPanel = null;

    [HideInInspector] public bool _isAlive = true;

    private Rigidbody _rb;
    private GameObject _art;
    private float _bonusMoveSpeed;
    private float _respawnTimerView;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _art = GameObject.Find("Art");
        if (_art == null) {
            Debug.Log("[PlayerShip] No \"Art\" game object found");
        }
        if (_shipParticles == null) {
            Debug.Log("[PlayerShip] No \"Ship Particles\" found");
        }
        if (_shipTrail != null) {
            _shipTrail.enabled = false;
        } else {
            Debug.Log("[PlayerShip] No \"Ship Trail\" found");
        }
        if (_speedViewText == null) {
            Debug.Log("[PlayerShip] No \"Speed View Text\" game object found");
        }
        if (_respawnViewText == null) {
            Debug.Log("[PlayerShip] No \"Respawn View Text\" game object found");
        }
        if (_winPanel == null) {
            Debug.Log("[PlayerShip] No \"Win Panel\" game object found");
        }
    }

    private void Update()
    {
        bool forwards = (_rb.velocity.normalized - transform.forward).magnitude < 1;
        if (_shipParticles != null) {
            _shipParticles.UpdateMovementParticles((Input.GetAxisRaw("Vertical") > 0 ? Input.GetAxisRaw("Vertical") : 0) * _rb.velocity.magnitude + (forwards ? _rb.velocity.magnitude : 0));
        }
        if (_speedViewText != null) {
            _speedViewText.text = (forwards ? "" : "-") + _rb.velocity.magnitude.ToString("F1") + " u/s";
        }
        if (!_isAlive && _respawnViewText != null) {
            _respawnTimerView -= Time.deltaTime;
            _respawnViewText.text = "Respawning in " + _respawnTimerView.ToString("F0") + "...";
        }
    }

    private void FixedUpdate()
    {
        if (!_isAlive) return;
        MoveShip();
        TurnShip();
    }

    private void MoveShip()
    {
        // S/Down = -1, W/Up = 1, None = 0. Scale by move speed
        float moveAmountThisFrame = Input.GetAxisRaw("Vertical") * (_moveSpeed + _bonusMoveSpeed);
        // Combine our direction with our calculated amount
        Vector3 moveDirection = transform.forward * moveAmountThisFrame;
        // Apply the movement to the physics object
        _rb.AddForce(moveDirection);
    }

    private void TurnShip()
    {
        // A/\Left = -1, D/Right = 1, None = 0. Scale by turn speed
        float turnAmountThisFrame = Input.GetAxisRaw("Horizontal") * _turnSpeed;
        // Specify an axis to apply our turn amount (x,y,z) as a rotation
        Quaternion turnOffset = Quaternion.Euler(0, turnAmountThisFrame, 0);
        // Spin/Rotate the rigidbody
        _rb.MoveRotation(_rb.rotation * turnOffset);
    }

    public void SetSpeed(float speedChange)
    {
        _bonusMoveSpeed += speedChange;
        if (_bonusMoveSpeed < 0) _bonusMoveSpeed = 0;
        // TODO: Audio and Visuals
    }

    public void SetBoosters(bool activeState)
    {
        if (_shipTrail == null) return;
        _shipTrail.enabled = activeState;
    }

    public void Kill()
    {
        StartCoroutine(Respawn());
        if (_shipParticles != null) {
            _shipParticles.PlayDeathParticles();
        }
        UnityEngine.Camera mainCamera = UnityEngine.Camera.current;
        CameraFollow cameraFollow = mainCamera.transform.parent.GetComponent<CameraFollow>();
        if (cameraFollow != null) {
            StartCoroutine(cameraFollow.ResetCamera(_respawnTimer));
        }
    }

    private IEnumerator Respawn()
    {
        _rb.velocity = Vector3.zero;
        _isAlive = false;
        _respawnTimerView = _respawnTimer;
        if (_respawnViewText != null) {
            _respawnViewText.gameObject.SetActive(true);
        }
        if (_art != null) {
            _art.SetActive(false);
        }

        yield return new WaitForSeconds(_respawnTimer);
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        _bonusMoveSpeed = 0;
        if (_shipTrail != null) {
            _shipTrail.enabled = false;
        }
        _isAlive = true;
        if (_respawnViewText != null) {
            _respawnViewText.gameObject.SetActive(false);
        }
        if (_art != null) {
            _art.SetActive(true);
        }
    }

    public void Win()
    {
        _isAlive = false;

        Cursor.lockState = CursorLockMode.None;
        if (_winPanel != null) {
            _winPanel.SetActive(true);
        }
    }
}
}