using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Boom_It
{
    //מחלקה של אוסף היריות
    public class Shots
    {
        private Shot[] shots; //מערך שבכל תא יש משתנה מטיפוס יריה
        bool first = true;
        DateTime last;
        TimeSpan timeDifference;
        bool[] alreadywentout;
        public Shots() //פעולה בונה ריקה
        {

        }
        //פעולה בונה המאתחלת את המערך ומגדילה את גודלו לפי הקלט
        public Shots(int num) 
        {
            this.shots = new Shot[num];
            this.alreadywentout = new bool[num];
            for (int i = 0; i < num; i++) 
            {
                this.shots[i] = null;
                this.alreadywentout[i] = false;
            }
        }
        //פעולה שמוסיפה יריה למערך בתא ריק בתנאי שלא נגמרו היריות
        public void Add(Shot s, int n)
        {
            if (n < this.shots.Length)
            {
                for (int i = 0; i < this.shots.Length; i++)
                {
                    if (this.shots[i] == null)
                    {
                        this.shots[i] = s;
                        break;
                    }
                        
                }
            }
            
        }
        //פעולה שמזיזה את היריות
        public void Move1()
        {
            for (int i = 0; i < this.shots.Length; i++)
            {
                if (this.shots[i]!=null)
                    this.shots[i].Move();
            }
        }

        //פעולה שמציירת את היריות
        public void Draw1()
        {
            for (int i = 0; i < this.shots.Length; i++)
            {
                if (this.shots[i] != null)
                {
                    this.shots[i].Draw();
                }                   
            }
        }
        public Shot[] GetShots() { return this.shots; } 
    }
}