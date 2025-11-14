using System.Collections.Generic;
using System.Linq;
using MusicEventManagementSystem.Models;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MusicEventManagementSystem.Services
{
    public class PdfTicketDocument : IDocument
    {
        private readonly EventRegistration _registration;
        private readonly List<RegistrationData> _dynamicData;

        public PdfTicketDocument(EventRegistration registration, List<RegistrationData> dynamicData)
        {
            _registration = registration;
            _dynamicData = dynamicData;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Size(PageSizes.A6); // Kích thước vé
                    page.Margin(30, Unit.Point);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text(_registration.MusicEvent.EventName)
                        .SemiBold().FontSize(16).FontColor(Colors.Grey.Darken2);

                    col.Item().Text("TICKET CONFIRMED")
                        .Bold().FontSize(20).FontColor(Colors.Green.Darken2);
                });

                row.ConstantItem(100)
                    .AlignRight()
    .Text(text =>
    {
        text.DefaultTextStyle(x => x.Bold().FontSize(14));

        // SỬA Ở ĐÂY:
        text.AlignRight(); // Căn lề cho toàn bộ khối text này
        text.Line($"REG ID\n#{_registration.RegistrationID}"); // Thêm nội dung
    });
            });
           
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20, Unit.Point).Column(col =>
            {
                col.Item().Element(ComposeTicketDetails);

                if (_dynamicData.Any())
                {
                    col.Item().PaddingTop(15, Unit.Point).Element(ComposeDynamicData);
                }

                // Bạn có thể thêm mã QR ở đây nếu muốn
                // col.Item().AlignCenter().Width(150, Unit.Point).Height(150, Unit.Point)
                //     .Barcode(BarcodeType.QRCode, _registration.ConfirmationCode);
            });
        }

        void ComposeTicketDetails(IContainer container)
        {
            container.Border(1, Unit.Point).BorderColor(Colors.Grey.Lighten1).Padding(10, Unit.Point)
                .Column(col =>
                {
                    col.Spacing(5, Unit.Point);
                    col.Item().Row(row =>
                    {
                        row.RelativeItem(1).Text("Attendee Email:").SemiBold();
                        row.RelativeItem(2).Text(_registration.ApplicationUser.Email);
                    });
                    col.Item().Row(row =>
                    {
                        row.RelativeItem(1).Text("Ticket Type:").SemiBold();
                        row.RelativeItem(2).Text(_registration.PricingTier.TierName);
                    });
                    col.Item().Row(row =>
                    {
                        row.RelativeItem(1).Text("Total Price:").SemiBold();
                        row.RelativeItem(2).Text($"{_registration.TotalPrice:N0} VND");
                    });
                });
        }

        void ComposeDynamicData(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().PaddingBottom(5, Unit.Point).Text("Information Provided:").Bold().FontSize(12);
                col.Item().Border(1, Unit.Point).BorderColor(Colors.Grey.Lighten1).Padding(10, Unit.Point)
                    .Column(innerCol =>
                    {
                        innerCol.Spacing(5, Unit.Point);
                        foreach (var data in _dynamicData)
                        {
                            innerCol.Item().Row(row =>
                            {
                                row.RelativeItem(1).Text($"{data.RequiredField.FieldName}:").SemiBold();
                                string value = data.RequiredField.FieldType == "Checkbox"
                                    ? (data.FieldValue == "true" ? "Agreed" : "No")
                                    : data.FieldValue;
                                row.RelativeItem(2).Text(value);
                            });
                        }
                    });
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(text =>
            {
                text.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Medium));
                text.Span("Thank you for registering! See you at ");
                text.Span(_registration.MusicEvent.EventName).SemiBold();
                text.Span(".");
            });
        }
    }
}