using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextFadeOut : MonoBehaviour {
        private TextMeshProUGUI _text;

        public void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        public void StartFadeOut(float t, bool destroy = true)
        {
            gameObject.SetActive(true);
            StartCoroutine(FadeOut(t, destroy));
        }

        private IEnumerator FadeOut(float t, bool destroy)
        {
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 1);
            Vector3 pos = transform.position;
            while (_text.color.a > 0.0f) {
                _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, _text.color.a - (Time.deltaTime / t));
                pos.y += 0.25f;
                transform.position = pos;
                yield return null;
            }
            if (destroy) {
                Destroy(gameObject);
            } else {
                gameObject.SetActive(false);
            }
        }
    }
}