using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boom_It
{
    [Activity(Label = "LevelsActivity")]
    public class LevelsActivity : Activity
    {
        ImageButton level1, level2, level3, exit; //התמונות של השלבים ודלת היציאה
        Button btn_score; //כפתור השולח ללוח התוצאות
        TextView tv_username; //טקסט המציג את שם השחקן
        string username; //משתנה השומר את שם השחקן
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.LevelsLayout);

            level1 = FindViewById<ImageButton>(Resource.Id.level1);
            level2 = FindViewById<ImageButton>(Resource.Id.level2);
            level3 = FindViewById<ImageButton>(Resource.Id.level3);
            tv_username = FindViewById<TextView>(Resource.Id.tv_username);
            exit = FindViewById<ImageButton>(Resource.Id.exit);
            btn_score = FindViewById<Button>(Resource.Id.btn_score);

            username = Intent.GetStringExtra("username"); //קבלת שם המשתמש לפי השם שאיתו השחקן נרשם

            BoardGame.username = username; //עדכון שם השחקן גם במחלקת המשחק
            tv_username.Text = "Hi "+username+" !"; //הצגת שם השחקן 

            level1.Click += Level1_Click;
            level2.Click += Level2_Click;
            level3.Click += Level3_Click;
            exit.Click += Exit_Click;
            btn_score.Click += Btn_score_Click;


        }
        //פעולה השולחת ללוח התוצאות
        private void Btn_score_Click(object sender, EventArgs e)
        {
            MainActivity.beep.Start();
            Intent intent = new Intent(this,typeof(ScoreBoardActivity));
            StartActivity(intent);
        }
        //פעולה השולחת את השחקן בחזרה למסך הבית
        private void Exit_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        //פעולה השולחת את השחקן לשלב 3
        private void Level3_Click(object sender, EventArgs e)
        {
            //אתחול הערכים המתאימים לשלב 3
            BoardGame.level = 3;
            BoardGame.scoreWin = 1200;
            BoardGame.MaxScore = 1400;
            BoardGame.time = 40;

            Intent intent = new Intent(this, typeof(MusicService)); //הפסקת מוזיקת הרקע
            StopService(intent);

            MainActivity.beep.Start(); //סאונד של לחיצה
            Settings.startedGame = true; //התחיל המשחק

            //שליחה למסך המשחק
            Intent intent1 = new Intent(this, typeof(BoardGameActivity));
            StartActivity(intent1);
        }
        //פעולה השולחת את השחקן לשלב 2
        private void Level2_Click(object sender, EventArgs e)
        {
            //אתחול הערכים המתאימים לשלב 2
            BoardGame.level = 2;
            BoardGame.scoreWin = 900;
            BoardGame.MaxScore = 1100;
            BoardGame.time = 30;

            Intent intent = new Intent(this, typeof(MusicService)); //הפסקת מוזיקת הרקע
            StopService(intent);

            MainActivity.beep.Start();//סאונד של לחיצה
            Settings.startedGame = true; //התחיל המשחק

            //שליחה למסך המשחק
            Intent intent1 = new Intent(this, typeof(BoardGameActivity));
            StartActivity(intent1);
        }
        //פעולה השולחת את השחקן לשלב 1
        private void Level1_Click(object sender, EventArgs e)
        {
            //אתחול הערכים המתאימים לשלב 1
            BoardGame.level = 1;
            BoardGame.scoreWin = 600;
            BoardGame.MaxScore = 800;
            BoardGame.time = 20;

            Intent intent = new Intent(this, typeof(MusicService)); //הפסקת מוזיקת הרקע
            StopService(intent);

            MainActivity.beep.Start(); //סאונד של לחיצה
            Settings.startedGame = true; //התחיל המשחק

            //שליחה למסך המשחק
            Intent intent1 = new Intent(this, typeof(BoardGameActivity));
            StartActivity(intent1);
        }

    }
}