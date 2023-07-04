using System.Collections;
using UnityEngine;

namespace Homeworks.homework_1 
{ 

    public class Unit : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _health;

        private IEnumerator _healingRoutine;
        [SerializeField] private int _healingHealth = 5;
        [SerializeField] private float _healingInterval = 0.5f;


        private void Start()
        {
            _health = 0;
            Debug.Log("Press H to heal the unit");
            Debug.Log($"Health: {_health}");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                ReceiveHealing();
            }
        }

        public void ReceiveHealing()
        {
            if (_healingRoutine != null)
                StopCoroutine(_healingRoutine);

            StartCoroutine(_healingRoutine = Heal_Coroutine());
        }

        private IEnumerator Heal_Coroutine()
        {
            for (float i = 0; i < 3.0f; i += _healingInterval)
            {
                _health += _healingHealth;

                if (_health > _maxHealth)
                {
                    _health = _maxHealth;
                }

                Debug.Log($"Health: {_health}");
                yield return new WaitForSeconds(_healingInterval);
            }
            yield return null;
        }
    }
}
