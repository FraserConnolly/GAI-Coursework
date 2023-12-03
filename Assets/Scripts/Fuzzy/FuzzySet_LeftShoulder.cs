using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    public class FuzzySet_LeftShoulder : FuzzySet
    {
        //-----------------------------------------------------------------------------
        //
        //  Author: Mat Buckland (www.ai-junkie.com)
        //
        //  Desc:   definition of a fuzzy set that has a left shoulder shape. (the
        //          minimum value this variable can accept is *any* value less than the
        //          midpoint.
        //-----------------------------------------------------------------------------

        //the values that define the shape of this FLV
        private double m_dPeakPoint;
        private double m_dRightOffset;
        private double m_dLeftOffset;

        public FuzzySet_LeftShoulder(double peak, double LeftOffset, double RightOffset)
            : base(((peak - LeftOffset) + peak) / 2)
        {
            m_dPeakPoint = peak;
            m_dLeftOffset = LeftOffset;
            m_dRightOffset = RightOffset;
        }

        //this method calculates the degree of membership for a particular value
        public override double CalculateDOM(double val)
        {
            //test for the case where the left or right offsets are zero
            //(to prevent divide by zero errors below)
            if (((m_dRightOffset == 0.0) && ((m_dPeakPoint == val))) ||
                 ((m_dLeftOffset == 0.0) && ((m_dPeakPoint == val))))
            {
                return 1.0;
            }

            //find DOM if right of center
            else if ((val >= m_dPeakPoint) && (val < (m_dPeakPoint + m_dRightOffset)))
            {
                double grad = 1.0 / -m_dRightOffset;

                return grad * (val - m_dPeakPoint) + 1.0;
            }

            //find DOM if left of center
            else if ((val < m_dPeakPoint) && (val >= m_dPeakPoint - m_dLeftOffset))
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