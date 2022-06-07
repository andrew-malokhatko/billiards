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
            ball = new Ball(0, 420, 300, Brushes.Black);
            ball1 = new Ball(1, 400, 200, Brushes.Red);
            ball2 = new Ball(2, 440, 200, Brushes.Green);

            balls.Add(ball);
            balls.Add(ball1);
            balls.Add(ball2);
            balls.Add(new Ball(3, 420, 240, Brushes.Blue));
            
            balls.Add(new Ball(4, 380, 160, Brushes.Blue));
            balls.Add(new Ball(5, 420, 160, Brushes.Blue));
            balls.Add(new Ball(6, 460, 160, Brushes.Blue));

            foreach (Ball ball in balls)
            {
                canvas.Children.Add(ball.UiElement);
            }

            updateTimer.Tick += new EventHandler(update);
            updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            updateTimer.Start();
        }

        public void update(object sender, EventArgs e)
        {
            var newVelocities = new Dictionary<int, Vector>();

            foreach (Ball ball in balls)
            {
                ball.update(balls);
            }

            List<Tuple<Ball, Ball>> collidedBalls = Ball.allCollisions(balls);
            List<Ball> ballsCopy = new List<Ball>();

            foreach (Ball ball in balls)
            {
                ballsCopy.Add(ball.Clone());
            }

            Dictionary<int, Vector> allVelocities = new Dictionary<int, Vector>();

            foreach (Tuple<Ball, Ball> collision in collidedBalls)
            {
                Dictionary<int, Vector> dict1 = Ball.Collide(collision.Item1, collision.Item2, ballsCopy);// probabaly i need to do collision between copies
                allVelocities = addDictionaries(allVelocities, dict1);
            }

            foreach (Ball ball in balls)
            {
                foreach (KeyValuePair<int, Vector> pair in allVelocities)
                {
                    if (pair.Key == ball.state.number)
                    {
                        ball.state.velocity = pair.Value;
                    }
                }
                Canvas.SetLeft(ball.UiElement, ball.state.x);
                Canvas.SetTop(ball.UiElement, ball.state.y);
            }

            lbl.Content = (Convert.ToInt32(ball.state.x), Convert.ToInt32(ball.state.y));
        }

        private Dictionary<int, Vector> addDictionaries (Dictionary<int, Vector> dict1, Dictionary<int, Vector> dict2)
        {
            foreach(KeyValuePair<int, Vector> pair in dict1)
            {
                if (dict2.ContainsKey(pair.Key))
                {
                    dict2[pair.Key] = Vector.Add(pair.Value, dict2[pair.Key]);
                }
                else
                {
                    dict2.Add(pair.Key, pair.Value);
                }
            }
            return dict2;
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
            ball.state.velocity = new Vector(XDif / 15, YDif / 15);
        }
    }
}
