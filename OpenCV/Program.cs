using System;
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
            MCvScalar Bluelower = new MCvScalar(100, 110, 65);
            MCvScalar Blacklower = new MCvScalar(0, 0, 0);

            MCvScalar Redupper = new MCvScalar(8, 255, 255);
            MCvScalar Blueupper = new MCvScalar(130, 255, 255);
            MCvScalar Blackupper = new MCvScalar(180, 150, 90);

            int xrange = 190;
            int yrange = 125;
            
            int xmin = 0, xmax = 0, ymin = 0, ymax = 0;

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
                    Console.WriteLine(xrange - ((float)xblue[0] - (float)xmin) / ((float)xmax - (float)xmin) * xrange);
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


                if (CvInvoke.WaitKey(1) == 27)
                {
                    break;
                }

            }

        }
    }
}