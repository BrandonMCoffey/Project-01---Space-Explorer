using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Assets.Scripts {
    [RequireComponent(typeof(Image))]
    public class ScreenFlash : MonoBehaviour {
        [SerializeField] private float _secondsForOneFlash = 2f;
        [SerializeField] [Range(0, 1)] private float _minAlpha = 0f;
        [SerializeField] [Range(0, 1)] private float _maxAlpha = 1f;

        private event Action FlashAgain = delegate { };
        private int _flashesRemaining;

        private Coroutine _flashRoutine;
        private Image _image;

        public float SecondsForOneFlash {
            get => _secondsForOneFlash;
            private set {
                if (value < 0) {
                    value = 0;
                }
                _secondsForOneFlash = value;
            }
        }

        public float MinAlpha {
            get => _minAlpha;
            private set => _minAlpha = Mathf.Clamp(value, 0, 1);
        }

        public float MaxAlpha {
            get => _maxAlpha;
            private set => _maxAlpha = Mathf.Clamp(value, 0, 1);
        }

        private void Awake()
        {
            _image = GetComponent<Image>();
            FlashAgain = OnFlashAgain;
            SetAlphaToDefault();
        }

        public void Flash(int flashCount = 1)
        {
            if (_secondsForOneFlash <= 0) return;
            // 0 speed wouldn't make sense

            if (_flashRoutine != null) {
                StopCoroutine(_flashRoutine);
            }

            _flashesRemaining = flashCount - 1;
            _flashRoutine = StartCoroutine(FlashOnce(SecondsForOneFlash, MinAlpha, MaxAlpha));
        }

        private IEnumerator FlashOnce(float secondsForOneFlash, float minAlpha, float maxAlpha)
        {
            // half the flash time should be on flash in, the other half for flash out
            float flashInDuration = secondsForOneFlash / 2;
            float flashOutDuration = secondsForOneFlash / 2;

            // flash in
            for (float t = 0f; t <= flashInDuration; t += Time.deltaTime) {
                Color newColor = _image.color;
                newColor.a = Mathf.Lerp(minAlpha, maxAlpha, t / flashInDuration);
                _image.color = newColor;
                yield return null;
            }
            // flash out
            for (float t = 0f; t <= flashOutDuration; t += Time.deltaTime) {
                Color newColor = _image.color;
                newColor.a = Mathf.Lerp(maxAlpha, minAlpha, t / flashOutDuration);
                _image.color = newColor;
                yield return null;
            }

            if (_flashesRemaining > 0) {
                FlashAgain?.Invoke();
            }
        }

        private void OnFlashAgain()
        {
            _flashesRemaining--;
            _flashRoutine = StartCoroutine(FlashOnce(SecondsForOneFlash, MinAlpha, MaxAlpha));
        }

        private void SetAlphaToDefault()
        {
            Color newColor = _image.color;
            newColor.a = 0;
            _image.color = newColor;
        }
    }
}