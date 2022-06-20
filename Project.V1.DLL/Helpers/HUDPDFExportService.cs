using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Drawing;
using System.IO;
using System.Data;

namespace Project.V1.DLL.Helpers;

public class HUDPDFExportService
{
    private static SiteHUDRequestModel Request { get; set; }

    public void SetData(SiteHUDRequestModel request)
    {
        Request = request;
    }

    //Export weather data to PDF document.
    public MemoryStream CreatePdf()
    {
        if (Request == null)
        {
            throw new ArgumentNullException("PDF Data cannot be null");
        }

        //Create a new PDF document
        using (PdfDocument pdfDocument = new())
        {
            //int paragraphAfterSpacing = 8;

            //Create a new font
            PdfStandardFont font = new(PdfFontFamily.TimesRoman, 18, PdfFontStyle.Bold);
            PdfStandardFont fontH2 = new(PdfFontFamily.TimesRoman, 16, PdfFontStyle.Bold);
            PdfStandardFont contentFont = new(PdfFontFamily.TimesRoman, 12);
            PdfStandardFont contentFontBold = new(PdfFontFamily.TimesRoman, 12, PdfFontStyle.Bold);
            //int cellMargin = 8;

            //Add page to the PDF document
            PdfPage page = pdfDocument.Pages.Add();

            PdfGraphics graphics = page.Graphics;

            Stream imageStream = File.OpenRead(Directory.GetCurrentDirectory() + "\\wwwroot\\images\\logo-doc.png");
            PdfBitmap image = new(imageStream);
            graphics.DrawImage(image, 0, 0);

            //Create a text element to draw a text in PDF page
            PdfTextElement title = new($"MTN Communications Nigeria Limited", font, PdfBrushes.Black);
            PdfLayoutResult result = title.Draw(page, new PointF(120, 13));

            PdfLine line = new(new PointF(0, 0), new PointF(page.GetClientSize().Width, 0))
            {
                Pen = PdfPens.DarkGray
            };

            PdfLine lineHalf = new(new PointF(0, 0), new PointF(page.GetClientSize().Width / 2, 0))
            {
                Pen = PdfPens.DarkGray
            };

            PdfLine lineThird = new(new PointF(0, 0), new PointF(page.GetClientSize().Width / 3, 0))
            {
                Pen = PdfPens.DarkGray
            };

            PdfLine lineQuater = new(new PointF(0, 0), new PointF(page.GetClientSize().Width / 4, 0))
            {
                Pen = PdfPens.DarkGray
            };

            PdfTextElement content = new($"Approval Report", fontH2, PdfBrushes.DarkBlue);
            PdfLayoutFormat format = new();
            format.Layout = PdfLayoutType.Paginate;

            result = line.Draw(page, new PointF(0, result.Bounds.Bottom + 25));

            //Draw a text to the PDF document
            result = content.Draw(page, new RectangleF(0, 70, page.GetClientSize().Width, page.GetClientSize().Height), format);

            (content, format, result) = GenerateContent(page, $"This is an approval to {Request.RequestAction} a request with ID: {Request.UniqueId}.", contentFont, PdfBrushes.Black, result);

            (content, format, result) = GenerateContent(page, $"", contentFont, PdfBrushes.Black, result);

            (content, format, result) = GenerateContent(page, $"Document ID: {Request.UniqueId}", contentFont, PdfBrushes.Black, result);
            (content, format, result) = GenerateContent(page, $"Request Type: {Request.RequestAction}", contentFont, PdfBrushes.Black, result);
            (content, format, result) = GenerateContent(page, $"Request Reason: {Request.Reason}", contentFont, PdfBrushes.Black, result);
            (content, format, result) = GenerateContent(page, $"Is Force Majeure: {((Request.IsForceMajeure) ? "Yes" : "No")}", contentFont, PdfBrushes.Black, result);
            (content, format, result) = GenerateContent(page, $"Date Created: {Request.DateCreated}", contentFont, PdfBrushes.Black, result);

            (content, format, result) = GenerateContent(page, $"", contentFont, PdfBrushes.Black, result);

            (content, format, result) = GenerateContent(page, $"Site ID: {Request.SiteIds}", contentFontBold, PdfBrushes.Black, result);

            (content, format, result) = GenerateContent(page, $"", contentFont, PdfBrushes.Black, result);

            if (Request.RequestAction != "UnHalt")
            {
                float colCount = (Request.ThirdApprover == null) ? 2F : 3F;
                float xPosition = result.Bounds.Bottom;

                (_, format, result) = GenerateHTMLApprover(page, Request.FirstApprover, contentFont, format, result, "First Approver", 0, result.Bounds.Bottom, page.GetClientSize().Width, page.GetClientSize().Height, 14);
                result = lineQuater.Draw(page, new PointF(0, result.Bounds.Bottom + 25));

                (_, format, result) = GenerateHTMLApprover(page, Request.SecondApprover, contentFont, format, result, "Second Approver", 0, result.Bounds.Bottom, page.GetClientSize().Width, page.GetClientSize().Height, 14);
                result = lineQuater.Draw(page, new PointF(0, result.Bounds.Bottom + 25));

                if (Request.ThirdApprover != null)
                {
                    (_, format, result) = GenerateHTMLApprover(page, Request.ThirdApprover, contentFont, format, result, "Third Approver", 0, result.Bounds.Bottom, page.GetClientSize().Width, page.GetClientSize().Height, 14);
                    result = lineQuater.Draw(page, new PointF(0, result.Bounds.Bottom + 25));
                }                
            }
            else
            {
                (content, format, result) = GenerateContent(page, $"Unhalt Request DOES NOT require approval", contentFontBold, PdfBrushes.Gold, result);

                (content, format, result) = GenerateContent(page, $"", contentFont, PdfBrushes.Black, result);
            }

            SetWaterMark(page, graphics, $"Motivation to {Request.RequestAction} {Request.UniqueId} - {Request.DateCreated}", PdfPens.Gold, PdfBrushes.Gold);

            //Create a PdfGrid
            //PdfGrid pdfGrid = new ();
            //pdfGrid.Style.CellPadding.Left = cellMargin;
            //pdfGrid.Style.CellPadding.Right = cellMargin;

            ////Applying built-in style to the PDF grid
            //pdfGrid.ApplyBuiltinStyle(PdfGridBuiltinStyle.GridTable4Accent1);

            ////Assign data source
            //pdfGrid.DataSource = Request;

            //pdfGrid.Style.Font = contentFont;

            //Draw PDF grid into the PDF page
            //pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(0, result.Bounds.Bottom + paragraphAfterSpacing));

            using (MemoryStream stream = new())
            {
                //Saving the PDF document into the stream
                pdfDocument.Save(stream);
                //Closing the PDF document
                pdfDocument.Close(true);
                return stream;

            }
        }
    }

