using System;
using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    //-----------------------------------------------------------------------------
    //
    //  Author: Mat Buckland (www.ai-junkie.com)
    //
    //  Desc:   class to provide a proxy for a fuzzy set. The proxy inherits from
    //          FuzzyTerm and therefore can be used to create fuzzy rules
    //-----------------------------------------------------------------------------
    [Serializable]
    public class FzSet : FuzzyTerm
    {

        //a reference to the fuzzy set this proxy represents
        [SerializeField]
        private FuzzySet _set;
        public FuzzySet m_Set { get => _set; private set => _set = value; }

        public FzSet() { }
        public FzSet(FuzzySet fs) { m_Set = fs; }

        private FzSet ( FzSet inst ) {  m_Set = inst.m_Set; }

        public override float GetDOM()
        {
            return m_Set.GetDOM();
        }

        public override void ClearDOM()
        {
            m_Set.ClearDOM();
        }

        public override void ORwithDOM(float val)
        {
            m_Set.ORwithDOM(val);
        }
    }
}