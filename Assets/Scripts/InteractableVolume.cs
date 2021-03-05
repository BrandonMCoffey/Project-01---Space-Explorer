using UnityEngine;

namespace Assets.Scripts {
    internal enum Interaction {
        Kill,
        Win
    }

    [RequireComponent(typeof(Collider), typeof(AudioSource))]
    public class InteractableVolume : MonoBehaviour {
        [SerializeField] private Interaction _interaction;
        [SerializeField] private AudioClip _win;
        [SerializeField] private AudioClip _death;

        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerShip playerShip = other.GetComponent<PlayerShip>();
            if (playerShip == null) return;

            switch (_interaction) {
                case Interaction.Kill:
                    _source.clip = _death;
                    _source.Play();
                    playerShip.Kill();
                    break;
                case Interaction.Win:
                    _source.clip = _win;
                    _source.Play();
                    playerShip.Win();
                    break;
            }
        }
    }
}