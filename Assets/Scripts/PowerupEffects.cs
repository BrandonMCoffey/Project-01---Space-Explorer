using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts {
    public enum AffectedProperties {
        Speed,
        Size,
        Laser
    }

    public class PowerupEffects : MonoBehaviour {
        [Header("Blue Powerup")]
        [SerializeField] private AffectedProperties _blueEffectedProperty = AffectedProperties.Speed;
        [SerializeField] private float _blueEffectAmount = 5f;
        [SerializeField] private float _blueEffectDuration = 6f;
        [SerializeField] private string _blueEffectText = "Speed Increase";

        [Header("Orange Powerup")]
        [SerializeField] private AffectedProperties _orangeEffectedProperty = AffectedProperties.Size;
        [SerializeField] private float _orangeEffectAmount = 0.5f;
        [SerializeField] private float _orangeEffectDuration = 4f;
        [SerializeField] private string _orangeEffectText = "Size Increase";

        [Header("Purple Powerup")]
        [SerializeField] private AffectedProperties _purpleEffectedProperty = AffectedProperties.Laser;
        [SerializeField] private float _purpleEffectDuration = 4f;
        [SerializeField] private string _purpleEffectText = "Powered Up Laser";

        [Header("Other")]
        [SerializeField] private AudioClip _clip = null;
        [SerializeField] private float _textFadeTime = 3f;

        public event Action<float> SizeChange = delegate { };
        public event Action<float> SpeedBoost = delegate { };
        public event Action<bool> LaserBoost = delegate { };
        private float _sizeChangeAmount = 1;
        private float _speedBoostAmount;

        private int _bluePowerupAmount;
        private int _orangePowerupAmount;
        private int _purplePowerupAmount;

        private List<GameObject> _powerupTexts = new List<GameObject>();

        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        public void ActivateBluePowerup()
        {
            _source.clip = _clip;
            _source.Play();
            StartCoroutine(BluePowerupEffects());
        }

        private IEnumerator BluePowerupEffects()
        {
            _bluePowerupAmount++;
            TriggerEffects(_blueEffectedProperty, _blueEffectAmount, true);
            CreateNewPowerupText(_blueEffectText + (_bluePowerupAmount > 1 ? " x" + _bluePowerupAmount : ""));
            yield return new WaitForSeconds(_blueEffectDuration);
            _bluePowerupAmount--;
            TriggerEffects(_blueEffectedProperty, _blueEffectAmount, false);
        }

        public void ActivateOrangePowerup()
        {
            _source.clip = _clip;
            _source.Play();
            StartCoroutine(OrangePowerupEffects());
        }

        private IEnumerator OrangePowerupEffects()
        {
            _orangePowerupAmount++;
            TriggerEffects(_orangeEffectedProperty, _orangeEffectAmount, true);
            CreateNewPowerupText(_orangeEffectText + (_orangePowerupAmount > 1 ? " x" + _orangePowerupAmount : ""));
            yield return new WaitForSeconds(_orangeEffectDuration);
            _orangePowerupAmount--;
            TriggerEffects(_orangeEffectedProperty, _orangeEffectAmount, false);
        }

        public void ActivatePurplePowerup()
        {
            _source.clip = _clip;
            _source.Play();
            StartCoroutine(PurplePowerupEffects());
        }

        private IEnumerator PurplePowerupEffects()
        {
            _purplePowerupAmount++;
            TriggerEffects(_purpleEffectedProperty, _purplePowerupAmount, true);
            CreateNewPowerupText(_purpleEffectText + (_purplePowerupAmount > 1 ? " x" + _purplePowerupAmount : ""));
            yield return new WaitForSeconds(_purpleEffectDuration);
            _purplePowerupAmount--;
            TriggerEffects(_purpleEffectedProperty, _purplePowerupAmount, false);
        }

        private void TriggerEffects(AffectedProperties properties, float change, bool option)
        {
            switch (properties) {
                case AffectedProperties.Speed:
                    _speedBoostAmount += change * (option ? 1 : -1);
                    SpeedBoost?.Invoke(_speedBoostAmount);
                    break;
                case AffectedProperties.Size:
                    _sizeChangeAmount *= option ? change : 1 / change;
                    SizeChange?.Invoke(_sizeChangeAmount);
                    break;
                case AffectedProperties.Laser:
                    LaserBoost?.Invoke(option);
                    break;
            }
        }

        private void CreateNewPowerupText(string text)
        {
            TextMeshProUGUI template = GameController._instance._canvasController._powerupTextTemplate;
            TextMeshProUGUI newPowerupText = Instantiate(template);
            // GameObjectUtility.SetParentAndAlign(newPowerupText.gameObject, _powerupTextTemplate.transform.parent.gameObject);
            newPowerupText.transform.SetParent(template.transform.parent);
            newPowerupText.transform.localPosition = template.transform.localPosition;
            newPowerupText.text = text;
            TextFadeOut control = newPowerupText.gameObject.AddComponent<TextFadeOut>();
            control.StartFadeOut(_textFadeTime);
            _powerupTexts.Add(newPowerupText.gameObject);
        }

        public void Reload()
        {
            StopAllCoroutines();
            _bluePowerupAmount = 0;
            _orangePowerupAmount = 0;
            SpeedBoost?.Invoke(_speedBoostAmount = 0);
            _sizeChangeAmount = 1;
            int powerupTextCount = _powerupTexts.Count;
            for (int i = 0; i < powerupTextCount; i++) {
                Destroy(_powerupTexts[0]);
            }
            _powerupTexts.Clear();
        }
    }
}