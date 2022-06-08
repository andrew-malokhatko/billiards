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
    public class Wall
    {
        public int x;
        public int y;
        public int width;
        public int height;
        public CollisionType type;
        public Rectangle rect = new Rectangle();

        public enum CollisionType
        {
            Vert,
            Horz
        }

        public Wall(int x, int y, int width, int height, CollisionType collisionType)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.type = collisionType;

            rect.Width = width;
            rect.Height = height;
            rect.Fill = Brushes.Black;
        }

    }
}
