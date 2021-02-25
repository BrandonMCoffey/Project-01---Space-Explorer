using System;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {
    public enum AffectedProperties {
        Speed,
        Size
    }

    public class PowerupEffects : MonoBehaviour {
        [Header("Blue Powerup")] [SerializeField]
        private AffectedProperties _blueEffectedProperty = AffectedProperties.Speed;
        [SerializeField] private float _blueEffectAmount = 5f;
        [SerializeField] private float _blueEffectDuration = 6f;
        [SerializeField] private string _blueEffectText = "Speed Increase";

        [Header("Orange Powerup")] [SerializeField]
        private AffectedProperties _orangeEffectedProperty = AffectedProperties.Size;
        [SerializeField] private float _orangeEffectAmount = 0.5f;
        [SerializeField] private float _orangeEffectDuration = 4f;
        [SerializeField] private string _orangeEffectText = "Size Increase";

        [Header("Text")] [SerializeField] private Text _powerupTextTemplate = null;
        [SerializeField] private float _textFadeTime = 3f;

        public event Action<float> SizeChange = delegate { };
        private float _sizeChangeAmount = 1;

        private int _bluePowerupAmount;
        private int _orangePowerupAmount;

        private List<GameObject> _powerupTexts = null;

        private void Awake()
        {
            if (_powerupTextTemplate != null) {
                _powerupTexts = new List<GameObject>();
            } else {
                Debug.Log("[PowerupEffects] No \"Powerup Text Template\" found on " + gameObject.name);
            }
        }

        public float GetSpeedEffect()
        {
            float effect = 0f;
            effect += _blueEffectedProperty == AffectedProperties.Speed ? _bluePowerupAmount * _blueEffectAmount : 0;
            effect += _orangeEffectedProperty == AffectedProperties.Speed ? _orangePowerupAmount * _orangeEffectAmount : 0;
            return effect;
        }

        public void ActivateBluePowerup()
        {
            StartCoroutine(BluePowerupEffects());
        }

        private IEnumerator BluePowerupEffects()
        {
            _bluePowerupAmount++;
            if (_blueEffectedProperty == AffectedProperties.Size) {
                _sizeChangeAmount *= _blueEffectAmount;
                SizeChange?.Invoke(_sizeChangeAmount);
            }
            CreateNewPowerupText(_blueEffectText + (_bluePowerupAmount > 1 ? " x" + _bluePowerupAmount : ""));
            yield return new WaitForSeconds(_blueEffectDuration);
            _bluePowerupAmount--;
            if (_blueEffectedProperty == AffectedProperties.Size) {
                _sizeChangeAmount /= _blueEffectAmount;
                SizeChange?.Invoke(_sizeChangeAmount);
            }
        }

        public void ActivateOrangePowerup()
        {
            StartCoroutine(OrangePowerupEffects());
        }

        private IEnumerator OrangePowerupEffects()
        {
            _orangePowerupAmount++;
            if (_orangeEffectedProperty == AffectedProperties.Size) {
                _sizeChangeAmount *= _orangeEffectAmount;
                SizeChange?.Invoke(_sizeChangeAmount);
            }
            CreateNewPowerupText(_orangeEffectText + (_orangePowerupAmount > 1 ? " x" + _orangePowerupAmount : ""));
            yield return new WaitForSeconds(_orangeEffectDuration);
            _orangePowerupAmount--;
            if (_orangeEffectedProperty == AffectedProperties.Size) {
                _sizeChangeAmount /= _orangeEffectAmount;
                SizeChange?.Invoke(_sizeChangeAmount);
            }
        }

        private void CreateNewPowerupText(string text)
        {
            Text newPowerupText = Instantiate(_powerupTextTemplate);
            GameObjectUtility.SetParentAndAlign(newPowerupText.gameObject, _powerupTextTemplate.transform.parent.gameObject);
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
            int powerupTextCount = _powerupTexts.Count;
            for (int i = 0; i < powerupTextCount; i++) {
                Destroy(_powerupTexts[0]);
            }
            _powerupTexts.Clear();
        }
    }
}