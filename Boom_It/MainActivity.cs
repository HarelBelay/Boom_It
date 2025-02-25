using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Widget;
using System.IO;
using SQLite;
using System;
using Android.Content;
using Android.Media;
using Android.Views;
    

namespace Boom_It
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Button btn_login; //כפתור השולח למסך התחברות
        Button btn_register; //כפתור השולח למסך הרשמה
        public static MediaPlayer beep; //סאונד של לחיצה
        public static bool IsMusicNotChanged = true; //משתנה הבודק אם המוזיקה לא השתנתה עדיין
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);  

            btn_login = FindViewById<Button>(Resource.Id.btn_login);
            btn_register = FindViewById<Button>(Resource.Id.btn_register);

            btn_login.Click += Btn_login_Click;
            btn_register.Click += Btn_register_Click;

            beep = MediaPlayer.Create(this, Resource.Raw.Beep);

        }

        //פעולה השולחת את המשתמש למסך ההרשמה
        private void Btn_register_Click(object sender, EventArgs e)
        {
            beep.Start();
            Intent intent = new Intent(this, typeof(RegisterActivity));
            StartActivity(intent);
        }

        //פעולה השולחת את המשתמש למסך ההתחברות
        private void Btn_login_Click(object sender, EventArgs e)
        {
            beep.Start();
            Intent intent = new Intent(this, typeof(LoginActivity));
            StartActivity(intent);
        }
        //פעולה היוצרת תפריט
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
            return true;
        }
        //פעולה הבודקת לחיצה על האפשרויות בתפריט
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.settings) //אם הייתה לחיצה על ההגדרות, תהיה שליחה למסך ההגדרות
            {
                Intent intent = new Intent(this, typeof(Settings));
                if(IsMusicNotChanged) //אם המוזיקה לא השתנתה, תשלח למסך ההגדרות הוראה להשתיק את המוזיקה משום שזו הכניסה הראשונה
                    intent.PutExtra("firstenter", true);
                StartActivity(intent);
                return true;
            }
            else if (item.ItemId == Resource.Id.instructions) // אם הייתה לחיצה על ההוראות, תהיה שליחה למסך ההוראות
            {
                Intent intent = new Intent(this, typeof(Instructions));
                StartActivity(intent);
                return true;
            }
            return base.OnOptionsItemSelected(item);

        }
    }
}