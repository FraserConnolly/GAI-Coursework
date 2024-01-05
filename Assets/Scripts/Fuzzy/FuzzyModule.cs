using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using com.cyborgAssets.inspectorButtonPro;

namespace GCU.FraserConnolly.AI.Fuzzy
{

    public class FuzzyModule : MonoBehaviour
    {

        //you must pass one of these values to the defuzzify method. This module
        //only supports the MaxAv and centroid methods.
        public enum DefuzzifyMethod { max_av, centroid };

        //when calculating the centroid of the fuzzy manifold this value is used
        //to determine how many cross-sections should be sampled
        public const int NumSamples = 100;

        //a map of all the fuzzy variables this module uses
        FuzzyVariable[] _variables = Array.Empty<FuzzyVariable>() ;

        //a vector containing all the fuzzy rules
        FuzzyRule[] _rules = Array.Empty<FuzzyRule>();

        //-------------------------- ClearConsequents ---------------------------------
        //
        //  zeros the DOMs of the consequents of each rule. Used by Defuzzify()
        //-----------------------------------------------------------------------------
        void SetConfidencesOfConsequentsToZero()
        {
            foreach (var curRule in _rules)
            {
                curRule.SetConfidenceOfConsequentToZero();
            }
        }

        //creates a new 'empty' fuzzy variable and returns a reference to it.
        [ProButton]
        public FuzzyVariable CreateFLV(string FlvName)
        {
            var go = new GameObject(FlvName);
            go.transform.SetParent(transform, false);
            var flv = go.AddComponent<FuzzyVariable>();

            getFLVs();

            return flv;
        }

        public void getFLVs ( )
        {
            _variables = GetComponentsInChildren<FuzzyVariable>();
        }

        public void getRules( )
        {
            _rules = GetComponentsInChildren<FuzzyRule>();
        }

        //adds a rule to the module
        [ProButton]
        public void AddRule(string name)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(transform, false);
            go.AddComponent<FuzzyRule>();
            getRules();
        }

        public void AddRule(string name, FuzzyTerm antecedent, FuzzyTerm consequence)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(transform, false);
            var rule = go.AddComponent<FuzzyRule>();
            rule.Initialise(antecedent, consequence);
            getRules();
        }

        //----------------------------- Fuzzify ---------------------------------------
        //
        //  this method calls the Fuzzify method of the variable with the same name
        //  as the key
        //-----------------------------------------------------------------------------
        [ProButton]
        public void Fuzzify(string NameOfFLV, float val)
        {
            FuzzyVariable flv = _variables.Where(v => v.VariableName == NameOfFLV).FirstOrDefault();

            //first make sure the key exists
            if (flv == null)
            {
                Debug.LogWarning("<FuzzyModule::Fuzzify>:key not found");
                return;
            }

            flv.Fuzzify(val);
        }

        //---------------------------- DeFuzzify --------------------------------------
        //
        //  given a fuzzy variable and a deffuzification method this returns a 
        //  crisp value
        //-----------------------------------------------------------------------------
        [ProButton]
        public float DeFuzzify(string NameOfFLV, DefuzzifyMethod method = DefuzzifyMethod.max_av)
        {
            getFLVs();
            getRules();
            FuzzyVariable flv = _variables.Where(v => v.VariableName == NameOfFLV).FirstOrDefault();

            //first make sure the key exists
            if (flv == null)
            {
                Debug.Log("<FuzzyModule::DeFuzzifyMaxAv>:key not found");
            }

            //clear the DOMs of all the consequents of all the rules
            SetConfidencesOfConsequentsToZero();

            //process the rules
            foreach (var curRule in _rules)
            {
                curRule.Calculate();
            }

            float defuzzedValue = 0f;

            //now defuzzify the resultant conclusion using the specified method
            switch (method)
            {
                case DefuzzifyMethod.centroid:
                    defuzzedValue = flv.DeFuzzifyCentroid(NumSamples);
                    break;
                case DefuzzifyMethod.max_av:
                    defuzzedValue = flv.DeFuzzifyMaxAv();
                    break;
            }

            Debug.Log($"{gameObject.name} - {flv.VariableName} = {defuzzedValue}", gameObject);

            return defuzzedValue;
        }

        //writes the DOMs of all the variables in the module to an output stream
        [ProButton]
        public void WriteAllDOMs()
        {
            StringBuilder log = new StringBuilder();
            log.AppendLine(gameObject.name);
            foreach (var curVar in _variables)
            {
                log.AppendLine(curVar.VariableName);
                curVar.WriteDOMs(log);
            }
            Debug.Log(log.ToString());
        }
    }
}