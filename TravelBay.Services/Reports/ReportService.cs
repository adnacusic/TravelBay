using System.Globalization;
using TravelBay.Model.Constants;
using TravelBay.Model.Enums;
using TravelBay.Model.Exceptions;
using TravelBay.Model.SearchObjects;
using TravelBay.Services.Database;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace TravelBay.Services.Reports;

/// <summary>
/// Builds the admin PDF reports. Each report is one aggregate SQL query (GroupBy/Count/Average
/// run by the database, only the result rows come back), laid out by <see cref="TableReportDocument"/>.
/// </summary>
public class ReportService : IReportService
{
    public const int DefaultTopDestinations = 10;
    public const int MaxTopDestinations = 50;

    /// <summary>Report times are shown in the local time of the app's users.</summary>
    private const string ReportTimeZoneId = "Europe/Sarajevo";

    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("bs-Latn-BA");

    private readonly TravelBayDbContext _dbContext;

    public ReportService(TravelBayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ReportFile> GetDestinationsByCategoryAsync()
    {
        // Soft-deleted destinations and reviews are excluded by the global query filters.
        var rows = await _dbContext.Categories
            .AsNoTracking()
            .Select(c => new
            {
                c.Name,
                c.IsActive,
                DestinationCount = c.Destinations.Count(),
                ApprovedReviewCount = c.Destinations
                    .SelectMany(d => d.Reviews)
                    .Count(r => r.Status == ReviewStatus.Approved),
                AverageRating = c.Destinations
                    .SelectMany(d => d.Reviews)
                    .Where(r => r.Status == ReviewStatus.Approved)
                    .Average(r => (double?)r.Rating)
            })
            .OrderByDescending(c => c.DestinationCount)
            .ThenBy(c => c.Name)
            .ToListAsync();

        var totalDestinations = rows.Sum(r => r.DestinationCount);
        var totalReviews = rows.Sum(r => r.ApprovedReviewCount);

        var document = new TableReportDocument(
            title: "Destinacije po kategorijama",
            subtitle: "Broj destinacija u svakoj kategoriji i prosječna ocjena iz odobrenih recenzija.",
            generatedAt: Now(),
            columns: new[]
            {
                new ReportColumn("Kategorija", 3),
                new ReportColumn("Status", 1.5f),
                new ReportColumn("Destinacija", 1.5f, AlignRight: true),
                new ReportColumn("Udio", 1.2f, AlignRight: true),
                new ReportColumn("Odobrenih recenzija", 2, AlignRight: true),
                new ReportColumn("Prosječna ocjena", 1.8f, AlignRight: true),
            },
            rows: rows.Select(r => new[]
            {
                r.Name,
                r.IsActive ? "Aktivna" : "Neaktivna",
                Number(r.DestinationCount),
                Percent(r.DestinationCount, totalDestinations),
                Number(r.ApprovedReviewCount),
                Rating(r.AverageRating),
            }).ToList(),
            summary: new[]
            {
                $"Ukupno kategorija: {rows.Count}",
                $"Ukupno destinacija: {totalDestinations}",
                $"Ukupno odobrenih recenzija: {totalReviews}",
            },
            emptyText: "Još nema unesenih kategorija.");

        return new ReportFile(document.GeneratePdf(), FileName("destinacije-po-kategorijama"));
    }

    public async Task<ReportFile> GetUserActivityAsync(UserActivityReportFilter filter)
    {
        if (filter.From.HasValue && filter.To.HasValue && filter.From.Value.Date > filter.To.Value.Date)
        {
            throw new ClientException("Datum \"od\" ne može biti nakon datuma \"do\".");
        }

        // Whole days: from the start of "From" up to (not including) the day after "To".
        var fromUtc = filter.From?.Date ?? DateTime.MinValue;
        var toExclusiveUtc = filter.To?.Date.AddDays(1) ?? DateTime.MaxValue;

        var rows = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.UserRoles.Any(ur => ur.Role.Name == RoleNames.User))
            .Select(u => new
            {
                u.FirstName,
                u.LastName,
                u.Username,
                u.IsActive,
                ReviewCount = _dbContext.Reviews.Count(r =>
                    r.UserId == u.Id && r.CreatedAt >= fromUtc && r.CreatedAt < toExclusiveUtc),
                ApprovedReviewCount = _dbContext.Reviews.Count(r =>
                    r.UserId == u.Id && r.Status == ReviewStatus.Approved
                    && r.CreatedAt >= fromUtc && r.CreatedAt < toExclusiveUtc),
                ViewCount = _dbContext.ViewHistories.Count(v =>
                    v.UserId == u.Id && v.ViewedAt >= fromUtc && v.ViewedAt < toExclusiveUtc),
                TripPlanCount = _dbContext.TripPlans.Count(t =>
                    t.UserId == u.Id && t.CreatedAt >= fromUtc && t.CreatedAt < toExclusiveUtc)
            })
            .OrderByDescending(u => u.ReviewCount + u.ViewCount + u.TripPlanCount)
            .ThenBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync();

        var period = (filter.From, filter.To) switch
        {
            (null, null) => "cijeli period",
            (DateTime from, null) => $"od {Date(from)}",
            (null, DateTime to) => $"do {Date(to)}",
            (DateTime from, DateTime to) => $"{Date(from)} – {Date(to)}",
        };

        var document = new TableReportDocument(
            title: "Aktivnost korisnika",
            subtitle: $"Period: {period}. Recenzije, pregledi destinacija i planovi putovanja po korisniku (bez administratora).",
            generatedAt: Now(),
            columns: new[]
            {
                new ReportColumn("Korisnik", 3),
                new ReportColumn("Korisničko ime", 2.2f),
                new ReportColumn("Status", 1.5f),
                new ReportColumn("Recenzije", 1.4f, AlignRight: true),
                new ReportColumn("Od toga odobrene", 1.8f, AlignRight: true),
                new ReportColumn("Pregledi", 1.3f, AlignRight: true),
                new ReportColumn("Planovi", 1.2f, AlignRight: true),
            },
            rows: rows.Select(r => new[]
            {
                $"{r.FirstName} {r.LastName}".Trim(),
                r.Username,
                r.IsActive ? "Aktivan" : "Neaktivan",
                Number(r.ReviewCount),
                Number(r.ApprovedReviewCount),
                Number(r.ViewCount),
                Number(r.TripPlanCount),
            }).ToList(),
            summary: new[]
            {
                $"Korisnika: {rows.Count} (aktivnih u periodu: {rows.Count(r => r.ReviewCount + r.ViewCount + r.TripPlanCount > 0)})",
                $"Ukupno recenzija: {rows.Sum(r => r.ReviewCount)} · pregleda: {rows.Sum(r => r.ViewCount)} · planova: {rows.Sum(r => r.TripPlanCount)}",
            },
            emptyText: "Nema registrovanih korisnika.");

        return new ReportFile(document.GeneratePdf(), FileName("aktivnost-korisnika"));
    }

