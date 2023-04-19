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

            MCvScalar Redlower = new MCvScalar(0, 150, 100);
            MCvScalar Bluelower = new MCvScalar(100, 100, 30);
            MCvScalar Blacklower = new MCvScalar(0, 0, 0);

            MCvScalar Redupper = new MCvScalar(8, 255, 255);
            MCvScalar Blueupper = new MCvScalar(140, 255, 255);
            MCvScalar Blackupper = new MCvScalar(180, 100, 35);



            while (true)
            {
                vc.Read(frame);
                
                CvInvoke.GaussianBlur(frame, frameBlur, new System.Drawing.Size(5, 5), 10.0);
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

                int[] yblue = new int[contourBlue.Size];
                int[] yred = new int[contourRed.Size];

                for (int i = 0; i < contourBlue.Size; i++)
                {
                    try
                    {
                        Rectangle boundingBlue = CvInvoke.BoundingRectangle(contourBlue[i]);
                        xblue[i] = boundingBlue.X;
                        yblue[i] = boundingBlue.Y;

                        Rectangle boundingRed = CvInvoke.BoundingRectangle(contourRed[i]);
                        xred[i] = boundingRed.X;
                        yred[i] = boundingRed.Y;

                        // Write the XY screen positions of each contour's bounding rectangle to the console

                    } catch { }

                }

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