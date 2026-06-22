using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TransportSystem.Core.Application.Common.Interfaces;
using TransportSystem.Core.Application.Dtos.Reports;

namespace TransportSystem.Infrastructure.Shared.Services
{
    public class ReportExportService : IReportExportService
    {
        public byte[] GenerateExcel(ReportsSummaryDto report)
        {
            using var workbook = new XLWorkbook();

            var kpiSheet = workbook.Worksheets.Add("KPIs");
            kpiSheet.Cell(1, 1).Value = "Indicador";
            kpiSheet.Cell(1, 2).Value = "Valor";
            kpiSheet.Row(1).Style.Font.Bold = true;

            var kpiRows = new (string Label, string Value)[]
            {
                ("Período", $"{report.Month:D2}/{report.Year}"),
                ("Viajes completados", report.TripsCompleted.ToString()),
                ("Tasa de cumplimiento (%)", report.CompletionRatePercent.ToString("N1")),
                ("Cancelados", report.CancelledCount.ToString()),
                ("Costo de combustible", report.FuelCost.ToString("N2")),
                ("Galones de combustible", report.FuelGallons.ToString("N2")),
                ("Costo de mantenimiento", report.MaintenanceCost.ToString("N2")),
                ("Servicios de mantenimiento", report.MaintenanceServicesCount.ToString()),
            };

            for (var i = 0; i < kpiRows.Length; i++)
            {
                kpiSheet.Cell(i + 2, 1).Value = kpiRows[i].Label;
                kpiSheet.Cell(i + 2, 2).Value = kpiRows[i].Value;
            }
            kpiSheet.Columns().AdjustToContents();

            AddSheet(workbook, "Viajes por mes", ["Mes", "Viajes completados"],
                report.TripsByMonth.Select(m => new[] { $"{m.MonthLabel} {m.Year}", m.Count.ToString() }));

            AddSheet(workbook, "Solicitudes por área", ["Área", "Solicitudes"],
                report.RequestsByArea.Select(a => new[] { a.Area, a.Count.ToString() }));

            AddSheet(workbook, "Conductores", ["Conductor", "Completados", "Cancelados"],
                report.TopDrivers.Select(d => new[] { d.DriverName, d.TripsCompleted.ToString(), d.TripsCancelled.ToString() }));

            AddSheet(workbook, "Vehículos", ["Vehículo", "Placa", "Viajes completados"],
                report.TopVehicles.Select(v => new[] { v.VehicleLabel, v.LicensePlate, v.TripsCompleted.ToString() }));

            AddSheet(workbook, "Combustible por mes", ["Mes", "Galones", "Costo"],
                report.FuelByMonth.Select(f => new[] { $"{f.MonthLabel} {f.Year}", f.Gallons.ToString("N2"), f.Cost.ToString("N2") }));

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static void AddSheet(XLWorkbook workbook, string sheetName, string[] headers, IEnumerable<string[]> rows)
        {
            var sheet = workbook.Worksheets.Add(sheetName);

            for (var c = 0; c < headers.Length; c++)
                sheet.Cell(1, c + 1).Value = headers[c];
            sheet.Row(1).Style.Font.Bold = true;

            var r = 2;
            foreach (var row in rows)
            {
                for (var c = 0; c < row.Length; c++)
                    sheet.Cell(r, c + 1).Value = row[c];
                r++;
            }

            sheet.Columns().AdjustToContents();
        }

        public byte[] GeneratePdf(ReportsSummaryDto report)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header()
                        .Text($"Reporte de transporte — {report.Month:D2}/{report.Year}")
                        .FontSize(16).Bold();

                    page.Content().Column(col =>
                    {
                        col.Spacing(12);
                        col.Item().PaddingTop(8);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Viajes completados: {report.TripsCompleted}");
                            row.RelativeItem().Text($"Tasa de cumplimiento: {report.CompletionRatePercent:N1}%");
                            row.RelativeItem().Text($"Cancelados: {report.CancelledCount}");
                        });

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Costo combustible: {report.FuelCost:N2} ({report.FuelGallons:N2} gal)");
                            row.RelativeItem().Text($"Costo mantenimiento: {report.MaintenanceCost:N2} ({report.MaintenanceServicesCount} servicios)");
                        });

                        AddTableSection(col, "Viajes por mes", ["Mes", "Viajes completados"],
                            report.TripsByMonth.Select(m => new[] { $"{m.MonthLabel} {m.Year}", m.Count.ToString() }));

                        AddTableSection(col, "Solicitudes por área", ["Área", "Solicitudes"],
                            report.RequestsByArea.Select(a => new[] { a.Area, a.Count.ToString() }));

                        AddTableSection(col, "Conductores con más viajes", ["Conductor", "Completados", "Cancelados"],
                            report.TopDrivers.Select(d => new[] { d.DriverName, d.TripsCompleted.ToString(), d.TripsCancelled.ToString() }));

                        AddTableSection(col, "Vehículos más utilizados", ["Vehículo", "Placa", "Viajes"],
                            report.TopVehicles.Select(v => new[] { v.VehicleLabel, v.LicensePlate, v.TripsCompleted.ToString() }));

                        AddTableSection(col, "Consumo de combustible por mes", ["Mes", "Galones", "Costo"],
                            report.FuelByMonth.Select(f => new[] { $"{f.MonthLabel} {f.Year}", f.Gallons.ToString("N2"), f.Cost.ToString("N2") }));
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static void AddTableSection(ColumnDescriptor column, string title, string[] headers, IEnumerable<string[]> rows)
        {
            column.Item().Text(title).Bold().FontSize(11);

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    foreach (var _ in headers)
                        columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    foreach (var h in headers)
                        header.Cell().Padding(3).Text(h).Bold();
                });

                foreach (var row in rows)
                {
                    foreach (var cell in row)
                        table.Cell().Padding(3).Text(cell);
                }
            });
        }
    }
}
