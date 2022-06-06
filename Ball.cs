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
    public class Ball
    {
        public Ellipse UiElement = new Ellipse();
        public Vector velocity = new Vector(0, 0);
        public int size = 40;
        public int radius;
        public double X;
        public double Y;
        public int number = 0;

        public Ball(int x, int y, SolidColorBrush brush1, int number)
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Color.FromArgb(0, 0, 0, 0);

            UiElement.Fill = brush;
            UiElement.StrokeThickness = 5;
            UiElement.Stroke = brush1;
            this.number = number;
            UiElement.Width = size;
            UiElement.Height = size;
            setPos(x, y);

            radius = size / 2;

            Canvas.SetLeft(UiElement, X);
            Canvas.SetTop(UiElement, Y);
        }

        public Ball()
        {
            ;
        }

        public Ball Clone()
        {
            Ball result = new Ball();

            result.UiElement = this.UiElement;
            result.velocity = this.velocity;
            result.size = this.size;
            result.radius = this.radius;
            result.X = this.X;
            result.Y = this.Y;
            result.number = this.number;

            return result;
        }

        public void update(List<Ball> balls, bool main) //i can do smth that will check will ball collide on the next move and if yes dont let it to pass another balls border  
        {
            if (velocity.X == 0 && velocity.Y == 0)
            {
                return;
            }

            velocity = Vector.Subtract(velocity, new Vector(velocity.X * 0.02, velocity.Y * 0.02));

            X += velocity.X;
            Y += velocity.Y;

            if (Double.IsNaN(X) || Double.IsNaN(Y))
            {
                ;
            }

            Vector oldVelo = velocity;
            List<Vector> velocities = new List<Vector>();
            List<Ball> collidedBalls = checkCollision(balls);
            foreach(Ball ball in collidedBalls)
            {
                Vector n = new Vector(X - ball.X, Y - ball.Y);
                Vector veloN = Vector.Divide(velocity, Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y));
                while (n.Length < radius * 2)
                {
                    X -= veloN.X;
                    Y -= veloN.Y;
                    n = new Vector(X - ball.X, Y - ball.Y);
                    if (Double.IsNaN(X) || Double.IsNaN(Y))
                    {
                        ;
                    }
                }
            }

            Canvas.SetLeft(UiElement, X);
            Canvas.SetTop(UiElement, Y);
        }

        public Vector countVelocities(List<Vector> velocities)
        {
            Vector finalVelo = new Vector(0,0);
            for(int i = 0; i < velocities.Count(); i++)
            {
                finalVelo = Vector.Add(finalVelo, velocities[i]);
            }
            return finalVelo;
        }

        public void onCollision(Ball ball, Vector oldVelo)
        {

            Vector n = new Vector(oldVelo.X - ball.X, oldVelo.Y - ball.Y);
            Vector un = Vector.Divide(n, Math.Sqrt(n.X * n.X + n.Y * n.Y));
            Vector ut = new Vector(-un.Y, un.X);
            
            Vector v1 = new Vector(velocity.X, velocity.Y);
            Vector v2 = new Vector(ball.velocity.X, ball.velocity.Y);

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

            if (Double.IsNaN(v1final.X) || Double.IsNaN(v1final.Y))
            {
                ;
            }
            ball.velocity = v2final;
            this.velocity =  v1final;
        }

        public double DotProduct(Vector vector1, Vector vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y;
        }

        public void setPos(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double getDistance(double x1, double y1, double x2, double y2)
        {
            double x_dist = x1 - x2;
            double y_dist = y1 - y2;

            return Math.Sqrt(Math.Pow(x_dist, 2) + Math.Pow(y_dist, 2));
        }

        public List<Ball> checkCollision(List<Ball> balls)
        {
            List<Ball> result = new List<Ball>();
            foreach(Ball ball in balls)
            {
                if (ball.number == this.number) {continue;}

                if (getDistance(X, Y, ball.X, ball.Y) <= radius * 2)
                {
                    result.Add(ball);
                }
            }
            return result;
        }
    }
}
