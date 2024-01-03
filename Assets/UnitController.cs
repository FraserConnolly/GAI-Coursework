using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GCU.FraserConnolly.PCG.Agents { 
    public class UnitController : MonoBehaviour
    {
        [SerializeField]
        private AllyAgentController _alliedPrefab;
        [SerializeField]
        private EnemyAgentController _enemyPrefab;
        [SerializeField]
        private GameObject _rocketPrefab;
        [SerializeField]
        private GameObject _bulletPrefab;

        private GameObject _managedAllies;
        private GameObject _managedEnemies;
        private GameObject _managedAttacks;

        private GameObject _attacks;

        private void Awake()
        {
            _managedAllies = new GameObject("Allies");
            _managedAllies.transform.SetParent(transform, false);

            _managedEnemies = new GameObject("Enemies");
            _managedEnemies.transform.SetParent ( transform, false );

            _managedAttacks = new GameObject("Attacks");
            _managedAttacks.transform.SetParent ( transform, false );
        }

        // Start is called before the first frame update
        IEnumerator Start()
        {
            AllyAgent[] allies = Array.Empty<AllyAgent>();

            while (true)
            {
                allies = FindObjectsByType<AllyAgent>(FindObjectsSortMode.None);

                if (allies.Length == 0)
                {
                    // GameData not ready
                    yield return null;
                }
                else
                {
                    break;
                }
            }

            foreach (var unit in allies)
            {
                var a = Instantiate(_alliedPrefab, _managedAllies.transform);
                a.name = unit.name;
                a.GetComponent<AllyAgentController>().Initialise(unit);
            }

            var enimies = FindObjectsByType<EnemyAgent>(FindObjectsSortMode.None);

            foreach (var unit in enimies)
            {
                var a = Instantiate(_alliedPrefab, _managedEnemies.transform);
                a.name = unit.name;
                a.GetComponent<AllyAgentController>().Initialise(unit);
            }
        }
    }
}