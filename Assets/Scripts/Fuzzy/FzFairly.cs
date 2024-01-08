
using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    public class FzFairly : FuzzyTerm
    {
        [SerializeField]
        private FuzzySet _set;

        public FuzzySet FuzzySet { get { return _set; } }

        public FzFairly() {  }
        public FzFairly(FuzzySet ft) { _set = ft; }

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
