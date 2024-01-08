using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    public class FuzzySet_LeftShoulder : FuzzySet
    {
        //the values that define the shape of this FLV
        [SerializeField]
        private float _PeakPoint;
        [SerializeField]
        private float _RightOffset;
        [SerializeField]
        private float _LeftOffset;

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
            if (((_RightOffset == 0.0) && ((_PeakPoint == val))) ||
                 ((_PeakPoint == val)))
            {
                return 1.0f;
            }

            //find DOM if right of center
            else if ((val >= _PeakPoint) && (val < (_PeakPoint + _RightOffset)))
            {
                float grad = 1.0f / -_RightOffset;

                return grad * (val - _PeakPoint) + 1.0f;
            }

            //find DOM if left of center
            else if ((val < _PeakPoint) && (val >= _PeakPoint - _LeftOffset))
            {
                return 1.0f;
            }

            //out of range of this FLV, return zero
            else
            {
                return 0.0f;
            }

        }
        protected override void OnValidate()
        {
            base.OnValidate();

            if (_LeftOffset < 0f)
            {
                _LeftOffset = 0f;
            }
        }

        public override void GetValueRange(out float min, out float max)
        {
            min = _PeakPoint - _LeftOffset;
            max = _PeakPoint + _RightOffset;
        }

        public override float GetRepresentativeVal()
        {
            return ((_PeakPoint - _LeftOffset) + _PeakPoint) / 2f;
        }
    }
}