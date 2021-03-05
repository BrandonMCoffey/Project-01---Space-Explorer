using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PowerupEffects), typeof(WeaponSystem))]
    public class PlayerShip : MonoBehaviour {
        [Header("Player Movement")]
        [SerializeField] private float _moveSpeed = 12f;
        [SerializeField] private float _turnSpeed = 3f;
        [SerializeField] private int _respawnTimer = 3;

        [Header("Player Visuals")]
        [SerializeField] private GameObject _artToDisable = null;
        [SerializeField] private PlayerParticles _shipParticles = null;

        [HideInInspector] public bool _isAlive = true;

        private Rigidbody _rb;
        private PowerupEffects _powerupEffects;
        private WeaponSystem _weaponSystem;
        private CanvasController _canvasController;

        private float _currentSpeedBoost;
        private Vector3 _startingPosition;
        private Quaternion _startingRotation;
        private float _startingDrag;
        private float _respawnTimerView;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _powerupEffects = GetComponent<PowerupEffects>();
            _weaponSystem = GetComponent<WeaponSystem>();
            _startingPosition = transform.position;
            _startingRotation = transform.rotation;
            _startingDrag = _rb.drag;
            if (_artToDisable == null) {
                Debug.Log("[PlayerShip] No \"Art\" game object found");
            }
            if (_shipParticles == null) {
                Debug.Log("[PlayerShip] No \"Ship Particles\" found");
            }
        }

        private void OnEnable()
        {
            _powerupEffects.SizeChange += ResizeShip;
            _powerupEffects.SpeedBoost += SetBoosters;
        }

        private void OnDisable()
        {
            _powerupEffects.SizeChange -= ResizeShip;
            _powerupEffects.SpeedBoost -= SetBoosters;
        }

        private void Start()
        {
            _canvasController = GameController._instance._canvasController;
        }

        private void Update()
        {
            bool forwards = (_rb.velocity.normalized - transform.forward).magnitude < 1;
            if (_shipParticles != null) {
                _shipParticles.UpdateMovementParticles((Input.GetAxisRaw("Vertical") > 0 ? Input.GetAxisRaw("Vertical") : 0) * _rb.velocity.magnitude + (forwards ? _rb.velocity.magnitude : 0));
            }
            _canvasController.SetSpeedViewText((_rb.velocity.magnitude * 20 / 23.7).ToString("F1") + " u/s");
            if (!_isAlive) {
                _respawnTimerView -= Time.deltaTime;
                _canvasController.SetRespawnViewText("Respawning in " + _respawnTimerView.ToString("F0") + "...");
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
            if (!_isAlive) return;
            GameController._instance._cameraController._cameraShake.ShakeCamera(collision.impulse.magnitude);
        }

        private void MoveShip()
        {
            // S/Down = -1, W/Up = 1, None = 0. Scale by move speed
            float moveAmountThisFrame = Input.GetAxisRaw("Vertical") * (_moveSpeed + _currentSpeedBoost);
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

        public void FireWeapon()
        {
            if (!_isAlive) return;
            Vector3 hitPosition = _weaponSystem.Fire();
            _shipParticles.PlayLaserHitParticles(hitPosition);
            _rb.velocity -= transform.forward / 2;
        }

        private void ResizeShip(float size)
        {
            _shipParticles.PlaySizeChangeParticles();
            transform.localScale = new Vector3(size, size, size);
        }

        public void SetBoosters(float boost)
        {
            _shipParticles.ChangeColor(boost == 0 ? 0 : _currentSpeedBoost < boost ? 1 : -1);
            if (_currentSpeedBoost < boost) {
                _shipParticles.PlayBoostParticles();
                float vel = _rb.velocity.magnitude;
                if (vel < 4) vel = 4;
                _rb.velocity = transform.forward * vel + transform.forward * boost / 2;
            } else {
                _rb.velocity -= transform.forward * boost;
            }
            _currentSpeedBoost = boost;
        }

        public void Kill()
        {
            if (!_isAlive) return;
            StartCoroutine(Respawn());
            if (_shipParticles != null) {
                _shipParticles.PlayDeathParticles();
            }
            _canvasController.FlashScreen();
            GameController._instance._cameraController._cameraShake.ShakeCamera();
        }

        private IEnumerator Respawn()
        {
            _rb.velocity = Vector3.zero;
            _isAlive = false;
            _respawnTimerView = _respawnTimer;
            _canvasController.EnableRespawnViewText();
            if (_artToDisable != null) {
                _artToDisable.SetActive(false);
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
            Cursor.lockState = CursorLockMode.None;
            _canvasController.EnableWinPanel();
        }

        public void MoveToStartingPosition()
        {
            transform.position = _startingPosition;
            transform.rotation = _startingRotation;
            transform.localScale = Vector3.one;
            _rb.velocity = Vector3.zero;
            _rb.drag = _startingDrag;
        }

        public void Reload()
        {
            MoveToStartingPosition();
            StopAllCoroutines();
            _powerupEffects.Reload();
            _shipParticles.Reload();
            _isAlive = true;
            _canvasController.EnableWinPanel(false);
            _canvasController.EnableRespawnViewText(false);
            if (_artToDisable != null) {
                _artToDisable.SetActive(true);
            }
        }
    }
}