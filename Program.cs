using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.IO;
using ACadSharp.Tables;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Acd
{
    internal class Program
    {
        const string file = "E:\\temp1\\1.dwg";


        static void Main(string[] args)
        {
            CadDocument doc;
            using (DwgReader reader = new DwgReader(file))
            {
                doc = reader.Read();
            }

            PdfDocument document = new PdfDocument();

            // Create a font
            XFont font = new XFont("YuGothic", 25, XFontStyle.Bold);
            //Pagesize=A4
            PageSize[] pageSizes = (PageSize[])Enum.GetValues(typeof(PageSize));
            PdfPage page = document.AddPage();
            //page.Size = pageSizes[5];
            page.Size = pageSizes[1];
            var vh = page.Height;
            //Console.WriteLine($"Page size: {pageSize}");

            XGraphics gfx = XGraphics.FromPdfPage(page);
            //gfx.DrawString(pageSize.ToString(), font, XBrushes.DarkRed, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);
            
            
            XColor mcolor = new XColor();
            XPen pdfpen = new XPen(mcolor, 1);
            //gfx.DrawLine(pdfpen, 0, 0, 100, 100); gfx.DrawLine(pdfpen, 50, 0, 500, 100);
            //gfx.DrawLine(pdfpen, 100,200, 700, 0);

            Line kline = new Line();
            Arc arc = new Arc();
            MText text = new MText();
            XRect xRect= new XRect();
            Circle circle = new Circle();
            Hatch hatch = new Hatch();

            LwPolyline polyline = new LwPolyline();
            //XPoint[] points= new XPoint[5];


            //Console.WriteLine($"---A----{doc.BlockRecords.ObjectName}");
            foreach (var item in doc.BlockRecords)
            {
                //Console.WriteLine($"--------B-------\tName: {item.Name}");

                if (item.Name == BlockRecord.ModelSpaceName && item is BlockRecord model)
                {
                    //Console.WriteLine($"------C-------\t\tEntities in the model:");
                    foreach (var element in model.Entities.GroupBy(i => i.GetType().FullName))
                    {
                        Console.WriteLine($"\t\t{element.Key}: {element.Count()}");
                        foreach (var e in element)
                        {
                            Console.WriteLine($"--type---{e.ObjectType}: {e.ToString()}");
                            if (e.ObjectType.ToString() == "CIRCLE")
                            {
                                Console.Write("\n--LineDraw--");
                                circle = (Circle)e;
                                xRect.X = circle.Center.X + circle.Radius;
                                xRect.Y = vh-circle.Center.Y - circle.Radius;
                                xRect.Width = circle.Radius *2;
                                xRect.Height = circle.Radius*2;
                                Console.WriteLine("--\t--"+xRect.ToString());
                                gfx.DrawEllipse(pdfpen, xRect);                                                 
                            }
                            if (e.ObjectType.ToString() == "LINE")
                            {
                                Console.Write("\n--LineDraw--");
                                kline = (Line)e;
                                gfx.DrawLine(pdfpen, kline.StartPoint.X, vh-kline.StartPoint.Y, kline.EndPoint.X, vh-kline.EndPoint.Y);                    
                            }
                            if (e.ObjectType.ToString() == "ARC")
                            {
                                Console.Write("\n--ARCDraw--");
                                arc=(Arc)e;
                                xRect.X = arc.Center.X + arc.Radius ;
                                xRect.Y=vh-arc.Center.Y - arc.Radius ;
                                xRect.Width = arc.Radius*2;
                                xRect.Height = arc.Radius*2;
                                //arc.
                                Console.WriteLine(arc.Radius + " " + arc.Center);
                                //Console.WriteLine($"{xRect.X}-----{xRect.Y}-------{xRect.Width}---------{xRect.Height}");
                                gfx.DrawArc(pdfpen, xRect, arc.StartAngle, arc.EndAngle);
                            }
                            if (e.ObjectType.ToString() == "LWPOLYLINE")
                            {
                                Console.Write("\n--LWP--");
                                //kline = (Line)e;
                                polyline = (LwPolyline)e;
                                Console.WriteLine(polyline.Vertices.Count);
                                foreach(LwPolyline.Vertex vet in polyline.Vertices)
                                {
                                    //vet.Bulge.
                                    //Console.WriteLine($"\n\t----vet-----+{vet.ToString()}");
                                }
                                //Console.WriteLine(polyline.Vertices.

                                //polyline.Vertices.ToArray();
                                //XPoint[] points = new XPoint[polyline.Vertices.Count];
                                //int i = 0;
                                //foreach (XPoint pt in points)
                                //{
                                   

                                //}
                                
                                //points =polyline.Vertices.ToArray();
                                //gfx.DrawLine(pdfpen, kline.StartPoint.X, kline.StartPoint.Y, kline.EndPoint.X, kline.EndPoint.Y);
                                //gfx.drawpoly
                            }
                            if (e.ObjectType.ToString() == "HATCH")
                            {
                                Console.Write("\n--HatchDraw--");
                                hatch = (Hatch)e;
                                //xRect.X = hatch.SeedPoints.Count
                                //xRect.Y = vh - circle.Center.Y - circle.Radius;
                                //.Width = circle.Radius * 2;
                                //xRect.Height = circle.Radius * 2;
                                Console.WriteLine("--\t--" + hatch.SeedPoints.Count);
                                //gfx.DrawEllipse(pdfpen, xRect);
                            }
                            if (e.ObjectType.ToString() == "HATCH")
                            {
                                Console.Write("\n--HatchDraw--");
                                hatch = (Hatch)e;
                                //xRect.X = hatch.SeedPoints.Count
                                //xRect.Y = vh - circle.Center.Y - circle.Radius;
                                //.Width = circle.Radius * 2;
                                //xRect.Height = circle.Radius * 2;
                                Console.WriteLine("--\t--" + hatch.SeedPoints.Count);
                                //gfx.DrawEllipse(pdfpen, xRect);
                            }
                            if (e.ObjectType.ToString() == "MTEXT")
                             {
                                 Console.Write("\n------TEXTinput--------");
                                 text=(MText)e;
                                 MText text_conternt= new MText();
                                 text_conternt.AdditionalText=text.AdditionalText;


                                 //text.MatchProperties(e);
                                 string ctk=text.Value;
                                 XPoint point= new XPoint(text.InsertPoint.X, vh-text.InsertPoint.Y);
                                 XSize size= new XSize(text.RectangleWitdth+10, text.RectangleHeight+10);

                                 gfx.DrawString(ctk, font, XBrushes.Black, new XRect(point,size), XStringFormats.Center);
                                /*Console.WriteLine("\n\n**********");
                                 Console.WriteLine(ctk);
                                 //Console.WriteLine("\n");
                                 Console.WriteLine(point.ToString());
                                 //Console.WriteLine("\n");
                                 Console.WriteLine(size.ToString());
                                 Console.WriteLine("\n**********\n");*/

                             }


                            //gfx.DrawLine(e.line)
                            //Console.WriteLine(e.Document.);
                        }

                        //Console.WriteLine($"\t\t{e.GetType().ToString()}");
                    }
                }
            }

            //if (pageSize == PageSize.Undefined)  //exit;
            // One page in Portrait...





            // Save the document...

            const string filename = "E:\\temp\\result.pdf";

            document.Save(filename);

            // ...and start a viewer.
            //   Process.Start(filename);
            Console.WriteLine("\n\n\t!!!!!end!!!!!");
            Console.ReadKey(true);

        }

        /// <summary>
        /// Logs in the console the document information
        /// </summary>
        /// <param name="doc"></param>
        /*static void ExploreDocument(CadDocument doc)
        {
            *//*            Console.WriteLine();
                        Console.WriteLine("SUMMARY INFO:");
                        Console.WriteLine($"\tTitle: {doc.SummaryInfo.Title}");
                        Console.WriteLine($"\tSubject: {doc.SummaryInfo.Subject}");
                        Console.WriteLine($"\tAuthor: {doc.SummaryInfo.Author}");
                        Console.WriteLine($"\tKeywords: {doc.SummaryInfo.Keywords}");
                        Console.WriteLine($"\tComments: {doc.SummaryInfo.Comments}");
                        Console.WriteLine($"\tLastSavedBy: {doc.SummaryInfo.LastSavedBy}");
                        Console.WriteLine($"\tRevisionNumber: {doc.SummaryInfo.RevisionNumber}");
                        Console.WriteLine($"\tHyperlinkBase: {doc.SummaryInfo.HyperlinkBase}");
                        Console.WriteLine($"\tCreatedDate: {doc.SummaryInfo.CreatedDate}");
                        Console.WriteLine($"\tModifiedDate: {doc.SummaryInfo.ModifiedDate}");*/

        /*            ExploreTable(doc.AppIds);*//*
        //ExploreTable(doc.BlockRecords);
        *//*         ExploreTable(doc.DimensionStyles);
                   ExploreTable(doc.Layers);
                   ExploreTable(doc.LineTypes);
                   ExploreTable(doc.TextStyles);
                   ExploreTable(doc.UCSs);
                   ExploreTable(doc.Views);
                   ExploreTable(doc.VPorts);*//*
        //doc.BlockRecords.Count

        *//*            foreach (var e in doc.BlockRecords.GroupBy(i => i.GetType().FullName))
                    {
                        Console.WriteLine($"\t\t{e.Key}: {e.Count()}");
                    }*//*


    }*/

        /*static void ExploreTable<T>(Table<T> table)
            where T : TableEntry
        {
            *//*Console.WriteLine($"------------{table.ObjectName}");
            foreach (var item in table)
            {
                Console.WriteLine($"\tName: {item.Name}");

                if (item.Name == BlockRecord.ModelSpaceName && item is BlockRecord model)
                {
                    Console.WriteLine($"\t\tEntities in the model:");
                    foreach (var e in model.Entities.GroupBy(i => i.GetType().FullName))
                    {
                        Console.WriteLine($"\t\t{e.Key}: {e.Count()}");
                    }
                }
            }*//*
        }*/
    }
}
