namespace Personnel.Tracker.Portal.Models
{
    public class ExportToExcelModel
    {
        public string SheetName { get; set; }
        public string FileName { get; set; }
        public string WebPath { get; set; }

        public ExportToExcelStylingModel Style { get; set; }
        //
    }

    public class ExportToExcelStylingModel
    {
        public string HeaderBackgroundColor { get; set; }
        public string HeaderFontColor { get; set; }

        public bool NoStrippedRows { get; set; }

        public string GroupProperty { get; set; }

    }
}
