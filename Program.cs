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
            const string input_filename = "..\\..\\sample_files\\2.dwg";
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
                XFont font = new XFont("YuGothic", 12, XFontStyle.Bold);
                //Pagesize=A4
                PageSize[] pageSizes = (PageSize[])Enum.GetValues(typeof(PageSize));
                PdfPage page = document.AddPage();
                //page.Size = pageSizes[5];//******A4 case
                page.Size = pageSizes[1];
                page.MediaBox =new PdfRectangle(new XPoint(150, -150), new XPoint(2270, 2270));
                double v_d = 400;
            
                var vh = page.Height;       

                XGraphics gfx = XGraphics.FromPdfPage(page);
                
                      
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
                DimensionLinear dimensionLinear = new DimensionLinear();
            
                foreach (var item in doc.BlockRecords)
                {
                    //Console.WriteLine($"--------A-------\tName: {item.Name}");
                    if (item.Name == BlockRecord.ModelSpaceName && item is BlockRecord model)
                    {
                        //Console.WriteLine($"------B-------\t\tEntities in the model:");
                        foreach (var element in model.Entities.GroupBy(i => i.GetType().FullName))
                        {
                            //Console.WriteLine($"\t\t{element.Key}: {element.Count()}");
                            foreach (var e in element)
                            {
                                Console.WriteLine($"\n\t--Type---{e.ObjectType}: {e.ToString()}");
                                if (e.ObjectType.ToString() == "CIRCLE")
                                {
                                    //Console.Write("\n\t\t--DrawCircle--");
                                    circle = (Circle)e;
                                    xRect.X = circle.Center.X - circle.Radius+ v_d;
                                    xRect.Y = vh-circle.Center.Y - circle.Radius- v_d;
                                    xRect.Width = circle.Radius *2;
                                    xRect.Height = circle.Radius*2;
                                    Console.WriteLine("--\t--"+xRect.ToString());
                                    gfx.DrawEllipse(pdfpen, xRect);                                                 
                                }
                                if (e.ObjectType.ToString() == "LINE")
                                {
                                    //Console.Write("\n\t\t--LineDraw--");
                                    kline = (Line)e;
                                    double ksx = kline.StartPoint.X+ v_d;
                                    double ksy = vh-kline.StartPoint.Y- v_d;
                                    double kex = kline.EndPoint.X+ v_d;
                                    double key = vh-kline.EndPoint.Y- v_d;

                                    //Console.WriteLine("\n\tStartPointX: " + ksx + "\n\tStartPointY: " + ksy + "\n\tEndpiontX: " + kex + "\n\tEndPointY: " + key);
                                    gfx.DrawLine(pdfpen, ksx, ksy, kex, key);                    
                                }
                                if (e.ObjectType.ToString() == "DIMENSION_LINEAR")
                                {
                                    //Console.Write("\n\t\t--LineDraw--");
                                    dimensionLinear = (DimensionLinear)e;
                                    Console.WriteLine("\n\t"+dimensionLinear.DimensionType.ToString());
                                    if (dimensionLinear.DimensionType.ToString() == "Linear")
                                    {

                                        Console.WriteLine("\n\tDifinitionPoint:\t" + dimensionLinear.DefinitionPoint);
                                        //Console.WriteLine("\n\t" + dimensionLinear.DefinitionPoint.Y);

                                        Console.WriteLine("\n\tFirstPoint:\t" + dimensionLinear.FirstPoint);
                                        Console.WriteLine("\n\tSecondPoint:\t" + dimensionLinear.SecondPoint);

                                        Console.WriteLine("\n\tTextMiddlePoint:\t" + dimensionLinear.TextMiddlePoint);
                                        //Console.WriteLine("\n\t" + dimensionLinear.TextMiddlePoint.Y);
                                        //Console.WriteLine("\n\t" + dimensionLinear.DefinitionPoint.X);

                                        //Console.WriteLine("\n\t" + dimensionLinear.TextRotation);
                                        Console.WriteLine("\n\t" + dimensionLinear.AttachmentPoint);
                                        double diff,diffx,diffy;
                                        double ksx, ksy, kex, key;
                                    
                                        diffx = dimensionLinear.FirstPoint.X- dimensionLinear.SecondPoint.X;
                                        diffy = dimensionLinear.FirstPoint.Y- dimensionLinear.SecondPoint.Y;
                                    //diff = diffx > diffy ? diffx:diffy;
                                        if (Math.Abs(diffx) < Math.Abs(diffy))
                                        {
                                            diff = dimensionLinear.DefinitionPoint.X - dimensionLinear.FirstPoint.X;
                                            ksx = dimensionLinear.FirstPoint.X + v_d +diff;
                                            ksy = vh - dimensionLinear.FirstPoint.Y-v_d;
                                            kex = dimensionLinear.SecondPoint.X + v_d+diff;
                                            key = vh - dimensionLinear.SecondPoint.Y-v_d;
                                            
                                            gfx.DrawLine(pdfpen, dimensionLinear.FirstPoint.X + v_d, vh - dimensionLinear.FirstPoint.Y - v_d, ksx, ksy);
                                        
                                            gfx.DrawLine(pdfpen, dimensionLinear.SecondPoint.X+v_d, vh-dimensionLinear.SecondPoint.Y-v_d, kex, key);
                                            gfx.DrawLine(pdfpen, ksx, ksy, ksx-3, ksy+5);
                                            gfx.DrawLine(pdfpen, ksx, ksy, ksx+3, ksy+5);
                                            gfx.DrawLine(pdfpen, kex, key, kex+3, key-5);
                                            gfx.DrawLine(pdfpen, kex, key, kex-3, key-5);
                                            gfx.DrawLine(pdfpen, ksx, ksy, kex, key);
                                        Console.WriteLine("\n\t\t\t\t\t" + diff);
                                        }
                                    else
                                    {
                                        diff = dimensionLinear.DefinitionPoint.X - dimensionLinear.FirstPoint.X;
                                        ksx = dimensionLinear.FirstPoint.X + v_d ;
                                        ksy = vh - dimensionLinear.FirstPoint.Y-v_d-diff;
                                        kex = dimensionLinear.SecondPoint.X + v_d;
                                        key = vh - dimensionLinear.SecondPoint.Y-v_d-diff;

                                        gfx.DrawLine(pdfpen, dimensionLinear.FirstPoint.X + v_d, vh - dimensionLinear.FirstPoint.Y - v_d, ksx, ksy);

                                        gfx.DrawLine(pdfpen, dimensionLinear.SecondPoint.X+v_d, vh-dimensionLinear.SecondPoint.Y-v_d, kex, key);
                                        gfx.DrawLine(pdfpen, ksx, ksy, ksx+5, ksy-3);
                                        gfx.DrawLine(pdfpen, ksx, ksy, ksx+5, ksy+3);
                                        gfx.DrawLine(pdfpen, kex, key, kex-5, key+3);
                                        gfx.DrawLine(pdfpen, kex, key, kex-5, key-3);
                                        gfx.DrawLine(pdfpen, ksx, ksy, kex, key);

                                        
                                    //Console.WriteLine("\n\t\t\t\t\t" + diff);
                                    }





                                }


                                Console.WriteLine("\ndimentionTEXT---\n" + dimensionLinear.Text);

                                    //Console.WriteLine("\n\tStartPointX: " + ksx + "\n\tStartPointY: " + ksy + "\n\tEndpiontX: " + kex + "\n\tEndPointY: " + key);
                                    
                                }
                                if (e.ObjectType.ToString() == "ARC")
                                {
                                    //Console.Write("\n\t\t--DrawArc--");
                                    double stA,edA;
                                    double goA;
                                    arc = (Arc)e;
                                    xRect.X = arc.Center.X - arc.Radius + v_d;
                                    xRect.Y = vh - arc.Center.Y - arc.Radius - v_d;
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
                                if (e.ObjectType.ToString() == "HATCH")
                                {
                                    Console.Write("\n\t\t--HatchDraw--");
                                    hatch = (Hatch)e;

                                    foreach(Hatch.BoundaryPath mpath in hatch.Paths)
                                    {
                                        foreach(var pe in mpath.Edges)
                                        {
                                            Console.WriteLine("\n\t\t\t"+pe.Type);
                                            //if (pe.Type.ToString() == "Polyline")
                                            //{
                                                Hatch.BoundaryPath.Polyline polyline1;
                                            
                                                polyline1 = (Hatch.BoundaryPath.Polyline)pe;                                                
                                                int count = polyline1.Vertices.Count;
                                                //Console.WriteLine("\n" + count);
                                                int index = 0;
                                                XPoint[] xpoints = new XPoint[count];
                                                foreach (CSMath.XY point in polyline1.Vertices)
                                                {
                                                    xpoints[index].X = v_d + point.X;
                                                    xpoints[index].Y = vh - point.Y - v_d;
                                                    //Console.WriteLine("\n\t" + point.X + "\t\t" + point.Y);
                                                    index++;
                                                }
                                                gfx.DrawPolygon(pdfpen, xBrush, xpoints,XFillMode.Winding);                                                                                          
                                            //}                                       
                                        }                                   
                                    }                               
                                }
                                if (e.ObjectType.ToString() == "LWPOLYLINE")
                                {
                                    //Console.Write("\n\t\t--LWPOLYLINE--");
                                    polyline = (LwPolyline)e;
                                    int count = polyline.Vertices.Count;
                                    //Console.WriteLine("\n" + count);
                                    int index = 0;
                                    XPoint[] xpoints = new XPoint[count];
                                    foreach (LwPolyline.Vertex point in polyline.Vertices)
                                    {
                                        xpoints[index].X = v_d + point.Location.X;
                                        xpoints[index].Y = vh - point.Location.Y - v_d;
                                        //Console.WriteLine("\n\t" + point.X + "\t\t" + point.Y);
                                        index++;
                                    }
                                    gfx.DrawPolygon(pdfpen, xpoints);
                                }
                                if (e.ObjectType.ToString() == "MTEXT")
                                {
                                    Console.Write("\n\t\t------TEXTinput--------");
                                    text = (MText)e;
                                    MText text_conternt = new MText();
                                    text_conternt.AdditionalText = text.AdditionalText;

                                    string ctk = text.Value;
                                    XPoint point = new XPoint(text.InsertPoint.X+v_d, vh - text.InsertPoint.Y-v_d);
                                    XSize size = new XSize(text.RectangleWitdth + 10, text.RectangleHeight + 10);

                                    gfx.DrawString(ctk, font, XBrushes.Black, new XRect(point, size), XStringFormats.Center);
                                }
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
