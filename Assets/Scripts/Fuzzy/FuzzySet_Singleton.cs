using System.Security.Cryptography;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    //-----------------------------------------------------------------------------
    //
    //  Author: Mat Buckland (www.ai-junkie.com)
    //
    //  Desc:   This defines a fuzzy set that is a singleton (a range
    //          over which the DOM is always 1.0)
    //-----------------------------------------------------------------------------
    public class FuzzySet_Singleton : FuzzySet
    {

        //the values that define the shape of this FLV
        private double m_dMidPoint;
        private double m_dLeftOffset;
        private double m_dRightOffset;

        public FuzzySet_Singleton(double mid, double lft, double rgt) : base(mid)
        {
            m_dMidPoint = mid;
            m_dLeftOffset = lft;
            m_dRightOffset = rgt;
        }
        

        //this method calculates the degree of membership for a particular value
        public override double CalculateDOM(double val)
        {
            if ((val >= m_dMidPoint - m_dLeftOffset) &&
                 (val <= m_dMidPoint + m_dRightOffset))
            {
                return 1.0;
            }

            //out of range of this FLV, return zero
            else
            {
                return 0.0;
            }
        }
    }
}