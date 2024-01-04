using GCU.FraserConnolly.AI.Fuzzy;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    //-----------------------------------------------------------------------------
    //
    //  Name:   FuzzyOperators.h
    //
    //  Author: Mat Buckland (www.ai-junkie.com)
    //
    //  Desc:   classes to provide the fuzzy AND and OR operators to be used in
    //          the creation of a fuzzy rule base
    //-----------------------------------------------------------------------------

    ///////////////////////////////////////////////////////////////////////////////
    //
    //  a fuzzy AND operator class
    //
    ///////////////////////////////////////////////////////////////////////////////
    public class FzAND : FuzzyTerm
    {
        //virtual ctor
        public override FuzzyTerm Clone()
        {
            return new FzAND(this);
        }

        //an instance of this class may AND together up to 4 terms
        private List<FuzzyTerm> m_Terms;

        ///////////////////////////////////////////////////////////////////////////////
        //
        //  implementation of FzAND
        //
        ///////////////////////////////////////////////////////////////////////////////
        public FzAND(FzAND fa)
        {
            foreach (var curTerm in m_Terms)
            {
                m_Terms.Add(curTerm.Clone());
            }
        }

        //ctor using two terms
        public FzAND(FuzzyTerm op1, FuzzyTerm op2)
        {
            m_Terms.Add(op1.Clone());
            m_Terms.Add(op2.Clone());
        }

        //ctor using three terms
        public FzAND(FuzzyTerm op1, FuzzyTerm op2, FuzzyTerm op3)
        {
            m_Terms.Add(op1.Clone());
            m_Terms.Add(op2.Clone());
            m_Terms.Add(op3.Clone());
        }

        //ctor using four terms
        public FzAND(FuzzyTerm op1, FuzzyTerm op2, FuzzyTerm op3, FuzzyTerm op4)
        {
            m_Terms.Add(op1.Clone());
            m_Terms.Add(op2.Clone());
            m_Terms.Add(op3.Clone());
            m_Terms.Add(op4.Clone());
        }

        //--------------------------- GetDOM ------------------------------------------
        //
        //  the AND operator returns the minimum DOM of the sets it is operating on
        //-----------------------------------------------------------------------------
        public override double GetDOM()
        {
            double smallest = double.MaxValue;

            foreach (var curTerm in m_Terms)
            {
                if (curTerm.GetDOM() < smallest)
                {
                    smallest = curTerm.GetDOM();
                }
            }

            return smallest;
        }

        //------------------------- ORwithDOM -----------------------------------------
        public override void ORwithDOM(double val)
        {
            foreach (var curTerm in m_Terms)
            {
                curTerm.ORwithDOM(val);
            }
        }

        //---------------------------- ClearDOM ---------------------------------------
        public override void ClearDOM()
        {
            foreach (var curTerm in m_Terms)
            {
                curTerm.ClearDOM();
            }
        }
    }


    ///////////////////////////////////////////////////////////////////////////////
    //
    //  a fuzzy OR operator class
    //
    ///////////////////////////////////////////////////////////////////////////////
    public class FzOR : FuzzyTerm
    {
        //an instance of this class may AND together up to 4 terms
        private List<FuzzyTerm> m_Terms;

        //virtual ctor
        public override FuzzyTerm Clone()
        {
            return new FzOR(this);
        }


        ///////////////////////////////////////////////////////////////////////////////
        //
        //  implementation of FzOR
        //
        ///////////////////////////////////////////////////////////////////////////////
        public FzOR(FzOR fa)
        {
            foreach (var curTerm in m_Terms)
            {
                m_Terms.Add(curTerm.Clone());
            }
        }

        //ctor using two terms
        public FzOR(FuzzyTerm op1, FuzzyTerm op2)
        {
            m_Terms.Add(op1.Clone());
            m_Terms.Add(op2.Clone());
        }

        //ctor using three terms
        public FzOR(FuzzyTerm op1, FuzzyTerm op2, FuzzyTerm op3)
        {
            m_Terms.Add(op1.Clone());
            m_Terms.Add(op2.Clone());
            m_Terms.Add(op3.Clone());
        }

        //ctor using four terms
        public FzOR(FuzzyTerm op1, FuzzyTerm op2, FuzzyTerm op3, FuzzyTerm op4)
        {
            m_Terms.Add(op1.Clone());
            m_Terms.Add(op2.Clone());
            m_Terms.Add(op3.Clone());
            m_Terms.Add(op4.Clone());
        }


        //--------------------------- GetDOM ------------------------------------------
        //
        //  the OR operator returns the maximum DOM of the sets it is operating on
        //----------------------------------------------------------------------------- 
        public override double GetDOM()
        {
            double largest = float.MinValue;

            foreach (var curTerm in m_Terms)
            {
                if (curTerm.GetDOM() > largest)
                {
                    largest = curTerm.GetDOM();
                }
            }

            return largest;
        }

        //unused
        public override void ClearDOM()
        {
            Debug.LogWarning("<FzOR::ClearDOM>: invalid context");
        }

        public override void ORwithDOM(double val)
        {
            Debug.LogWarning("<FzOR::ORwithDOM>: invalid context");
        }
    }
}
