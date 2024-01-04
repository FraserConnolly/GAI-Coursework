using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    //-----------------------------------------------------------------------------
    //
    //  Name:   FuzzyVariable.h
    //
    //  Author: Mat Buckland (www.ai-junkie.com)
    //
    //  Desc:   Class to define a fuzzy linguistic variable (FLV).
    //
    //          An FLV comprises of a number of fuzzy sets  
    //
    //-----------------------------------------------------------------------------
    public class FuzzyVariable
    {

        //the minimum and maximum value of the range of this variable
        private double m_dMinRange;
        private double m_dMaxRange;


        //a map of the fuzzy sets that comprise this variable
        private Dictionary<string, FuzzySet> m_MemberSets;

        //---------------------------- AdjustRangeToFit -------------------------------
        //
        //  this method is called with the upper and lower bound of a set each time a
        //  new set is added to adjust the upper and lower range values accordingly
        //-----------------------------------------------------------------------------
        private void AdjustRangeToFit(double minBound, double maxBound)
        {
            if (minBound < m_dMinRange) m_dMinRange = minBound;
            if (maxBound > m_dMaxRange) m_dMaxRange = maxBound;
        }

        //--------------------------- Fuzzify -----------------------------------------
        //
        //  takes a crisp value and calculates its degree of membership for each set
        //  in the variable.
        //-----------------------------------------------------------------------------
        public void Fuzzify(double val)
        {
            //make sure the value is within the bounds of this variable
            if ((val >= m_dMinRange) && (val <= m_dMaxRange))
            {
                Debug.Log("<FuzzyVariable::Fuzzify>: value out of range");
            }

            //for each set in the flv calculate the DOM for the given value
            foreach (var curSet in m_MemberSets)
            {
                curSet.Value.SetDOM(curSet.Value.CalculateDOM(val));
            }
        }


        //------------------------- DeFuzzifyCentroid ---------------------------------
        //
        //  defuzzify the variable using the centroid method
        //-----------------------------------------------------------------------------
        public double DeFuzzifyCentroid(int NumSamples)
        {
            //calculate the step size
            double StepSize = (m_dMaxRange - m_dMinRange) / (double)NumSamples;

            double TotalArea = 0.0;
            double SumOfMoments = 0.0;

            //step through the range of this variable in increments equal to StepSize
            //adding up the contribution (lower of CalculateDOM or the actual DOM of this
            //variable's fuzzified value) for each subset. This gives an approximation of
            //the total area of the fuzzy manifold.(This is similar to how the area under
            //a curve is calculated using calculus... the heights of lots of 'slices' are
            //summed to give the total area.)
            //
            //in addition the moment of each slice is calculated and summed. Dividing
            //the total area by the sum of the moments gives the centroid. (Just like
            //calculating the center of mass of an object)
            for (int samp = 1; samp <= NumSamples; ++samp)
            {
                //for each set get the contribution to the area. This is the lower of the 
                //value returned from CalculateDOM or the actual DOM of the fuzzified 
                //value itself   
                foreach (var curSet in m_MemberSets)
                {
                    double contribution = Math.Min(curSet.Value.CalculateDOM(m_dMinRange + samp * StepSize),
                              curSet.Value.GetDOM());

                    TotalArea += contribution;

                    SumOfMoments += (m_dMinRange + samp * StepSize) * contribution;
                }
            }

            //make sure total area is not equal to zero
            if (0d == TotalArea)
            {
                return 0.0;
            }

            return (SumOfMoments / TotalArea);
        }

        //--------------------------- DeFuzzifyMaxAv ----------------------------------
        //
        // defuzzifies the value by averaging the maxima of the sets that have fired
        //
        // OUTPUT = sum (maxima * DOM) / sum (DOMs) 
        //-----------------------------------------------------------------------------
        public double DeFuzzifyMaxAv()
        {
            double bottom = 0.0;
            double top = 0.0;

            foreach ( var curSet in m_MemberSets)
            {
                bottom += curSet.Value.GetDOM();

                top += curSet.Value.GetRepresentativeVal() * curSet.Value.GetDOM();
            }

            //make sure bottom is not equal to zero
            if (0 == bottom)
            {
                return 0.0;
            }

            return top / bottom;
        }

        //the following methods create instances of the sets named in the method
        //name and add them to the member set map. Each time a set of any type is
        //added the m_dMinRange and m_dMaxRange are adjusted accordingly. All of the
        //methods return a proxy class representing the newly created instance. This
        //proxy set can be used as an operand when creating the rule base.


        //------------------------- AddTriangularSet ----------------------------------
        //
        //  adds a triangular shaped fuzzy set to the variable
        //-----------------------------------------------------------------------------
        public FzSet AddTriangularSet(string name,
                                             double minBound,
                                             double peak,
                                             double maxBound)
        { 
            m_MemberSets[name] = new FuzzySet_Triangle(peak,
                                                       peak - minBound,
                                                       maxBound - peak);
            //adjust range if necessary
            AdjustRangeToFit(minBound, maxBound);

            return new FzSet(m_MemberSets[name]);
        }

        //--------------------------- AddLeftShoulder ---------------------------------
        //
        //  adds a left shoulder type set
        //-----------------------------------------------------------------------------
        public FzSet AddLeftShoulderSet( string name,
                                                double minBound,
                                                double peak,
                                                double maxBound)
        {
            m_MemberSets[name] = new FuzzySet_LeftShoulder(peak, peak - minBound, maxBound - peak);

            //adjust range if necessary
            AdjustRangeToFit(minBound, maxBound);

            return new FzSet(m_MemberSets[name]);
        }


        //--------------------------- AddRightShoulder ---------------------------------
        //
        //  adds a left shoulder type set
        //-----------------------------------------------------------------------------
        public FzSet AddRightShoulderSet(string name,
                                                 double minBound,
                                                 double peak,
                                                 double maxBound)
        {
            m_MemberSets[name] = new FuzzySet_RightShoulder(peak, peak - minBound, maxBound - peak);

            //adjust range if necessary
            AdjustRangeToFit(minBound, maxBound);

            return new FzSet( m_MemberSets[ name ] );
        }

        //--------------------------- AddSingletonSet ---------------------------------
        //
        //  adds a singleton to the variable
        //-----------------------------------------------------------------------------
        public FzSet AddSingletonSet( string name,
                                            double minBound,
                                            double peak,
                                            double maxBound)
        {
            m_MemberSets[name] = new FuzzySet_Singleton(peak,
                                                        peak - minBound,
                                                        maxBound - peak);

            AdjustRangeToFit(minBound, maxBound);

            return new FzSet( m_MemberSets[ name ] );
        }

        //---------------------------- WriteDOMs --------------------------------------
        public void WriteDOMs(StringBuilder log)
        {
            foreach (var it in m_MemberSets)
            {
                log.AppendLine($"{it.Key} is {it.Value.GetDOM()}");
            }

            log.AppendLine($"Min Range: {m_dMinRange} Max Range: {m_dMaxRange} ");
        }
    }
}
