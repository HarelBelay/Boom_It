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

namespace Boom_It
{
    //מחלקה שאחראית על הצגת הסוללה של המשתמש
    public class Broadcast_Receiver : BroadcastReceiver
    {
        TextView tv; //הטקסט שיציג את הסוללה
        public Broadcast_Receiver() //פעולה בונה ריקה
        {

        }
        public Broadcast_Receiver(TextView tv) //פעולה בונה שמאתחלת את הערך
        {
            this.tv = tv;
        }
        public override void OnReceive(Context context, Intent intent) //פעולה שמקבלת את הערך העדכני של הסוללה של המשתמש
        {
            int battery = intent.GetIntExtra("level", 0); //משתנה המקבל את אחוזי הסוללה
            tv.Text = "your battery is: " + battery + "%"; //טקסט שמציג הודעה עם אחוזי הסוללה
        }
    }
}