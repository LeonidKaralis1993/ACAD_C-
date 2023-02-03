using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.IO;
using ACadSharp.Tables;
using PdfSharp;
using PdfSharp.Charting;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Content.Objects;

//test change
//this is dev branch test

namespace Acd
{
    internal class Program
    {
        const string input_filename = "..\\..\\sample_files\\1.dwg";
        const string output_filename = "..\\..\\sample_files\\1.pdf";
        const double pi = 3.14159265358979;

        static void Main(string[] args)
        {
            CadDocument doc;
            using (DwgReader reader = new DwgReader(input_filename))
            {
                doc = reader.Read();
            }
            /*using (DxfReader reader=new DxfReader(input_filename))
            {
                doc = reader.Read();
            }*/

            PdfDocument document = new PdfDocument();

            // Create a font
            XFont font = new XFont("YuGothic", 25, XFontStyle.Bold);
            //Pagesize=A4
            PageSize[] pageSizes = (PageSize[])Enum.GetValues(typeof(PageSize));
            PdfPage page = document.AddPage();
            //page.Size = pageSizes[5];//******A4 case
            page.Size = pageSizes[1];
            
            var vh = page.Height;       

            XGraphics gfx = XGraphics.FromPdfPage(page);
            //gfx.DrawString(page.Size.ToString(), font, XBrushes.DarkRed, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);
                      
            XColor mcolor = new XColor();
            XPen pdfpen = new XPen(mcolor, 0.1);
            XBrush xBrush = new XSolidBrush(mcolor);
            


            Line kline = new Line();
            Arc arc = new Arc();
            MText text = new MText();
            XRect xRect= new XRect();
            Circle circle = new Circle();
            Hatch hatch = new Hatch();
            Insert insert= new Insert();    

            LwPolyline polyline = new LwPolyline();
            
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
                            Console.WriteLine($"\n\t--type---{e.ObjectType}: {e.ToString()}");
                            if (e.ObjectType.ToString() == "CIRCLE")
                            {
                                Console.Write("\n\t\t--DrawCircle--");
                                circle = (Circle)e;
                                xRect.X = circle.Center.X - circle.Radius+200;
                                xRect.Y = vh-circle.Center.Y - circle.Radius-200;
                                xRect.Width = circle.Radius *2;
                                xRect.Height = circle.Radius*2;
                                Console.WriteLine("--\t--"+xRect.ToString());
                                gfx.DrawEllipse(pdfpen, xRect);                                                 
                            }
                            if (e.ObjectType.ToString() == "LINE")
                            {
                                Console.Write("\n\t\t--LineDraw--");
                                kline = (Line)e;
                                double ksx = kline.StartPoint.X+200;
                                double ksy = vh-kline.StartPoint.Y-200;
                                double kex = kline.EndPoint.X+200;
                                double key = vh-kline.EndPoint.Y-200;

                                Console.WriteLine("\n\tStartPointX: " + ksx + "\n\tStartPointY: " + ksy + "\n\tEndpiontX: " + kex + "\n\tEndPointY: " + key);
                                gfx.DrawLine(pdfpen, ksx, ksy, kex, key);                    
                            }
                            if (e.ObjectType.ToString() == "ARC")
                            {
                                Console.Write("\n\t\t--DrawArc--");
                                double stA,edA;
                                double goA;
                                arc = (Arc)e;
                                xRect.X = arc.Center.X - arc.Radius + 200;
                                xRect.Y = vh - arc.Center.Y - arc.Radius - 200;
                                xRect.Width = arc.Radius * 2;
                                xRect.Height = arc.Radius * 2;

                                if (arc.EndAngle < arc.StartAngle)
                                {
                                    stA = 360 - arc.StartAngle / pi * 180;
                                    gfx.DrawArc(pdfpen, xRect, 0, stA);
                                    edA = 360 - arc.EndAngle / pi * 180;
                                    gfx.DrawArc(pdfpen, xRect, edA, 359.999999 - edA);
                                    
                                }
                                else {
                                    stA = 360 - arc.EndAngle / pi * 180;
                                    goA = 360 - arc.StartAngle / pi * 180 - stA;
                                    gfx.DrawArc(pdfpen, xRect, stA, goA);          
                               }     
                                
                            }
                            /*if (e.ObjectType.ToString() == "LWPOLYLINE")
                            {
                                Console.Write("\n\t\t--DrawLWP--");
                                
                                polyline = (LwPolyline)e;
                                Console.WriteLine(polyline.Vertices.Count);
                                foreach (LwPolyline.Vertex vet in polyline.Vertices)
                                {
                                    //vet.Bulge.
                                    //Console.WriteLine($"\n\t----vet-----+{vet.ToString()}");
                                }
                                //polyline.Vertices.ToArray();
                                //XPoint[] points = new XPoint[polyline.Vertices.Count];
                                //int i = 0;
                                //foreach (XPoint pt in points)
                                //{
                                //}
                                //points =polyline.Vertices.ToArray();
                                //gfx.DrawLine(pdfpen, kline.StartPoint.X, kline.StartPoint.Y, kline.EndPoint.X, kline.EndPoint.Y);
                                //gfx.drawpoly
                            }*/
                            if (e.ObjectType.ToString() == "HATCH")
                            {
                                Console.Write("\n\t\t--HatchDraw--");
                                hatch = (Hatch)e;

                                foreach(Hatch.BoundaryPath mpath in hatch.Paths)
                                {
                                    foreach(var pe in mpath.Edges)
                                    {
                                        Console.WriteLine("\n\t\t\t"+pe.Type);
                                        if (pe.Type.ToString() == "Polyline")
                                        {
                                            Hatch.BoundaryPath.Polyline polyline1;
                                            
                                            polyline1 = (Hatch.BoundaryPath.Polyline)pe;
                                            //polyline1.Vertices
                                            int count = polyline1.Vertices.Count;
                                            Console.WriteLine("\n" + count);
                                            int index = 0;

                                            XPoint[] xpoints = new XPoint[count];
                                            foreach (CSMath.XY point in polyline1.Vertices)
                                            {
                                                xpoints[index].X = 200 + point.X;
                                                xpoints[index].Y = vh - point.Y - 200;
                                                Console.WriteLine("\n\t" + point.X + "\t\t" + point.Y);
                                                index++;
                                            }
                                            gfx.DrawPolygon(pdfpen, xBrush, xpoints,XFillMode.Winding);
                                            //gfx.DrawClosedCurve(xBrush, xpoints);                                            
                                        }                                       
                                    }                                   
                                }
                               
                            }
                            /*if (e.ObjectType.ToString() == "INSERT")
                            {
                                Console.Write("\n--HatchDraw--");
                                insert = (Insert)e;
                                //xRect.X = hatch.SeedPoints.Count
                                //xRect.Y = vh - circle.Center.Y - circle.Radius;
                                //.Width = circle.Radius * 2;
                                //xRect.Height = circle.Radius * 2;
                                Console.WriteLine("--\t--" + insert.InsertPoint.ToString());
                                //gfx.DrawEllipse(pdfpen, xRect);
                            }*/
                            /*if (e.ObjectType.ToString() == "MTEXT")
                            {
                                Console.Write("\n\t\t------TEXTinput--------");
                                text = (MText)e;
                                MText text_conternt = new MText();
                                text_conternt.AdditionalText = text.AdditionalText;

                                string ctk = text.Value;
                                XPoint point = new XPoint(text.InsertPoint.X, vh - text.InsertPoint.Y);
                                XSize size = new XSize(text.RectangleWitdth + 10, text.RectangleHeight + 10);

                                gfx.DrawString(ctk, font, XBrushes.Black, new XRect(point, size), XStringFormats.Center);
                            }*/
                        }
                    }
                        
                }
            }

            // Save the document...

            document.Save(output_filename);

            // ...and start a viewer.
            //   Process.Start(filename);
            Console.WriteLine("\n\n\t!!!!!end!!!!!");
            Console.ReadKey(true);

        }

    }
}
