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
    public class LoginTable
    {
        [PrimaryKey, AutoIncrement, Column("_Id")]
        public int id { get; set; } //המספר המזהה של המשתמש
        [MaxLength(25)]

        public string username { get; set; } //שם המשתמש 
        [MaxLength(25)]

        public string password { get; set; } //הסיסמה של המשתמש
    }
}