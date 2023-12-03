using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    public class FuzzySet_RightShoulder : FuzzySet
    {

        //-----------------------------------------------------------------------------
        //
        //  Author: Mat Buckland (www.ai-junkie.com)
        //
        //  Desc:   definition of a fuzzy set that has a right shoulder shape. (the
        //          maximum value this variable can accept is *any* value greater than 
        //          the midpoint.
        //-----------------------------------------------------------------------------

        //the values that define the shape of this FLV
        private double m_dPeakPoint;
        private double m_dLeftOffset;
        private double m_dRightOffset;

        public FuzzySet_RightShoulder(double peak, double LeftOffset, double RightOffset) :
                  base(((peak + RightOffset) + peak) / 2)
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

            //find DOM if left of center
            else if ((val <= m_dPeakPoint) && (val > (m_dPeakPoint - m_dLeftOffset)))
            {
                double grad = 1.0 / m_dLeftOffset;

                return grad * (val - (m_dPeakPoint - m_dLeftOffset));
            }
            //find DOM if right of center and less than center + right offset
            else if ((val > m_dPeakPoint) && (val <= m_dPeakPoint + m_dRightOffset))
            {
                return 1.0;
            }

            else
            {
                return 0;
            }
        }
    }
}