using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

//using System.Drawing;
//using System.Drawing.Printing;
//using System.Drawing.Imaging;
//using System.Drawing.Drawing2D;
using System.Windows.Media.Media3D;

namespace Animatronic_Head
{
    /// <summary>
    /// Interaction logic for Simulator.xaml
    /// </summary>
    public partial class Simulator : UserControl
    {
        int mouth1Value = 50;
        int mouth2Value = 128;
        int mouth3Value = 200;
        int mouth4Value = 128;

        Pen mouthPen = new Pen(new SolidColorBrush(Colors.Black), 10.0f);
        Polygon p = new Polygon();

        public Simulator()
        {
            InitializeComponent();

            //visual.Content = createGroup();
/*
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection
            {
                new Point3D(-1, 1, 0),
                new Point3D(1, 1, 0),
                new Point3D(1, -1, 0),
                new Point3D(-1, -1, 0)
            };

            mesh.TriangleIndices = new Int32Collection
            {
                2, 1, 0,
                2, 0, 3
            };

            mesh.TextureCoordinates = new System.Windows.Media.PointCollection
            {
                new Point(0,0),
                new Point(1,0),
                new Point(1,1),
                new Point(0,1)
            };

            GeometryModel3D model = new GeometryModel3D();
            model.Geometry = mesh;
            model.Material = new DiffuseMaterial(new SolidColorBrush(Colors.Red));

            ModelVisual3D modelVisual = new ModelVisual3D();
            modelVisual.Content = model;

            viewport = new Viewport3D();
            viewport.HorizontalAlignment = HorizontalAlignment.Stretch;
            viewport.VerticalAlignment = VerticalAlignment.Stretch;

            viewport.Camera = new PerspectiveCamera(new Point3D(0, 0, 10),
                                                    new Vector3D(0, 0, -1),
                                                    new Vector3D(0, 1, 0),
                                                    45);
            viewport.Children.Add(modelVisual);
 */
 /*
            int p1x = 0, p1y = 0;
            int p2x = 0, p2y = 0;
            int p3x = 0, p3y = 0;
            int p4x = 0, p4y = 0;

            int mouth1Radius = (int)mouth1.Width;
            Rect mouth1Bounds = mouth1.RenderedGeometry.Bounds;
            int mouth1X = (int)((mouth1Bounds.X + mouth1Bounds.Width) / 2);
            int mouth1Y = (int)((mouth1Bounds.Y + mouth1Bounds.Height) / 2);

            int mouth2Radius = (int)mouth2.Width;
            Rect mouth2Bounds = mouth2.RenderedGeometry.Bounds;
            int mouth2X = (int)((mouth2Bounds.X + mouth2Bounds.Width) / 2);
            int mouth2Y = (int)((mouth2Bounds.Y + mouth2Bounds.Height) / 2);

            int mouth3Radius = (int)mouth3.Width;
            Rect mouth3Bounds = mouth3.RenderedGeometry.Bounds;
            int mouth3X = (int)((mouth3Bounds.X + mouth3Bounds.Width) / 3);
            int mouth3Y = (int)((mouth3Bounds.Y + mouth3Bounds.Height) / 3);

            int mouth4Radius = (int)mouth4.Width;
            Rect mouth4Bounds = mouth4.RenderedGeometry.Bounds;
            int mouth4X = (int)((mouth4Bounds.X + mouth4Bounds.Width) / 4);
            int mouth4Y = (int)((mouth4Bounds.Y + mouth4Bounds.Height) / 4);

            RotatePoint(mouth1X + mouth1Radius, mouth1Y, ref p1x, ref p1y, (float)((mouth1Value * Math.PI) / 128.0f), mouth1X, mouth1Y);
            RotatePoint(mouth2X + mouth2Radius, mouth2Y, ref p2x, ref p2y, (float)((mouth2Value * Math.PI) / 228.0f), mouth2X, mouth2Y);
            RotatePoint(mouth3X + mouth3Radius, mouth3Y, ref p3x, ref p3y, (float)((mouth3Value * Math.PI) / 338.0f), mouth3X, mouth3Y);
            RotatePoint(mouth4X + mouth4Radius, mouth4Y, ref p4x, ref p4y, (float)((mouth4Value * Math.PI) / 448.0f), mouth4X, mouth4Y);

            Polygon poly = new Polygon();
            PointCollection polyPoints = new PointCollection();
            polyPoints.Add(new Point(p1x, p1y));
            polyPoints.Add(new Point(p2x, p2y));
            polyPoints.Add(new Point(p3x, p3y));
            polyPoints.Add(new Point(p4x, p4y));
            poly.Points = polyPoints;

            p.Fill = new SolidColorBrush(Colors.Purple);
            p.Fill.Opacity = 0.3;
            this.LayoutRoot.Children.Add(p);

            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            geometry.Figures.Add(figure);

            
            //figure..Segments.Add(poly);
            Visual3D vis = new Visual3D();
            vis.
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = poly;
            viewport.Children.Add(poly);
            // draw the arc
            drawingContext.DrawGeometry(null, mouthPen, geometry);
            gr.DrawEllipse(mouthServoPen, new Rectangle(upperLeftMouthX - mouthRadiusPixel, upperLeftMouthY - mouthRadiusPixel, mouthRadiusPixel + mouthRadiusPixel, mouthRadiusPixel + mouthRadiusPixel));
            gr.DrawEllipse(mouthServoPen, new Rectangle(upperRightMouthX - mouthRadiusPixel, upperRightMouthY - mouthRadiusPixel, mouthRadiusPixel + mouthRadiusPixel, mouthRadiusPixel + mouthRadiusPixel));
            gr.DrawEllipse(mouthServoPen, new Rectangle(lowerLeftMouthX - mouthRadiusPixel, lowerLeftMouthY - mouthRadiusPixel, mouthRadiusPixel + mouthRadiusPixel, mouthRadiusPixel + mouthRadiusPixel));
            gr.DrawEllipse(mouthServoPen, new Rectangle(lowerRightMouthX - mouthRadiusPixel, lowerRightMouthY - mouthRadiusPixel, mouthRadiusPixel + mouthRadiusPixel, mouthRadiusPixel + mouthRadiusPixel));
*/
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

        protected override void OnRender(DrawingContext drawingContext)
        {
        }
    }
}
