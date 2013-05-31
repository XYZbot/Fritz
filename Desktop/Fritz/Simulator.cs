using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;
using System.Xml;

namespace Fritz
{
    public partial class Simulator : UserControl
    {
        const int leftPupilSelected = 1;
        const int rightPupilSelected = 2;
        const int leftEyebrowSelected = 3;
        const int rightEyebrowSelected = 4;
        const int leftEyelidSelected = 5;
        const int rightEyelidSelected = 6;
        const int leftLipSelected = 7;
        const int rightLipSelected = 8;
        const int jawSelected = 9;
        const int neckTiltSelected = 10;
        const int neckTwistSelected = 11;

        float eyeRadius = 0.24f;
        int eyeRadiusPixels;
        float eyeXLocation = 0.40f;
        float eyeYLocation = 0.37f;
        int pupilDiameter = 15;
        int pupilRadius;
        float eyebrowRadius = 0.40f;
        float eyebrowArc = 40.0f;
        float eyebrowMaxRotate = 0.8f;
        float eyebrowMinRotate = 0.2f;
        float noseX = 0.02f;
        float noseStartY = -0.55f;
        float noseEndY = 0.85f;
        int eyebrowRadiusPixels;
        float mouthCenterX = 0.0f;
        float mouthCenterY = 0.6f;
        const float mouthRadius = 0.15f;
        bool showMotionPoints = false;
        bool mouseDown = false;
        int mouseDownX = 0;
        int mouseDownY = 0;
        float leftLipX = -0.35f;
        float leftLipY = 0.0f;
        float upperLipX = 0.0f;
        float upperLipY = 0.0f;
        float rightLipX = 0.35f;
        float rightLipY = 0.0f;
        float jawX = 0.0f;
        float jawY = 0.05f;
        int leftLipControlX;
        int leftLipControlY;
        int rightLipControlX;
        int rightLipControlY;
        int jawControlX;
        int jawControlY;
        float jawControlZ;

        int upperLipControlY;

        int leftLipCenterX;
        int leftLipCenterY;

        int upperLipCenterX;
        int upperLipCenterY;

        int rightLipCenterX;
        int rightLipCenterY;

        int jawCenterX;
        int jawCenterY;

        int neckTiltControlX;
        int neckTiltControlY;
        float neckTiltControlZ;
        int neckTwistControlX;
        int neckTwistControlY;
        float neckTwistControlZ;
        const int MAX_NECK_Tilt = 100;
        const int MAX_NECK_TWIST = 100;
        const float NECK_SENSITIVITY = 550.0f;
        const int MAX_JAW = 40;

        int controlPointPixels = 15;

        int leftEyeX;
        int leftEyeY;
        int rightEyeX;
        int rightEyeY;

        int leftEyebrowControlX;
        int leftEyebrowControlY;
        float leftEyebrowControlZ;
        int leftEyebrowBaseX;
        int leftEyebrowBaseY;

        int rightEyebrowControlX;
        int rightEyebrowControlY;
        float rightEyebrowControlZ;
        int rightEyebrowBaseX;
        int rightEyebrowBaseY;

        int leftEyelidControlX;
        int leftEyelidControlY;
        float leftEyelidControlZ;

        int rightEyelidControlX;
        int rightEyelidControlY;
        float rightEyelidControlZ;

        Point[] headOutline = new Point[] { new Point(31, 203), new Point(36, 209), new Point(44, 221), new Point(47, 226), new Point(53, 236), new Point(55, 242), new Point(59, 251), new Point(69, 277), new Point(71, 287), new Point(73, 292), new Point(75, 303), new Point(75, 309), new Point(76, 335), new Point(81, 338), new Point(88, 340), new Point(104, 347), new Point(104, 310), new Point(250, 309), new Point(252, 346), new Point(260, 343), new Point(269, 340), new Point(280, 335), new Point(280, 312), new Point(282, 296), new Point(283, 291), new Point(285, 283), new Point(287, 276), new Point(294, 255), new Point(306, 231), new Point(310, 224), new Point(312, 219), new Point(325, 201), new Point(325, 50), new Point(321, 41), new Point(316, 34), new Point(309, 28), new Point(301, 22), new Point(293, 19), new Point(63, 19), new Point(56, 21), new Point(49, 25), new Point(44, 29), new Point(34, 41), new Point(31, 48), new Point(31, 203) };
        Point[] jawOutline = new Point[] { new Point(106, 355), new Point(112, 358), new Point(112, 363), new Point(118, 363), new Point(119, 360), new Point(130, 364), new Point(224, 365), new Point(230, 363), new Point(238, 360), new Point(241, 359), new Point(249, 356), new Point(250, 311), new Point(106, 311), new Point(106, 355) };
        Pen outlinePen = new Pen(Color.Gray);
        SolidBrush pupilBrush = new SolidBrush(Color.Black);
        Pen mouthPen = new Pen(Color.Black, 10.0f);
        Pen mouthServoPen = new Pen(Color.LightGray);
        SolidBrush mouthBrush = new SolidBrush(Color.Black);
        Pen eyebrowPen = new Pen(Color.Black, 10.0f);
        SolidBrush eyelidBrush = new SolidBrush(Color.FromArgb(185, 160, 115));
        SolidBrush noseBrush = new SolidBrush(Color.FromArgb(185, 160, 115));
        Pen noseOutlinePen = new Pen(Color.Black, 1);
        Pen highlightPen = new Pen(Color.LightGreen, 3);
        Brush highlightBrush = new SolidBrush(Color.LightGreen);
        SolidBrush mouthPointBrush = new SolidBrush(Color.FromArgb(0, 0, 0));
        //Color startHeadBrush = Color.FromArgb(160, 150, 136);
        //Color endHeadBrush = Color.FromArgb(110, 100, 86);
        Color startHeadBrush = Color.FromArgb(255, 255, 150);
        Color endHeadBrush = Color.FromArgb(185, 160, 115);

        int leftPupilControlX;
        int leftPupilControlY;
        int rightPupilControlX;
        int rightPupilControlY;

        int leftHorizontalEyePrime;
        int leftVerticalEyePrime;
        float leftPupilZPrime;
        int rightHorizontalEyePrime;
        int rightVerticalEyePrime;
        float rightPupilZPrime;

        float leftHorizontalEye = 0.5f;
        float leftVerticalEye = 0.5f;
        float rightHorizontalEye = 0.5f;
        float rightVerticalEye = 0.5f;
        float leftLip = 0.5f;
        float rightLip = 0.5f;
        float jaw = 0.5f;
        float neckTilt = 0.5f;
        float neckTwist = 0.5f;
        float leftEyebrow = 0.5f;
        float rightEyebrow = 0.5f;
        float leftEyelid = 0.5f;
        float rightEyelid = 0.5f;

        Rectangle headDimensions;
        Rectangle leftEyeDimensions;
        Rectangle rightEyeDimensions;
        Rectangle leftPupilDimensions;
        Rectangle rightPupilDimensions;

        float middleX = 0;
        float middleY = 0;

        Pen arrowPen = new Pen(Color.Red, 5);

        int selected = 0;

        float rx;
        float ry;
        float rz;
        float fl;

        const int padding = 20;

        public event EventHandler StateChangeCallback;

        public bool isPlaying = false;

        public Simulator()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            arrowPen.EndCap = LineCap.ArrowAnchor;

            RobotState initState = new RobotState();

            leftHorizontalEye = initState.leftHorizontalEye;
            leftVerticalEye = initState.leftVerticalEye;
            rightHorizontalEye = initState.rightHorizontalEye;
            rightVerticalEye = initState.rightVerticalEye;
            leftEyebrow = initState.leftEyebrow;
            rightEyebrow = initState.rightEyebrow;
            rightEyelid = initState.rightEyelid;
            leftEyelid = initState.leftEyelid;
            leftLip = initState.leftLip;
            rightLip = initState.rightLip;
            jaw = initState.jaw;
            neckTilt = initState.neckTilt;
            neckTwist = initState.neckTwist;
        }

