using Project.V1.Lib.Helpers.Excel;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Project.V1.LibTest;

public class ExcelLineReaderTests : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly ExcelRequestObj excelRequestObj;

    public ExcelLineReaderTests(ITestOutputHelper output)
    {
        _output = output;
        excelRequestObj = new()
        {
            Headers = new List<string>
                        {
                            "Technology", "Site Id", "Site Name", "RNC/BSC", "Region", "Spectrum", "Bandwidth (MHz)", "Latitude", "Longitude",
                            "Antenna Make", "Antenna Type", "Antenna Height", "Tower Height - (M)", "Antenna Azimuth", "M Tilt", "E Tilt", "Baseband", "RRU TYPE", "Power - (w)",
                            "Project Type", "Project Year", "Summer Config", "Software", "RRU Power - (w)", "CSFB Status GSM", "CSFB Status WCDMA",
                            "Integrated Date", "RET Configured", "Carrier Aggregation", "State", "Project Name", "Comment"
                        }
        };
    }

    [Theory]
    [InlineData("SA_bulk_request_5_4_202255710PM", "29/04/2022")]
    [InlineData("SA_bulk_request_5_3_2022112517PM", "28/04/2022")]
    [InlineData("SA_bulk_request__23__22_04_2022140024", "11/10/2021")]
    public void ToDataTable_ShouldReturnValidIntegrationDate(string filename, string expectedString)
    {
        var excelPath = @$"C:\Users\adekadey\source\repos\MTNN.SiteAcceptance - Copy\Project.V1.Web\Documents\Bulk\{filename}.xlsx";

        var expected = DateTime.Parse(expectedString);

        var (table, Error) = ExcelProcessor.ToDataTable(
            excelRequestObj, excelPath, "adekadey");

        _output.WriteLine($"Message: {Error.ErrorDesc}");
        _output.WriteLine($"Expected: {expected}");

        if (Error.ErrorType.Length <= 0)
        {
            _output.WriteLine($"Actual: {table.Rows[0]["Integrated Date"]}");
        }

        Assert.NotNull(table);
        Assert.True(table.Rows.Count > 0, $"Datatable is empty: {Error.ErrorDesc}");
        Assert.True(Error.ErrorType.Length <= 0, $"{Error.ErrorDesc}");

        if (table.Rows.Count > 0)
        {
            Assert.Equal(expected, table.Rows[0]["Integrated Date"]);
        }
    }

    public void Dispose()
    {
        _output.WriteLine("this has been disposed");
    }
}
