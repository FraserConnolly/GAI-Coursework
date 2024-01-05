using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    public class FuzzySet_RightShoulder : FuzzySet
    {

        //-----------------------------------------------------------------------------
        //
        //  Author: Mat Buckland (www.ai-junkie.com)
        //
        //  Desc:   definition of a fuzzy set that has a right shoulder shape. (the
        //          maximum value this variable can accept is *any* value greater than 
        //          the midpoint.
        //-----------------------------------------------------------------------------

        //the values that define the shape of this FLV
        [SerializeField]
        private float _PeakPoint;
        [SerializeField]
        private float _LeftOffset;
        [SerializeField]
        private float _RightOffset;

        public void Initialise(string name, float peak, float LeftOffset, float RightOffset)
        {
            base.Initialise(name);
            _PeakPoint = peak;
            _LeftOffset = LeftOffset;
            _RightOffset = RightOffset;
        }

        //this method calculates the degree of membership for a particular value
        public override float CalculateDOM(float val)
        {
            //test for the case where the left or right offsets are zero
            //(to prevent divide by zero errors below)
            if (_PeakPoint == val ||
                 ((_LeftOffset == 0.0) && _PeakPoint == val))
            {
                return 1.0f;
            }

            //find DOM if left of center
            else if ((val <= _PeakPoint) && (val > (_PeakPoint - _LeftOffset)))
            {
                float grad = 1.0f / _LeftOffset;

                return grad * (val - (_PeakPoint - _LeftOffset));
            }
            //find DOM if right of center and less than center + right offset
            else if ((val > _PeakPoint) && (val <= _PeakPoint + _RightOffset))
            {
                return 1.0f;
            }

            else
            {
                return 0f;
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if ( _RightOffset < 0f )
            {
                _RightOffset = 0f;
            }
        }

        public override void GetValueRange(out float min, out float max)
        {
            min = _PeakPoint - _LeftOffset;
            max = _PeakPoint + _RightOffset;
        }

        public override float GetRepresentativeVal()
        {
            return ((_PeakPoint + _RightOffset) + _PeakPoint) / 2;
        }
    }
}