        public GraphicsPath RotatePoints(ref GraphicsPath gPath, int cx, int cy, float rx, float ry, float rz, float fl, int offsetX=0, int offsetY=0)
        {
            if (gPath.PointCount <= 0) return new GraphicsPath();

            PointF[] pts = gPath.PathPoints;
            byte[] types = gPath.PathTypes;

            float cosrx = (float)Math.Cos(rx);
            float sinrx = (float)Math.Sin(rx);

            float cosry = (float)Math.Cos(ry);
            float sinry = (float)Math.Sin(ry);

            float cosrz = (float)Math.Cos(rz);
            float sinrz = (float)Math.Sin(rz);

            int i;
            for (i = 0; i < pts.Length; i++)
            {
                float x = (float)pts[i].X - (float)cx;
                float y = (float)pts[i].Y - (float)cy;
                float z = 1.0f;

                // X rotation
                float X = x;
                float Y = y * cosrx + z * sinrx;
                float Z = -y * sinrx + z * cosrx;

                // Y rotation
                x = X * cosry - Z * sinry;
                y = Y;
                z = X * sinry + Z * cosry;

                // Z rotation
                X = x * cosrz + y * sinrz;
                Y = -x * sinrz + y * cosrz;
                Z = z;

                // perspective based on focal length
                if (fl != 0.0f)
                {
                    float ZF = fl + Z;
                    if (ZF != 0.0f)
                    {
                        ZF = fl / ZF;
                        X *= ZF;
                        Y *= ZF;
                    }
                }

                pts[i].X = X + cx + offsetX;
                pts[i].Y = Y + cy + offsetY;
            }

            return new GraphicsPath(pts, types);
        }

        public void RotatePoint(ref int xs, ref int ys, int cx, int cy, float rx, float ry, float rz, float fl)
        {
            float x = (float)xs - (float)cx;
            float y = (float)ys - (float)cy;
            float z = 1.0f;

            // X rotation
            float cosrx = (float)Math.Cos(rx);
            float sinrx = (float)Math.Sin(rx);
            float X = x;
            float Y = y * cosrx + z * sinrx;
            float Z = -y * sinrx + z * cosrx;

            // Y rotation
            float cosry = (float)Math.Cos(ry);
            float sinry = (float)Math.Sin(ry);
            x = X * cosry - Z * sinry;
            y = Y;
            z = X * sinry + Z * cosry;

            // Z rotation
            float cosrz = (float)Math.Cos(rz);
            float sinrz = (float)Math.Sin(rz);
            X = x * cosrz + y * sinrz;
            Y = -x * sinrz + y * cosrz;
            Z = z;

            // perspective based on focal length
            if (fl != 0.0f)
            {
                float ZF = fl / (fl + Z);
                X *= ZF;
                Y *= ZF;
            }

            xs = (int)X + cx;
            ys = (int)Y + cy;
        }

        public void RotatePoint(ref int xs, ref int ys, ref float zs, int cx, int cy, float rx, float ry, float rz, float fl)
        {
            float x = (float)xs - (float)cx;
            float y = (float)ys - (float)cy;
            float z = 1.0f;

            // X rotation
            float cosrx = (float)Math.Cos(rx);
            float sinrx = (float)Math.Sin(rx);
            float X = x;
            float Y = y * cosrx + z * sinrx;
            float Z = -y * sinrx + z * cosrx;

            // Y rotation
            float cosry = (float)Math.Cos(ry);
            float sinry = (float)Math.Sin(ry);
            x = X * cosry - Z * sinry;
            y = Y;
            z = X * sinry + Z * cosry;

            // Z rotation
            float cosrz = (float)Math.Cos(rz);
            float sinrz = (float)Math.Sin(rz);
            X = x * cosrz + y * sinrz;
            Y = -x * sinrz + y * cosrz;
            Z = z;

            // perspective based on focal length
            if (fl != 0.0f)
            {
                float ZF = fl / (fl + Z);
                X *= ZF;
                Y *= ZF;
            }

            xs = (int)X + cx;
            ys = (int)Y + cy;
            zs = Z;
        }

        // This rotates a rectangle but will use the middle points to estimate actual size. When rotating a square with
        // perspective the extreme points will cause the rectangle to widen drastically. Since rectangle rotation is use
        // for hit testing, we'd like to keep this as tight as possible.
        public void RotateRect(ref Rectangle rect, int cx, int cy, float rx, float ry, float rz, float fl)
        {
            int x1 = rect.X + (rect.Width / 2);
            int y1 = rect.Y;

            int x2 = rect.X + (rect.Width / 2);
            int y2 = rect.Y + rect.Height;

            int x3 = rect.X + rect.Width;
            int y3 = rect.Y + (rect.Height / 2);

            int x4 = rect.X;
            int y4 = rect.Y + (rect.Height / 2);

            RotatePoint(ref x1, ref y1, cx, cy, rx, ry, rz, fl);
            RotatePoint(ref x2, ref y2, cx, cy, rx, ry, rz, fl);
            RotatePoint(ref x3, ref y3, cx, cy, rx, ry, rz, fl);
            RotatePoint(ref x4, ref y4, cx, cy, rx, ry, rz, fl);

            int minX = x1;
            int maxX = x1;
            int minY = y1;
            int maxY = y1;

            if (x2 < minX) minX = x2;
            if (x3 < minX) minX = x3;
            if (x4 < minX) minX = x4;
            if (x2 > maxX) maxX = x2;
            if (x3 > maxX) maxX = x3;
            if (x4 > maxX) maxX = x4;

            if (y2 < minY) minY = y2;
            if (y3 < minY) minY = y3;
            if (y4 < minY) minY = y4;
            if (y2 > maxY) maxY = y2;
            if (y3 > maxY) maxY = y3;
            if (y4 > maxY) maxY = y4;

            rect.X = minX;
            rect.Y = minY;
            rect.Width = maxX - minX;
            rect.Height = maxY - minY;
        }

        public void RotatePoint(int px, int py, ref int rx, ref int ry, float angle, int cx, int cy)
        {
            float cosAngle = (float)Math.Cos(angle);
            float sinAngle = (float)Math.Sin(angle);

            px -= cx;
            py -= cy;

            rx = (int)(((cosAngle * (float)px) - (sinAngle * (float)py)) + cx);
            ry = (int)(((sinAngle * (float)px) + (cosAngle * (float)py)) + cy);
        }

        public float calculateAngle(int cx, int cy, int ax, int ay, int bx, int by)
        {
            float ang = (float)Math.Atan2((float)ay - cy, (float)ax - cx) - (float)Math.Atan2((float)by - cy, (float)bx - cx);
            if (ang < -Math.PI) ang += (float)Math.PI*2.0f;
            if (ang > Math.PI) ang -= (float)Math.PI*2.0f;
            return ang;
        }

        public void InvRotatePoint(ref int xs, ref int ys, float zs, int cx, int cy, float rx, float ry, float rz, float fl)
        {
            float X, Y, Z;

            X = xs - cx;
            Y = ys - cy;
            Z = zs;

            // perspective based on focal length
            if (fl != 0.0f)
            {
                float ZF = fl / (fl + Z);
                X /= ZF;
                Y /= ZF;
            }

            // Z rotation
            float cosrz = (float)Math.Cos(-rz);
            float sinrz = (float)Math.Sin(-rz);
            float x = X * cosrz + Y * sinrz;
            float y = -X * sinrz + Y * cosrz;
            float z = Z;

            // Y rotation
            float cosry = (float)Math.Cos(-ry);
            float sinry = (float)Math.Sin(-ry);
            X = x * cosry - z * sinry;
            Y = y;
            Z = x * sinry + z * cosry;

            // X rotation
            float cosrx = (float)Math.Cos(-rx);
            float sinrx = (float)Math.Sin(-rx);
            x = X;
            y = Y * cosrx + Z * sinrx;
            //z = -Y * sinrx + Z * cosrx; // should be ~1.0

            xs = (int)X + cx;
            ys = (int)Y + cy;
        }


