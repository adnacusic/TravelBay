using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace TravelBay.Services.Reports;

public record ReportColumn(string Header, float RelativeWidth, bool AlignRight = false);

/// <summary>
/// Shared A4 layout of every TravelBay report: brand header with title, subtitle and
/// generation time, one data table (header row repeats on every page), optional summary
/// lines under it, and page numbers in the footer.
/// </summary>
public class TableReportDocument : IDocument
{
    private const string BrandColor = "#2F6F73";
    private const string HeaderBackground = "#E3EEEE";
    private const string StripeBackground = "#F6F8F8";
    private const string RowBackground = "#FFFFFF";
    private const string BorderColor = "#C9D6D6";

    private readonly string _title;
    private readonly string _subtitle;
    private readonly string _generatedAt;
    private readonly IReadOnlyList<ReportColumn> _columns;
    private readonly IReadOnlyList<string[]> _rows;
    private readonly IReadOnlyList<string> _summary;
    private readonly string _emptyText;

    public TableReportDocument(
        string title,
        string subtitle,
        string generatedAt,
        IReadOnlyList<ReportColumn> columns,
        IReadOnlyList<string[]> rows,
        IReadOnlyList<string> summary,
        string emptyText)
    {
        _title = title;
        _subtitle = subtitle;
        _generatedAt = generatedAt;
        _columns = columns;
        _rows = rows;
        _summary = summary;
        _emptyText = emptyText;
    }

    public DocumentMetadata GetMetadata() => new()
    {
        Title = _title,
        Author = "TravelBay",
        Creator = "TravelBay admin",
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(36);
            page.DefaultTextStyle(style => style.FontSize(10));

            page.Header().Element(ComposeHeader);
            page.Content().PaddingVertical(16).Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(brand =>
                {
                    brand.Item().Text("TravelBay").FontSize(20).Bold().FontColor(BrandColor);
                    brand.Item().Text("Administracija turističkih destinacija").FontSize(9).FontColor(Colors.Grey.Darken1);
                });
                row.ConstantItem(170).AlignRight().Text($"Generisano: {_generatedAt}").FontSize(9).FontColor(Colors.Grey.Darken1);
            });

            column.Item().PaddingTop(12).Text(_title).FontSize(15).Bold();
            column.Item().PaddingTop(2).Text(_subtitle).FontSize(9).FontColor(Colors.Grey.Darken2);
            column.Item().PaddingTop(8).LineHorizontal(1).LineColor(BrandColor);
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            if (_rows.Count == 0)
            {
                column.Item().PaddingVertical(24).AlignCenter().Text(_emptyText).Italic();
                return;
            }

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(definition =>
                {
                    foreach (var reportColumn in _columns)
                    {
                        definition.RelativeColumn(reportColumn.RelativeWidth);
                    }
                });

                table.Header(header =>
                {
                    foreach (var reportColumn in _columns)
                    {
                        var cell = header.Cell()
                            .Background(HeaderBackground)
                            .BorderBottom(1).BorderColor(BrandColor)
                            .PaddingVertical(5).PaddingHorizontal(4);
                        (reportColumn.AlignRight ? cell.AlignRight() : cell).Text(reportColumn.Header).Bold().FontSize(9);
                    }
                });

                for (var rowIndex = 0; rowIndex < _rows.Count; rowIndex++)
                {
                    var background = rowIndex % 2 == 1 ? StripeBackground : RowBackground;
                    for (var columnIndex = 0; columnIndex < _columns.Count; columnIndex++)
                    {
                        var cell = table.Cell()
                            .Background(background)
                            .BorderBottom(0.5f).BorderColor(BorderColor)
                            .PaddingVertical(4).PaddingHorizontal(4);
                        (_columns[columnIndex].AlignRight ? cell.AlignRight() : cell).Text(_rows[rowIndex][columnIndex]);
                    }
                }
            });

            if (_summary.Count > 0)
            {
                column.Item().PaddingTop(14).Column(summary =>
                {
                    foreach (var line in _summary)
                    {
                        summary.Item().Text(line).FontSize(9);
                    }
                });
            }
        });
    }

    private static void ComposeFooter(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Text("TravelBay · izvještaj").FontSize(8).FontColor(Colors.Grey.Darken1);
            row.RelativeItem().AlignRight().Text(text =>
            {
                text.DefaultTextStyle(style => style.FontSize(8).FontColor(Colors.Grey.Darken1));
                text.Span("Strana ");
                text.CurrentPageNumber();
                text.Span(" od ");
                text.TotalPages();
            });
        });
    }
}
