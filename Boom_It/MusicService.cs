using Android.App;
using Android.Content;
using Android.Media;
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
    [Service(Label ="MusicService")]
    public class MusicService : Service
    {

        public static MediaPlayer mp; //משתנה של המוזיקה

        [return: GeneratedEnum]
        //פעולה המתחילה את מוזיקת הרקע
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {

            mp = MediaPlayer.Create(this,Resource.Raw.bgmusic);
            mp.Looping = true; //המוזיקה תחזור בלופים
            mp.Start(); //התחלה של המוזיקה

            return base.OnStartCommand(intent, flags, startId);
        }
        //פעולה שחייבים לממש, מכיוון שאין בה שימוש מחזירים null
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        //עצירה של המוזיקה
        public override void OnDestroy()
        {
            base.OnDestroy();
            if(mp!= null)
            {
                mp.Stop(); //עצירה של המוזיקה
                mp.Release(); //משחרר מקום בזיכרון
                mp = null;
            }
            
        }

    }
}