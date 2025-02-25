using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;

namespace Boom_It
{
    //מחלקת טיימר האחראית על הזמן של המשחק
    public class Timer
    {
        ThreadStart threadStart;
        public Thread thread;
        int timer; //הזמן
        int keeptimer; //משתנה השומר את הזמן המקורי
        public bool threadRunning = true; //פעולה הבודקת אם ה thread הופעל
        public bool isRunning = true; //משתנה הבודק אם הזמן רץ
        
        //פעולה בונה
        public Timer(int timer)
        {
            this.timer = timer;
            keeptimer = timer;
            threadStart = new ThreadStart(Run);
            thread = new Thread(threadStart);
        }
        //פעולה המתחילה את הזמן
        public void Start()
        {
            if (thread != null)
                thread.Start();
        }
        //פעולה המחזירה את הזמן
        public int getTimer()
        {
            return timer;
        }
        //פעולה המאתחלת את הזמן
        public void setTimer(int timer)
        {
            this.timer = timer;
            keeptimer = timer;
        }
        //פעולה הרצה ברקע ומורידה את הטיימר בכל שנייה
        public void Run()
        {
            while (threadRunning)
            {
                try
                {
                    if (isRunning)
                    {
                        timer--;
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        //פעולה העוצרת את הזמן
        public void StopTimer()
        {
            this.isRunning = false;
        }
        //פעולה הממשיכה את הזמן
        public void ResumeTimer()
        {
            this.isRunning = true;
        }
        public int GetKeepTimer()
        {
            return this.keeptimer;
        }

    }
}