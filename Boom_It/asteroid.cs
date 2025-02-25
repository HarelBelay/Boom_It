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
    //מחלקה של אסטרואיד שנע כלפי מטה אך ורק בציר האנכי וממנו יורשים עוד אסטרואידים
    public class asteroid
    {
        private float y; //המיקום של האסטרואיד לפי הציר האופקי
        private float x; // המיקום של האסטרואיד לפי הציר האנכי
        private float deltaY; //מהירות הירידה של האסטרואיד
        private Bitmap bitmap; //התמונה של האסטרואיד
        private Canvas canvas; //הקנבס עליו מציירים את האסטרואיד
        private int life; //החיים של האסטרואיד

        //פעולה בונה ואתחול של המשתנים
        public asteroid(float y,float deltaY,float x ,Bitmap bitmap, Canvas canvas,int life)
        {
            this.y = y;            
            this.deltaY = deltaY;
            this.bitmap = bitmap;    
            this.canvas = canvas;
            this.life = life;
            this.x = x;
        }
        //פעולה המציירת את האסטרואיד על הקנבס
        public virtual void Draw()
        {
            canvas.DrawBitmap(bitmap, this.x,this.y,null);
        }
        

        //פעולה הגורמת לאסטרואיד לנוע שהמחלקה היורשת גם משתמשת בה
        public virtual void Move()
        {
            this.y+=this.deltaY; //קידום המיקום של האסטרואיד לפי הציר האנכי
            //אם האסטרואיד יוצא מהגבולות של המסך הוא יחזור מלמעלה
            if(this.y > this.canvas.Height)
            {
                this.y = -1 * this.bitmap.Height;
            }              
        }
        //פעולה המחזירה אמת אם היריה פגעה באסטרואיד ושקר אחרת
        public virtual bool GotShot(float x, float y)
        {
            if (y <= this.y + this.bitmap.Height && y >= this.y && x >= this.x && x <= this.x + this.bitmap.Width) //בדיקה של שיעורי האיקס והווי של שני העצמים
                return true;
            return false;
        }
        //פעולה המחזירה אמת אם האסטרואיד פגע בחללית
        public virtual bool Crash(float x, float y,Bitmap bitmap)
        {
            if(this.y+200 >= y)
            {
                if (Math.Abs((this.x + (this.bitmap.Width / 2)) - (x + (bitmap.Width / 2))) <= 130) //בדיקה של שיעורי האיקס והווי של שני העצמים
                    return true;
            }
            return false;
        }

        
        //פעולות Get&Set
        public float GetY() { return this.y; }
        public void SetY(float y) { this.y = y; }

        public float GetDeltaY() { return this.deltaY; }
        public void SetDeltaY(float deltaY) { this.deltaY = deltaY; }
        public Bitmap GetBitmap() { return this.bitmap; }
        public void SetBitmap(Bitmap bitmap) { this.bitmap = bitmap; }
        public Canvas GetCanvas() { return this.canvas; }
        public void SetCanvas(Canvas canvas) { this.canvas = canvas; }
        public int GetLife() { return this.life; }
        public void SetLife(int life) { this.life = life; }

    }
}