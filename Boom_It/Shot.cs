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
    public class Shot
    {
        private Bitmap bitmap; //התמונה של היריה
        private float x; //המיקום של היריה לפי הציר האנכי
        private float y; // המיקום של היריה לפי הציר האנכי
        private Canvas canvas; //הקנבס עליו מציירים את היריה

        //פעולה בונה המאתחלת את הערכים
        public Shot(float x, float y, Bitmap bitmap,Canvas canvas)
        {
            this.x = x;
            this.y = y;
            this.bitmap = bitmap;
            this.canvas = canvas;
        }
        //פעולות Get & Set
        public float GetY() { return this.y; }
        public float GetX() { return this.x; }
        public void SetY(float y) { this.y = y; }
        public void SetX(float x) { this.x = x; }

        //פעולה המעלה את היריה כלפי מעלה עד שיוצאת מגבולות המסך
        public void Move()
        {           
            if (this.y > 0-bitmap.Height)
                this.y -= 7;
        }
        //פעולה המציירת את היריה על הקנבס
        public void Draw()
        {
            this.canvas.DrawBitmap(bitmap,this.x,this.y,null);
        }
    }
}