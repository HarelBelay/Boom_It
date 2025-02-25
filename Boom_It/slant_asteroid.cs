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
    // מחלקה של אסטרואיד שנע באלכסון שיורש ממחלקת אסטרואיד
    public class slant_asteroid : asteroid
    {
        private float x; //המיקום של האסטרואיד לפי הציר האנכי
        private float deltaX; //מהירות הירידה של האסטרואיד
        private int direction; //משתנה הקובע אם האלכסון ירד מהפינה הימנית העליונה אל הפינה השמאלית התחתונה כאשר שווה למינוס 1 או להיפך כאשר שווה ל1

        //פעולה בונה ואתחול של משתנים
        public slant_asteroid(float y,float deltaY, float x,float deltaX ,Bitmap bitmap, Canvas canvas,int life, int direction) : base(y,deltaY,x ,bitmap, canvas,life)
        {
            this.x = x;
            this.deltaX = deltaX;
            this.direction = direction;
        }
        //פעולת Get
        public float GetX() { return this.x; }

        //פעולה המציירת את האסטרואיד על הקנבס
        public override void Draw()
        {
            this.GetCanvas().DrawBitmap(this.GetBitmap(), this.x, this.GetY(), null);
        }


        //פעולה הגורמת לאסטרואיד לנוע, יורשת ממחלקת האב 
        public override void Move()
        {
            if(this.direction == 1) //אם האסטרואיד נע משמאל לימין
            {
                // קידום מיקום האסטרואיד לפי הציר האנכי והאופקי
                this.x += deltaX; 
                SetY(GetY() + GetDeltaY());

                //אם האסטרואיד יוצא מגבולות המסך הוא יחזור לאותו המסלול מההתחלה
                if (GetY() > GetCanvas().Height || this.x > GetCanvas().Width) 
                {
                    SetY(-1 * GetBitmap().Height);
                    this.x = -1 * GetBitmap().Width;
                }
            }
            else if(this.direction == -1) //אם האסטרואיד נע מימין לשמאל
            {
                // קידום מיקום האסטרואיד לפי הציר האנכי והאופקי
                this.x -= deltaX;
                SetY(GetY() + GetDeltaY());

                //אם האסטרואיד יוצא מגבולות המסך הוא יחזור לאותו המסלול מההתחלה
                if (GetY() > GetCanvas().Height || this.x < 0)
                {
                    SetY(-1 * GetBitmap().Height);
                    this.x = GetBitmap().Width + GetCanvas().Width;
                }
            }    
        }
        //פעולה המחזירה אמת אם היריה פגעה באסטרואיד ושקר אחרת
        public override bool GotShot(float x, float y)
        {
            if (y <= GetY() + this.GetBitmap().Height && y >= GetY() && x >= this.x && x <= this.x + GetBitmap().Width)
                return true;
            return false;
        }
        //פעולה המחזירה אמת אם האסטרואיד פגע בחללית
        public override bool Crash(float x, float y , Bitmap bitmap)
        {
            if (GetY() + 250 >= y)
            {
                if(Math.Abs((this.x + (this.GetBitmap().Width/2)) - (x + (bitmap.Width/2)))<=50)
                    return true;
            }
            return false;

        }
    }
}