    public async Task<ReportFile> GetPopularDestinationsAsync(PopularDestinationsReportFilter filter)
    {
        var top = filter.Top ?? DefaultTopDestinations;
        if (top < 1 || top > MaxTopDestinations)
        {
            throw new ClientException($"Broj destinacija mora biti između 1 i {MaxTopDestinations}.");
        }

        var rows = await _dbContext.Destinations
            .AsNoTracking()
            .Select(d => new
            {
                d.Name,
                CategoryName = d.Category.Name,
                CityName = d.City.Name,
                ViewCount = d.ViewHistoryEntries.Count(),
                VisitorCount = d.ViewHistoryEntries.Select(v => v.UserId).Distinct().Count(),
                ApprovedReviewCount = d.Reviews.Count(r => r.Status == ReviewStatus.Approved),
                AverageRating = d.Reviews
                    .Where(r => r.Status == ReviewStatus.Approved)
                    .Average(r => (double?)r.Rating)
            })
            .OrderByDescending(d => d.ViewCount)
            .ThenByDescending(d => d.ApprovedReviewCount)
            .ThenByDescending(d => d.AverageRating)
            .ThenBy(d => d.Name)
            .Take(top)
            .ToListAsync();

        var document = new TableReportDocument(
            title: $"Najpopularnije destinacije (top {top})",
            subtitle: "Poredano po broju pregleda, zatim po broju odobrenih recenzija i prosječnoj ocjeni.",
            generatedAt: Now(),
            columns: new[]
            {
                new ReportColumn("#", 0.6f, AlignRight: true),
                new ReportColumn("Destinacija", 3.2f),
                new ReportColumn("Kategorija", 1.8f),
                new ReportColumn("Grad", 1.6f),
                new ReportColumn("Pregledi", 1.2f, AlignRight: true),
                new ReportColumn("Posjetioci", 1.3f, AlignRight: true),
                new ReportColumn("Recenzije", 1.3f, AlignRight: true),
                new ReportColumn("Ocjena", 1.1f, AlignRight: true),
            },
            rows: rows.Select((r, index) => new[]
            {
                $"{index + 1}.",
                r.Name,
                r.CategoryName,
                r.CityName,
                Number(r.ViewCount),
                Number(r.VisitorCount),
                Number(r.ApprovedReviewCount),
                Rating(r.AverageRating),
            }).ToList(),
            summary: new[]
            {
                "Pregledi = ukupan broj otvaranja destinacije; posjetioci = broj različitih korisnika koji su je otvorili.",
                "Recenzije i ocjena računaju se samo iz odobrenih recenzija.",
            },
            emptyText: "Još nema destinacija.");

        return new ReportFile(document.GeneratePdf(), FileName("najpopularnije-destinacije"));
    }

    private static string Number(int value) => value.ToString(Culture);

    private static string Rating(double? value) => value.HasValue ? value.Value.ToString("0.0", Culture) : "–";

    private static string Percent(int part, int total) =>
        total == 0 ? "–" : (part * 100.0 / total).ToString("0.0", Culture) + " %";

    private static string Date(DateTime value) => value.ToString("dd.MM.yyyy.", Culture);

    /// <summary>Null when the time zone database is missing; times are then shown as UTC.</summary>
    private static readonly TimeZoneInfo? ReportTimeZone = FindReportTimeZone();

    private static TimeZoneInfo? FindReportTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(ReportTimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return null;
        }
    }

    private static DateTime LocalNow() =>
        ReportTimeZone == null ? DateTime.UtcNow : TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ReportTimeZone);

    private static string Now() =>
        LocalNow().ToString("dd.MM.yyyy. HH:mm", Culture) + (ReportTimeZone == null ? " UTC" : string.Empty);

    private static string FileName(string reportName) =>
        $"travelbay-{reportName}-{LocalNow():yyyy-MM-dd}.pdf";
}
