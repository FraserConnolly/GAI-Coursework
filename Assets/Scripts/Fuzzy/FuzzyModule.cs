using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{

    class FuzzyModule
    {

        //you must pass one of these values to the defuzzify method. This module
        //only supports the MaxAv and centroid methods.
        public enum DefuzzifyMethod { max_av, centroid };

        //when calculating the centroid of the fuzzy manifold this value is used
        //to determine how many cross-sections should be sampled
        public const int NumSamples = 15;

        //a map of all the fuzzy variables this module uses
        Dictionary<string, FuzzyVariable> m_Variables;

        //a vector containing all the fuzzy rules
        List<FuzzyRule> m_Rules;

        //-------------------------- ClearConsequents ---------------------------------
        //
        //  zeros the DOMs of the consequents of each rule. Used by Defuzzify()
        //-----------------------------------------------------------------------------
        void SetConfidencesOfConsequentsToZero()
        {
            foreach (var curRule in m_Rules)
            {
                curRule.SetConfidenceOfConsequentToZero();
            }
        }

        //creates a new 'empty' fuzzy variable and returns a reference to it.
        public FuzzyVariable CreateFLV(string VarName)
        {
            m_Variables[VarName] = new FuzzyVariable(); ;

            return m_Variables[VarName];
        }

        //adds a rule to the module
        public void AddRule(FuzzyTerm antecedent, FuzzyTerm consequence)
        {
            m_Rules.Add(new FuzzyRule(antecedent, consequence));
        }

        //----------------------------- Fuzzify ---------------------------------------
        //
        //  this method calls the Fuzzify method of the variable with the same name
        //  as the key
        //-----------------------------------------------------------------------------
        public void Fuzzify(string NameOfFLV, double val)
        {
            //first make sure the key exists
            if (!m_Variables.ContainsKey(NameOfFLV))
            {
                Debug.LogWarning("<FuzzyModule::Fuzzify>:key not found");
                return;
            }

            m_Variables[NameOfFLV].Fuzzify(val);
        }

        //---------------------------- DeFuzzify --------------------------------------
        //
        //  given a fuzzy variable and a deffuzification method this returns a 
        //  crisp value
        //-----------------------------------------------------------------------------
        public double DeFuzzify(string NameOfFLV, DefuzzifyMethod method = DefuzzifyMethod.max_av)
        {
            //first make sure the key exists
            if (!m_Variables.ContainsKey(NameOfFLV))
            {
                Debug.Log("<FuzzyModule::DeFuzzifyMaxAv>:key not found");
            }

            //clear the DOMs of all the consequents of all the rules
            SetConfidencesOfConsequentsToZero();

            //process the rules
            foreach (var curRule in m_Rules)
            {
                curRule.Calculate();
            }

            //now defuzzify the resultant conclusion using the specified method
            switch (method)
            {
                case DefuzzifyMethod.centroid:
                    return m_Variables[NameOfFLV].DeFuzzifyCentroid(NumSamples);
                case DefuzzifyMethod.max_av:
                    return m_Variables[NameOfFLV].DeFuzzifyMaxAv();
            }

            return 0;
        }

        //writes the DOMs of all the variables in the module to an output stream
        public void WriteAllDOMs()
        {
            foreach (var curVar in m_Variables)
            {
                StringBuilder log = new StringBuilder();
                log.Append(curVar.Key);
                curVar.Value.WriteDOMs(log);
                Debug.Log(log.ToString());
            }
        }
    }
}