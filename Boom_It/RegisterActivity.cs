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

namespace Boom_It
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : Activity
    {
        EditText txtUsername; //מקום להכנסת שם השחקן
        EditText txtPassword; //מקום להכנסת הסיסמה שלו
        Button btnCreate; //כפתור השומר את הנתונים ומעביר למסך ההתחברות
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.NewUser);
            // Create your application here

            txtUsername = FindViewById<EditText>(Resource.Id.txtUsername);
            txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
            btnCreate = FindViewById<Button>(Resource.Id.btnCreate);

            btnCreate.Click += BtnCreate_Click;
        }
        private void BtnCreate_Click(object sender, EventArgs e)
        {
            MainActivity.beep.Start(); //סאונד של לחיצה
            try
            {
                string dpPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "user.db3"); //קריאה למסד הנתונים
                var db = new SQLiteConnection(dpPath);
                var data = db.Table<LoginTable>(); //קריאה לטבלה  
                try
                {
                    var data1 = data.Where(x => x.username == txtUsername.Text && x.password == txtPassword.Text).FirstOrDefault(); //רשימה מקושרת  
                    if (!(data1 != null || txtUsername.Text == "" || txtPassword.Text == "" || txtUsername.Text.Length > 10)) // אם המשתמש לא נרשם בעבר והכניס ערכים כאשר אורך שם המשתמש שלו קטן מעשרה תווים
                    {
                        //שמירה של הנתונים במסד הנתונים
                        var data_base = new SQLiteConnection(dpPath);
                        data_base.CreateTable<LoginTable>();
                        LoginTable tbl = new LoginTable();
                        tbl.username = txtUsername.Text;
                        tbl.password = txtPassword.Text;
                        data_base.Insert(tbl);
                        Toast.MakeText(this, "Details Added Successfully !", ToastLength.Long).Show();

                        Intent intent = new Intent(this, typeof(LoginActivity)); //מעבר למסך ההתחברות, ישנה העברה של הנתונים שהוזנו
                        intent.PutExtra("saveusername", txtUsername.Text);
                        intent.PutExtra("savepassword", txtPassword.Text);
                        StartActivity(intent);
                    }
                    else //אחרת תוצג למשתמש הודעה עם התקלה בהרשמה
                    {
                        if (data1 != null)
                            Toast.MakeText(this, "This user is already registered, please try to login", ToastLength.Long).Show();
                        if (txtUsername.Text == "" || txtPassword.Text == "")
                            Toast.MakeText(this, "please fill in the username and the password", ToastLength.Long).Show();
                        if (txtUsername.Text.Length > 10)
                            Toast.MakeText(this, "the username length should not be over 10 characters", ToastLength.Long).Show();
                    }
                }
                catch (Exception ex) //במקרה שזו הכניסה הראשונה לאפליקציה לא תהיה בדיקה של משתמש שנרשם בעבר
                {
                    if (!(txtUsername.Text == "" || txtPassword.Text == "" || txtUsername.Text.Length > 10))
                    {
                        var data_base = new SQLiteConnection(dpPath);
                        data_base.CreateTable<LoginTable>();
                        LoginTable tbl = new LoginTable();
                        tbl.username = txtUsername.Text;
                        tbl.password = txtPassword.Text;
                        data_base.Insert(tbl);
                        Toast.MakeText(this, "Details Added Successfully !", ToastLength.Long).Show();

                        Intent intent = new Intent(this, typeof(LoginActivity));
                        intent.PutExtra("saveusername", txtUsername.Text);
                        intent.PutExtra("savepassword", txtPassword.Text);
                        StartActivity(intent);
                    }
                    else //אחרת תוצג למשתמש הודעה עם התקלה בהרשמה
                    {
                        if (txtUsername.Text == "" || txtPassword.Text == "")
                            Toast.MakeText(this, "please fill in the username and the password", ToastLength.Long).Show();
                        if (txtUsername.Text.Length > 10)
                            Toast.MakeText(this, "the username length should not be over 10 characters", ToastLength.Long).Show();
                    }
                }
                
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
            }
        }


    }
}