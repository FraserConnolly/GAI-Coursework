using System;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    //-----------------------------------------------------------------------------
    //
    //  Name:   FuzzyTerm.h
    //
    //  Author: Mat Buckland (www.ai-junkie.com)
    //
    //  Desc:   abstract class to provide an interface for classes able to be
    //          used as terms in a fuzzy if-then rule base.
    //-----------------------------------------------------------------------------
    public abstract class FuzzyTerm
    {

        //all terms must implement a virtual constructor
        public abstract FuzzyTerm Clone();

        //retrieves the degree of membership of the term
        public abstract double GetDOM();

        //clears the degree of membership of the term
        public abstract void ClearDOM();

        //method for updating the DOM of a consequent when a rule fires
        public abstract void ORwithDOM(double val);

    }
}


