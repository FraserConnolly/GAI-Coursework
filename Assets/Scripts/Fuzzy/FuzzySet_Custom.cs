using System;
using System.Linq;
using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{
    public class FuzzySet_Custom : FuzzySet
    {
        private void Awake()
        {
            if (_curve == null)
            {
                _curve = new AnimationCurve();
                _curve.AddKey(0f,0f);
            }
        }

        public override float CalculateDOM(float val)
        {
            if (_curve == null || _curve.keys.Count() == 0)
            {
                return 0f;
            }

            return _curve.Evaluate((float)val);
        }

        protected override void OnValidate()
        {
            // prevent base buildCurve from being executed.
        }

        public override void GetValueRange(out float min, out float max)
        {
            if ( _curve == null || _curve.keys.Count() == 0)
            {
                min = float.MaxValue;
                max = float.MinValue;
                return;
            }
            
            min = _curve.keys.Min(kf => kf.time);
            max = _curve.keys.Max(kf => kf.time);
        }

        public override float GetRepresentativeVal()
        {
            throw new NotImplementedException();
        }
    }
}
