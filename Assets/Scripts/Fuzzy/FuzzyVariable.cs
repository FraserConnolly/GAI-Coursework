using com.cyborgAssets.inspectorButtonPro;
using System.Collections.Generic;
using System.Linq;
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
    public class FuzzyVariable : MonoBehaviour
    {
        //the minimum and maximum value of the range of this variable
        [SerializeField]
        [Tooltip("Readonly - Calculated based on the valid ranges of the Fuzzy Sets for this variable.")]
        private float _MinRange;

        public float MinRange => _MinRange;

        [SerializeField]
        [Tooltip("Readonly - Calculated based on the valid ranges of the Fuzzy Sets for this variable.")]
        private float _MaxRange;
        
        public float MaxRange => _MaxRange;

        public string VariableName => gameObject.name;

        public float CrispValue { get; private set; }

        //a map of the fuzzy sets that comprise this variable
        private FuzzySet[] _Sets;

        public IReadOnlyCollection<FuzzySet> GetSets() => _Sets;

        public void UpdateSets()
        {
            _Sets = GetComponentsInChildren<FuzzySet>();

            if ( ! _Sets.Any() )
            {
                _MaxRange = 1f;
                _MinRange = 0f;
                return;
            }

            _MaxRange = float.MinValue;
            _MinRange = float.MaxValue;

            foreach (FuzzySet set in _Sets)
            {
                set.GetValueRange(out float min, out float max);

                if ( max > _MaxRange )
                {
                    _MaxRange = max;
                }

                if ( min < _MinRange )
                {
                    _MinRange = min;
                }
            }

            if ( _MaxRange == _MinRange )
            {
                _MaxRange = 1f;
                _MinRange = 0f;
                return;
            }
        }

        //--------------------------- Fuzzify -----------------------------------------
        //
        //  takes a crisp value and calculates its degree of membership for each set
        //  in the variable.
        //-----------------------------------------------------------------------------
        public void Fuzzify(float val)
        {
            if ( _Sets == null )
            {
                UpdateSets();
            }

            //make sure the value is within the bounds of this variable
            if (!((val >= _MinRange) && (val <= _MaxRange)))
            {
                Debug.Log("<FuzzyVariable::Fuzzify>: value out of range", gameObject);
            }

            CrispValue = val;

            //for each set in the FLV calculate the DOM for the given value
            foreach (var curSet in _Sets)
            {
                curSet.SetDOM(curSet.CalculateDOM(val));
            }
        }

        //------------------------- DeFuzzifyCentroid ---------------------------------
        //
        //  defuzzify the variable using the centroid method
        //-----------------------------------------------------------------------------
        public float DeFuzzifyCentroid(int NumSamples)
        {
            if (_Sets == null)
            {
                UpdateSets();
            }

            //calculate the step size
            float StepSize = (_MaxRange - _MinRange) / NumSamples;

            float TotalArea = 0.0f;
            float SumOfMoments = 0.0f;

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
                foreach (var curSet in _Sets)
                {
                    float contribution = Mathf.Min(curSet.CalculateDOM(_MinRange + samp * StepSize),
                              curSet.GetDOM());

                    TotalArea += contribution;

                    SumOfMoments += (_MinRange + samp * StepSize) * contribution;
                }
            }

            //make sure total area is not equal to zero
            if (0f == TotalArea)
            {
                return 0.0f;
            }

            return (SumOfMoments / TotalArea);
        }

        //--------------------------- DeFuzzifyMaxAv ----------------------------------
        //
        // defuzzifies the value by averaging the maxima of the sets that have fired
        //
        // OUTPUT = sum (maxima * DOM) / sum (DOMs) 
        //-----------------------------------------------------------------------------
        public float DeFuzzifyMaxAv()
        {
            if (_Sets == null)
            {
                UpdateSets();
            }

            float bottom = 0.0f;
            float top = 0.0f;

            foreach ( var curSet in _Sets)
            {
                bottom += curSet.GetDOM();

                top += curSet.GetRepresentativeVal() * curSet.GetDOM();
            }

            //make sure bottom is not equal to zero
            if (0 == bottom)
            {
                return 0.0f;
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
        public FuzzySet AddTriangularSet(string name,
                                             float minBound,
                                             float peak,
                                             float maxBound)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            var set = go.AddComponent<FuzzySet_Triangle>();
            set.Initialise(name, peak, peak - minBound, maxBound - peak);
            UpdateSets();
            return set;
        }

        //--------------------------- AddLeftShoulder ---------------------------------
        //
        //  adds a left shoulder type set
        //-----------------------------------------------------------------------------
        public FuzzySet AddLeftShoulderSet( string name,
                                                float minBound,
                                                float peak,
                                                float maxBound)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            var set = go.AddComponent<FuzzySet_LeftShoulder>();
            set.Initialise(name, peak, peak - minBound, maxBound - peak);
            UpdateSets();
            return set;
        }


        //--------------------------- AddRightShoulder ---------------------------------
        //
        //  adds a left shoulder type set
        //-----------------------------------------------------------------------------
        public FuzzySet AddRightShoulderSet(string name,
                                                 float minBound,
                                                 float peak,
                                                 float maxBound)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            var set = go.AddComponent<FuzzySet_RightShoulder>();
            set.Initialise(name, peak, peak - minBound, maxBound - peak);
            UpdateSets();
            return set;
        }

        //--------------------------- AddSingletonSet ---------------------------------
        //
        //  adds a singleton to the variable
        //-----------------------------------------------------------------------------
        public FuzzySet AddSingletonSet( string name,
                                            float minBound,
                                            float peak,
                                            float maxBound)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            var set = go.AddComponent<FuzzySet_Singleton>();
            set.Initialise(name, peak, peak - minBound, maxBound - peak);
            UpdateSets();
            return set;
        }

        //---------------------------- WriteDOMs --------------------------------------
        public void WriteDOMs(StringBuilder log)
        {
            foreach (var it in _Sets)
            {
                log.AppendLine($"{it.name} is {it.GetDOM()}");
            }

            log.AppendLine($"Min Range: {_MinRange} Max Range: {_MaxRange} ");
        }

        public float SumOfMembership(float val)
        {
            //make sure the value is within the bounds of this variable
            if (!((val >= _MinRange) && (val <= _MaxRange)))
            {
                Debug.Log("<FuzzyVariable::SumOfMembership>: value out of range");
            }

            float sum = 0f;

            //for each set in the FLV calculate the DOM for the given value
            foreach (var curSet in _Sets)
            {
                sum += curSet.CalculateDOM(val);
            }

            return sum;
        }

        public void OnValidate()
        {
            UpdateSets();
        }
    }
}
