using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Common;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Portal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Personnel.Tracker.Portal.Helpers
{
    public class ExcelHelper
    {
        private readonly ILogger<ExcelHelper> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor; 
       
        public ExcelHelper(ILogger<ExcelHelper> logger, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor; 
        }


        private void SetCellValue(IXLCell cell, dynamic data, PropertyInfo prop)
        {
            var obj = prop.GetValue(data, null);
            if (obj == null)
            {
                cell.SetValue("");
                return;
            }
            switch (Type.GetTypeCode(obj.GetType()))
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                    var value = (double)obj;
                    cell.Value = value;
                    cell.Style.NumberFormat.SetNumberFormatId((int)XLPredefinedFormat.Number.Precision2WithSeparator);
                    cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                    break;

                case TypeCode.Int32:
                    var intValue = (int)obj;
                    cell.Value = intValue;
                    break;
                case TypeCode.String:
                    var stringValue = (string)obj;
                    cell.Value = stringValue;
                    cell.DataType = XLDataType.Text;
                    break;
                case TypeCode.Boolean:
                    var boolValue = (bool)obj;
                    cell.Value = boolValue ? "Yes" : "No";
                    cell.DataType = XLDataType.Text;
                    break;
                case TypeCode.DateTime:
                    var DateTimeValue = (DateTime)obj;
                    cell.Value = DateTimeValue;
                    cell.Style.DateFormat.SetNumberFormatId((int)XLPredefinedFormat.DateTime.MonthDayYear4WithDashesHour24Minutes);
                    cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    break;
            }

        } 

        public OperationResult<UploadedFile> ExportToExcel(ExportToExcelModel model, IEnumerable<dynamic> data, dynamic header = null)
        {
            OperationResult<UploadedFile> result = new OperationResult<UploadedFile>();
            try
            {
                string fileName = $"{model.FileName}.xlsx";
                try
                {

                    int index = 1;

                    int rowStyleIndex = 0;
                    using (var workbook = new XLWorkbook())
                    {

                        IXLWorksheet worksheet =
                        workbook.Worksheets.Add(model.SheetName);
                        var firstColume = data.FirstOrDefault();

                        Type myType = firstColume.GetType();
                        IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
                        var colIndex = 1;
                        foreach (PropertyInfo prop in props)
                        { 
                            worksheet.Cell(index, colIndex).Value = prop.Name;
                            colIndex++;
                        }


                        XLColor backgroundColor = XLColor.Purple;
                        worksheet.Range(index, 1, index, colIndex - 1).Style.Fill.BackgroundColor = backgroundColor;
                         
                        XLColor fontColor = XLColor.White;
                        worksheet.Range(index, 1, index, colIndex - 1).Style.Font.FontColor = fontColor;

                        worksheet.Range(index, 1, index, colIndex - 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        index++;

                        bool useStrippedRows = model.Style != null && model.Style.NoStrippedRows ? false : true;
                        bool groupStrippedRows = model.Style != null && model.Style.GroupProperty.IsNotEmpty();

                        object groupValue = null;
                        object preGroupValue = null;
                        XLColor lastGroupColor = null;

                        foreach (var line in data)
                        {
                            colIndex = 1;
                            foreach (PropertyInfo prop in props)
                            {
                                
                                var cell = worksheet.Cell(index, colIndex);

                                SetCellValue(cell, line, prop);

                                if (groupStrippedRows)
                                {
                                    if (prop.Name.EqualsInsensitive(model.Style.GroupProperty))
                                    {
                                        groupValue = prop.GetValue(line, null);
                                    }
                                }


                                colIndex++;
                            }

                            if (useStrippedRows)
                            {
                                if (groupStrippedRows)
                                {
                                    lastGroupColor = groupValue == preGroupValue && lastGroupColor != null ? lastGroupColor : (lastGroupColor == XLColor.White ? XLColor.FromHtml("#f2f4f4") : XLColor.White);
                                    worksheet.Range(index, 1, index, colIndex - 1).Style.Fill.BackgroundColor = lastGroupColor;
                                    preGroupValue = groupValue;
                                }
                                else
                                {
                                    worksheet.Range(index, 1, index, colIndex - 1).Style.Fill.BackgroundColor = rowStyleIndex % 2 == 0 ? XLColor.White : XLColor.WhiteSmoke;

                                }
                            }

                            worksheet.Range(index, 1, index, colIndex - 1).Rows().Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            worksheet.Range(index, 1, index, colIndex - 1).Rows().Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            worksheet.Range(index, 1, index, colIndex - 1).Rows().Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            worksheet.Range(index, 1, index, colIndex - 1).Rows().Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            worksheet.Range(index, 1, index, colIndex - 1).Rows().Style.Border.TopBorderColor = XLColor.FromHtml("#d4d4d4");
                            worksheet.Range(index, 1, index, colIndex - 1).Rows().Style.Border.BottomBorderColor = XLColor.FromHtml("#d4d4d4");
                            worksheet.Range(index, 1, index, colIndex - 1).Rows().Style.Border.RightBorderColor = XLColor.FromHtml("#d4d4d4");
                            worksheet.Range(index, 1, index, colIndex - 1).Rows().Style.Border.LeftBorderColor = XLColor.FromHtml("#d4d4d4");


                            rowStyleIndex++;
                            index++;
                        }
                        rowStyleIndex = 0;


                        worksheet.Rows().AdjustToContents();
                        worksheet.Columns().AdjustToContents(); 

                        var webPath = string.IsNullOrEmpty(model.WebPath) ? $"Uploads/xls/{DateTime.Now.ToString()}/" : model.WebPath;
                        if (webPath.StartsWith("/") || webPath.StartsWith("\\"))
                        {
                            webPath = webPath.Substring(1, webPath.Length - 1);
                        }
                        var uploads = Path.Combine(_environment.WebRootPath, webPath);
                        if (!Directory.Exists(uploads))
                        {
                            Directory.CreateDirectory(uploads);
                        }
                        workbook.SaveAs(Path.Combine(uploads, fileName));
                        var FilePath = $"/{Path.Combine(webPath, fileName)}";
                        result.Result = true;
                        result.Response = new UploadedFile { FileName = fileName, FilePath = FilePath };
                        
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ExportToExcel {@Model}", model);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExportToExcel {@Model} ", model);
            }

            return result; 

        }
    }

}