        private void drawPupilArrows(Graphics gr, int X, int Y, int Width, int Height, float middleX, float middleY)
        {
            int cx = X + (Width >> 1);
            int cy = Y + (Height >> 1);

            GraphicsPath gPath = new GraphicsPath();
            gPath.AddLine(cx, Y - 5, cx, Y - 20);
            GraphicsPath dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
            gr.DrawPath(arrowPen, dPath);

            gPath.Reset();
            gPath.AddLine(cx, (Y + Height) + 5, cx, (Y + Height) + 20);
            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
            gr.DrawPath(arrowPen, dPath);

            gPath.Reset();
            gPath.AddLine(X - 5, cy, X - 20, cy);
            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
            gr.DrawPath(arrowPen, dPath);

            gPath.Reset();
            gPath.AddLine((X + Width) + 5, cy, (X + Width) + 20, cy);
            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
            gr.DrawPath(arrowPen, dPath);
        }

        private void drawVerticalArrows(Graphics gr, int cx, int cy, int middleX, int middleY)
        {
            int radius = controlPointPixels / 2;

            GraphicsPath gPath = new GraphicsPath();
            gPath.AddLine(cx, cy - 5 - radius, cx, cy - 20 - radius);
            GraphicsPath dPath = RotatePoints(ref gPath, middleX, middleY, rx, ry, rz, fl);
            gr.DrawPath(arrowPen, dPath);

            gPath.Reset();
            gPath.AddLine(cx, cy + 5 + radius, cx, cy + 20 + radius);
            dPath = RotatePoints(ref gPath, middleX, middleY, rx, ry, rz, fl);
            gr.DrawPath(arrowPen, dPath);
        }

        private void drawHorizontalArrows(Graphics gr, int cx, int cy, int middleX, int middleY)
        {
            int radius = controlPointPixels / 2;
            GraphicsPath gPath = new GraphicsPath();
            gPath.AddLine(cx - 5 - radius, cy, cx - 20 - radius, cy);
            GraphicsPath dPath = RotatePoints(ref gPath, middleX, middleY, rx, ry, rz, fl);
            gr.DrawPath(arrowPen, dPath);

            gPath.Reset();
            gPath.AddLine(cx + 5 + radius, cy, cx + 20 + radius, cy);
            dPath = RotatePoints(ref gPath, middleX, middleY, rx, ry, rz, fl);
            gr.DrawPath(arrowPen, dPath);
        }

        private void drawCurvedArrows(Graphics gr, int px, int py, int width, int height, int angle, int middleX, int middleY)
        {
            if (angle < 0) angle += 360;

            GraphicsPath gPath = new GraphicsPath();
            gPath.AddArc(px, py, width, height, angle - 30, -80);

            GraphicsPath dPath = RotatePoints(ref gPath, middleX, middleY, rx, ry, rz, fl);
            gr.DrawPath(arrowPen, dPath);

            gPath.Reset();
            gPath.AddArc(px, py, width, height, angle + 30, 80);
            dPath = RotatePoints(ref gPath, middleX, middleY, rx, ry, rz, fl);
            gr.DrawPath(arrowPen, dPath);
        }

        private int orderPointsByAngle(ref Point[] pts)
        {
            int i, j;
            // order points in clockwise fashion
            // determine center of gravity as a point of reference
            float cogx = 0;
            float cogy = 0;

            foreach (Point p in pts)
            {
                cogx += p.X;
                cogy += p.Y;
            }

            cogx /= pts.Length;
            cogy /= pts.Length;

            float[] angle = new float[pts.Length];
            // determine angle from center of gravity
            for (i = 0; i < pts.Length; i++)
                angle[i] = (float)(Math.Atan2(cogy - pts[i].Y, cogx - pts[i].X) + Math.PI);

            for (i = 0; i < pts.Length - 1; i++)
            {
                for (j = 0; j < pts.Length - i - 1; j++)
                {
                    if (angle[j] > angle[j + 1])
                    {
                        float a = angle[j];
                        angle[j] = angle[j + 1];
                        angle[j + 1] = a;

                        Point p = pts[j];
                        pts[j] = pts[j + 1];
                        pts[j + 1] = p;
                    }
                }
            }

            float max = 0;
            int maxIndex = 0;
            float diff;
            for (i = 0; i < pts.Length - 1; i++)
            {
                diff = angle[i + 1] - angle[i];
                if (diff > max) { max = diff; maxIndex = i; }
            }
            diff = (float)(Math.PI * 2.0) - angle[pts.Length - 1] + angle[0];
            if (diff > max) maxIndex = pts.Length - 1;

            return maxIndex;
        }

