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
using SQLite;

namespace Boom_It
{
    [Activity(Label = "ScoreBoardActivity")]
    public class ScoreBoardActivity : Activity
    {
        public static string dbname = "db"; //יצירת השם למסד הנתונים
        ListView lv1,lv2,lv3; //רשימות שמציגות את עשרת הטובים בכל שלב
        ArrayAdapter<string> arrayAdapter; //רשימה המקשרת בין ממשק המשתמש למסד הנתונים
        public static List<string> list1,list2,list3; //רשימות של עשרת הטובים בכל שלב
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.ResultsList);

            lv1 = FindViewById<ListView>(Resource.Id.lv1);
            lv2 = FindViewById<ListView>(Resource.Id.lv2);
            lv3 = FindViewById<ListView>(Resource.Id.lv3);

            list1 = new List<string>();
            list2 = new List<string>();
            list3 = new List<string>();

            GetAll(); //העברה של הנתונים במסד נתונים לרשימות

            if (list1 != null && list1.Count > 0) //אם ברשימה הראשונה כלומר בשלב הראשון יש נתונים
            {
                OrderTime(list1); //סידור של הרשימה לפי הזמן
                OrderScore(list1); //סידור של הרשימה לפי הניקוד
                if(list1.Count > 10) //מחיקה של כל השחקנים שהם במקום יותר נמוך מ10
                    list1.RemoveRange(10, list1.Count-10);
                Rank(list1); //הוספת דירוג לשחקנים
                arrayAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, list1);
                lv1.Adapter = arrayAdapter;
            }
            if (list2 != null && list2.Count > 0) //אם ברשימה השנייה כלומר בשלב השני יש נתונים
            {
                OrderTime(list2); //סידור של הרשימה לפי הזמן
                OrderScore(list2); //סידור של הרשימה לפי הניקוד
                if (list2.Count > 10) //מחיקה של כל השחקנים שהם במקום יותר נמוך מ10
                    list2.RemoveRange(10, list2.Count - 10);
                Rank(list2); //הוספת דירוג לשחקנים
                arrayAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, list2);
                lv2.Adapter = arrayAdapter;
            }
            if (list3 != null && list3.Count > 0) //אם ברשימה השלישית כלומר בשלב השלישי יש נתונים
            {
                OrderTime(list3);  //סידור של הרשימה לפי הזמן
                OrderScore(list3); //סידור של הרשימה לפי הניקוד
                if (list3.Count > 10) //מחיקה של כל השחקנים שהם במקום יותר נמוך מ10
                    list3.RemoveRange(10, list3.Count - 10);
                Rank(list3); //הוספת דירוג לשחקנים
                arrayAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, list3);
                lv3.Adapter = arrayAdapter;
            }
        }

        public static void Create() //יצירה של מסד נתונים
        {
            string path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),dbname);
            var db = new SQLiteConnection(path);
            db.CreateTable<ScoreBoardTable>();
        }
        public static void Insert(int score, int time, string username,int level) //הכנסה של שחקן ונתוניו למסד הנתונים
        {
            Create(); //יצירה של מסד נתונים

            ScoreBoardTable p = new ScoreBoardTable(); //שמירה של הנתונים במסד הנתונים
            p.score = score;
            p.time = time;
            p.username = username;
            p.level = level;
            string path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbname);
            var db = new SQLiteConnection(path);
            db.CreateTable<ScoreBoardTable>();
            db.Insert(p);
        }
        public static void GetAll() //פעולה המעבירה את הנתונים לרשימות
        {
            Create();//יצירה של מסד נתונים

            string path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbname);
            var db = new SQLiteConnection(path);
            string strsql = string.Format("SELECT * FROM ScoreBoardTable");
            var results = db.Query<ScoreBoardTable>(strsql);
            if(results != null && results.Count > 0) //אם יש נתונים
            {
                foreach(var item in results) //מעבר על כל תא ברשימה
                {
                    string str = item.username + "\nscore:" + item.score + "  time:" + item.time; //שמירה של הנתונים בסטרינג והוספה לשלב המתאים
                    if (item.level==1)
                        list1.Add(str);
                    else if(item.level==2)
                        list2.Add(str);
                    else if(item.level==3)
                        list3.Add(str);
                }
            }

        }
        public static void OrderScore(List<string> list) //פעולה שמסדרת את הרשימה לפי הניקוד הכי גבוה
        {
            string[] TempKeep = new string[list.Count]; //מערך השומר זמנית את נתונים של הרשימה
            string [] usernames = new string[list.Count]; //מערך של המשתמשים ששיחקו
            int i = 0; //מספר השחקנים שנוספו - כדי להוסיף את השחקנים במקום המתאים במערכים
            while(list.Count > 1)
            {
                foreach (string str in list) //מעבר על כל תא ברשימה
                {
                    if(Exist(usernames, Username(str))) //אם המשתמש כבר נוסף למערך של המשתמשים אין צורך להוסיף אותו שוב ולכן יש צורך למחוק אותו מהרשימה
                    {
                        list.Remove(str);
                        break;
                    }
                    if (Score(str) == Max(list) && !Exist(usernames, Username(str))) //אם הניקוד של שחקן זה הוא המקסימלי והוא לא התווסף כבר בעבר יש לשמור אותו במערך הזמני
                    {
                        TempKeep[i] = str; //שמירה של השחקן במערך הזמני
                        usernames[i] = Username(str); //הוספה של שם השחקן לשמות שכבר נוספו
                        i++; //התווסף שחקן
                        list.Remove(str); //מחיקה של השחקן מהרשימה
                        break;
                    }
                    if (list.Count == 1) //אם נשאר עוד שחקן אחד יש לצאת מהלולאה
                        break;
                }
                //נותר כרגע שחקן אחרון ברשימה
            }
            if (Exist(usernames, Username(list[0])))  //אם המשתמש כבר נוסף למערך של המשתמשים אין צורך להוסיף אותו שוב ולכן יש צורך למחוק אותו מהרשימה
            {
                list.Remove(list[0]);
            }
            else //אחרת, יש להוסיף אותו למערך הזמני
            {
                TempKeep[i] = list[0];
                list.RemoveAt(0);
            }            
            for (int j = 0; j < TempKeep.Length; j++) //החזרה של השחקנים לרשימה, כעת הם מסודרים לפי הניקוד שלהם
            {
                if(TempKeep[j] != null)
                    list.Add(TempKeep[j]);
            }
        }
        public static int Max(List<string> list) //פעולה שמוצאת את הניקוד המקסימלי בכל הרשימה
        {
            int maxScore = 0;
            foreach(string str in list)
            {
                if(Score(str)>maxScore)
                    maxScore = Score(str);
            }
            return maxScore;
        }
        public static string Username(string str) //פעולה שמוצאת את שם המשתמש מתוך הרשימה
        {
            string username = "";
            username = str.Substring(0, str.IndexOf("\n"));
            return username;
        }
        public static int Score(string str) //פעולה שמוצאת את הניקוד מתוך הרשימה
        {
            string score = "";
            score = str.Substring(str.IndexOf(":") + 1, str.IndexOf(" ") - str.IndexOf(":"));

            return int.Parse(score);
        }
        public static int Time(string str) //פעולה שמוצאת את הזמן מתוך הרשימה
        {
            string time = "";
            int counter = 0;
            int i;
            for (i = 0; i < str.Length; i++)
            {
                if (str[i] == ':')
                    counter++;
                if (counter == 2)
                    break;
            }
            time = str.Substring(i + 1, str.Length - 1 - i);

            return int.Parse(time);
        }
        public static int Min(List<string> list) //פעולה שמוצאת את הזמן המינימלי בכל הרשימה
        {
            int minTime = int.MaxValue;
            foreach (string str in list)
            {
                if (Time(str) < minTime)
                    minTime = Time(str);
            }
            return minTime;
        }
        public static void OrderTime(List<string> list) // פעולה שמסדרת את הרשימה לפי הזמנים של השחקנים, הראשון יהיה בעל הזמן הנמוך ביותר
        {
            string[] TempKeep = new string[list.Count]; //מערך השומר זמנית את נתונים של הרשימה
            int i = 0; //מספר השחקנים שנוספו - כדי להוסיף את השחקנים במקום המתאים במערכים
            while (list.Count > 1)
            {
                foreach (string str in list) //מעבר על כל תא ברשימה
                {
                    if (Time(str) == Min(list)) //אם הזמן של השחקן הוא הזמן המינימלי 
                    {
                        TempKeep[i] = str; //שמירה של השחקן במערך הזמני
                        i++; //התווסף שחקן
                        list.Remove(str); //מחיקה של השחקן מהרשימה
                        break;
                    }
                    if (list.Count == 1) //אם נשאר שחקן אחד נצא מהלולאה
                        break;
                }

            }
            //נשאר שחקן אחד אז נעביר אותו למערך ששומר זמנית ונמחק אותו
            TempKeep[i] = list[0];
            list.RemoveAt(0);
            for (int j = 0; j < TempKeep.Length; j++) //החזרה של השחקנים לרשימה, כעת הם מסודרים לפי הזמן שלהם
            {
                list.Add(TempKeep[j]);
            }
        }
        public static bool Exist(string [] usernames, string username) //פעולה הבודקת האם המשתמש נמצא כבר ברשימה
        {
            for (int i = 0; i < usernames.Length; i++)
            {
                if(username == usernames[i])
                    return true;
            }
            return false;
        }
        public static void Rank(List<string> list) //פעולה העוברת על הרשימה ומוסיפה את המקום של כל שחקן
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = i+1+") "+list[i];
            }
        }
    }
}