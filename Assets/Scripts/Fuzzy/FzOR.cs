using com.cyborgAssets.inspectorButtonPro;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GCU.FraserConnolly.AI.Fuzzy
{


    ///////////////////////////////////////////////////////////////////////////////
    //
    //  a fuzzy OR operator class
    //
    ///////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class FzOR : FuzzyTerm
    {

        //an instance of this class may AND together up to 4 terms
        [SerializeField]
        private List<FuzzyTerm> m_Terms;

        ///////////////////////////////////////////////////////////////////////////////
        //
        //  implementation of FzOR
        //
        ///////////////////////////////////////////////////////////////////////////////
        public FzOR()
        {
            m_Terms = new List<FuzzyTerm>();
        }

        //ctor using four terms
        public FzOR(params FuzzyTerm [] op) : this()
        {
            foreach(var curTerm in op) {
                m_Terms.Add(curTerm);
            }
        }


        //--------------------------- GetDOM ------------------------------------------
        //
        //  the OR operator returns the maximum DOM of the sets it is operating on
        //----------------------------------------------------------------------------- 
        public override float GetDOM()
        {
            float largest = float.MinValue;

            foreach (var curTerm in m_Terms)
            {
                if (curTerm.GetDOM() > largest)
                {
                    largest = curTerm.GetDOM();
                }
            }

            return largest;
        }

        //unused
        public override void ClearDOM()
        {
            Debug.LogWarning("<FzOR::ClearDOM>: invalid context");
        }

        public override void ORwithDOM(float val)
        {
            Debug.LogWarning("<FzOR::ORwithDOM>: invalid context");
        }
    }
}
