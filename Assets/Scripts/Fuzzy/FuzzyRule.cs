using com.cyborgAssets.inspectorButtonPro;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{

    //-----------------------------------------------------------------------------
    //
    //  Name:   FuzzyRule.h
    //
    //  Author: Mat Buckland (www.ai-junkie.com)
    //
    //  Desc:   This class implements a fuzzy rule of the form:
    //  
    //          IF fzVar1 AND fzVar2 AND ... fzVarn THEN fzVar.c
    //
    //          
    //-----------------------------------------------------------------------------
    public class FuzzyRule : MonoBehaviour
    {
        public void Initialise (FuzzyTerm antecedent, FuzzyTerm consequence)
        {
            Antecedent = antecedent;
            Consequence = consequence;
        }

        [Header("Antecedent")]
        [SerializeField]
        private FuzzyTerm _antecedent;

        [SerializeField]
        private FuzzyTerm _consequence;

        //antecedent (usually a composite of several fuzzy sets and operators)
        public FuzzyTerm Antecedent { get => _antecedent; set => _antecedent = value; }
        
        //consequence (usually a single fuzzy set, but can be several ANDed together)
        public FuzzyTerm Consequence { get => _consequence; set => _consequence = value; }

        //this method updates the DOM (the confidence) of the consequent term with
        //the DOM of the antecedent term. 
        internal void Calculate()
        {
            Consequence.ORwithDOM(Antecedent.GetDOM());
        }

        internal void SetConfidenceOfConsequentToZero()
        {
            Consequence.ClearDOM();
        }
    }
}
