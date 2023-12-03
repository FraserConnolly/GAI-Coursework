namespace GCU.FraserConnolly.AI.Fuzzy
{
    //-----------------------------------------------------------------------------
    //
    //  Author: Mat Buckland (www.ai-junkie.com)
    //
    //  Desc:   class to provide a proxy for a fuzzy set. The proxy inherits from
    //          FuzzyTerm and therefore can be used to create fuzzy rules
    //-----------------------------------------------------------------------------

    public class FzSet : FuzzyTerm
    {

        //a reference to the fuzzy set this proxy represents
        public FuzzySet m_Set { get; private set; }

        FzSet(FuzzySet fs)
        {
            m_Set = fs;
        }

        public override FuzzyTerm Clone()
        {
            return new FzSet(this);
        }

        public override double GetDOM()
        {
            return m_Set.GetDOM();
        }

        public override void ClearDOM()
        {
            m_Set.ClearDOM();
        }

        public override void ORwithDOM(double val)
        {
            m_Set.ORwithDOM(val);
        }
    }
}