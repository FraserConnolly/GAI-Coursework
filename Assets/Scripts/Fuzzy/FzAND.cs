using System;
using System.Collections.Generic;
using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    [Serializable]
    public class FzAND : FuzzyTerm
    {

        //an instance of this class may AND together up to 4 terms
        [SerializeField]
        private List<FuzzyTerm> m_Terms;

        public FzAND(FzAND fa) : this()
        {
            foreach (var curTerm in m_Terms)
            {
                m_Terms.Add(curTerm);
            }
        }

        public FzAND()
        {
            m_Terms = new List<FuzzyTerm>();
        }

        public FzAND( params FuzzyTerm[] op) : this()
        {
            foreach (var term in op)
            {
                m_Terms.Add(term);
            }
        }

        public override float GetDOM()
        {
            float smallest = float.MaxValue;

            foreach (var curTerm in m_Terms)
            {
                if (curTerm.GetDOM() < smallest)
                {
                    smallest = curTerm.GetDOM();
                }
            }

            return smallest;
        }

        public override void ORwithDOM(float val)
        {
            foreach (var curTerm in m_Terms)
            {
                curTerm.ORwithDOM(val);
            }
        }

        public override void ClearDOM()
        {
            foreach (var curTerm in m_Terms)
            {
                curTerm.ClearDOM();
            }
        }
    }
}
