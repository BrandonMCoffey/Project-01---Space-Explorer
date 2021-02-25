using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Camera {
    public class CamShake : MonoBehaviour {
        [SerializeField] private float _duration = 1;
        [SerializeField] private float _magnitude = 1;
        [SerializeField] private float _fadeTime = 0.2f;

        private float _magnitudeMultiplier = 1;
        private Coroutine _shakeRoutine;

        public void SetMagnitude(float value)
        {
            _magnitudeMultiplier = value;
        }

        public void ShakeCamera()
        {
            if (_shakeRoutine != null) {
                StopCoroutine(_shakeRoutine);
            }
            _shakeRoutine = StartCoroutine(Shake(_duration, _magnitude * _magnitudeMultiplier, _fadeTime));
        }

        public void ShakeCamera(float power)
        {
            if (_shakeRoutine != null) {
                StopCoroutine(_shakeRoutine);
            }
            _shakeRoutine = StartCoroutine(Shake(_duration * power / 12, _magnitude * _magnitudeMultiplier * power / 9, _fadeTime * power / 12));
        }

        private IEnumerator Shake(float duration, float magnitude, float fadeTime)
        {
            Vector3 originalPos = transform.localPosition;
            float elapsedTime = 0f;
            while (elapsedTime < duration) {
                float fadeIn = elapsedTime < fadeTime ? (1 - elapsedTime / fadeTime) * magnitude : 0;
                float fadeOut = elapsedTime > duration - fadeTime ? (1 - (duration - elapsedTime) / fadeTime) * magnitude : 0;
                float x = Random.Range(-1f, 1f) * (magnitude - fadeIn - fadeOut);
                float y = Random.Range(-1f, 1f) * (magnitude - fadeIn - fadeOut);

                transform.localPosition = new Vector3(x, y, originalPos.z);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPos;
        }
    }
}