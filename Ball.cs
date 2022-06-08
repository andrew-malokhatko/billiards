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
using System.Diagnostics;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace billiards
{
    public struct BallState
    {
        public Vector velocity;
        public double x;
        public double y;
        public int number;
    }

    public class Ball
    {
        public Ellipse UiElement = new Ellipse();
        public int size = 40;
        public static int radius = 20;

        public BallState state;

        public Ball(int number, int x, int y, SolidColorBrush brush1)
        {
            this.state.number = number;

            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Color.FromArgb(0, 0, 0, 0);

            UiElement.Fill = brush;
            UiElement.StrokeThickness = 5;
            UiElement.Stroke = brush1;
            UiElement.Width = size;
            UiElement.Height = size;
            setPos(x, y);

            radius = size / 2;
        }

        public Ball() {; }

        //public List<BallState> update(List<Ball> balls)
        public void update(List<Ball> balls)// divide update into 2 functios first will add velo and second will count new velo and situationally move balls a bit
        {
            List<BallState> result = new List<BallState>();

            state.velocity = Vector.Subtract(state.velocity, new Vector(state.velocity.X * 0.02, state.velocity.Y * 0.02));

            state.x += state.velocity.X;
            state.y += state.velocity.Y;
        }

        public static Dictionary<int, Vector> Collide(Ball b1, Ball b2, List<Ball> ballsCopy)
        {
            Vector n = new Vector(b1.state.x - b2.state.x, b1.state.y - b2.state.y);

            Vector veloN1 = Vector.Divide(b1.state.velocity, 
                Math.Sqrt(b1.state.velocity.X * b1.state.velocity.X + b1.state.velocity.Y * b1.state.velocity.Y));

            Vector veloN2 = Vector.Divide(b2.state.velocity,
                Math.Sqrt(b2.state.velocity.X * b2.state.velocity.X + b2.state.velocity.Y * b2.state.velocity.Y));

            Vector veloN = new Vector();
            Ball applyBall = null;

            if (Double.IsNaN(veloN1.Length))
            {
                veloN = veloN2;
                applyBall = b2;
            }
            else
            {
                if (Double.IsNaN(veloN2.Length))
                {
                    veloN = veloN1;
                    applyBall = b1;
                }
                else
                { 
                    veloN = b1.state.velocity.Length >= b2.state.velocity.Length ? veloN1 : veloN2;
                    applyBall = b1.state.velocity.Length >= b2.state.velocity.Length ? b1 : b2;
                }
            }

            while (n.Length < radius * 2)
            {
                applyBall.state.x -= veloN.X / 2; // try to change only copy of the ball
                applyBall.state.y -= veloN.Y / 2;

                Debug.WriteLine("applyBall " + applyBall.state.number);

                n = new Vector(b1.state.x - b2.state.x, b1.state.y - b2.state.y);
            }

            Dictionary<int, Vector> newVelocities = CalculateVelocity(b1, b2);
            return newVelocities;
        }

        public Ball Clone()
        {
            Ball result = new Ball();

            result.UiElement = this.UiElement;
            result.size = this.size;
            result.state = this.state;

            return result;
        }

        public static List<Tuple<Ball, Ball>> allCollisions(List<Ball> balls)
        {
            List<Tuple<Ball, Ball>> result = new List<Tuple<Ball, Ball>>();
            foreach(Ball b1 in balls)
            {
                foreach(Ball b2 in balls)
                {
                    if (b1 == b2) { continue; }

                    if (getDistance(b1.state.x, b1.state.y, b2.state.x, b2.state.y) <= radius * 2)
                    {
                        Tuple<Ball, Ball> t = Tuple.Create(
                            Math.Min(b1.state.number, b2.state.number) == b1.state.number ? b1 : b2,
                            Math.Max(b1.state.number, b2.state.number) == b1.state.number ? b1 : b2);
                        if (result.Contains(t) == false)
                        {
                            result.Add(t);
                        }
                    }
                }
            }
            return result;
        }

        public static Dictionary<int, Vector> CalculateVelocity(Ball b1, Ball b2)
        {
            Dictionary<int, Vector> result = new Dictionary<int, Vector>();

            Vector n = new Vector(b1.state.x - b2.state.x, b1.state.y - b2.state.y);
            Vector un = Vector.Divide(n, Math.Sqrt(n.X * n.X + n.Y * n.Y));

            Vector ut = new Vector(-un.Y, un.X);

            Vector v1 = new Vector(b1.state.velocity.X, b1.state.velocity.Y);
            Vector v2 = new Vector(b2.state.velocity.X, b2.state.velocity.Y);

            double v1n = DotProduct(un, v1);
            double v1t = DotProduct(ut, v1);
            double v2n = DotProduct(un, v2);
            double v2t = DotProduct(ut, v2);

            double finalv1n = v2n;
            double finalv2n = v1n;

            Vector finalv1nvec = Vector.Multiply(un, finalv1n);
            Vector finalv1tvec = Vector.Multiply(ut, v1t);
            Vector finalv2nvec = Vector.Multiply(un, finalv2n);
            Vector finalv2tvec = Vector.Multiply(ut, v2t);

            Vector v1final = Vector.Add(finalv1nvec, finalv1tvec);
            Vector v2final = Vector.Add(finalv2nvec, finalv2tvec);

            result.Add(b1.state.number, v1final);
            result.Add(b2.state.number, v2final);

            return result;
        }

        public static double DotProduct(Vector vector1, Vector vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y;
        }

        public void setPos(double x, double y)
        {
            state.x = x;
            state.y = y;
        }

        public static double getDistance(double x1, double y1, double x2, double y2)
        {
            double x_dist = x1 - x2;
            double y_dist = y1 - y2;

            return Math.Sqrt(Math.Pow(x_dist, 2) + Math.Pow(y_dist, 2));
        }

        public List<Ball> checkCollision(List<Ball> balls)
        {
            List<Ball> result = new List<Ball>();

            foreach (Ball ball in balls)
            {
                if (ball.state.number == this.state.number) {continue;}

                if (getDistance(state.x, state.y, ball.state.x, ball.state.y) <= radius * 2)
                {
                    result.Add(ball);
                }
            }
            return result;
        }

        public List<Wall> isInWall(List<Wall> walls, Ball ball)
        {
            List<Wall> result = new List<Wall>();
            foreach (Wall wall in walls)
            {
                if (wall.x > state.x || wall.y > state.y)
                {
                    if (ball.state.x + ball.size > wall.x && ball.state.x + ball.size < wall.x + wall.width &&
                        ball.state.y + ball.size > wall.y && ball.state.y + ball.size < wall.y + wall.height)
                    {
                        result.Add(wall);
                    }
                }
                else
                {
                    if (ball.state.x > wall.x && ball.state.x < wall.x + wall.width &&
                        ball.state.y > wall.y && ball.state.y < wall.y + wall.height)
                    {
                        result.Add(wall);
                    }
                }
            }
            if (result.Count <= 0) { return null; }
            return result;
        }

        public void bounce(List<Wall> walls)
        {
            Ball ball = this.Clone();
            List<Wall> collidedWalls = isInWall(walls, this);
            while (true)
            {
                bool br = false;
                if (collidedWalls == null) {break;}

                foreach (Wall wall in collidedWalls)
                {
                    ball.state.x -= this.state.velocity.X;
                    List<Wall> w = isInWall(collidedWalls, ball);
                    if (w == null) 
                    { 
                        this.state.velocity.X = -state.velocity.X;
                        br = true;
                    }

                    ball.state.y -= this.state.velocity.Y;
                    List<Wall> w1 = isInWall(collidedWalls, ball);
                    if (w1 == null) 
                    {
                        this.state.velocity.Y = -state.velocity.Y;
                        br = true;
                    }
                }
                if (br)
                {
                    break;
                }
            }
        }
    }
}
