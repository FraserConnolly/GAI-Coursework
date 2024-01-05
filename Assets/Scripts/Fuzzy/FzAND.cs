using System;
using System.Collections.Generic;
using UnityEngine;

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
    [Serializable]
    public class FzAND : FuzzyTerm
    {

        //an instance of this class may AND together up to 4 terms
        [SerializeField]
        private List<FuzzyTerm> m_Terms;

        ///////////////////////////////////////////////////////////////////////////////
        //
        //  implementation of FzAND
        //
        ///////////////////////////////////////////////////////////////////////////////
        public FzAND(FzAND fa) : this()
        {
            foreach (var curTerm in m_Terms)
            {
                m_Terms.Add(curTerm);
            }
        }

        public FzAND()
        {
            m_Terms = new List<FuzzyTerm>();
        }

        public FzAND( params FuzzyTerm[] op) : this()
        {
            foreach (var term in op)
            {
                m_Terms.Add(term);
            }
        }

        //--------------------------- GetDOM ------------------------------------------
        //
        //  the AND operator returns the minimum DOM of the sets it is operating on
        //-----------------------------------------------------------------------------
        public override float GetDOM()
        {
            float smallest = float.MaxValue;

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
        public override void ORwithDOM(float val)
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
}
