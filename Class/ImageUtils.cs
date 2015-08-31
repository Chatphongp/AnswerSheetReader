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
using AForge.Math.Geometry;


namespace AnswerSheetReader.Class
{
    public class ImageUtils
    {
        public Image<Bgr, Byte> originalImage;
        private List<Rectangle> vBlobs = new List<Rectangle>();
        private List<Rectangle> hBlobs = new List<Rectangle>();
        private List<Rectangle> idBlobs = new List<Rectangle>();
        private List<Rectangle> answerBlobs = new List<Rectangle>();

        private List<Rectangle> outerMarker = new List<Rectangle>();

        private Rectangle vBoundary;
        private Rectangle hBoundary;
        private Rectangle idBoundary;
        private Rectangle answerBoundary;

        private List<Answer> applicantIDList;

        public ImageUtils (Image<Bgr, Byte> image) {
            originalImage = image.Copy();
            vBoundary  = new Rectangle(new Point(0, 0), new Size(140, originalImage.Height));
            hBoundary  = new Rectangle(new Point(0, originalImage.Height - 120), new Size( originalImage.Width, 110));
            idBoundary = new Rectangle(new Point(1255, 155), new Size(333, 550 ));
            answerBoundary = new Rectangle(new Point(158, 809), new Size(1422, 1394));
        }

        public int FindCorner()
        {
            Emgu.CV.Cvb.CvBlobDetector bDetect = new Emgu.CV.Cvb.CvBlobDetector();
            Emgu.CV.Cvb.CvBlobs markerBlob = new Emgu.CV.Cvb.CvBlobs();

            List<Rectangle> blobs = new List<Rectangle>();

            Image<Gray, Byte> preprocessImage = originalImage.Convert<Gray, Byte>();

            preprocessImage = preprocessImage.ThresholdBinary(new Gray(100), new Gray(255));
            preprocessImage = preprocessImage.Not();

            

            markerBlob.Clear();

            bDetect.Detect(preprocessImage, markerBlob);
            //preprocessImage.Dispose();
            //preprocessImage = null;

            //markerBlob.FilterByArea(250, 1800);

            foreach (Emgu.CV.Cvb.CvBlob targetBlob in markerBlob.Values)
            {
                if (targetBlob.BoundingBox.Width <= 32 && targetBlob.BoundingBox.Width >= 29)
                {
                    if (targetBlob.BoundingBox.Height <= 32 && targetBlob.BoundingBox.Height >= 29)
                    {
                        Rectangle r = new Rectangle(targetBlob.BoundingBox.X, targetBlob.BoundingBox.Y, targetBlob.BoundingBox.Width, targetBlob.BoundingBox.Height);
                        outerMarker.Add(r);
                    }
                }
                CvInvoke.PutText(preprocessImage, targetBlob.BoundingBox.Width + " " + targetBlob.BoundingBox.Height, new Point(targetBlob.BoundingBox.X, targetBlob.BoundingBox.Y), FontFace.HersheyComplex, 1, new Bgr(Color.Red).MCvScalar, 2);
            }

            return outerMarker.Count;

        }

        public void PerspectiveTransform(string path)
        {
            List<Rectangle> minX = outerMarker.OrderBy(r => r.X).Take(2).ToList();
            List<Rectangle> maxX = outerMarker.OrderBy(r => r.X).Reverse().Take(2).ToList();

            Rectangle topLeft = minX.OrderBy(r => r.Y).First();
            Rectangle bottomLeft = minX.OrderBy(r => r.Y).Reverse().First();

            Rectangle topRight = maxX.OrderBy(r => r.Y).First();
            Rectangle bottomRight = maxX.OrderBy(r => r.Y).Reverse().First();

            List<AForge.IntPoint> corners = new List<AForge.IntPoint>();
            corners.Add(new AForge.IntPoint( topLeft.X, topLeft.Y));
            corners.Add(new AForge.IntPoint(topRight.X + topRight.Width , topRight.Y ));
            corners.Add(new AForge.IntPoint(bottomRight.X + bottomRight.Width , bottomRight.Y + bottomRight.Height));
            corners.Add(new AForge.IntPoint(bottomLeft.X, bottomLeft.Y + bottomRight.Height ));

            QuadrilateralTransformation filter = new QuadrilateralTransformation(corners, 1000, 1375);
            Bitmap newImage = filter.Apply(originalImage.ToBitmap());

            newImage.Save(path);
            
        }
        
        public void Save(string path)
        {
            originalImage.Save(path);
        }
        public void DeSkew()
        {
            Rectangle vBoundary = new Rectangle(new Point(0, 0), new Size(140, originalImage.Height));

            Emgu.CV.Cvb.CvBlobDetector bDetect = new Emgu.CV.Cvb.CvBlobDetector();
            Emgu.CV.Cvb.CvBlobs markerBlob = new Emgu.CV.Cvb.CvBlobs();

            List<Rectangle> blobs = new List<Rectangle>();

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
                        Rectangle r = new Rectangle(targetBlob.BoundingBox.X, targetBlob.BoundingBox.Y, targetBlob.BoundingBox.Width, targetBlob.BoundingBox.Height);
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

            a.Dispose();
            a = null;
            outimage.Dispose();
            outimage = null;
            blobs = null;
        }

