using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{

    [Serializable]
    public abstract class FuzzyTerm : MonoBehaviour
    {
        //retrieves the degree of membership of the term
        public abstract float GetDOM();

        //clears the degree of membership of the term
        public abstract void ClearDOM();

        //method for updating the DOM of a consequent when a rule fires
        public abstract void ORwithDOM(float val);
    }
}


