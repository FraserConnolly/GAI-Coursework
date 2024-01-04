
using GCU.FraserConnolly.AI.Fuzzy;
using System;

namespace Assets.Scripts.Fuzzy
{
    //-----------------------------------------------------------------------------
    //
    //  Author: Mat Buckland (www.ai-junkie.com)
    //
    //  Desc:   classes to implement fuzzy hedges 
    //-----------------------------------------------------------------------------
    public class FzVery : FuzzyTerm
    {

        private FuzzySet m_Set;

        public FzVery(FzSet ft) { m_Set = ft.m_Set; }

        private FzVery(FzVery inst) { m_Set = inst.m_Set; }

        public override double GetDOM()
        {
            return m_Set.GetDOM() * m_Set.GetDOM();
        }

        public override FuzzyTerm Clone()
        {
            return new FzVery(this);
        }

        public override void ClearDOM()
        {
            m_Set.ClearDOM();
        }

        public override void ORwithDOM(double val)
        {
            m_Set.ORwithDOM(val * val);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    class FzFairly : FuzzyTerm
    {
        private FuzzySet m_Set;

        public FzFairly(FzSet ft) { m_Set = ft.m_Set; }

        private FzFairly(FzFairly inst) { m_Set = inst.m_Set; }

        public override double GetDOM()
        {
            return Math.Sqrt(m_Set.GetDOM());
        }

        public override FuzzyTerm Clone()
        {
            return new FzFairly(this);
        }

        public override void ClearDOM()
        {
            m_Set.ClearDOM();
        }

        public override void ORwithDOM(double val)
        {
            m_Set.ORwithDOM(Math.Sqrt(val));
        }
    }
}
