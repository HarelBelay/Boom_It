using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.Util.Log;
using Android.Util;
using System.Threading;

namespace Boom_It
{
    [Activity(Label = "BoardGameActivity")]
    public class BoardGameActivity : Activity
    {
        BoardGame boardGame; //משתנה שמכיל את לוח המשחק
        bool Back = false; //משתנה של האם המשתמש חזר אחורה
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            boardGame = new BoardGame(this);
            SetContentView(boardGame);
        }
        //פעולה הממשיכה את המשחק
        protected override void OnResume()
        {
            base.OnResume();
            if (boardGame != null)
            {
                boardGame.resume();
            }

        }
        protected override void OnStart()
        {
            base.OnStart();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        protected override void OnStop()
        {
            base.OnStop();
        }
        protected override void OnPause()
        {
            base.OnPause();
            if (Back)
            {
                //Toast.MakeText(this,"Exit",ToastLength.Short).Show();
            }
            else if (boardGame != null && boardGame.isRunning == true)
            {
                boardGame.pause();
            }
        }
        //פעולה המסיימת את המשחק
        public override void Finish()
        {
            base.Finish();
            Back = true; //המשתמש חזר אחורה
            boardGame.threadRunning = false; //המשחק לא רץ יותר
            while (true) 
            {
                try
                {
                    if(BoardGame.gameMusic!=null)
                        BoardGame.gameMusic.Stop(); //עצירה של מוזיקת המשחק
                    boardGame.gameThread.Join(); //הורס את הthread
                }
                catch (Exception e)
                {
                }
                break;
            }
            //עדכון מוזיקת הרקע, במידה ולפני שהתחיל המשחק הייתה מוזיקת רקע גם בצאת המשחק תהיה המוזיקה
            if (Settings.isPlayingMusic)
            {
                if (Settings.startedGame)
                {
                    Intent intent = new Intent(this, typeof(MusicService));
                    StartService(intent);
                    Settings.startedGame = false;
                }
            }
        }
        public override void OnBackPressed() //כאשר המשתמש חוזר אחרוה מהכפתור בטלפון שלו המשחק יעצר ויוצג דיאלוג של עצירה
        {
            boardGame.pause();
            boardGame.handler.Post(boardGame.PauseDialog);
        }
    }
}