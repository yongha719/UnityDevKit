namespace MyGoogleSheetsParser
{
    public interface IGoogleSheetData
    {
        int Index { get; set; }

        void InitializeData(string[] rows);
    }
}