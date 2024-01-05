using com.cyborgAssets.inspectorButtonPro;
using GCU.FraserConnolly.AI.Fuzzy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Attack;

namespace GCU.FraserConnolly.AI
{
    public class WeaponSelection : MonoBehaviour
    {
        [SerializeField]
        AttackTypeFuzzyPair [ ] FuzzyLogicModules = Array.Empty<AttackTypeFuzzyPair>();

        [Serializable]
        private class AttackTypeFuzzyPair
        {
            public AttackType AttackType;
            public FuzzyModule FuzzyModule;
            public FuzzyVariable DistanceToTarget;
            public FuzzyVariable Ammo;
            public FuzzyVariable Desirability;
        }

        [ProButton]
        public AttackType GetWeapon( float distanceToTarget, float rocketAmmo)
        {
            AttackType mostDesirableAttackType = AttackType.None;
            float highestDesiribility = 0f;

            foreach (var pair in FuzzyLogicModules)
            {
                if (pair == null || pair.FuzzyModule == null || pair.Ammo == null || pair.DistanceToTarget == null || pair.Desirability == null)
                {
                    Debug.Log("Invalid Fuzzy Logic Module.", gameObject);
                    continue;
                }

                pair.Ammo.Fuzzify(pair.AttackType == AttackType.Rocket ? rocketAmmo : 1f);
                pair.DistanceToTarget.Fuzzify(distanceToTarget);

                var d = pair.FuzzyModule.DeFuzzify(pair.Desirability.VariableName, FuzzyModule.DefuzzifyMethod.max_av);

                if ( d > highestDesiribility )
                {
                    highestDesiribility = d;
                    mostDesirableAttackType = pair.AttackType;
                }
            }

            return mostDesirableAttackType;
        }
    }
}