using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    public class FuzzySet_Triangle : FuzzySet
    {
        //the values that define the shape of this FLV
        [SerializeField]
        float _PeakPoint;
        
        [SerializeField]
        float _LeftOffset;
        
        [SerializeField]
        float _RightOffset;

        public void Initialise (string name, float mid, float left, float right)
        {
            base.Initialise(name);
            _PeakPoint = mid;
            _LeftOffset = left;
            _RightOffset = right;
        }


        //this method calculates the degree of membership for a particular value
        public override float CalculateDOM(float val)
        {
            //test for the case where the triangle's left or right offsets are zero
            //(to prevent divide by zero errors below)
            if ((_RightOffset == 0.0 && _PeakPoint == val) ||
                 (_LeftOffset == 0.0 && _PeakPoint == val))
            {
                return 1.0f;
            }

            //find DOM if left of center
            if ((val <= _PeakPoint) && (val >= (_PeakPoint - _LeftOffset)))
            {
                float grad = 1.0f / _LeftOffset;

                return grad * (val - (_PeakPoint - _LeftOffset));
            }
            //find DOM if right of center
            else if ((val > _PeakPoint) && (val < (_PeakPoint + _RightOffset)))
            {
                float grad = 1.0f / -_RightOffset;

                return grad * (val - _PeakPoint) + 1.0f;
            }
            //out of range of this FLV, return zero
            else
            {
                return 0.0f;
            }
        }

        public override void GetValueRange(out float min, out float max)
        {
            min = _PeakPoint - _LeftOffset;
            max = _PeakPoint + _RightOffset;
        }

        public override float GetRepresentativeVal()
        {
            return _PeakPoint;
        }
    }
}