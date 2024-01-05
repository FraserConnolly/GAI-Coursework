using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    //-----------------------------------------------------------------------------
    //
    //  Name:   FuzzySet.h
    //
    //  Author: Mat Buckland (www.ai-junkie.com)
    //
    //  Desc:   class to define an interface for a fuzzy set
    //-----------------------------------------------------------------------------
    public abstract class FuzzySet : FuzzyTerm
    {

        public string SetName => gameObject.name;
        
        [SerializeField]
        private Color _colour = Color.white;
        
        public Color Colour => _colour;

        [SerializeField]
        protected AnimationCurve _curve;

        private void buildCurve()
        {
            if ( _curve == null )
            {
                _curve = new AnimationCurve();
            }

            _curve.ClearKeys();
            GetValueRange(out float min, out float max);
            float StepSize = (max - min) / (float)FuzzyModule.NumSamples;

            for (int i = 0; i < FuzzyModule.NumSamples; i++)
            {
                float x = (float) ( i * StepSize ) + min;
                Keyframe kf = new Keyframe(x, (float)CalculateDOM(x));
                _curve.AddKey(kf);
            }
        }

        //this will hold the degree of membership of a given value in this set 
        protected float _DOM;

        
        public abstract void GetValueRange(out float min, out float max);

        public void Initialise(string name)
        {
            _curve = new AnimationCurve();
            gameObject.name = name;
            _DOM = 0.0f;
        }

        //return the degree of membership in this set of the given value. NOTE,
        //this does not set m_dDOM to the DOM of the value passed as the parameter.
        //This is because the centroid defuzzification method also uses this method
        //to determine the DOMs of the values it uses as its sample points.
        public abstract float CalculateDOM(float val);

        //if this fuzzy set is part of a consequent FLV, and it is fired by a rule 
        //then this method sets the DOM (in this context, the DOM represents a
        //confidence level)to the maximum of the parameter value or the set's 
        //existing _DOM value
        public override void ORwithDOM(float val)
        {
            if (val > _DOM)
            {
                _DOM = val;
            }
        }

        //this is the maximum of the set's membership function. For instance, if
        //the set is triangular then this will be the peak point of the triangular.
        //if the set has a plateau then this value will be the mid point of the 
        //plateau.
        public abstract float GetRepresentativeVal();

        public override void ClearDOM()
        {
            _DOM = 0.0f;
        }

        public override float GetDOM()
        {
            return _DOM;
        }

        public void SetDOM(float val)
        {
            if (! ( (val <= 1f) && (val >= 0f) ) )
            {
                Debug.Log("<FuzzySet::SetDOM>: invalid value");
            }
            _DOM = val;
        }

        protected virtual void OnValidate()
        {
            buildCurve();
        }
    }
}