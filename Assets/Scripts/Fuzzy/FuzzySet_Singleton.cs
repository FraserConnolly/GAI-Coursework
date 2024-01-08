using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    public class FuzzySet_Singleton : FuzzySet
    {

        //the values that define the shape of this FLV
        [SerializeField]
        private float _MidPoint;
        [SerializeField]
        private float _LeftOffset;
        [SerializeField]
        private float _RightOffset;

        public void Initialise(string name, float mid, float left, float right)
        {
            base.Initialise(name);
            _MidPoint = mid;
            _LeftOffset = left;
            _RightOffset = right;
        }
        
        //this method calculates the degree of membership for a particular value
        public override float CalculateDOM(float val)
        {
            if ((val >= _MidPoint - _LeftOffset) &&
                 (val <= _MidPoint + _RightOffset))
            {
                return 1.0f;
            }

            //out of range of this FLV, return zero
            else
            {
                return 0.0f;
            }
        }

        public override void GetValueRange(out float min, out float max)
        {
            min = _MidPoint - _LeftOffset;
            max = _MidPoint + _RightOffset;
        }

        public override float GetRepresentativeVal()
        {
            return _MidPoint;
        }
    }
}