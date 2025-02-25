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
    [Activity(Label = "Settings")]
    public class Settings : Activity
    {
        SeekBar sb; //משתנה שאחראי על הסיק בר
        AudioManager am; //משתנה שאחראי על התאמת המוזיקה לסיק בר
        ImageButton audio; //תמונה שמציגה אם יש סאונד או לא
        TextView tv_battery; //טקסט שמציג את אחוזי הסוללה
        Broadcast_Receiver broadcast_Receiver; //אחראי על הצגת הסוללה
        public static bool isPlayingMusic; //משתנה הבודק אם המוזיקה רצה
        public static bool startedGame; //משתנה הבודק אם התחיל משחק כי יש צורך לעצור את המוזיקה הזו
        ISharedPreferences sp; //שמירה של הסיק בר
        bool enter; //משתנה הבודק אם זאת הכניסה הראשונה למסך ההגדרות
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.settings);

            sb = FindViewById<SeekBar>(Resource.Id.sb);
            audio = FindViewById<ImageButton>(Resource.Id.audio);
            tv_battery = FindViewById<TextView>(Resource.Id.tv_battery);
            broadcast_Receiver = new Broadcast_Receiver(tv_battery);

            sb.ProgressChanged += Sb_ProgressChanged;
            audio.Click += Audio_Click;

            sp = this.GetSharedPreferences("details", FileCreationMode.Private);
            am = (AudioManager)GetSystemService(AudioService);

            float seekbar = sp.GetFloat("seekbar", (float)0.5); //משתנה שמקבל את עוצמת הסיק בר שנשמרה
            if (seekbar != (float)0.5) //אם העוצמה היא לא ברירת מחדל כלומר הייתה שמירה
            {
                sb.Progress = (int)(seekbar * 100); //התאמה של הסיק בר למה שנשמר
                am.SetStreamVolume(Stream.Music, sb.Progress, VolumeNotificationFlags.PlaySound); //התאמה של עוצמת הקול לסיק בר
            }
            enter = Intent.GetBooleanExtra("firstenter", false); //בדיקה אם ישנה הוראה להשתיק את המוזיקה משום שזו הכניסה הראשונה למסך ההגדרות
            if (enter) //אם כן יש להשתיק את המוזיקה ולשמור זאת
            {
                isPlayingMusic = false; //לא מושמעת מוזיקה
                var editor = sp.Edit(); //שמירה של מצב המוזיקה
                editor.PutBoolean("audio", false);
                editor.Commit();
            }
            else //אם הייתה כבר כניסה למסך ההגדרות המוזיקה תהיה לפי מה שנקבע אחרון
            {
                isPlayingMusic = sp.GetBoolean("audio", false); 
            }
            
            if (isPlayingMusic) //אם מושמעת מוזיקה 
            {
                audio.SetBackgroundResource(Resource.Drawable.audio); // יש לשים את התמונה שמראה על כך שיש מוזיקה
                if (startedGame || am.GetStreamVolume(Stream.Music) == am.GetStreamMinVolume(Stream.Music)) //אם התחיל משחק או שעוצמת המוזיקה לא השתנתה יש להתחיל את המוזיקה
                {
                    Intent intent = new Intent(this, typeof(MusicService));
                    StartService(intent);
                    startedGame = false;
                }
            }
            else //אם לא מושמעת מוזיקה
            {
                audio.SetBackgroundResource(Resource.Drawable.noaudio); //יש לשים את התמונה שמראה על כך שאין מוזיקה
            }
        }
        //לחיצה על התמונה של הסאונד
        private void Audio_Click(object sender, EventArgs e)
        {
            MainActivity.beep.Start(); //סאונד של לחיצה
            MainActivity.IsMusicNotChanged = false; //המוזיקה השתנתה וכעת אין צורך לשלוח הוראה למסך ההגדרות להשתיק את המוזיקה
            var editor = sp.Edit();
            if (isPlayingMusic) //אם המוזיקה עכשיו נשמעת יש לעצור אותה
            {
                audio.SetBackgroundResource(Resource.Drawable.noaudio);
                Intent intent = new Intent(this, typeof(MusicService));
                StopService(intent);
                isPlayingMusic = false;
                editor.PutBoolean("audio", false);
            }
            else //אם המוזיקה לא נשמעת יש להתחיל אותה
            {
                audio.SetBackgroundResource(Resource.Drawable.audio);
                Intent intent = new Intent(this, typeof(MusicService));
                StartService(intent);
                isPlayingMusic = true;
                editor.PutBoolean("audio", true);
            }

            editor.Commit();
        }
        //שינוי של הסיק בר
        private void Sb_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            am.SetStreamVolume(Stream.Music, e.Progress, VolumeNotificationFlags.PlaySound); //התאמה של עוצמת המוזיקה לסיק בר
            var editor = sp.Edit(); //שמירה של הסיק בר
            editor.PutFloat("seekbar", (float)e.Progress / 100);
            editor.Commit();
        }
        //פעולה המקבלת את השינוי באחוזי הסוללה ומעבירה למחלקה את המספר המעודכן
        protected override void OnResume()
        {
            base.OnResume();
            RegisterReceiver(broadcast_Receiver, new IntentFilter(Intent.ActionBatteryChanged));
        }
    }
}