using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace billiards
{
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer updateTimer = new System.Windows.Threading.DispatcherTimer();

        double mouseX = 0;
        double mouseY = 0;
        Ball ball;
        Ball ball1;
        Ball ball2;
        List<Ball> balls = new List<Ball>();


        public MainWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);
            ball = new Ball(canvas, 420, 400, Brushes.Black);
            ball1 = new Ball(canvas, 400, 200, Brushes.Red);
            ball2 = new Ball(canvas, 440, 200, Brushes.Green);

            balls.Add(ball1);
            balls.Add(ball);
            balls.Add(ball2);

            updateTimer.Tick += new EventHandler(update);
            updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            updateTimer.Start();
        }

        public void update(object sender, EventArgs e)
        {
            ball.update(balls, true);
            ball1.update(balls, false);
            ball2.update(balls, false);
            lbl.Content = (Convert.ToInt32(ball.X), Convert.ToInt32(ball.Y));
        }


        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mouseDownPoint = e.GetPosition(canvas);
            mouseX = mouseDownPoint.X;
            mouseY = mouseDownPoint.Y;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point mouseUpPoint = e.GetPosition(canvas);
            double XDif = (mouseX - mouseUpPoint.X);
            double YDif = (mouseY - mouseUpPoint.Y);
            ball.velocity = new Vector(XDif / 15, YDif / 15);
        }
    }
}
