using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {
[RequireComponent(typeof(Text))]
public class PowerupText : MonoBehaviour {
    private Text _text;

    public void Awake()
    {
        _text = GetComponent<Text>();
    }

    public void StartFadeOut(float t)
    {
        gameObject.SetActive(true);
        StartCoroutine(TextFadeOut(t));
    }

    private IEnumerator TextFadeOut(float t)
    {
        _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, 1);
        Vector3 pos = transform.position;
        while (_text.color.a > 0.0f)
        {
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, _text.color.a - (Time.deltaTime / t));
            pos.y++;
            transform.position = pos;
            yield return null;
        }
        Destroy(gameObject);
    }
}
}
