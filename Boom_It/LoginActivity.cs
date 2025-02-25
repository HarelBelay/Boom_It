using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android.Views.Animations;

namespace Boom_It
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        EditText txtUsername; //מקום להכנסת שם השחקן
        EditText txtPassword; //מקום להכנסת הסיסמה שלו
        Button btn_levels; //כפתור השולח למסך השלבים
        Button btn_clear; //כפתור המוחק את השם, הסיסמה והתמונה שכרגע נמצאים

        ISharedPreferences keep; //שומר את המשתמש האחרון שנכנס
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Login);

            btn_levels = FindViewById<Button>(Resource.Id.btn_levels);
            txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
            txtUsername = FindViewById<EditText>(Resource.Id.txtUsername);
            btn_clear = FindViewById<Button>(Resource.Id.btn_clear);

            keep = this.GetSharedPreferences("details", FileCreationMode.Private);          

            //אם המשתמש כרגע נרשם והגיע ממסך ההרשמה אז ערכים אלו יעודכנו לפי מה שנרשם איתו
            if (Intent.GetStringExtra("saveusername")!=null && Intent.GetStringExtra("savepassword") != null) 
            {
                txtUsername.Text = Intent.GetStringExtra("saveusername");
                txtPassword.Text = Intent.GetStringExtra("savepassword");

                var editor = keep.Edit();
                editor.PutString("username", txtUsername.Text);
                editor.PutString("password", txtPassword.Text);
                editor.Commit();
            }
            else //אחרת, המשתמש הגיע למסך זה בלי להירשם קודם לכן יש להציג את המשתמש האחרון שנכנס
            {
                if(keep.GetString("username",null)!=null && keep.GetString("password", null) != null)
                {
                    txtUsername.Text = keep.GetString("username", null);
                    txtPassword.Text = keep.GetString("password", null);
                    
                }
            }                               

            btn_levels.Click += Btn_levels_Click;
            btn_clear.Click += Btn_clear_Click;
        }

        
        //פעולה המוחקת את כל הערכים כדי שמשתמש שרוצה להתחבר יוכל לעשות זאת יותר מהר
        private void Btn_clear_Click(object sender, EventArgs e)
        {
            MainActivity.beep.Start();
            txtUsername.Text = "";
            txtPassword.Text = "";
        }
        //פעולה ששולחת את השחקן למסך השלבים אם התחבר כראוי
        private void Btn_levels_Click(object sender, EventArgs e)
        {
            MainActivity.beep.Start(); //סאונד של לחיצה
            try
            {
                string dpPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3"); //קריאה למסד הנתונים 
                var db = new SQLiteConnection(dpPath);
                var data = db.Table<LoginTable>(); //קריאה לטבלה  
                var data1 = data.Where(x => x.username == txtUsername.Text && x.password == txtPassword.Text).FirstOrDefault(); //קישור בין הרשימות 
                if (data1 != null) //המשתמש נרשם בעבר
                {
                    //שמירה חד פעמית למשתמש זה
                    var editor = keep.Edit();
                    editor.PutString("username", txtUsername.Text);
                    editor.PutString("password", txtPassword.Text);
                    editor.Commit();

                    Toast.MakeText(this, "Login Success", ToastLength.Long).Show();
                    //מעבר למסך השלבים והעברה של שם השחקן 
                    Intent intent = new Intent(this,typeof(LevelsActivity));
                    intent.PutExtra("username",txtUsername.Text);
                    StartActivity(intent);
                }
                else //אחרת משתמש זה לא נרשם בעבר
                {
                    Toast.MakeText(this, "Username or Password invalid", ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
            }
        }

        //יצירה של מסד נתונים במידה ולא נוצר כבר
        public string CreateDB()
        {
            var output = "";
            output += "Creating Databse if it doesnt exists";
            string dpPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3"); //Create New Database  
            var db = new SQLiteConnection(dpPath);
            output += "\n Database Created....";
            return output;
        }
      
    }
}