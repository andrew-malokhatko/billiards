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
        List<Ball> balls = new List<Ball>();
        List<Wall> walls = new List<Wall>();

        public MainWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);

            int thikness = 40;
            int topOffset = 200;
            int leftOffset = 250;
            int tableWidth = 800;
            int tableHeight = 400;
            int sizeOf = 500;
            int lunka = 70;

            Wall wall3 = new Wall(leftOffset + lunka + sizeOf * 2 + lunka, topOffset + lunka, thikness, sizeOf + thikness - 2 * lunka, Wall.CollisionType.Vert); //right wall

            Wall wall4 = new Wall(leftOffset - thikness - lunka, topOffset + lunka, thikness, sizeOf + thikness - 2 * lunka, Wall.CollisionType.Vert); //left wall

            Wall wall6 = new Wall(leftOffset, topOffset + sizeOf, sizeOf, thikness, Wall.CollisionType.Horz); //bottom wall 1
            Wall wall5 = new Wall(leftOffset + sizeOf + lunka, topOffset + sizeOf, sizeOf, thikness, Wall.CollisionType.Horz); //bottom wall 2

            Wall wall1 = new Wall(leftOffset, topOffset, sizeOf, thikness, Wall.CollisionType.Horz); //top wall 1
            Wall wall2 = new Wall(leftOffset + sizeOf + lunka, topOffset, sizeOf, thikness, Wall.CollisionType.Horz); //top wall 2


            ball = new Ball(0, 300, topOffset + sizeOf/2, Brushes.Black);
            balls.Add(ball);

            balls.Add(new Ball(1, 800, topOffset + sizeOf / 2, Brushes.Yellow));

            balls.Add(new Ball(2, 840, topOffset + 230, Brushes.Blue));
            balls.Add(new Ball(3, 840, topOffset + 270, Brushes.Blue));

            balls.Add(new Ball(4, 880, topOffset + sizeOf / 2, Brushes.Red));
            balls.Add(new Ball(5, 880, topOffset + sizeOf / 2 - 40, Brushes.Red));
            balls.Add(new Ball(6, 880, topOffset + sizeOf / 2 + 40, Brushes.Red));

            balls.Add(new Ball(7, 920, topOffset + sizeOf / 2 - 60, Brushes.Green));
            balls.Add(new Ball(8, 920, topOffset + sizeOf / 2 - 20, Brushes.Green));
            balls.Add(new Ball(9, 920, topOffset + sizeOf / 2 + 20, Brushes.Green));
            balls.Add(new Ball(10, 920, topOffset + sizeOf / 2 + 60, Brushes.Green));

            foreach (Ball ball in balls)
            {
                canvas.Children.Add(ball.UiElement);
            }



            walls.Add(wall1);
            walls.Add(wall2);
            walls.Add(wall3);
            walls.Add(wall4);
            walls.Add(wall5);
            walls.Add(wall6);

            foreach (Wall wall in walls)
            {
                canvas.Children.Add(wall.rect);
                Canvas.SetTop(wall.rect, wall.y);
                Canvas.SetLeft(wall.rect, wall.x);
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
                ball.bounce(walls);
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
                    dict2[pair.Key] = Vector.Add(Vector.Divide(pair.Value, 2), Vector.Divide(dict2[pair.Key], 2));
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
