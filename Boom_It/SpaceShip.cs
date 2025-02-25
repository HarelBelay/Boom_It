using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boom_It
{
    //מחלקה של החללית 
    public class SpaceShip
    {
        private float X; //המיקום של החללית לפי הציר האנכי
        private float Y; //המיקום של החללית לפי הציר האופקי
        private Bitmap bitmap; //התמונה של החללית
        private Canvas canvas; //הקנבס שעליו מציירים את החללית

        //פעולה בונה
        public SpaceShip(float x, Bitmap bitmap, Canvas canvas)
        {
            this.X = x;
            this.Y = canvas.Height - bitmap.Height;
            this.bitmap = bitmap;
            this.canvas = canvas;
        }

        //פעולה המציירת את החללית על הקנבס
        public void Draw()
        {
            this.canvas.DrawBitmap(bitmap, this.X, this.Y, null);
        }

        //פעולות Get&Set
        public float GetY() { return this.Y; }
        public float GetX() { return this.X; }
        public void SetX(float x) { this.X = x; }
        public Canvas GetCanvas() { return this.canvas; }
    }
}