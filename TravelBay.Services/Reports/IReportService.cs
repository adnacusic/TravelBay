using TravelBay.Model.SearchObjects;

namespace TravelBay.Services.Reports;

/// <summary>A generated PDF report and the file name it is downloaded under.</summary>
public record ReportFile(byte[] Content, string FileName)
{
    public const string ContentType = "application/pdf";
}

public interface IReportService
{
    Task<ReportFile> GetDestinationsByCategoryAsync();
    Task<ReportFile> GetUserActivityAsync(UserActivityReportFilter filter);
    Task<ReportFile> GetPopularDestinationsAsync(PopularDestinationsReportFilter filter);
}
