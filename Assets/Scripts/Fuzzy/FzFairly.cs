
using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    ///////////////////////////////////////////////////////////////////////////////
    class FzFairly : FuzzyTerm
    {
        [SerializeField]
        private FuzzySet _set;

        public FuzzySet FuzzySet { get { return _set; } }

        public FzFairly() {  }
        public FzFairly(FzSet ft) { _set = ft.m_Set; }

        private FzFairly(FzFairly inst) { _set = inst._set; }

        public override float GetDOM()
        {
            return Mathf.Sqrt(_set.GetDOM());
        }

        public override void ClearDOM()
        {
            _set.ClearDOM();
        }

        public override void ORwithDOM(float val)
        {
            _set.ORwithDOM(Mathf.Sqrt(val));
        }
    }
}