    private string GenerateApproverHtmlString(RequestApproverModel approver, string title)
    {
        var date = approver.DateActioned.Date.ToShortDateString();

        var approverHtml = $"<font color='#003366'><b>{title} </b></font>";
        approverHtml += $"<br/><br/><b>Name: </b>{approver.Fullname}";
        approverHtml += $"<br/><b>Status: </b>{(approver.IsApproved ? "Approved" : "Disapproved")}";
        approverHtml += $"<br/><b>Comment: </b>{approver.ApproverComment}";
        approverHtml += $"<br/><b>Date Actioned: </b>{date}";

        return approverHtml;
    }

    private (PdfHTMLTextElement content, PdfLayoutFormat format, PdfLayoutResult result) GenerateHTMLApprover(PdfPage page, RequestApproverModel approver, PdfStandardFont font, PdfLayoutFormat format,
        PdfLayoutResult result, string title, float X = 0, float Y = 0, float Width = 0, float Height = 0, int paragraphAfterSpacing = 12)
    {
        string approverHtml = GenerateApproverHtmlString(approver, title);

        PdfHTMLTextElement richTextElement = new(approverHtml, font, PdfBrushes.Black);

        format.Layout = PdfLayoutType.Paginate;
        format.Break = PdfLayoutBreakType.FitPage;


        PdfLayoutResult resultNew = richTextElement.Draw(page, new RectangleF(X, Y + paragraphAfterSpacing, Width, Height), format);

        return (richTextElement, format, resultNew);
    }

    private static void SetWaterMark(PdfPage page, PdfGraphics graphics, string watermark, PdfPen pen = null, PdfBrush brush = null)
    {
        pen ??= PdfPens.Red;
        brush ??= PdfBrushes.Red;

        PdfStandardFont contentFont = new(PdfFontFamily.TimesRoman, 28, PdfFontStyle.Bold);
        //for(int i = 0; i < 8; i++)
        //{
        //    int positionX = 50;
        //    while (positionX < 1000)
        //    {
        //        int positionY = 50;
        //        while (positionY < 1000)
        //        {

        PdfGraphicsState state = graphics.Save();
        graphics.SetTransparency(0.25f);
        graphics.RotateTransform(50);
        //graphics.DrawString(watermark, contentFont, pen, brush, new PointF(positionX * i, positionX * i));
        graphics.DrawString(watermark, contentFont, pen, brush, new PointF(20, 0));
        //positionY += 100;
        //        }

        //        positionX += 50;
        //    }
        //}
    }

    private static (PdfTextElement content, PdfLayoutFormat format, PdfLayoutResult result) GenerateContent(PdfPage page,
        string contentString, PdfStandardFont contentFont, PdfBrush brush, PdfLayoutResult result, int paragraphAfterSpacing = 12)
    {
        PdfTextElement content = new(contentString, contentFont, brush);
        PdfLayoutFormat format = new();
        format.Layout = PdfLayoutType.Paginate;

        //Draw a text to the PDF document
        PdfLayoutResult resultNew = content.Draw(page, new RectangleF(0, result.Bounds.Bottom + paragraphAfterSpacing, page.GetClientSize().Width, page.GetClientSize().Height), format);

        return (content, format, resultNew);
    }
}
