namespace GCU.FraserConnolly.AI.Fuzzy
{
    public interface IFuzzyTerm
    {
        void ClearDOM();
        float GetDOM();
        void ORwithDOM(float val);
    }
}