        private void Simulator_Paint(object sender, PaintEventArgs e)
        {
            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;
            if (width > height)
                width = (int)(0.9417989 * (float)height);
            else
                height = (int)(1.06179774 * (float)width);

            GraphicsPath gPath = new GraphicsPath();
            GraphicsPath dPath;
            int controlPointPixelsX = controlPointPixels;
            int controlPointPixelsY = controlPointPixels;
            float headRadiusPixels = width / 2;
            if ((height / 2) < headRadiusPixels) headRadiusPixels = height / 2;

            rx = (neckTilt - 0.5f) * ((float)Math.PI/6.0f);
            ry = (neckTwist - 0.5f) * ((float)Math.PI/2.0f);
            rz = 0.0f;// (neckRotate - 0.5f) * (float)Math.PI;
            fl = 400.0f;

            middleX = width / 2.0f;
            middleY = height / 2.0f;

            if (showMotionPoints&&(!isPlaying))
            {
                RotatePoint(ref controlPointPixelsX, ref controlPointPixelsY, 0, 0, rx, ry, rz, fl);
            }

            //Bitmap doubleBuffer = new Bitmap(width, height);
            //Graphics gr = Graphics.FromImage(doubleBuffer);
            Graphics gr = e.Graphics;

            //gr.InterpolationMode = InterpolationMode.Bilinear;
            gr.SmoothingMode = SmoothingMode.AntiAlias;

            /////HEAD/////////////

            // draw the head
            headDimensions = new Rectangle(padding, padding, width - (padding*2), height - (padding * 2));
            Brush headGradient = new LinearGradientBrush(headDimensions, startHeadBrush, endHeadBrush, 45, false);
            gPath.Reset();
            gPath.AddLines(headOutline);
            Matrix smat = new Matrix();
            smat.Scale((float)width / 356, (float)height / 380);
            gPath.Transform(smat);

            //gPath.AddEllipse(headDimensions);
            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
            gr.FillPath(headGradient, dPath);
            gr.DrawPath(outlinePen, dPath);

            /////NECK (control points) /////////////

            neckTiltControlX = headDimensions.X + (int)((float)headDimensions.Width * 0.9f);
            neckTiltControlY = headDimensions.Y + (int)((float)headDimensions.Height / 2.0f) + (int)((rx * NECK_SENSITIVITY) / (float)Math.PI);

            neckTwistControlX = headDimensions.X + (int)((float)headDimensions.Width / 2.0f) + (int)((ry * NECK_SENSITIVITY) / (float)Math.PI);
            neckTwistControlY = headDimensions.Y + (int)((float)headDimensions.Height * 0.05f);

            RotateRect(ref headDimensions, (int)middleX, (int)middleY, rx, ry, rz, fl);
            //gr.DrawRectangle(outlinePen, headDimensions);

            if (showMotionPoints && (!mouseDown) && (!isPlaying))
            {
                gPath.Reset();
                gPath.AddEllipse(neckTiltControlX - controlPointPixelsX / 2, neckTiltControlY - controlPointPixelsY / 2, controlPointPixelsX, controlPointPixelsY);
                dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
                gr.FillPath(highlightBrush, dPath);
                //gr.FillEllipse(highlightBrush, neckTiltControlX - controlPointPixelsX / 2, neckTiltControlY - controlPointPixelsY / 2, controlPointPixelsX, controlPointPixelsY);

                gPath.Reset();
                gPath.AddEllipse(neckTwistControlX - controlPointPixelsX / 2, neckTwistControlY - controlPointPixelsY / 2, controlPointPixelsX, controlPointPixelsY);
                dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
                gr.FillPath(highlightBrush, dPath);
                //gr.FillEllipse(highlightBrush, neckTwistControlX - controlPointPixelsX / 2, neckTwistControlY - controlPointPixelsY / 2, controlPointPixelsX, controlPointPixelsY);

                if (selected == neckTiltSelected)
                    drawVerticalArrows(gr, neckTiltControlX, neckTiltControlY, (int)middleX, (int)middleY);

                if (selected == neckTwistSelected)
                    drawHorizontalArrows(gr, neckTwistControlX, neckTwistControlY, (int)middleX, (int)middleY);
            }

            // update control points to rotated view
            RotatePoint(ref neckTiltControlX, ref neckTiltControlY, ref neckTiltControlZ, (int)middleX, (int)middleY, rx, ry, rz, fl);
            RotatePoint(ref neckTwistControlX, ref neckTwistControlY, ref neckTwistControlZ, (int)middleX, (int)middleY, rx, ry, rz, fl);

            /////EYES/////////////

            // draw the eyes
            eyeRadiusPixels = (int)(eyeRadius * middleX);
            leftEyeX = (int)(middleX - (middleX * eyeXLocation));
            leftEyeY = (int)(middleY - (middleY * eyeYLocation));
            rightEyeX = (int)(middleX + (middleX * eyeXLocation));
            rightEyeY = (int)(middleY - (middleY * eyeYLocation));

            // draw the left eye
            leftEyeDimensions = new Rectangle(leftEyeX - eyeRadiusPixels, leftEyeY - eyeRadiusPixels, eyeRadiusPixels + eyeRadiusPixels, eyeRadiusPixels + eyeRadiusPixels);
            Brush leftEyeGradient = new LinearGradientBrush(leftEyeDimensions, Color.FromArgb(250, 250, 250), Color.FromArgb(225, 225, 255), 45, false);
            gPath.Reset();
            gPath.AddEllipse(leftEyeDimensions);
            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
            gr.FillPath(leftEyeGradient, dPath);
            gr.DrawPath(outlinePen, dPath);

            // draw the right eye
            rightEyeDimensions = new Rectangle(rightEyeX - eyeRadiusPixels, rightEyeY - eyeRadiusPixels, eyeRadiusPixels + eyeRadiusPixels, eyeRadiusPixels + eyeRadiusPixels);
            Brush rightEyeGradient = new LinearGradientBrush(rightEyeDimensions, Color.FromArgb(250, 250, 250), Color.FromArgb(225, 225, 255), 45, false);
            gPath.Reset();
            gPath.AddEllipse(rightEyeDimensions);
            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
            gr.FillPath(rightEyeGradient, dPath);
            gr.DrawPath(outlinePen, dPath);

            /////PUPILS/////////////

            pupilRadius = pupilDiameter / 2;
            // draw left pupil
            leftPupilControlX = leftEyeX - (int)((eyeRadiusPixels - pupilRadius) * (0.5f - leftHorizontalEye) * 2.0f);
            leftPupilControlY = leftEyeY - (int)((eyeRadiusPixels - pupilRadius) * (0.5f - leftVerticalEye) * 2.0f);
            leftPupilDimensions = new Rectangle(leftPupilControlX - pupilRadius, leftPupilControlY - pupilRadius, pupilDiameter, pupilDiameter);
            // draw right pupil
            rightPupilControlX = rightEyeX - (int)((eyeRadiusPixels - pupilRadius) * (0.5f - rightHorizontalEye) * 2.0f);
            rightPupilControlY = rightEyeY - (int)((eyeRadiusPixels - pupilRadius) * (0.5f - rightVerticalEye) * 2.0f);
            rightPupilDimensions = new Rectangle(rightPupilControlX - pupilRadius, rightPupilControlY - pupilRadius, pupilDiameter, pupilDiameter);

            // rotate control points ... keep Z
            leftHorizontalEyePrime = leftPupilControlX;
            leftVerticalEyePrime = leftPupilControlY;
            rightHorizontalEyePrime = rightPupilControlX;
            rightVerticalEyePrime = rightPupilControlY;
            RotatePoint(ref leftHorizontalEyePrime, ref leftVerticalEyePrime, ref leftPupilZPrime, (int)middleX, (int)middleY, rx, ry, rz, fl);
            RotatePoint(ref rightHorizontalEyePrime, ref rightVerticalEyePrime, ref rightPupilZPrime, (int)middleX, (int)middleY, rx, ry, rz, fl);

            if (!showMotionPoints || mouseDown || isPlaying)
            {
                gPath.Reset();
                gPath.StartFigure();
                gPath.AddEllipse(rightPupilDimensions);
                gPath.CloseFigure();
                gPath.StartFigure();
                gPath.AddEllipse(leftPupilDimensions);
                gPath.CloseFigure();
                dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
                gr.FillPath(pupilBrush, dPath);

                RotateRect(ref leftPupilDimensions, (int)middleX, (int)middleY, rx, ry, rz, fl);
                RotateRect(ref rightPupilDimensions, (int)middleX, (int)middleY, rx, ry, rz, fl);
            }

            /////EYELIDS/////////////

            // draw upper LEFT eyelid
            gPath.Reset();
            int leftEyelidDegree = (int)((Math.Acos(((1.0f - leftEyelid) * (float)eyeRadiusPixels) / (float)eyeRadiusPixels) * 180.0f) / Math.PI);
            gPath.AddArc(leftEyeDimensions, -leftEyelidDegree - 90, leftEyelidDegree + leftEyelidDegree);
            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
            gr.FillPath(eyelidBrush, dPath);

            // draw lower LEFT eyelid
            gPath.Reset();
            gPath.AddArc(leftEyeDimensions, 90 - leftEyelidDegree, leftEyelidDegree + leftEyelidDegree);
            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
            gr.FillPath(eyelidBrush, dPath);

            // draw upper RIGHT eyelid
            gPath.Reset();
            int rightEyelidDegree = (int)((Math.Acos(((1.0f - rightEyelid) * (float)eyeRadiusPixels) / (float)eyeRadiusPixels) * 180.0f) / Math.PI);
            gPath.AddArc(rightEyeDimensions, -rightEyelidDegree - 90, rightEyelidDegree + rightEyelidDegree);
            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
            gr.FillPath(eyelidBrush, dPath);

            // draw lower RIGHT eyelid
            gPath.Reset();
            gPath.AddArc(rightEyeDimensions, 90-rightEyelidDegree, rightEyelidDegree + rightEyelidDegree);
            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
            gr.FillPath(eyelidBrush, dPath);

            leftEyelidControlX = leftEyeX - (controlPointPixels / 2);
            leftEyelidControlY = leftEyeY - eyeRadiusPixels + (int)((float)(eyeRadiusPixels) * (leftEyelid)) - (controlPointPixels / 2);

            rightEyelidControlX = rightEyeX - (controlPointPixels / 2);
            rightEyelidControlY = rightEyeY - eyeRadiusPixels + (int)((float)(eyeRadiusPixels) * (rightEyelid)) - (controlPointPixels / 2);

            if (showMotionPoints && (!mouseDown) && (!isPlaying))
            {
                gPath.Reset();
                gPath.AddEllipse(leftEyelidControlX, leftEyelidControlY, controlPointPixels, controlPointPixels);
                dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
                gr.FillPath(highlightBrush, dPath);

                gPath.Reset();
                gPath.AddEllipse(rightEyelidControlX, rightEyelidControlY, controlPointPixels, controlPointPixels);
                dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
                gr.FillPath(highlightBrush, dPath);

                if (selected == rightEyelidSelected)
                    drawVerticalArrows(gr, rightEyelidControlX + (controlPointPixels / 2), rightEyelidControlY + (controlPointPixels / 2), (int)middleX, (int)middleY);

                if (selected == leftEyelidSelected)
                    drawVerticalArrows(gr, leftEyelidControlX + (controlPointPixels / 2), leftEyelidControlY + (controlPointPixels / 2), (int)middleX, (int)middleY);
            }

            // update control points to rotated view
            RotatePoint(ref leftEyelidControlX, ref leftEyelidControlY, ref leftEyelidControlZ, (int)middleX, (int)middleY, rx, ry, rz, fl);
            RotatePoint(ref rightEyelidControlX, ref rightEyelidControlY, ref rightEyelidControlZ, (int)middleX, (int)middleY, rx, ry, rz, fl);

            /////EYELIDS cont/////////////

            if (showMotionPoints && (!mouseDown) && (!isPlaying))
            {
                // draw the movement arrows
                if (selected == leftPupilSelected)
                    drawPupilArrows(gr, leftPupilDimensions.X, leftPupilDimensions.Y, leftPupilDimensions.Width, leftPupilDimensions.Height, middleX, middleY);

                if (selected == rightPupilSelected)
                    drawPupilArrows(gr, rightPupilDimensions.X, rightPupilDimensions.Y, rightPupilDimensions.Width, rightPupilDimensions.Height, middleX, middleY);

                RotateRect(ref leftPupilDimensions, (int)middleX, (int)middleY, rx, ry, rz, fl);
                RotateRect(ref rightPupilDimensions, (int)middleX, (int)middleY, rx, ry, rz, fl);
                gr.FillEllipse(highlightBrush, leftPupilDimensions);
                gr.FillEllipse(highlightBrush, rightPupilDimensions);
            }

            /////MOUTH/////////////

            int jawOffset = (int)(jaw * MAX_JAW);

            gPath.Reset();
            gPath.AddLines(jawOutline);
            gPath.Transform(smat);

            //gPath.AddEllipse(headDimensions);
            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl, 0, jawOffset);
            gr.FillPath(headGradient, dPath);
            gr.DrawPath(outlinePen, dPath);

