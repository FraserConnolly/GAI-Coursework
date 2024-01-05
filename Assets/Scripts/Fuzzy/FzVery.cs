using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    //-----------------------------------------------------------------------------
    //
    //  Author: Mat Buckland (www.ai-junkie.com)
    //
    //  Desc:   classes to implement fuzzy hedges 
    //-----------------------------------------------------------------------------
    public class FzVery : FuzzyTerm
    {

        [SerializeField] 
        private FuzzySet _set;

        public FuzzySet FuzzySet { get { return _set; } }

        public FzVery() { }

        public FzVery(FuzzySet ft) { _set = ft; }

        public override float GetDOM()
        {
            return _set.GetDOM() * _set.GetDOM();
        }

        public override void ClearDOM()
        {
            _set.ClearDOM();
        }

        public override void ORwithDOM(float val)
        {
            _set.ORwithDOM(val * val);
        }
    }
}
