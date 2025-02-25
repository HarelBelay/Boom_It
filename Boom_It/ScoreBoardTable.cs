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
    [Table("ScoreBoardTable")]
    public class ScoreBoardTable
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; } //המספר המזהה של המשתמש
        public int score { get; set; } //התוצאה של המשתמש
        public int time { get; set; } //הזמן שלקח למשתמש לסיים את השלב
        public string username { get; set; } //שם המשתמש
        public int level { get; set; } //מספר השלב

        public ScoreBoardTable(int score, int time, string username) //פעולה בונה מאתחלת
        {
            this.score = score;
            this.time = time;
            this.username = username;

        }
        public ScoreBoardTable() { } //פעולה בונה ריקה
    }
}