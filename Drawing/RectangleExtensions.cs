namespace Memorandum.Drawing
{
    using System;
    using System.Drawing;

    public static class RectangleExtensions
    {
        public static Rectangle Centering(this Rectangle r, Size size)
        {
            return Centering(r, new Rectangle(new Point(0, 0), size));
        }

        public static Rectangle Centering(this Rectangle r, Rectangle rect)
        {
            return new Rectangle(
                ((rect.Width - r.Width) / 2) + rect.X,
                ((rect.Height - r.Height) / 2) + rect.Y,
                r.Width,
                r.Height);
        }

        public static Rectangle Zoom(this Rectangle r, Size size)
        {
            return Zoom(r, new Rectangle(new Point(0, 0), size));
        }

        public static Rectangle Zoom(this Rectangle r, Rectangle rect)
        {
            try
            {
                if ((float)r.Width / r.Height < (float)rect.Width / rect.Height)
                {
                    var width = rect.Height * r.Width / r.Height;
                    return new Rectangle(
                        rect.X + ((rect.Width - width) / 2),
                        rect.Y,
                        width,
                        rect.Height);
                }
                else
                {
                    var height = rect.Width * r.Height / r.Width;
                    return new Rectangle(
                        rect.X,
                        rect.Y + ((rect.Height - height) / 2),
                        rect.Width,
                        height);
                }
            }
            catch (DivideByZeroException)
            {
                return Rectangle.Empty;
            }
        }
    }
}