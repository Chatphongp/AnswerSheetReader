using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System.Drawing.Imaging;
using Emgu.CV.Util;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using Emgu.CV.CvEnum;

namespace AnswerSheetReader.Class
{
    public class ImageUtils
    {
        public Image<Bgr, Byte> originalImage;
        public Rectangle vBoundary;

        public String test()
        {
            return "aa";
        }

        public void DeSkew(int i)
        {
            vBoundary = new Rectangle(new Point(0, 0), new Size(140, originalImage.Height));
            
            Emgu.CV.Cvb.CvBlobDetector bDetect = new Emgu.CV.Cvb.CvBlobDetector();
            Emgu.CV.Cvb.CvBlobs markerBlob = new Emgu.CV.Cvb.CvBlobs();

            List<RectangleF> blobs = new List<RectangleF>();
                            
            Image<Gray, Byte> preprocessImage = originalImage.Convert<Gray, Byte>();
            preprocessImage = preprocessImage.ThresholdBinary(new Gray(200), new Gray(255));
            preprocessImage = preprocessImage.Not();

            markerBlob.Clear();
            
            bDetect.Detect(preprocessImage, markerBlob);
            preprocessImage.Dispose();
            preprocessImage = null;

            markerBlob.FilterByArea(250, 1800);

            foreach (Emgu.CV.Cvb.CvBlob targetBlob in markerBlob.Values)
            {
                    if (vBoundary.Contains(targetBlob.BoundingBox))
                    {
                        if (targetBlob.BoundingBox.Width >= targetBlob.BoundingBox.Height - 5)
                        {
                            RectangleF r = new Rectangle(targetBlob.BoundingBox.X, targetBlob.BoundingBox.Y, targetBlob.BoundingBox.Width, targetBlob.BoundingBox.Height);
                            blobs.Add(r);
                        }
                    }
                       
            }

            RectangleF temp = blobs.First();
            RectangleF temp2 = blobs.Last();

            double dY = Math.Abs(temp.Y - temp2.Y);
            double dX = Math.Abs(temp.X - temp2.X);

            double angle = Math.Atan2(dX, dY);
            angle = angle * (180 / Math.PI);

            if (temp2.X > temp.X)
            {
                angle = angle * -1;
            }

            RotatedRect rot_rec = new RotatedRect();
            rot_rec.Center = new PointF(temp.X, temp.Y);
            RotationMatrix2D rot_mat = new RotationMatrix2D(rot_rec.Center, angle, 1);
            Image<Bgr, Byte> outimage = originalImage.CopyBlank();
            CvInvoke.WarpAffine(originalImage, outimage, rot_mat, originalImage.Size, Inter.Cubic, Warp.Default, BorderType.Constant, new Bgr(Color.White).MCvScalar);

            int xOffset = 80 - (int)temp.X;
            int yOffset = 45 - (int)temp.Y;

            originalImage = outimage.Copy();

            Bitmap a = originalImage.ToBitmap();
            CanvasMove filter = new CanvasMove(new AForge.IntPoint(xOffset, yOffset), Color.White);
            a = filter.Apply(a);
            originalImage = new Image<Bgr, Byte>(a);

            originalImage.Save("C:\\Users\\Chadpong\\Desktop\\output\\" + (i + 1) + ".jpg");

            a.Dispose();
            a = null;
            outimage.Dispose();
            outimage = null;
            originalImage.Dispose();
            originalImage = null;
            blobs = null;
        }
    }
}
