using com.cyborgAssets.inspectorButtonPro;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    [Serializable]
    public class FzOR : FuzzyTerm
    {
        [SerializeField]
        private List<FuzzyTerm> m_Terms;

        public FzOR()
        {
            m_Terms = new List<FuzzyTerm>();
        }

        public FzOR(params FuzzyTerm [] op) : this()
        {
            foreach(var curTerm in op) {
                m_Terms.Add(curTerm);
            }
        }
        
        public override float GetDOM()
        {
            float largest = float.MinValue;

            foreach (var curTerm in m_Terms)
            {
                if (curTerm.GetDOM() > largest)
                {
                    largest = curTerm.GetDOM();
                }
            }

            return largest;
        }

        //unused
        public override void ClearDOM()
        {
            Debug.LogWarning("<FzOR::ClearDOM>: invalid context");
        }

        public override void ORwithDOM(float val)
        {
            Debug.LogWarning("<FzOR::ORwithDOM>: invalid context");
        }
    }
}
