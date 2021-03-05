using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts {
    public class EnemyController : MonoBehaviour {
        private List<EnemyAI> _enemies;
        private List<GameObject> _dummies;

        private void Awake()
        {
            _enemies = new List<EnemyAI>();
            _dummies = new List<GameObject>();
            foreach (Transform obj in transform) {
                EnemyAI enemyAI = obj.GetComponent<EnemyAI>();
                if (enemyAI != null) {
                    _enemies.Add(enemyAI);
                } else {
                    _dummies.Add(obj.gameObject);
                }
            }
        }

        public void Reload()
        {
            foreach (EnemyAI enemyAI in _enemies) {
                enemyAI.Reload();
            }
            foreach (GameObject dummy in _dummies) {
                dummy.SetActive(true);
            }
        }
    }
}