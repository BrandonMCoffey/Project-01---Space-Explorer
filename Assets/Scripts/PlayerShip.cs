using System.Collections;
using Assets.Scripts.Camera;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {
    [RequireComponent(typeof(Rigidbody), typeof(PowerupEffects))]
    public class PlayerShip : MonoBehaviour {
        [Header("Player Movement")] [SerializeField]
        private float _moveSpeed = 12f;
        [SerializeField] private float _turnSpeed = 3f;
        [SerializeField] private int _respawnTimer = 3;

        [Header("Player Visuals")] [SerializeField]
        private PlayerParticles _shipParticles = null;
        [SerializeField] private TrailRenderer _shipTrail = null;

        [Header("Player Required References")] [SerializeField]
        private CamControl _cameraController = null;
        [SerializeField] private ScreenFlash _screenFlash = null;
        [SerializeField] private CamShake _cameraShake = null;
        [SerializeField] private Text _speedViewText = null;
        [SerializeField] private Text _respawnViewText = null;
        [SerializeField] private GameObject _winPanel = null;

        [HideInInspector] public bool _isAlive = true;

        private Rigidbody _rb;
        private PowerupEffects _powerupEffects;
        private GameObject _art;
        private Vector3 _startingPosition;
        private Quaternion _startingRotation;
        private float _startingDrag;
        private float _respawnTimerView;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _powerupEffects = GetComponent<PowerupEffects>();
            _art = GameObject.Find("Art");
            _startingPosition = transform.position;
            _startingRotation = transform.rotation;
            _startingDrag = _rb.drag;
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
            if (_cameraController == null) {
                Debug.Log("[PlayerShip] No \"CamControl\" game object found");
            }
            if (_screenFlash == null) {
                Debug.Log("[PlayerShip] No \"Screen Flash\" image found");
            }
            if (_cameraShake == null) {
                Debug.Log("[PlayerShip] No \"Camera Shake\" found");
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

        private void OnEnable()
        {
            _powerupEffects.SizeChange += ResizeShip;
        }

        private void OnDisable()
        {
            _powerupEffects.SizeChange -= ResizeShip;
        }

        private void Update()
        {
            bool forwards = (_rb.velocity.normalized - transform.forward).magnitude < 1;
            if (_shipParticles != null) {
                _shipParticles.UpdateMovementParticles((Input.GetAxisRaw("Vertical") > 0 ? Input.GetAxisRaw("Vertical") : 0) * _rb.velocity.magnitude + (forwards ? _rb.velocity.magnitude : 0));
            }
            if (_speedViewText != null) {
                _speedViewText.text = _rb.velocity.magnitude.ToString("F1") + " u/s";
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

        private void OnCollisionEnter(Collision collision)
        {
            if (_cameraShake == null || !_isAlive) return;
            _cameraShake.ShakeCamera(collision.impulse.magnitude);
        }

        private void MoveShip()
        {
            // S/Down = -1, W/Up = 1, None = 0. Scale by move speed
            float moveAmountThisFrame = Input.GetAxisRaw("Vertical") * (_moveSpeed + _powerupEffects.GetSpeedEffect());
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

        private void ResizeShip(float size)
        {
            transform.localScale = new Vector3(size, size, size);
        }

        public void SetBoosters(bool activeState)
        {
            if (_shipTrail == null) return;
            _shipTrail.enabled = activeState;
        }

        public void Kill()
        {
            if (!_isAlive) return;
            StartCoroutine(Respawn());
            if (_shipParticles != null) {
                _shipParticles.PlayDeathParticles();
            }
            if (_screenFlash != null) {
                _screenFlash.Flash(1);
            }
            if (_cameraShake != null) {
                _cameraShake.ShakeCamera();
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

            yield return new WaitForSeconds(_respawnTimer - 1);
            MoveToStartingPosition();
            yield return new WaitForSeconds(1);
            Reload();
        }

        public void Win()
        {
            _isAlive = false;
            _rb.drag = _startingDrag * 6f;
            if (_winPanel != null) {
                Cursor.lockState = CursorLockMode.None;
                _winPanel.SetActive(true);
            }
        }

        public void MoveToStartingPosition()
        {
            transform.position = _startingPosition;
            transform.rotation = _startingRotation;
            _rb.velocity = Vector3.zero;
            _rb.drag = _startingDrag;
        }

        public void Reload()
        {
            MoveToStartingPosition();
            StopAllCoroutines();
            _powerupEffects.Reload();
            _shipParticles.Reset();
            _isAlive = true;

            if (_shipTrail != null) {
                _shipTrail.enabled = false;
            }
            if (_winPanel != null) {
                _winPanel.SetActive(false);
            }
            if (_respawnViewText != null) {
                _respawnViewText.gameObject.SetActive(false);
            }
            if (_art != null) {
                _art.SetActive(true);
            }
        }
    }
}