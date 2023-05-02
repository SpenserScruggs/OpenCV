using System;
using System.Threading;
using System.Drawing;
using System.Globalization;
using Emgu.CV;
using Emgu.CV.Dnn;
using Emgu.CV.Features2D;
using Emgu.CV.ImgHash;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.IO.Ports;
using System.Xml.Serialization;

namespace OpenCV
{
    class Program
    {
        static void Main(string[] args)
        {
            var vc = new VideoCapture(0, Emgu.CV.VideoCapture.API.DShow);

            Mat frame = new();
            Mat frameBlur = new();
            Mat hsv = new();

            Mat redmask = new();
            Mat bluemask = new();
            Mat blackmask = new();

            Mat redhier = new();
            Mat bluehier = new();
            Mat blackhier = new();

            MCvScalar Redlower = new MCvScalar(0, 100, 100);
            MCvScalar Bluelower = new MCvScalar(100, 100, 60);
            MCvScalar Blacklower = new MCvScalar(0, 0, 0);

            MCvScalar Redupper = new MCvScalar(8, 255, 255);
            MCvScalar Blueupper = new MCvScalar(130, 255, 255);
            MCvScalar Blackupper = new MCvScalar(180, 150, 90);

            int xrange = 190;
            int yrange = 125;
            int ystart = 80;
            
            int xmin = 0, xmax = 0, ymin = 0, ymax = 0;

            int redXpos = 0, redYpos = 0;
            int blueXpos = 0, blueYpos  = 0;

            int loading = 0;

            while (true)
            {
                vc.Read(frame);
                
                CvInvoke.GaussianBlur(frame, frameBlur, new System.Drawing.Size(9, 9), 20.0);
                CvInvoke.CvtColor(frameBlur, hsv, Emgu.CV.CvEnum.ColorConversion.Bgr2Hsv);

                CvInvoke.InRange(hsv, new ScalarArray(Bluelower), new ScalarArray(Blueupper), bluemask);
                CvInvoke.InRange(hsv, new ScalarArray(Redlower), new ScalarArray(Redupper), redmask);
                CvInvoke.InRange(hsv, new ScalarArray(Blacklower), new ScalarArray(Blackupper), blackmask);

                CvInvoke.Imshow("bluemask", bluemask);
                CvInvoke.Imshow("redmask", redmask);
                CvInvoke.Imshow("blackmask", blackmask);

                VectorOfVectorOfPoint contourRed = new VectorOfVectorOfPoint();
                VectorOfVectorOfPoint contourBlue = new VectorOfVectorOfPoint();
                VectorOfVectorOfPoint contourBlack = new VectorOfVectorOfPoint();


                CvInvoke.FindContours(bluemask, contourBlue, bluehier, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                CvInvoke.FindContours(redmask, contourRed, redhier, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                CvInvoke.FindContours(blackmask, contourBlack, blackhier, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

                CvInvoke.DrawContours(frame, contourBlue, -1, new MCvScalar(255, 100, 0), 2);
                CvInvoke.DrawContours(frame, contourRed, -1, new MCvScalar(0, 100, 255), 2);
                CvInvoke.DrawContours(frame, contourBlack, -1, new MCvScalar(0, 0, 0), 2);

                CvInvoke.Imshow("cam", frame);

                int[] xblue = new int[contourBlue.Size];
                int[] xred = new int[contourRed.Size];
                int[] xblack = new int[contourBlack.Size];

                int[] yblue = new int[contourBlue.Size];
                int[] yred = new int[contourRed.Size];
                int[] yblack = new int[contourBlack.Size];

                

                for (int i = 0; i < contourBlack.Size; i++)
                {
                    try
                    {
                        Rectangle boundingBlack = CvInvoke.BoundingRectangle(contourBlack[i]);
                        xblack[i] = boundingBlack.X;
                        yblack[i] = boundingBlack.Y;

                        xmin = xblack.Min();
                        xmax = xblack.Max();

                        ymin = yblack.Min();
                        ymax = yblack.Max();

                    } catch { }
                }               
                
                for (int i = 0; i < contourBlue.Size; i++)
                {
                    try
                    {
                        Rectangle boundingBlue = CvInvoke.BoundingRectangle(contourBlue[i]);
                        xblue[i] = boundingBlue.X;
                        yblue[i] = boundingBlue.Y;

                        //Console.WriteLine(xblue[i]);

                    } catch { }

                }

                for (int i = 0; i < contourRed.Size; i++)
                {
                    try
                    {
                        Rectangle boundingRed = CvInvoke.BoundingRectangle(contourRed[i]);
                        xred[i] = boundingRed.X;
                        yred[i] = boundingRed.Y;
                    }
                    catch { }
                }

                try
                {
                    //Console.WriteLine(xblue[0] + " " + xmin + " " + xmax + " " + xrange);
                    blueXpos = (blueXpos + (int)(xrange - ((float)xblue[0] - (float)xmin) / ((float)xmax - (float)xmin) * xrange - 8)) / 2;
                    blueYpos = (blueYpos + (int)(ystart + ((float)yblue[0] - (float)ymin) / ((float)ymax - (float)xmin) * yrange + 8)) / 2;

                    redXpos = (redXpos + (int)(xrange - ((float)xred[0] - (float)xmin) / ((float)xmax - (float)xmin) * xrange - 8)) / 2;
                    redYpos = (redYpos + (int)(ystart + ((float)yred[0] - (float)ymin) / ((float)ymax - (float)xmin) * yrange + 8)) / 2;

                    Console.Write(blueXpos);
                    Console.Write("    ");
                    Console.Write(blueYpos);
                    Console.Write("    ");
                    Console.Write(redXpos);
                    Console.Write("    ");
                    Console.WriteLine(redYpos);
                    loading += 1;

                } catch { }



                //if (xblue.Length > 0)
                //{
                //    Console.Write("Blue Contour " + contourBlue.Size + " ");

                //    for (int i = 0; i < contourBlue.Size; i++)
                //    {
                //        Console.Write(" - X: " + xblue[i] + ", Y: " + yblue[i]);

                //    }
                //    Console.WriteLine();
                //}

                //if (xred.Length > 0)
                //{
                //    Console.Write("Red Contour " + contourRed.Size + " ");

                //    for (int i = 0; i < contourRed.Size; i++)
                //    {
                //        Console.Write(" - X: " + xred[i] + ", Y: " + yred[i]);

                //    }
                //    Console.WriteLine();
                //}


                //if (CvInvoke.WaitKey(1) == 13)
                //{
                //    port_output = "";
                //    Console.WriteLine(port_output);
                //}
                if(loading > 100)
                {
                    break;
                }

                if (CvInvoke.WaitKey(1) == 27)
                {
                    break;
                }

            }
            string Size(int x)
            {
                string output = x.ToString();
                if (output.Length == 1)
                {
                    output = "00" + output;
                }
                else if (output.Length == 2)
                {
                    output = "0" + output;
                }
                return output;
            }

            Console.Write("use current value? (y/n): ");
            string awnser = Console.ReadLine();
            if (awnser != null)
            {
                if (awnser == "y" || awnser == "Y")
                {
                    SerialPort port = new SerialPort("COM4", 9600);
                    port.Parity = Parity.None;
                    port.DataBits = 8;
                    port.StopBits = StopBits.One;
                    try
                    {
                        Console.WriteLine(Size(blueYpos) + Size(blueXpos) + Size(redYpos) + Size(redXpos));                        
                        port.Open();
                        Thread.Sleep(1000);
                        port.WriteLine(Size(blueYpos) + Size(blueXpos) + Size(redYpos) + Size(redXpos));
                        Thread.Sleep(1000);
                        port.Close();
                    } catch { }

                }
            }

        }

    }
}