            // draw mouth
            int mouthRadiusPixel = (int)(mouthRadius * middleX);
            int mouthCenterXPixel = (int)(middleX + (middleX * mouthCenterX));
            int mouthCenterYPixel = (int)(middleY + (middleY * mouthCenterY));

            leftLipCenterX = (int)(mouthCenterXPixel + (middleX * leftLipX));
            leftLipCenterY = (int)(mouthCenterYPixel + (middleX * leftLipY));

            upperLipCenterX = (int)(mouthCenterXPixel + (middleX * upperLipX));
            upperLipCenterY = (int)(mouthCenterYPixel + (middleX * upperLipY));

            rightLipCenterX = (int)(mouthCenterXPixel + (middleX * rightLipX));
            rightLipCenterY = (int)(mouthCenterYPixel + (middleX * rightLipY));

            jawCenterX = (int)(mouthCenterXPixel + (middleX * jawX));
            jawCenterY = (int)(mouthCenterYPixel + (middleX * jawY) + jawOffset);

            int p1x = 0, p1y = 0;
            int p2x = 0, p2y = 0;
            int p3x = 0, p3y = 0;
            int p4x = 0, p4y = 0;

            RotatePoint(leftLipCenterX + mouthRadiusPixel, leftLipCenterY, ref p1x, ref p1y, (float)(leftLip * Math.PI) + (float)(Math.PI / 2.0f), leftLipCenterX, leftLipCenterY);
            p2x = upperLipCenterX;
            p2y = upperLipCenterY;
            RotatePoint(rightLipCenterX + mouthRadiusPixel, rightLipCenterY, ref p3x, ref p3y, (float)(rightLip * Math.PI) - (float)(Math.PI / 2.0f), rightLipCenterX, rightLipCenterY);
            p4x = jawCenterX;
            p4y = jawCenterY;

            gPath.Reset();
            gPath.StartFigure();
            gPath.AddEllipse(new Rectangle(leftLipCenterX - mouthRadiusPixel, leftLipCenterY - mouthRadiusPixel, mouthRadiusPixel + mouthRadiusPixel, mouthRadiusPixel + mouthRadiusPixel));
            gPath.CloseFigure();
            gPath.StartFigure();
            gPath.AddEllipse(new Rectangle(rightLipCenterX - mouthRadiusPixel, rightLipCenterY - mouthRadiusPixel, mouthRadiusPixel + mouthRadiusPixel, mouthRadiusPixel + mouthRadiusPixel));
            gPath.CloseFigure();

            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
            gr.DrawPath(mouthServoPen, dPath);

            gPath.Reset();
            gPath.StartFigure();
            gPath.AddLine(p1x, p1y, p2x, p2y);
            gPath.CloseFigure();
            gPath.StartFigure();
            gPath.AddLine(p2x, p2y, p3x, p3y);
            gPath.CloseFigure();
            gPath.StartFigure();
            gPath.AddLine(p3x, p3y, p4x, p4y);
            gPath.CloseFigure();
            gPath.StartFigure();
            gPath.AddLine(p4x, p4y, p1x, p1y);
            gPath.CloseFigure();
            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
            gr.DrawPath(mouthPen, dPath);

            leftLipControlX = p1x - controlPointPixels / 2;
            leftLipControlY = p1y - controlPointPixels / 2;
            rightLipControlX = p3x - controlPointPixels / 2;
            rightLipControlY = p3y - controlPointPixels / 2;
            jawControlX = p4x - controlPointPixels / 2;
            jawControlY = p4y - (controlPointPixels / 2);
            upperLipControlY = p2y + controlPointPixels / 2;

