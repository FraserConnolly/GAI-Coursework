using System;
using System.Runtime.Serialization;
using UnityEngine;

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
    [Serializable]
    public abstract class FuzzyTerm : MonoBehaviour, IFuzzyTerm
    {
        //retrieves the degree of membership of the term
        public abstract float GetDOM();

        //clears the degree of membership of the term
        public abstract void ClearDOM();

        //method for updating the DOM of a consequent when a rule fires
        public abstract void ORwithDOM(float val);
    }
}