        public int Validate()
        {            

            Emgu.CV.Cvb.CvBlobDetector bDetect = new Emgu.CV.Cvb.CvBlobDetector();
            Emgu.CV.Cvb.CvBlobs markerBlob = new Emgu.CV.Cvb.CvBlobs();

            Image<Gray, Byte> preprocessImage = originalImage.Convert<Gray, Byte>();

            UMat pyr = new UMat();
            CvInvoke.PyrDown(preprocessImage, pyr);
            CvInvoke.PyrUp(pyr, preprocessImage);

            preprocessImage = preprocessImage.ThresholdBinary(new Gray(200), new Gray(255));
            preprocessImage = preprocessImage.Not();
           

            markerBlob.Clear();

            bDetect.Detect(preprocessImage, markerBlob);

            preprocessImage.Dispose();
            preprocessImage = null;

            markerBlob.FilterByArea(400, 1800);

            
            foreach (Emgu.CV.Cvb.CvBlob targetBlob in markerBlob.Values)
            {
                if (vBoundary.Contains(targetBlob.BoundingBox))
                {
                    if (targetBlob.BoundingBox.Width >= targetBlob.BoundingBox.Height - 5)
                    {
                        Rectangle r = new Rectangle(targetBlob.BoundingBox.X, targetBlob.BoundingBox.Y, targetBlob.BoundingBox.Width, targetBlob.BoundingBox.Height);
                        vBlobs.Add(r);
                    }
                }
                else if (hBoundary.Contains(targetBlob.BoundingBox))
                {
                    if (targetBlob.BoundingBox.Height >= targetBlob.BoundingBox.Width - 5)
                    {
                        Rectangle r = new Rectangle(targetBlob.BoundingBox.X, targetBlob.BoundingBox.Y, targetBlob.BoundingBox.Width, targetBlob.BoundingBox.Height);
                        hBlobs.Add(r);
                    }
                }
                else if (idBoundary.Contains(targetBlob.BoundingBox))
                {
                    Rectangle r = new Rectangle(targetBlob.BoundingBox.X, targetBlob.BoundingBox.Y, targetBlob.BoundingBox.Width, targetBlob.BoundingBox.Height);
                    idBlobs.Add(r);
                }

                else if (answerBoundary.Contains(targetBlob.BoundingBox))
                {
                    Rectangle r = new Rectangle(targetBlob.BoundingBox.X, targetBlob.BoundingBox.Y, targetBlob.BoundingBox.Width, targetBlob.BoundingBox.Height);
                    answerBlobs.Add(r);
                }                
            }

            vBlobs.RemoveAt(0);
            int bCount = 0;

            hBlobs = hBlobs.OrderBy(r => r.X).ToList();
            vBlobs = vBlobs.OrderBy(r => r.Y).ToList();

            markerBlob.Dispose();
            markerBlob = null;
            return bCount;
        }

        public List<Answer> ReadAppicantId()
        {
            applicantIDList = new List<Answer>();

            foreach (Rectangle r in idBlobs)
            {
                int vIndex = 0 , hIndex = 0;

                foreach (Rectangle vRect in vBlobs)
                {
                    int avgY = (vRect.Y * 2 + vRect.Height) / 2;
                    if (avgY >= r.Y && avgY <= r.Y + r.Height)
                    {                        
                        break;
                    }
                    vIndex++;
                }


                foreach (Rectangle hRect in hBlobs)
                {
                    int avgX = (hRect.X * 2 + hRect.Width) / 2;
                    if (avgX >= r.X && avgX <= r.X + r.Width)
                    {
                        break;
                    }
                    hIndex++;
                }

                Answer ans = new Answer(vIndex, hIndex);
                ans.rect = r;
                applicantIDList.Add(ans);

            }


            return applicantIDList;
        }

        public void SaveImage(string path)
        {
            int vCount = 0;
            foreach (Rectangle r in vBlobs)
            {
                originalImage.Draw(r, new Bgr(Color.Red), 2);
                int avgY = (r.Y * 2 + r.Height) / 2;
                LineSegment2D line = new LineSegment2D(new Point(0, avgY), new Point(originalImage.Width, avgY));
                originalImage.Draw(line, new Bgr(Color.Red), 1);
                CvInvoke.PutText(originalImage, vCount.ToString(), new Point(r.X, r.Y), FontFace.HersheyComplex, 0.8, new Bgr(Color.Red).MCvScalar, 2);
                vCount++;
            }

            int hCount = 0;
            foreach (Rectangle r in hBlobs)
            {
                originalImage.Draw(r, new Bgr(Color.Blue), 2);
                int avgX = (r.X * 2 + r.Width) / 2;
                LineSegment2D line = new LineSegment2D(new Point(avgX, 0), new Point(avgX, originalImage.Height));
                originalImage.Draw(line, new Bgr(Color.Blue), 1);
                CvInvoke.PutText(originalImage, hCount.ToString(), new Point(r.X, r.Y), FontFace.HersheyComplex, 0.8, new Bgr(Color.Blue).MCvScalar, 2);
                hCount++;
            }


            foreach (Rectangle r in answerBlobs)
            {
                originalImage.Draw(r, new Bgr(Color.Violet), 2);
                CvInvoke.PutText(originalImage, Utils.getRectangleArea(r).ToString(), new Point(r.X, r.Y), FontFace.HersheyComplex, 0.8, new Bgr(Color.Blue).MCvScalar, 2);
            }

            originalImage.Draw(answerBoundary, new Bgr(Color.Red), 2);

            foreach (Answer ans in applicantIDList)
            {
                originalImage.Draw(ans.rect , new Bgr(Color.Red), 2);
                CvInvoke.PutText(originalImage, ans.vIndex + "," + ans.hIndex, new Point(ans.rect.X, ans.rect.Y), FontFace.HersheyComplex, 0.8, new Bgr(Color.Red).MCvScalar, 2);
            }

            originalImage.Save(path + ".jpg");
        }

        private Point[] ToPointsArray(List<AForge.IntPoint> points)
        {
            Point[] array = new Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new Point(points[i].X, points[i].Y);
            }

            return array;
        }
    }
}