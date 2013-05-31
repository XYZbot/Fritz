            /*
            Point[] apt = new Point[5];
            apt[0] = new Point(p1x, p1y);
            apt[1] = new Point(p2x, p2y);
            apt[2] = new Point(p3x, p3y);
            apt[3] = new Point(p4x, p4y);
            apt[4] = apt[0];
            gr.DrawPolygon(mouthPen, apt);
            */ 
            //gr.DrawCurve(mouthPen, apt, 0, 4, 0.1f);
/*
            int split = orderPointsByAngle(ref apt);
            int nextSplit = split+1;
            if (nextSplit >= 4) nextSplit = 0;

            Point[] mouthCurve = new Point[6];
            mouthCurve[0] = new Point((apt[split].X+apt[nextSplit].X)/2, (apt[split].Y+apt[nextSplit].Y)/2);
            mouthCurve[1] = apt[nextSplit++];
            if (nextSplit >= 4) nextSplit = 0;
            mouthCurve[2] = apt[nextSplit++];
            if (nextSplit >= 4) nextSplit = 0;
            mouthCurve[3] = apt[nextSplit++];
            if (nextSplit >= 4) nextSplit = 0;
            mouthCurve[4] = apt[nextSplit++];
            mouthCurve[5] = mouthCurve[0];
            gr.DrawCurve(mouthPen, mouthCurve, 0, 5, 0.2f);
*/


        /*
         * http://stackoverflow.com/questions/660304/4-point-transform-images
                public partial class Window1 : Window
                {
                    const float ANGLE = 30;
                    const float WIDTH = 8;
                    public Window1()
                    {
                        InitializeComponent();

                        var group = new Model3DGroup();
                        group.Children.Add(Create3DImage(@"C:\Users\fak\Pictures\so2.png"));
                        group.Children.Add(new AmbientLight(Colors.White));

												Rec

                        ModelVisual3D visual = new ModelVisual3D();
                        visual.Content = group;
                        viewport.Children.Add(visual);
                    }

                    private GeometryModel3D Create3DImage(string imgFilename)
                    {
                        var image = LoadImage(imgFilename);

                        var mesh = new MeshGeometry3D();
                        var height = (WIDTH * image.PixelHeight) / image.PixelWidth;
                        var w2 = WIDTH / 2.0;
                        var h2 = height / 2.0;
                        mesh.Positions.Add(new Point3D(-w2, -h2, 0));
                        mesh.Positions.Add(new Point3D(w2, -h2, 0));
                        mesh.Positions.Add(new Point3D(w2, h2, 0));
                        mesh.Positions.Add(new Point3D(-w2, h2, 0));
                        mesh.TriangleIndices.Add(0);
                        mesh.TriangleIndices.Add(1);
                        mesh.TriangleIndices.Add(2);
                        mesh.TriangleIndices.Add(0);
                        mesh.TriangleIndices.Add(2);
                        mesh.TriangleIndices.Add(3);
                        mesh.TextureCoordinates.Add(new Point(0, 1)); // 0, 0
                        mesh.TextureCoordinates.Add(new Point(1, 1));
                        mesh.TextureCoordinates.Add(new Point(1, 0));
                        mesh.TextureCoordinates.Add(new Point(0, 0));

                        var mat = new DiffuseMaterial(new ImageBrush(image));
                        mat.AmbientColor = Colors.White;

                        var geometry = new GeometryModel3D();
                        geometry.Geometry = mesh;
                        geometry.Material = mat;
                        geometry.BackMaterial = mat;

                        geometry.Transform = new RotateTransform3D(
                            new AxisAngleRotation3D(new Vector3D(0, 1, 0), ANGLE),
                            new Point3D(0, 0, 0));

                        return geometry;
                    }

                    public static BitmapSource LoadImage(string filename)
                    {
                        return BitmapDecoder.Create(new Uri(filename, UriKind.RelativeOrAbsolute),
                            BitmapCreateOptions.None, BitmapCacheOption.Default).Frames[0];
                    }
                }
         * 
         * ******
         http://stackoverflow.com/questions/1270822/how-can-i-do-3d-transformation-in-wpf
         * public partial class Window1 : Window
        {
            public Window1()
            {
                InitializeComponent();
                Init();
            }


            private Timer _timer;
            private ModelVisual3D _model = new ModelVisual3D();
            private double _angle = 0;

            public void Init()
            {
                _model = GetCube(GetSurfaceMaterial(Colors.Red), new Point3D(10, 10, 1), new Point3D(-10,-10,-1));
                mainViewport.Children.Add(_model);    
                _timer = new Timer(10);
                _timer.Elapsed += TimerElapsed;
                _timer.Enabled = true;
            }

            void TimerElapsed(object sender, ElapsedEventArgs e)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double>(Transform), 0.5d);
            }

            public MaterialGroup GetSurfaceMaterial(Color colour)
            {
                var materialGroup = new MaterialGroup();
                var emmMat = new EmissiveMaterial(new SolidColorBrush(colour));
                materialGroup.Children.Add(emmMat);
                materialGroup.Children.Add(new DiffuseMaterial(new SolidColorBrush(colour)));
                var specMat = new SpecularMaterial(new SolidColorBrush(Colors.White), 30);
                materialGroup.Children.Add(specMat);
                return materialGroup;
            }

            public ModelVisual3D GetCube(MaterialGroup materialGroup, Point3D nearPoint, Point3D farPoint)
            {
                var cube = new Model3DGroup();
                var p0 = new Point3D(farPoint.X, farPoint.Y, farPoint.Z);
                var p1 = new Point3D(nearPoint.X, farPoint.Y, farPoint.Z);
                var p2 = new Point3D(nearPoint.X, farPoint.Y, nearPoint.Z);
                var p3 = new Point3D(farPoint.X, farPoint.Y, nearPoint.Z);
                var p4 = new Point3D(farPoint.X, nearPoint.Y, farPoint.Z);
                var p5 = new Point3D(nearPoint.X, nearPoint.Y, farPoint.Z);
                var p6 = new Point3D(nearPoint.X, nearPoint.Y, nearPoint.Z);
                var p7 = new Point3D(farPoint.X, nearPoint.Y, nearPoint.Z);
                //front side triangles
                cube.Children.Add(CreateTriangleModel(materialGroup, p3, p2, p6));
                cube.Children.Add(CreateTriangleModel(materialGroup, p3, p6, p7));
                //right side triangles
                cube.Children.Add(CreateTriangleModel(materialGroup, p2, p1, p5));
                cube.Children.Add(CreateTriangleModel(materialGroup, p2, p5, p6));
                //back side triangles
                cube.Children.Add(CreateTriangleModel(materialGroup, p1, p0, p4));
                cube.Children.Add(CreateTriangleModel(materialGroup, p1, p4, p5));
                //left side triangles
                cube.Children.Add(CreateTriangleModel(materialGroup, p0, p3, p7));
                cube.Children.Add(CreateTriangleModel(materialGroup, p0, p7, p4));
                //top side triangles
                cube.Children.Add(CreateTriangleModel(materialGroup, p7, p6, p5));
                cube.Children.Add(CreateTriangleModel(materialGroup, p7, p5, p4));
                //bottom side triangles
                cube.Children.Add(CreateTriangleModel(materialGroup, p2, p3, p0));
                cube.Children.Add(CreateTriangleModel(materialGroup, p2, p0, p1));
                var model = new ModelVisual3D();
                model.Content = cube;
                return model;
            }

            private Model3DGroup CreateTriangleModel(Material material, Point3D p0, Point3D p1, Point3D p2)
            {
                var mesh = new MeshGeometry3D();
                mesh.Positions.Add(p0);
                mesh.Positions.Add(p1);
                mesh.Positions.Add(p2);
                mesh.TriangleIndices.Add(0);
                mesh.TriangleIndices.Add(1);
                mesh.TriangleIndices.Add(2);
                var normal = CalculateNormal(p0, p1, p2);
                mesh.Normals.Add(normal);
                mesh.Normals.Add(normal);
                mesh.Normals.Add(normal);

                var model = new GeometryModel3D(mesh, material);

                var group = new Model3DGroup();
                group.Children.Add(model);
                return group;
            }

            private Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
            {
                var v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
                var v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
                return Vector3D.CrossProduct(v0, v1);
            }

            void Transform(double adjustBy)
            {
                _angle += adjustBy;


                var rotateTransform3D = new RotateTransform3D {CenterX = 0, CenterZ = 0};
                var axisAngleRotation3D = new AxisAngleRotation3D {Axis = new Vector3D(0, 1, 0), Angle = _angle};
                rotateTransform3D.Rotation = axisAngleRotation3D;
                var myTransform3DGroup = new Transform3DGroup();
                myTransform3DGroup.Children.Add(rotateTransform3D);
                _model.Transform = myTransform3DGroup;

            }


        }
        */

            
        private GeometryModel3D Create3DImage(System.Windows.Media.ImageSource image)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            var w2 = image.Width / 2.0;
            var h2 = image.Height / 2.0;
            mesh.Positions.Add(new Point3D(-w2, -h2, 0));
            mesh.Positions.Add(new Point3D(w2, -h2, 0));
            mesh.Positions.Add(new Point3D(w2, h2, 0));
            mesh.Positions.Add(new Point3D(-w2, h2, 0));
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TextureCoordinates.Add(new System.Windows.Point(0, 1)); // 0, 0
            mesh.TextureCoordinates.Add(new System.Windows.Point(1, 1));
            mesh.TextureCoordinates.Add(new System.Windows.Point(1, 0));
            mesh.TextureCoordinates.Add(new System.Windows.Point(0, 0));

            var mat = new DiffuseMaterial(new System.Windows.Media.ImageBrush(image));
            mat.AmbientColor = System.Windows.Media.Color.FromRgb(255,255,255);

            var geometry = new GeometryModel3D();
            geometry.Geometry = mesh;
            geometry.Material = mat;
            geometry.BackMaterial = mat;

            double ANGLE = 0.1;

            geometry.Transform = new RotateTransform3D(
                new AxisAngleRotation3D(new Vector3D(0, 1, 0), ANGLE),
                new Point3D(0, 0, 0));

            return geometry;
        }


        public static BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private GeometryModel3D Create3DImage(System.Windows.Media.ImageSource image)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            var w2 = image.Width / 2.0;
            var h2 = image.Height / 2.0;
            mesh.Positions.Add(new Point3D(-w2, -h2, 0));
            mesh.Positions.Add(new Point3D(w2, -h2, 0));
            mesh.Positions.Add(new Point3D(w2, h2, 0));
            mesh.Positions.Add(new Point3D(-w2, h2, 0));
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TextureCoordinates.Add(new System.Windows.Point(0, 1)); // 0, 0
            mesh.TextureCoordinates.Add(new System.Windows.Point(1, 1));
            mesh.TextureCoordinates.Add(new System.Windows.Point(1, 0));
            mesh.TextureCoordinates.Add(new System.Windows.Point(0, 0));

            var mat = new DiffuseMaterial(new System.Windows.Media.ImageBrush(image));
            mat.AmbientColor = System.Windows.Media.Color.FromRgb(255,255,255);

            var geometry = new GeometryModel3D();
            geometry.Geometry = mesh;
            geometry.Material = mat;
            geometry.BackMaterial = mat;

            double ANGLE = 0.1;

            geometry.Transform = new RotateTransform3D(
                new AxisAngleRotation3D(new Vector3D(0, 1, 0), ANGLE),
                new Point3D(0, 0, 0));

            return geometry;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // Create the ElementHost control for hosting the
            // WPF UserControl.
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;

            // Create the WPF UserControl.
            Simulator sim = new Simulator();

            // Assign the WPF UserControl to the ElementHost control's
            // Child property.
            host.Child = sim;

            // Add the ElementHost control to the form's
            // collection of child controls.
            this.Controls.Add(host);

        }


            var group = new Model3DGroup();
            group.Children.Add(Create3DImage(CreateBitmapSourceFromBitmap(doubleBuffer)));
            group.Children.Add(new AmbientLight(System.Windows.Media.Color.FromRgb(255,255,255)));

            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = group;
            viewport.Children.Add(visual);


        /*
         * http://stackoverflow.com/questions/660304/4-point-transform-images
                public partial class Window1 : Window
                {
                    const float ANGLE = 30;
                    const float WIDTH = 8;
                    public Window1()
                    {
                        InitializeComponent();

                        var group = new Model3DGroup();
                        group.Children.Add(Create3DImage(@"C:\Users\fak\Pictures\so2.png"));
                        group.Children.Add(new AmbientLight(Colors.White));

                        ModelVisual3D visual = new ModelVisual3D();
                        visual.Content = group;
                        viewport.Children.Add(visual);
                    }

                    private GeometryModel3D Create3DImage(string imgFilename)
                    {
                        var image = LoadImage(imgFilename);

                        var mesh = new MeshGeometry3D();
                        var height = (WIDTH * image.PixelHeight) / image.PixelWidth;
                        var w2 = WIDTH / 2.0;
                        var h2 = height / 2.0;
                        mesh.Positions.Add(new Point3D(-w2, -h2, 0));
                        mesh.Positions.Add(new Point3D(w2, -h2, 0));
                        mesh.Positions.Add(new Point3D(w2, h2, 0));
                        mesh.Positions.Add(new Point3D(-w2, h2, 0));
                        mesh.TriangleIndices.Add(0);
                        mesh.TriangleIndices.Add(1);
                        mesh.TriangleIndices.Add(2);
                        mesh.TriangleIndices.Add(0);
                        mesh.TriangleIndices.Add(2);
                        mesh.TriangleIndices.Add(3);
                        mesh.TextureCoordinates.Add(new Point(0, 1)); // 0, 0
                        mesh.TextureCoordinates.Add(new Point(1, 1));
                        mesh.TextureCoordinates.Add(new Point(1, 0));
                        mesh.TextureCoordinates.Add(new Point(0, 0));

                        var mat = new DiffuseMaterial(new ImageBrush(image));
                        mat.AmbientColor = Colors.White;

                        var geometry = new GeometryModel3D();
                        geometry.Geometry = mesh;
                        geometry.Material = mat;
                        geometry.BackMaterial = mat;

                        geometry.Transform = new RotateTransform3D(
                            new AxisAngleRotation3D(new Vector3D(0, 1, 0), ANGLE),
                            new Point3D(0, 0, 0));

                        return geometry;
                    }

                    public static BitmapSource LoadImage(string filename)
                    {
                        return BitmapDecoder.Create(new Uri(filename, UriKind.RelativeOrAbsolute),
                            BitmapCreateOptions.None, BitmapCacheOption.Default).Frames[0];
                    }
                }
         * 
         * ******
         http://stackoverflow.com/questions/1270822/how-can-i-do-3d-transformation-in-wpf
         * public partial class Window1 : Window
        {
            public Window1()
            {
                InitializeComponent();
                Init();
            }


            private Timer _timer;
            private ModelVisual3D _model = new ModelVisual3D();
            private double _angle = 0;

            public void Init()
            {
                _model = GetCube(GetSurfaceMaterial(Colors.Red), new Point3D(10, 10, 1), new Point3D(-10,-10,-1));
                mainViewport.Children.Add(_model);    
                _timer = new Timer(10);
                _timer.Elapsed += TimerElapsed;
                _timer.Enabled = true;
            }

            void TimerElapsed(object sender, ElapsedEventArgs e)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double>(Transform), 0.5d);
            }

            public MaterialGroup GetSurfaceMaterial(Color colour)
            {
                var materialGroup = new MaterialGroup();
                var emmMat = new EmissiveMaterial(new SolidColorBrush(colour));
                materialGroup.Children.Add(emmMat);
                materialGroup.Children.Add(new DiffuseMaterial(new SolidColorBrush(colour)));
                var specMat = new SpecularMaterial(new SolidColorBrush(Colors.White), 30);
                materialGroup.Children.Add(specMat);
                return materialGroup;
            }

            public ModelVisual3D GetCube(MaterialGroup materialGroup, Point3D nearPoint, Point3D farPoint)
            {
                var cube = new Model3DGroup();
                var p0 = new Point3D(farPoint.X, farPoint.Y, farPoint.Z);
                var p1 = new Point3D(nearPoint.X, farPoint.Y, farPoint.Z);
                var p2 = new Point3D(nearPoint.X, farPoint.Y, nearPoint.Z);
                var p3 = new Point3D(farPoint.X, farPoint.Y, nearPoint.Z);
                var p4 = new Point3D(farPoint.X, nearPoint.Y, farPoint.Z);
                var p5 = new Point3D(nearPoint.X, nearPoint.Y, farPoint.Z);
                var p6 = new Point3D(nearPoint.X, nearPoint.Y, nearPoint.Z);
                var p7 = new Point3D(farPoint.X, nearPoint.Y, nearPoint.Z);
                //front side triangles
                cube.Children.Add(CreateTriangleModel(materialGroup, p3, p2, p6));
                cube.Children.Add(CreateTriangleModel(materialGroup, p3, p6, p7));
                //right side triangles
                cube.Children.Add(CreateTriangleModel(materialGroup, p2, p1, p5));
                cube.Children.Add(CreateTriangleModel(materialGroup, p2, p5, p6));
                //back side triangles
                cube.Children.Add(CreateTriangleModel(materialGroup, p1, p0, p4));
                cube.Children.Add(CreateTriangleModel(materialGroup, p1, p4, p5));
                //left side triangles
                cube.Children.Add(CreateTriangleModel(materialGroup, p0, p3, p7));
                cube.Children.Add(CreateTriangleModel(materialGroup, p0, p7, p4));
                //top side triangles
                cube.Children.Add(CreateTriangleModel(materialGroup, p7, p6, p5));
                cube.Children.Add(CreateTriangleModel(materialGroup, p7, p5, p4));
                //bottom side triangles
                cube.Children.Add(CreateTriangleModel(materialGroup, p2, p3, p0));
                cube.Children.Add(CreateTriangleModel(materialGroup, p2, p0, p1));
                var model = new ModelVisual3D();
                model.Content = cube;
                return model;
            }

            private Model3DGroup CreateTriangleModel(Material material, Point3D p0, Point3D p1, Point3D p2)
            {
                var mesh = new MeshGeometry3D();
                mesh.Positions.Add(p0);
                mesh.Positions.Add(p1);
                mesh.Positions.Add(p2);
                mesh.TriangleIndices.Add(0);
                mesh.TriangleIndices.Add(1);
                mesh.TriangleIndices.Add(2);
                var normal = CalculateNormal(p0, p1, p2);
                mesh.Normals.Add(normal);
                mesh.Normals.Add(normal);
                mesh.Normals.Add(normal);

                var model = new GeometryModel3D(mesh, material);

                var group = new Model3DGroup();
                group.Children.Add(model);
                return group;
            }

            private Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
            {
                var v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
                var v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
                return Vector3D.CrossProduct(v0, v1);
            }

            void Transform(double adjustBy)
            {
                _angle += adjustBy;


                var rotateTransform3D = new RotateTransform3D {CenterX = 0, CenterZ = 0};
                var axisAngleRotation3D = new AxisAngleRotation3D {Axis = new Vector3D(0, 1, 0), Angle = _angle};
                rotateTransform3D.Rotation = axisAngleRotation3D;
                var myTransform3DGroup = new Transform3DGroup();
                myTransform3DGroup.Children.Add(rotateTransform3D);
                _model.Transform = myTransform3DGroup;
            }
        }
        */
