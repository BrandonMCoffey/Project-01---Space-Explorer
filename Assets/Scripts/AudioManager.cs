using UnityEngine;

namespace Assets.Scripts {
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour {
        public static AudioManager _instance = null;

        private AudioSource _audioSource;

        private void Awake()
        {
            if (_instance == null) {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                _audioSource = GetComponent<AudioSource>();
            } else {
                Destroy(gameObject);
            }
        }

        public void PlaySong(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }

        public bool IsPlaying()
        {
            return _audioSource.isPlaying;
        }
    }
}