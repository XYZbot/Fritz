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
using System.ComponentModel;

namespace ArcControl
{
    /// <summary>
    /// Interaction logic for Arc.xaml
    /// </summary>
    [System.Drawing.ToolboxBitmap(typeof(Arc))]
    public partial class Arc : UserControl
    {
        protected int m_startAngle = 0;
        protected int m_endAngle = 90;
        protected int m_thickness = 10;

        public int StartAngle
        {
            get { return m_startAngle; }
            set { m_startAngle = value; this.InvalidateVisual(); }
        }

        public int EndAngle
        {
            get { return m_endAngle; }
            set { m_endAngle = value; this.InvalidateVisual(); }
        }

        public int Thickness
        {
            get { return m_thickness; }
            set { m_thickness = value; this.InvalidateVisual(); }
        }

        public Arc()
        {
            InitializeComponent();
        }

        void DrawArc(DrawingContext drawingContext, Brush brush,
            Pen pen, Point start, Point end, Size radius)
        {
            // setup the geometry object
        }

        protected Point getPointAtAngle(int _centerx, int _centery, float _angleInDegrees, int _radius)
        {
            double angle = ((double)_angleInDegrees/180.0)*Math.PI;
            int x =  (int)(_radius*Math.Cos(angle))+_centerx;
            int y =  (int)(_radius*Math.Sin(angle))+_centery;
            return new Point(x,y);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            int radius = (int)this.RenderSize.Width;
            if (this.RenderSize.Height<radius) radius = (int)this.RenderSize.Height;

            radius = (radius - 20) / 2;

            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            geometry.Figures.Add(figure);
            figure.StartPoint = getPointAtAngle((int)(this.RenderSize.Width / 2), (int)(this.RenderSize.Height / 2), m_endAngle, radius);

            // add the arc to the geometry
            figure.Segments.Add(new ArcSegment(getPointAtAngle((int)(this.RenderSize.Width / 2), (int)(this.RenderSize.Height / 2), m_startAngle, radius), new Size(radius, radius), 0, false, SweepDirection.Counterclockwise, true));

            // draw the arc
            drawingContext.DrawGeometry(null, new Pen(Brushes.Black, m_thickness), geometry);
        }
    }
}
