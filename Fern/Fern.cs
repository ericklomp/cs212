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
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace FernNamespace
{
    /*
     * this class draws a fractal fern when the constructor is called.
     * Written as sample C# code for a CS 212 assignment -- October 2011.
     * 
     * Bugs: WPF and shape objects are the wrong tool for the task 
     */
    class Fern
    {
        private static int BERRYMIN = 10;
        private static int TENDRILS = 7;
        private static int TENDRILMIN = 10;
        private static double DELTATHETA = 0.1;
        private static double SEGLENGTH = 3.0;

        /* 
         * $safeprojectname$ constructor erases screen and draws a fern
         * 
         * Size: number of 3-pixel segments of tendrils
         * Redux: how much smaller children clusters are compared to parents
         * Turnbias: how likely to turn right vs. left (0=always left, 0.5 = 50/50, 1.0 = always right)
         * canvas: the canvas that the fern will be drawn on
         */
        public Fern(double size, double redux, double turnbias, Canvas canvas)
        {
            ImageBrush myImageBrush = new ImageBrush();
            myImageBrush.ImageSource = new BitmapImage(new Uri("/Users/Eric/Downloads/Fern-proj/Fern/forest.jpg", UriKind.Relative));
            canvas.Background = myImageBrush;
            canvas.Children.Clear();          // delete old canvas contents
            // draw a new fern at the center of the canvas with given parameters
            branch((int)(canvas.Width / 2), (int)(50 + (canvas.Height/4)), size, redux, turnbias, canvas);
            Flower(canvas, 50, 480);        //Draw flowers on the left of the fractal fern
            Flower(canvas, 200, 480);
            Flower(canvas, 100, 480);        //Draw flowers on the left of the fractal fern
            Flower(canvas, 150, 480);
        }

        /*
       * draws the intital branch that others will branch off of
       */
        private void branch(int x, int y, double size, double redux, double turnbias, Canvas canvas)
        {
            int x2 = x;
            int y2 = y;
            int x3;
            int y3;

            Random rand = new Random();
            while (redux > 0)
            {
                x3 = x2 + rand.Next(45, 90);
                y3 = y2 + rand.Next(45, 90);
                line(x2, y2, x3, y3, 0, 255, 0, 1 + size / 90, canvas);
                if (size > 1.0)
                {
                    leftbranch(x2, y2, 3 * size / 4, redux, turnbias - 1, 1.0, canvas);
                    rightbranch(x2, y2, 3 * size / 4, redux, turnbias - 1, canvas);
                }
                redux--;
                x2 = x3;
                y2 = y3;
            }
        }

        /*
      * draws the left branch using recursion
      */
        private void leftbranch(int x, int y, double size, double redux, double turnbias, double depth, Canvas canvas)
        {
            Random rand = new Random();
            int x2 = x;
            int y2 = y;
            int x3 = x;
            int y3 = y;

            if (depth % 4 == 1)
            {
                x2 = -(rand.Next(45, 90) + x2 - x2) + x2;
                y2 = -(rand.Next(45, 90) + y2 - y2) + y2 + 40;
            }

            line(x2, y2, x3, y3, 0, 125, 0, 1 + size / 80, canvas);

            if(turnbias > 1)
            {
                leftbranch(x2, y2, 3 * size / 4, redux, turnbias - 1, depth + 1.0, canvas);
            }

            
        }

        /*
     * draws the right branch using recursion
     */
        private void rightbranch(int x, int y, double size, double redux, double turnbias, Canvas canvas)
        {
            Random RNG = new Random();
            int x2 = x + RNG.Next(45, 90) + Convert.ToInt32((50 * turnbias));
            int y2 = y - RNG.Next(45, 90) + Convert.ToInt32((50 * turnbias)) + 50;
            line(x, y, x2, y2 , 0, 255, 0, 1 + size / 80, canvas);

            if (turnbias > 1)
            {
                rightbranch(x2, y2, 3 * size / 4, redux, turnbias - 1, canvas);
            }
        }

        //Function to draw a flower on the canvas at a particular starting x and y coordinate point
        private void Flower(Canvas canvas, int start_x, int start_y)
        {
            Random rand = new Random();
            int flower_height = rand.Next(30, 60);        //Second element of randomness, height of the drawn flowers is randomly selected
            line(start_x, start_y, start_x, start_y - flower_height, 44, 143, 58, 3, canvas);       //Draw line for the stem of the flower

            int circle_centerX = start_x;       //Get the center points for the circle to use for drawing the petals
            int circle_centerY = start_y - flower_height;

            Ellipse myEllipse = new Ellipse();      //Create a new ellipse for the top of the flower
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 200, 0, 0);
            myEllipse.Fill = mySolidColorBrush;        //Fill the circle of the flower to be red
            myEllipse.StrokeThickness = 2;      //Set the thickness of the circle's lines
            myEllipse.HorizontalAlignment = HorizontalAlignment.Center;
            myEllipse.VerticalAlignment = VerticalAlignment.Center;
            myEllipse.Width = 20;       //Set the height and the width of the circle of the flower
            myEllipse.Height = 20;
            myEllipse.SetCenter(start_x, start_y - flower_height);
            //Petals(circle_centerX, circle_centerY, canvas, flower_height);      //Call DrawPetals to draw two petals on the flower
            canvas.Children.Add(myEllipse);     //Add the flower to the canvas
        }
    
        /*
         * draw a line segment (x1,y1) to (x2,y2) with given color, thickness on canvas
         */
        private void line(int x1, int y1, int x2, int y2, byte r, byte g, byte b, double thickness, Canvas canvas)
        {
            Line myLine = new Line();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, r, g, b);
            myLine.X1 = x1;
            myLine.Y1 = y1;
            myLine.X2 = x2;
            myLine.Y2 = y2;
            myLine.Stroke = mySolidColorBrush;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.StrokeThickness = thickness;
            canvas.Children.Add(myLine);
        }
    }
}

/*
 * this class is needed to enable us to set the center for an ellipse (not built in?!)
 */
public static class EllipseX
{
    public static void SetCenter(this Ellipse ellipse, double X, double Y)
    {
        Canvas.SetTop(ellipse, Y - ellipse.Height / 2);
        Canvas.SetLeft(ellipse, X - ellipse.Width / 2);
    }
}