            gPath.Reset();
            gPath.StartFigure();
            gPath.AddEllipse(leftLipControlX, leftLipControlY, controlPointPixels, controlPointPixels);
            gPath.CloseFigure();
            gPath.StartFigure();
            gPath.AddEllipse(rightLipControlX, rightLipControlY, controlPointPixels, controlPointPixels);
            gPath.CloseFigure();
            gPath.StartFigure();
            gPath.AddEllipse(jawControlX, jawControlY, controlPointPixels, controlPointPixels);
            gPath.CloseFigure();

            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);

            //draw mouth control points
            if (showMotionPoints && (!mouseDown) && (!isPlaying))
            {
                gr.FillPath(highlightBrush, dPath);

                if (selected == leftLipSelected)
                    drawCurvedArrows(gr, (int)leftLipCenterX - mouthRadiusPixel, (int)leftLipCenterY - mouthRadiusPixel, mouthRadiusPixel + mouthRadiusPixel, mouthRadiusPixel + mouthRadiusPixel, (int)(leftLip * 180.0) + 90, (int)middleX, (int)middleY);

                if (selected == rightLipSelected)
                    drawCurvedArrows(gr, (int)rightLipCenterX - mouthRadiusPixel, (int)rightLipCenterY - mouthRadiusPixel, mouthRadiusPixel + mouthRadiusPixel, mouthRadiusPixel + mouthRadiusPixel, (int)(rightLip * 180.0) - 90, (int)middleX, (int)middleY);

                if (selected == jawSelected)
                    drawVerticalArrows(gr, (int)jawCenterX, (int)jawCenterY, (int)middleX, (int)middleY);
            }
            else
            {
                gr.FillPath(mouthBrush, dPath);
            }

            // update control points to rotated view
            RotatePoint(ref leftLipControlX, ref leftLipControlY, (int)middleX, (int)middleY, rx, ry, rz, fl);
            RotatePoint(ref rightLipControlX, ref rightLipControlY, (int)middleX, (int)middleY, rx, ry, rz, fl);
            RotatePoint(ref jawControlX, ref jawControlY, ref jawControlZ, (int)middleX, (int)middleY, rx, ry, rz, fl);

            /////EYEBROWS/////////////

            // draw the eyebrows
            eyebrowRadiusPixels = (int)(eyebrowRadius * middleX);

            float leftEyebrowAngle = ((1.0f-leftEyebrow) * (eyebrowMaxRotate - eyebrowMinRotate)) + eyebrowMinRotate;
            float rightEyebrowAngle = (rightEyebrow * (eyebrowMaxRotate - eyebrowMinRotate)) + eyebrowMinRotate;

            float leftEyebrowDegrees = (float)((((leftEyebrowAngle - 0.5f) * (float)(Math.PI / 2.0)) * 180.0f) / Math.PI);
            float rightEyebrowDegrees = (float)((((rightEyebrowAngle - 0.5f) * (float)(Math.PI / 2.0)) * 180.0f) / Math.PI);

            // draw LEFT eyebrow
            gPath.Reset();
            Matrix mat = new Matrix();
            mat.Translate(-leftEyeX, -leftEyeY + eyebrowRadiusPixels);
            mat.Rotate(leftEyebrowDegrees, MatrixOrder.Append);
            mat.Translate(leftEyeX, leftEyeY - eyebrowRadiusPixels, MatrixOrder.Append);
            gPath.AddArc(leftEyeX - eyebrowRadiusPixels, leftEyeY - eyebrowRadiusPixels, eyebrowRadiusPixels + eyebrowRadiusPixels, eyebrowRadiusPixels + eyebrowRadiusPixels, -eyebrowArc + 270, eyebrowArc + eyebrowArc);
            gPath.Transform(mat);
            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
            gr.DrawPath(eyebrowPen, dPath);

            // draw RIGHT eyebrow
            gPath.Reset();
            mat.Reset();
            mat.Translate(-rightEyeX, -rightEyeY + eyebrowRadiusPixels);
            mat.Rotate(rightEyebrowDegrees, MatrixOrder.Append);
            mat.Translate(rightEyeX, rightEyeY - eyebrowRadiusPixels, MatrixOrder.Append);
            gPath.AddArc(rightEyeX - eyebrowRadiusPixels, rightEyeY - eyebrowRadiusPixels, eyebrowRadiusPixels + eyebrowRadiusPixels, eyebrowRadiusPixels + eyebrowRadiusPixels, -eyebrowArc + 270, eyebrowArc + eyebrowArc);
            gPath.Transform(mat);
            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
            gr.DrawPath(eyebrowPen, dPath);

            // rotate from center of eyebrow to center face 
            RotatePoint(leftEyeX, leftEyeY - eyebrowRadiusPixels, ref leftEyebrowBaseX, ref leftEyebrowBaseY, (float)(((eyebrowArc) * Math.PI) / 180.0f), leftEyeX, leftEyeY);
            // rotate by the angle of the eyebrow
            RotatePoint(leftEyebrowBaseX, leftEyebrowBaseY, ref leftEyebrowControlX, ref leftEyebrowControlY, (float)(((leftEyebrowDegrees) * Math.PI) / 180.0f), leftEyeX, leftEyeY - eyebrowRadiusPixels);

            // rotate from center of eyebrow to center of face
            RotatePoint(rightEyeX, rightEyeY - eyebrowRadiusPixels, ref rightEyebrowBaseX, ref rightEyebrowBaseY, (float)(((-eyebrowArc) * Math.PI) / 180.0f), rightEyeX, rightEyeY);
            // rotate by the angle of the eyebrow
            RotatePoint(rightEyebrowBaseX, rightEyebrowBaseY, ref rightEyebrowControlX, ref rightEyebrowControlY, (float)(((rightEyebrowDegrees) * Math.PI) / 180.0f), rightEyeX, rightEyeY - eyebrowRadiusPixels);

            if (showMotionPoints && (!mouseDown) && (!isPlaying))
            {
                // draw the LEFT eyebrow control point
                gPath.Reset();
                gPath.AddEllipse(leftEyebrowControlX - (controlPointPixels / 2), leftEyebrowControlY - (controlPointPixels / 2), controlPointPixels, controlPointPixels);
                dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
                gr.FillPath(highlightBrush, dPath);

                // draw the RIGHT eyebrow
                gPath.Reset();
                gPath.AddEllipse(rightEyebrowControlX - (controlPointPixels / 2), rightEyebrowControlY - (controlPointPixels / 2), controlPointPixels, controlPointPixels);
                dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
                gr.FillPath(highlightBrush, dPath);

                if (selected == leftEyebrowSelected)
                    drawVerticalArrows(gr, leftEyebrowControlX, leftEyebrowControlY, (int)middleX, (int)middleY);

                if (selected == rightEyebrowSelected)
                    drawVerticalArrows(gr, rightEyebrowControlX, rightEyebrowControlY, (int)middleX, (int)middleY);
            }

            RotatePoint(ref leftEyebrowControlX, ref leftEyebrowControlY, ref leftEyebrowControlZ, (int)middleX, (int)middleY, rx, ry, rz, fl);
            RotatePoint(ref rightEyebrowControlX, ref rightEyebrowControlY, ref rightEyebrowControlZ, (int)middleX, (int)middleY, rx, ry, rz, fl);

            // draw nose
            int noseXPixel = (int)(noseX * middleX);
            int noseStartYPixel = (int)((noseStartY * middleX) + middleY);
            int noseEndYPixel = (int)(noseEndY * middleX);

            Rectangle noseDimensions = new Rectangle((int)middleX - noseXPixel, noseStartYPixel, noseXPixel + noseXPixel, noseEndYPixel);
            gPath.Reset();
            gPath.AddRectangle(noseDimensions);
            dPath = RotatePoints(ref gPath, (int)middleX, (int)middleY, rx, ry, rz, fl);
            gr.FillPath(noseBrush, dPath);
            gr.DrawPath(noseOutlinePen, dPath);

            /*
                        // draw ear
                        int earXPixel = (int)((earX * middleX) + middleX);
                        int earYPixel = (int)((earY * middleX) + middleY);
                        int earRadiusPixel = (int)(earRadius * middleX);

                        Rectangle earDimensions = new Rectangle(earXPixel - earRadiusPixel, earYPixel - earRadiusPixel, earRadiusPixel + earRadiusPixel, earRadiusPixel + earRadiusPixel);
                        Brush earGradient = new LinearGradientBrush(earDimensions, Color.FromArgb(255, 255, 0), Color.FromArgb(255, 200, 50), 45, false);
                        gr.FillEllipse(earGradient, earDimensions);
                        gr.DrawEllipse(earOutlinePen, earDimensions);
             */
            //Graphics act = CreateGraphics();
            //act.DrawImage(doubleBuffer, new Point(0, 0));
        }

        bool distanceTo(int x, int y, int X, int Y, int limit)
        {
            float dx = x - X;
            float dy = y - Y;
            return (Math.Sqrt((dx * dx) + (dy * dy)) < limit);
        }

        public RobotState GetState()
        {
            RobotState state = new RobotState();

            state.leftHorizontalEye = leftHorizontalEye;
            state.leftVerticalEye = leftVerticalEye;
            state.rightHorizontalEye = rightHorizontalEye;
            state.rightVerticalEye = rightVerticalEye;
            state.leftEyebrow = leftEyebrow;
            state.rightEyebrow = rightEyebrow;
            state.rightEyelid = rightEyelid;
            state.leftEyelid = leftEyelid;
            state.leftLip = leftLip;
            state.rightLip = rightLip;
            state.jaw = jaw;
            state.neckTilt = neckTilt;
            state.neckTwist = neckTwist;

            return state;
        }

        public void SetState(RobotState state)
        {
            leftHorizontalEye = state.leftHorizontalEye;
            leftVerticalEye = state.leftVerticalEye;
            rightHorizontalEye = state.rightHorizontalEye;
            rightVerticalEye = state.rightVerticalEye;
            leftEyebrow = state.leftEyebrow;
            rightEyebrow = state.rightEyebrow;
            rightEyelid = state.rightEyelid;
            leftEyelid = state.leftEyelid;
            leftLip = state.leftLip;
            rightLip = state.rightLip;
            jaw = state.jaw;
            neckTilt = state.neckTilt;
            neckTwist = state.neckTwist;

            Invalidate();
        }

        private void triggerStateChangeCallback()
        {
            if (StateChangeCallback != null)
            {
                StateChangeCallback(this, GetState());
            }
        }

        private void Simulator_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;

            if (mouseDown)
            {
                int ix = x;
                int iy = y;

                if (selected == leftPupilSelected)
                {
                    InvRotatePoint(ref ix, ref iy, leftPupilZPrime, (int)middleX, (int)middleY, rx, ry, rz, fl);

                    // only assign new location if within the eye boundary
                    if (distanceTo(ix, iy, leftEyeX, leftEyeY, eyeRadiusPixels - pupilRadius))
                    {
                        // create inverted x and y for calculations as we may be interacting with a rotated head
                        leftHorizontalEye = ((float)(ix - leftEyeX) / leftEyeDimensions.Width) + 0.5f;
                        leftVerticalEye = ((float)(iy - leftEyeY) / leftEyeDimensions.Height) + 0.5f;

                        if (Control.ModifierKeys == Keys.Shift)
                        {
                            rightHorizontalEye = leftHorizontalEye;
                            rightVerticalEye = leftVerticalEye;
                        }
                    }
                }
                else
                    if (selected == rightPupilSelected)
                    {
                        InvRotatePoint(ref ix, ref iy, rightPupilZPrime, (int)middleX, (int)middleY, rx, ry, rz, fl);

                        // only assign new location if within the eye boundary
                        if (distanceTo(ix, iy, rightEyeX, rightEyeY, eyeRadiusPixels - pupilRadius))
                        {
                            rightHorizontalEye = ((float)(ix - rightEyeX) / rightEyeDimensions.Width) + 0.5f;
                            rightVerticalEye = ((float)(iy - rightEyeY) / rightEyeDimensions.Height) + 0.5f;

                            if (Control.ModifierKeys == Keys.Shift)
                            {
                                leftHorizontalEye = rightHorizontalEye;
                                leftVerticalEye = rightVerticalEye;
                            }
                        }
                    }
                    else
                        if (selected == leftEyebrowSelected)
                        {
                            InvRotatePoint(ref ix, ref iy, leftEyebrowControlZ, (int)middleX, (int)middleY, rx, ry, rz, fl);

                            // determine appropriate angle based on inverse rotation based on Y location
                            float leftEyebrowDegrees = calculateAngle(leftEyeX, leftEyeY - eyebrowRadiusPixels, ix, iy, leftEyebrowBaseX, leftEyebrowBaseY);
                            float leftEyebrowAngle = (leftEyebrowDegrees / (float)(Math.PI / 2.0)) + 0.5f;

                            if (leftEyebrowAngle < eyebrowMinRotate)
                                leftEyebrowAngle = eyebrowMinRotate;
                            if (leftEyebrowAngle > eyebrowMaxRotate)
                                leftEyebrowAngle = eyebrowMaxRotate;

                            leftEyebrow = 1.0f - ((leftEyebrowAngle - eyebrowMinRotate) / (eyebrowMaxRotate - eyebrowMinRotate));

                            if (Control.ModifierKeys == Keys.Shift)
                            {
                                rightEyebrow = leftEyebrow;
                            }
                        }
                        else
                            if (selected == rightEyebrowSelected)
                            {
                                InvRotatePoint(ref ix, ref iy, rightEyebrowControlZ, (int)middleX, (int)middleY, rx, ry, rz, fl);

                                // determine appropriate angle based on inverse rotation based on Y location
                                float rightEyebrowDegrees = calculateAngle(rightEyeX, rightEyeY - eyebrowRadiusPixels, ix, iy, rightEyebrowBaseX, rightEyebrowBaseY);

                                float rightEyebrowAngle = (rightEyebrowDegrees / (float)(Math.PI / 2.0)) + 0.5f;

                                if (rightEyebrowAngle < eyebrowMinRotate)
                                    rightEyebrowAngle = eyebrowMinRotate;
                                if (rightEyebrowAngle > eyebrowMaxRotate)
                                    rightEyebrowAngle = eyebrowMaxRotate;

                                rightEyebrow = (rightEyebrowAngle - eyebrowMinRotate) / (eyebrowMaxRotate - eyebrowMinRotate);

                                if (Control.ModifierKeys == Keys.Shift)
                                {
                                    leftEyebrow = rightEyebrow;
                                }
                            }
                            else
                                if (selected == rightEyelidSelected)
                                {
                                    InvRotatePoint(ref ix, ref iy, rightEyelidControlZ, (int)middleX, (int)middleY, rx, ry, rz, fl);

                                    rightEyelid = (float)(iy - (rightEyeY - eyeRadiusPixels)) / (eyeRadiusPixels);
                                    if (rightEyelid < 0.01f) rightEyelid = 0.01f;
                                    if (rightEyelid > 1.0f) rightEyelid = 1.0f;

                                    if (Control.ModifierKeys == Keys.Shift)
                                    {
                                        leftEyelid = rightEyelid;
                                    }
                                }
                                else
                                    if (selected == leftEyelidSelected)
                                    {
                                        InvRotatePoint(ref ix, ref iy, leftEyelidControlZ, (int)middleX, (int)middleY, rx, ry, rz, fl);

                                        leftEyelid = (float)(iy - (rightEyeY - eyeRadiusPixels)) / (eyeRadiusPixels);
                                        if (leftEyelid < 0.01f) leftEyelid = 0.01f;
                                        if (leftEyelid > 1.0f) leftEyelid = 1.0f;

                                        if (Control.ModifierKeys == Keys.Shift)
                                        {
                                            rightEyelid = leftEyelid;
                                        }
                                    }
                                    else
                                        if (selected == leftLipSelected)
                                        {
                                            leftLip = (float)((Math.Atan2(leftLipCenterY - y, leftLipCenterX - x) / (Math.PI)) + 0.5f);
                                            if (leftLip > 1.0f) leftLip = 1.0f;
                                            if (leftLip < 0.0f) leftLip = 0.0f;
                                            if (Control.ModifierKeys == Keys.Shift)
                                                rightLip = (1.0f - leftLip);
                                        }
                                        else
                                                if (selected == rightLipSelected)
                                                {
                                                    rightLip = (float)((Math.Atan2(y-rightLipCenterY, x-rightLipCenterX) / (Math.PI)) + 0.5f);
                                                    if (rightLip > 1.0f) rightLip = 1.0f;
                                                    if (rightLip < 0.0f) rightLip = 0.0f;
                                                    if (Control.ModifierKeys == Keys.Shift)
                                                        leftLip = (1.0f - rightLip);
                                                }
                                                else
                                                    if (selected == jawSelected)
                                                    {
                                                        InvRotatePoint(ref ix, ref iy, jawControlZ, (int)middleX, (int)middleY, 0.0f, ry, rz, fl);

                                                        int span = iy - (int)upperLipControlY - controlPointPixels / 2;
                                                        if (span < 0) span = 0;
                                                        if (span > MAX_JAW) span = MAX_JAW;
                                                        jaw = ((float)span / MAX_JAW);
                                                    }
                                                    else
                                                        if (selected == neckTiltSelected)
                                                        {
                                                            InvRotatePoint(ref ix, ref iy, neckTiltControlZ, (int)middleX, (int)middleY, 0.0f, ry, rz, fl);

                                                            int span = iy - (int)middleY;
                                                            if (span < -MAX_NECK_Tilt) span = -MAX_NECK_Tilt;
                                                            if (span > MAX_NECK_Tilt) span = MAX_NECK_Tilt;
                                                            neckTilt = ((float)span / ((MAX_NECK_Tilt) * 2)) + 0.5f;
                                                        }
                                                        else
                                                            if (selected == neckTwistSelected)
                                                            {
                                                                InvRotatePoint(ref ix, ref iy, neckTwistControlZ, (int)middleX, (int)middleY, rx, 0.0f, rz, fl);

                                                                int span = ix - (int)middleX;
                                                                if (span < -MAX_NECK_TWIST) span = -MAX_NECK_TWIST;
                                                                if (span > MAX_NECK_TWIST) span = MAX_NECK_TWIST;
                                                                neckTwist = ((float)span / ((MAX_NECK_Tilt + MAX_NECK_Tilt)*2)) + 0.5f;
                                                            }

                this.Invalidate();

                triggerStateChangeCallback();
            }
            else
                if (!isPlaying)
                {
                    if (distanceTo(x, y, rightEyebrowControlX, rightEyebrowControlY, controlPointPixels))
                    {
                        selected = rightEyebrowSelected;
                        this.Invalidate();
                    }
                    else
                        if (distanceTo(x, y, leftEyebrowControlX, leftEyebrowControlY, controlPointPixels))
                        {
                            selected = leftEyebrowSelected;
                            this.Invalidate();
                        }
                        else
                            if (distanceTo(x, y, rightEyelidControlX, rightEyelidControlY, controlPointPixels))
                            {
                                selected = rightEyelidSelected;
                                this.Invalidate();
                            }
                            else
                                if (distanceTo(x, y, leftEyelidControlX, leftEyelidControlY, controlPointPixels))
                                {
                                    selected = leftEyelidSelected;
                                    this.Invalidate();
                                }
                                else
                                    if (distanceTo(x, y, leftLipControlX, leftLipControlY, controlPointPixels))
                                    {
                                        selected = leftLipSelected;
                                        this.Invalidate();
                                    }
                                    else
                                        if (distanceTo(x, y, rightLipControlX, rightLipControlY, controlPointPixels))
                                        {
                                            selected = rightLipSelected;
                                            this.Invalidate();
                                        }
                                        else
                                            if (distanceTo(x, y, jawControlX, jawControlY, controlPointPixels))
                                            {
                                                selected = jawSelected;
                                                this.Invalidate();
                                            }
                                            else
                                                if (distanceTo(x, y, neckTiltControlX, neckTiltControlY, controlPointPixels))
                                                    {
                                                        selected = neckTiltSelected;
                                                        this.Invalidate();
                                                    }
                                                    else
                                                        if (distanceTo(x, y, neckTwistControlX, neckTwistControlY, controlPointPixels))
                                                        {
                                                            selected = neckTwistSelected;
                                                            this.Invalidate();
                                                        }
                                                        else
                                                            if (headDimensions.Contains(x, y))
                                                            {
                                                                if (leftPupilDimensions.Contains(x, y))
                                                                {
                                                                    if (selected != leftPupilSelected)
                                                                    {
                                                                        selected = leftPupilSelected;
                                                                        this.Invalidate();
                                                                    }
                                                                }
                                                                else
                                                                    if (rightPupilDimensions.Contains(x, y))
                                                                    {
                                                                        if (selected != rightPupilSelected)
                                                                        {
                                                                            selected = rightPupilSelected;
                                                                            this.Invalidate();
                                                                        }
                                                                    }
                                                                    else
                                                                        if (selected != 0)
                                                                        {
                                                                            selected = 0;
                                                                            this.Invalidate();
                                                                        }
                                                                        else
                                                                            if (!showMotionPoints)
                                                                            {
                                                                                showMotionPoints = true;
                                                                                this.Invalidate();
                                                                            }
                                                            }
                                                            else
                                                            {
                                                                if (showMotionPoints)
                                                                {
                                                                    this.Invalidate();
                                                                    showMotionPoints = false;
                                                                }
                                                            }
                }
        }

        private void Simulator_Load(object sender, EventArgs e)
        {

        }

        private void Simulator_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            mouseDownX = e.X;
            mouseDownY = e.Y;
        }

        private void Simulator_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        public void SaveState()
        {
            StreamWriter oWrite = null;

            try
            {
                oWrite = File.CreateText("c:\\temp\\state.txt");
/*
                oWrite.WriteLine("ss.leftHorizontalEye = "+leftHorizontalEye+"f;");
                oWrite.WriteLine("ss.leftVerticalEye = "+leftVerticalEye+"f;");
                oWrite.WriteLine("ss.rightHorizontalEye = "+rightHorizontalEye+"f;");
                oWrite.WriteLine("ss.rightVerticalEye = "+rightVerticalEye+"f;");
                oWrite.WriteLine("ss.leftEyebrow = "+leftEyebrow+"f;");
                oWrite.WriteLine("ss.rightEyebrow = "+rightEyebrow+"f;");
                oWrite.WriteLine("ss.rightEyelid = "+rightEyelid+"f;");
                oWrite.WriteLine("ss.leftEyelid = "+leftEyelid+"f;");
                oWrite.WriteLine("ss.leftLip = "+leftLip+"f;");
                oWrite.WriteLine("ss.rightLip = "+rightLip+"f;");
                oWrite.WriteLine("ss.jaw = "+jaw+"f;");
                oWrite.WriteLine("ss.neckTilt = "+neckTilt+"f");
                oWrite.WriteLine("ss.neckTwist = "+neckTwist+"f");
 */ 
                oWrite.WriteLine(leftHorizontalEye + "f/*leftHorizontalEye*/," + leftVerticalEye + "f/*leftVerticalEye*/," + rightHorizontalEye + "f/*rightHorizontalEye*/," + rightVerticalEye + "f/*rightVerticalEye*/," + leftEyebrow + "f/*leftEyebrow*/," + rightEyebrow + "f/*rightEyebrow*/," + rightEyelid + "f/*rightEyelid*/," + leftEyelid + "f/*leftEyelid*/," + leftLip + "f/*leftLip*/," + rightLip + "f/*rightLip*/," + jaw + "f/*jaw*/," + neckTilt + "f/*neckTilt*/," + neckTwist + "f/*neckTwist*/");
                oWrite.Close();
            }
            catch //(Exception ex)
            {
            }
        }

        public void SetLeftEyebrow(float v)
        {
            leftEyebrow = v;
            Invalidate();
        }

        public void SetRightEyebrow(float v)
        {
            rightEyebrow = v;
            Invalidate();
        }

        public void SetEyebrows(float v)
        {
            rightEyebrow = v;
            leftEyebrow = v;
            Invalidate();
        }

        public void SetLeftEye(float x, float y)
        {
            if (x >= 0.0f) leftHorizontalEye = x;
            if (y >= 0.0f) leftVerticalEye = y;

            Invalidate();
        }

        public void SetRightEye(float x, float y)
        {
            if (x >= 0.0f) rightHorizontalEye = x;
            if (y >= 0.0f) rightVerticalEye = y;
            Invalidate();
        }

        public void SetEyesHorizontal(float x)
        {
            rightHorizontalEye = leftHorizontalEye = x;
            Invalidate();
        }

        public void SetLeftEyeHorizontal(float x)
        {
            leftHorizontalEye = x;
            Invalidate();
        }

        public void SetRightEyeHorizontal(float x)
        {
            rightHorizontalEye = x;
            Invalidate();
        }

        public void SetEyesVertical(float y)
        {
            rightVerticalEye = leftVerticalEye = y;
            Invalidate();
        }

        public void SetLeftLip(float v)
        {
            leftLip = 1.0f - v;
            Invalidate();
        }

        public void SetRightLip(float v)
        {
            rightLip = v;
            Invalidate();
        }

        public void SetLips(float v)
        {
            rightLip = 1.0f - v;
            leftLip = 1.0f - v;
            Invalidate();
        }

        public void SetJaw(float v)
        {
            jaw = v;
            Invalidate();
        }

        public void SetNeckTilt(float v)
        {
            neckTilt = v;
            Invalidate();
        }

        public void SetNeckTwist(float v)
        {
            neckTwist = v;
            Invalidate();
        }

        public void SetLeftEyelid(float v)
        {
            leftEyelid = 1.0f - v;
            Invalidate();
        }

        public void SetRightEyelid(float v)
        {
            rightEyelid = v;
            Invalidate();
        }

        public void SetEyelids(float v)
        {
            rightEyelid = v;
            leftEyelid = v;
            Invalidate();
        }

    }
}
