using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DaVIEW
{
    public partial class MainWindow : Window
    {
        private Point origin;
        private Point start;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                var file = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault();
                // MessageBox.Show("File: " + file);
                image.Source = new BitmapImage(new Uri(file));

                TransformGroup group = new TransformGroup();
                ScaleTransform xform = new ScaleTransform();
                group.Children.Add(xform);

                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(tt);

                image.RenderTransform = group;

                MouseRightButtonDown += (_, __) => tt.X = tt.Y = 0;

                image.MouseLeftButtonDown += image_MouseLeftButtonDown;
                image.MouseLeftButtonUp += image_MouseLeftButtonUp;
                image.MouseRightButtonDown += image_MouseLeftButtonDown;
                image.MouseRightButtonUp += image_MouseLeftButtonUp;

                image.MouseMove += image_MouseMove;
                image.MouseWheel += image_MouseWheel;
                image.Stretch = Stretch.Uniform;

                Title = "";
                WindowStyle = System.Windows.WindowStyle.None;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(),"");
            }
        }

        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            image.ReleaseMouseCapture();
        }

        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            if (!image.IsMouseCaptured) return;

            var currentPoint = e.GetPosition(border);
            if (e.LeftButton == MouseButtonState.Pressed)
             {
                 image.ReleaseMouseCapture();
                 DragMove();
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                var tt = (TranslateTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is TranslateTransform);
                Vector v = start - e.GetPosition(border);
                tt.X = origin.X - v.X;
                tt.Y = origin.Y - v.Y;
                //Debug.WriteLine(string.Format("{0}x {1}y", tt.X, tt.Y));
            }
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            image.CaptureMouse();
            var tt = (TranslateTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is TranslateTransform);
            start = e.GetPosition(border);
            origin = new Point(tt.X, tt.Y);
        }

        Point zpos = new Point(0.5, 0.5);

        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            TransformGroup transformGroup = (TransformGroup)image.RenderTransform;
            ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];

            double zoom = e.Delta > 0 ? .2 : -.2;
            if (zoom > 0)
            {
                // dunno why, but it's a Point expressing a % 0-1.
                var p = e.GetPosition(border);
                p.X = p.X / image.ActualWidth;
                p.Y = p.Y / image.ActualHeight;

                image.RenderTransformOrigin = p;
            }
            // Disallow zooming out
            if (transform.ScaleX < 1.2 && zoom < 0) { return; }

            transform.ScaleX += zoom;
            transform.ScaleY += zoom;
        }

        private void Window_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { Environment.Exit(0); }
        }
    }
}
