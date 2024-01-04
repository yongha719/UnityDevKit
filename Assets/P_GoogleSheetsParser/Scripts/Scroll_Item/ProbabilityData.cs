using MyInfinityScrollUI;

namespace MyGoogleSheetsParser
{
    public class ProbabilityData : ScrollItemDataBase
    {
        public string Title;
        public double Probability;

        /// <param name="probability">확률</param>
        public void Init(string title, double probability)
        {
            Title = title;
            Probability = probability;
        }
    }
}
