using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;
using Android.Media;

namespace Boom_It
{
    public class BoardGame : SurfaceView
    {
        Context context;
        //התמונות של האסטרואידים והחללית
        Bitmap Slant_rl_Pic, Slant_lr_Pic, RegularPic, SCpic, ShotPic, Backpic, PausePic;
        //יצירת עצמים של אסטרואידים
        slant_asteroid slantRightToLeft1; //אסטרואיד הנע באלכסון מהפינה הימינית העליונה לפינה השמאלית התחתונה
        slant_asteroid slantLeftToRight1; //אסטרואיד הנע באלכסון מהפינה השמאלית העליונה לפינה הימנית התחתונה
        asteroid regular1; //אסטרואיד הנע כלפי מטה אך ורק בציר האנכי

        //האסטרואידים שמגיעים מאוחר יותר
        slant_asteroid slantRightToLeft2; 
        slant_asteroid slantLeftToRight2; 
        asteroid regular2; 

        SpaceShip spaceship; //יצירת עצם של החללית

        Shots shots; //יצירת ירייה
        int max_shots; //כמות היריות שיש לחללית 
        int NumOfShots = 0; //מספר יריות שכבר נורו      
        bool shot = false; //משתנה הבודק אם יצאה יריה
        bool first_shot = true; //משתנה הבודק אם זאת יריה ראשונה
        //משתנים הבודקים מתי יצאה היריה האחרונה
        DateTime last;
        TimeSpan timeDifference;

        //משתנים הבודקים אם האסטרואידים הושמדו
        bool regular_dead1, slant_lr_dead1, slant_rl_dead1;

        //2
        bool regular_dead2, slant_lr_dead2, slant_rl_dead2;

        //שיעור האיקס והווי של האסטרואידים והחללית
        float Xrl1, Xlr1, regular_x1; //שיעורי האיקס של האסטרואידים שנעים באלכסון
        float Slant_rl_Y1, Slant_lr_Y1, RegularY1, shotY; //שיעורי הווי של כל האסטרואידים והיריות
        float Xsc; //שיעור האיקס של החללית

        //2
        float Xrl2, Xlr2, regular_x2; 
        float Slant_rl_Y2, Slant_lr_Y2, RegularY2;

        //מהירות האסטרואידים
        float regular_deltaY, slant_deltaY;
        float slant_deltaX;

        //החיים ההתחלתיים של האסטרואידים והחללית
        int lr_life1, rl_life1, regular_life1;

        //2
        int lr_life2, rl_life2, regular_life2;

        int SpeedOfTheMovingScreen = 0; //המהירות של המסך הנע ברקע

        //טקסטים על האסטרואיד המייצגים את חייהם
        Paint rl_paint1 = new Paint();
        Paint lr_paint1 = new Paint();
        Paint regular_paint1 = new Paint();
        Paint spaceship_paint = new Paint();

        //2
        Paint rl_paint2 = new Paint();
        Paint lr_paint2 = new Paint();
        Paint regular_paint2 = new Paint();

        //משתנה הבודק אם זאת כניסה ראשונה למשחק
        bool first_enter = true;

        //משתנה הגורם לאסטרואידים השניים לרדת
        bool fall = false;

        Canvas canvas = null; //הלוח עליו מציירים את כל המשחק

        public bool threadRunning = true; //is game thread running
        public bool isRunning = true; //משתנה הבודק האם המשחק רץ
        //Game Threads
        public Thread gameThread;
        ThreadStart ts;

        //טיימר 
        Timer myTimer; //משתנה האחראי על זמן המשחק מטיפוס זמן
        Paint paintTimer; //טקסט המציג את הזמן של המשחק 
        
        
        int score; //הניקוד של השחקן
        Paint scoreText = new Paint(); //טקסט המציג את הניקוד של השחקן


        Dialog dWin,dLose,dPause; //דיאלוגים של ניצחון, הפסד ועצירה
        public Handler handler = new Handler(); //הנדלר שאחראי להציג את הדיאלוגים - ההנדלר גורם לכך שהס'רד לא יהרס ואחראי על מה יוצג קודם

        //מאפיינים של כל שלב
        public static int level; //משתנה של השלב שבו נמצאים
        public static int scoreWin; //משתנה של הניקוד המינימלי הדרוש לניצחון השלב
        public static int MaxScore; // משתנה של הניקוד המקסימלי שאפשר להשיג בכל שלב
        public static int time; //משתנה של זמן המשחק

        Paint levelText = new Paint(); //טקסט המציג את השלב שבו נמצאים

        public static string username; //משתנה של שם השחקן שמשחק כעת

        MediaPlayer destroyedAsteroid,shotTaken,winSound,loseSound; //סאונדים של המשחק - פיצוץ אסטרואיד, יריה, ניצחון והפסד
        public static MediaPlayer gameMusic; // משתנה של מוזיקת הרקע בזמן המשחק
        public BoardGame(Context context) : base(context) //פעולה בונה
        {
            this.context = context;

            //הזנת התמונות המתאימות לאסטרואידים
            Slant_rl_Pic = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.slant_asteroid);
            Slant_lr_Pic = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.slant_asteroid);
            RegularPic = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.regular_asteroid);
            SCpic = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.spaceship);
            ShotPic = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.shot);
            Backpic = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.back);
            PausePic = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.pause);

            //הקטנת התמונות והתאמתן לגדלים הרצויים
            Slant_rl_Pic = ResizeBitmap(Slant_rl_Pic, Slant_rl_Pic.Width - 30, Slant_rl_Pic.Height - 70);
            Slant_lr_Pic = ResizeBitmap(Slant_lr_Pic, Slant_lr_Pic.Width - 30, Slant_lr_Pic.Height - 70);
            RegularPic = ResizeBitmap(RegularPic, RegularPic.Width - 50, RegularPic.Height - 50);
            SCpic = ResizeBitmap(SCpic, SCpic.Width - 70, SCpic.Height - 70);
            ShotPic = ResizeBitmap(ShotPic, ShotPic.Width, ShotPic.Height - 30);
            PausePic = ResizeBitmap(PausePic, PausePic.Width - 1200, PausePic.Height - 1200);
            

            //קביעת מהירותם של האסטרואידים
            regular_deltaY = 10;
            slant_deltaY = 10;
            slant_deltaX = 4f;

            

            //טקסטים על האסטרואידים והחללית המייצגים את חייהם
            rl_paint1.SetARGB(50, 20, 50, 100);
            rl_paint1.TextAlign = Paint.Align.Center;
            rl_paint1.Color = Color.White;

            lr_paint1.SetARGB(50, 20, 50, 100);
            lr_paint1.TextAlign = Paint.Align.Center;
            lr_paint1.Color = Color.White;

            regular_paint1.SetARGB(50, 20, 50, 100);
            regular_paint1.TextAlign = Paint.Align.Center;
            regular_paint1.Color = Color.White;

            spaceship_paint.SetARGB(50, 20, 50, 100);
            spaceship_paint.TextAlign = Paint.Align.Center;
            spaceship_paint.Color = Color.White;

            //2
            rl_paint2.SetARGB(50, 20, 50, 100);
            rl_paint2.TextAlign = Paint.Align.Center;
            rl_paint2.Color = Color.White;

            lr_paint2.SetARGB(50, 20, 50, 100);
            lr_paint2.TextAlign = Paint.Align.Center;
            lr_paint2.Color = Color.White;

            regular_paint2.SetARGB(50, 20, 50, 100);
            regular_paint2.TextAlign = Paint.Align.Center;
            regular_paint2.Color = Color.White;

            // גודל הטקסט שמייצג את החיים של האסטרואידים לפי גודל התמונה
            rl_paint1.TextSize = Slant_rl_Pic.Height / 10;
            lr_paint1.TextSize = Slant_lr_Pic.Height / 10;
            regular_paint1.TextSize = RegularPic.Height / 7;

            //2
            rl_paint2.TextSize = Slant_rl_Pic.Height / 10;
            lr_paint2.TextSize = Slant_lr_Pic.Height / 10;
            regular_paint2.TextSize = RegularPic.Height / 7;

            //טקסט של הזמן, הניקוד והשלב
            paintTimer = new Paint();
            paintTimer.Color = Color.Yellow;
            paintTimer.StrokeWidth = 20;
            paintTimer.TextSize = 70;

            scoreText.SetARGB(50, 20, 50, 100);
            scoreText.TextAlign = Paint.Align.Center;
            scoreText.Color = Color.Yellow;
            scoreText.TextSize = 70;

            levelText.SetARGB(50, 20, 50, 100);
            levelText.TextAlign = Paint.Align.Center;
            levelText.Color = Color.Yellow;
            levelText.TextSize = 70;

            

        }
        //קביעת ערכים התחלתיים אם זוהי הכניסה הראשונה למשחק
        public void FirstEnter()
        {
            //קביעת הערכים ההתחלתיים של שיעורי האיקס והווי של האסטרואיד שנע באלכסון משמאל לימין
            //האסטרואיד יתחיל לנוע מהפינה השמאלית העליונה
            Xlr1 = -1 * Slant_lr_Pic.Width;
            Slant_lr_Y1 = -1 * Slant_lr_Pic.Height;

            //קביעת הערכים ההתחלתיים של שיעורי האיקס והווי של האסטרואיד שנע באלכסון מימין לשמאל
            //האסטרואיד יתחיל לנוע מהפינה הימנית העליונה
            Xrl1 = canvas.Width + Slant_rl_Pic.Width;
            Slant_rl_Y1 = -1 * Slant_rl_Pic.Height;

            //קביעת שיעור הווי ההתחלתי של האסטרואיד שנע כלפי מטה אך ורק בציר האנכי
            //האסטרואיד יתחיל לנוע בנקודה העליונה ביותר של המסך
            RegularY1 = 0;
            regular_x1 = canvas.Width / 2;

            shotY = canvas.Height - SCpic.Height; //קביעת שיעור הווי ממנו יוצאים היריות

            //קביעת הערך ההתחלתי של שיעור האיקס של החללית
            Xsc = canvas.Width / 2 - SCpic.Width / 2;

            //2

            Xlr2 = -1 * Slant_lr_Pic.Width;
            Slant_lr_Y2 = -1 * Slant_lr_Pic.Height;

            Xrl2 = canvas.Width + Slant_rl_Pic.Width;
            Slant_rl_Y2 = -1 * Slant_rl_Pic.Height;

            RegularY2 = 0;
            regular_x2 = canvas.Width / 2;

            //קביעת החיים ההתחלתיים של האסטרואידים תלוי בכל שלב
            if (level == 1)
            {
                lr_life1 = 150;
                rl_life1 = 150;
                regular_life1 = 100;
            }
            else if (level == 2)
            {
                lr_life1 = 200;
                rl_life1 = 200;
                regular_life1 = 150;
            }
            else if (level == 3)
            {
                lr_life1 = 250;
                rl_life1 = 250;
                regular_life1 = 200;
            }

            //2
            //התאמת שלושת האסטרואידים האחרים לאסטרואידים הראשונים
            lr_life2 = lr_life1;
            rl_life2 = rl_life1;
            regular_life2 = regular_life1;

            //בכניסה הראשונה האסטרואידים עוד לא הושמדו
            regular_dead1 = false;
            slant_lr_dead1 = false;
            slant_rl_dead1 = false;

            //2
            regular_dead2 = true;
            slant_lr_dead2 = true;
            slant_rl_dead2 = true;

            //אתחול העצם וקביעת כמות היריות - התחמושת
            max_shots = 100;
            shots = new Shots(max_shots);

            //איפוס הנקודות
            score = 0;

            //אתחול העצמים של האסטרואידים והחללית
            slantRightToLeft1 = new slant_asteroid(Slant_rl_Y1, slant_deltaY, Xrl1, slant_deltaX, Slant_rl_Pic, canvas, rl_life1, -1);
            slantLeftToRight1 = new slant_asteroid(Slant_lr_Y1, slant_deltaY, Xlr1, slant_deltaX, Slant_lr_Pic, canvas, lr_life1, 1);
            regular1 = new asteroid(RegularY1, regular_deltaY, regular_x1, RegularPic, canvas, regular_life1);
            spaceship = new SpaceShip(Xsc, SCpic, canvas);

            //2
            slantRightToLeft2 = new slant_asteroid(Slant_rl_Y2, slant_deltaY, Xrl2, slant_deltaX, Slant_rl_Pic, canvas, rl_life2, -1);
            slantLeftToRight2 = new slant_asteroid(Slant_lr_Y2, slant_deltaY, Xlr2, slant_deltaX, Slant_lr_Pic, canvas, lr_life2, 1);
            regular2 = new asteroid(RegularY2, regular_deltaY, regular_x2, RegularPic, canvas, regular_life2);

            //קישור הסאונדים
            shotTaken = MediaPlayer.Create(context, Resource.Raw.shotmusic);
            destroyedAsteroid = MediaPlayer.Create(context, Resource.Raw.deadmusic);
            winSound = MediaPlayer.Create(context, Resource.Raw.WinSound);
            loseSound = MediaPlayer.Create(context, Resource.Raw.LoseSound);
            gameMusic = MediaPlayer.Create(context, Resource.Raw.gamemusic);
            gameMusic.Start(); //התחלת מוזיקת הרקע

            //התרחשה כבר כניסה ראשונה ולכן משתנה זה שקר כעת
            first_enter = false;

        }
        //פעולה המציירת על הקנבס
        public void Draw()
        {
            canvas = this.Holder.LockCanvas(); //נועל את הקנבס ולא מאפשר לאחרים לצייר עליו
            canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
            //קביעת ערכים התחלתיים אם זוהי הכניסה הראשונה למשחק
            if (first_enter)
                FirstEnter();

            //יצירת מסך רקע זז - יש שני מסכים שיורדים באותו זמן בתזמון מושלם כדי להיצור אשליה כאילו זה מסך אחד
            MoveScreen(canvas, Backpic, 0, SpeedOfTheMovingScreen, canvas.Width, canvas.Height); //המסך הראשון שיורד
            MoveScreen(canvas, Backpic, 0, SpeedOfTheMovingScreen - canvas.Height, canvas.Width, canvas.Height); //המסך השני שיורד
            SpeedOfTheMovingScreen = SpeedOfTheMovingScreen % canvas.Height; //התאמה של מהירות הירידה
            SpeedOfTheMovingScreen += 8; 

            //אם האסטרואיד עוד לא הושמד נבצע עליו מספר פעולות
            if (!slant_rl_dead1)
            {
                slantRightToLeft1.Draw(); //ציור האסטרואיד על הקנבס                
                canvas.DrawText(rl_life1.ToString(), Xrl1 + Slant_rl_Pic.Width / 2, Slant_rl_Y1 + Slant_rl_Pic.Height - 80, rl_paint1);  //שיוך של הטקסט המציג את החיים לאסטרואיד
                //הנעה של האסטרואיד ועדכון המיקום במשתנים הרלוונטים
                slantRightToLeft1.Move();
                Slant_rl_Y1 = slantRightToLeft1.GetY();
                Xrl1 = slantRightToLeft1.GetX();

            }
            if (!slant_lr_dead1)
            {
                slantLeftToRight1.Draw(); //ציור האסטרואיד על הקנבס               
                canvas.DrawText(lr_life1.ToString(), Xlr1 + Slant_lr_Pic.Width / 2, Slant_lr_Y1 + Slant_lr_Pic.Height - 80, lr_paint1);  //שיוך של הטקסט המציג את החיים לאסטרואיד
                //הנעה של האסטרואיד ועדכון המיקום במשתנים הרלוונטים
                slantLeftToRight1.Move();
                Slant_lr_Y1 = slantLeftToRight1.GetY();
                Xlr1 = slantLeftToRight1.GetX();

            }
            if (!regular_dead1)
            {
                regular1.Draw(); //ציור האסטרואיד על הקנבס         
                canvas.DrawText(regular_life1.ToString(), regular_x1 + RegularPic.Width / 2, RegularY1 + RegularPic.Height / 2, regular_paint1);  //שיוך של הטקסט המציג את החיים לאסטרואיד
                //הנעה של האסטרואיד ועדכון המיקום במשתנים הרלוונטים
                regular1.Move();
                RegularY1 = regular1.GetY();
            }

            //2
            if (!slant_rl_dead2)
            {
                slantRightToLeft2.Draw(); //ציור האסטרואיד על הקנבס                
                canvas.DrawText(rl_life2.ToString(), Xrl2 + Slant_rl_Pic.Width / 2, Slant_rl_Y2 + Slant_rl_Pic.Height - 80, rl_paint2);  //שיוך של הטקסט המציג את החיים לאסטרואיד
                //הנעה של האסטרואיד ועדכון המיקום במשתנים הרלוונטים
                slantRightToLeft2.Move();
                Slant_rl_Y2 = slantRightToLeft2.GetY();
                Xrl2 = slantRightToLeft2.GetX();

            }
            if (!slant_lr_dead2)
            {
                slantLeftToRight2.Draw(); //ציור האסטרואיד על הקנבס               
                canvas.DrawText(lr_life2.ToString(), Xlr2 + Slant_lr_Pic.Width / 2, Slant_lr_Y2 + Slant_lr_Pic.Height - 80, lr_paint2);  //שיוך של הטקסט המציג את החיים לאסטרואיד
                //הנעה של האסטרואיד ועדכון המיקום במשתנים הרלוונטים
                slantLeftToRight2.Move();
                Slant_lr_Y2 = slantLeftToRight2.GetY();
                Xlr2 = slantLeftToRight2.GetX();

            }
            if (!regular_dead2)
            {
                regular2.Draw(); //ציור האסטרואיד על הקנבס         
                canvas.DrawText(regular_life2.ToString(), regular_x2 + RegularPic.Width / 2, RegularY2 + RegularPic.Height / 2, regular_paint2);  //שיוך של הטקסט המציג את החיים לאסטרואיד
                //הנעה של האסטרואיד ועדכון המיקום במשתנים הרלוונטים
                regular2.Move();
                RegularY2 = regular2.GetY();
            }
            if (slant_lr_dead1 && slant_rl_dead1 && regular_dead1 && slant_lr_dead2 && slant_rl_dead2 && regular_dead2) //אם כל האסטרואידים הושמדו נגמר המשחק
            {
                myTimer.isRunning = false; //זמן המשחק הופסק
                isRunning = false; //המשחק לא רץ
                ScoreBoardActivity.Insert(score, myTimer.GetKeepTimer() - myTimer.getTimer(), username, level); //שמירה של הנתונים - שם השחקן, התוצאה, השלב והזמן
                checkWinOrLose(); //בדיקה אם השחקן ניצח או הפסיד
            }

            //ציור החללית על הקנבס                        
            spaceship.Draw();

            //ציור היריות והזזתן על המסך
            //אם היריה פגעה באסטרואיד היא תעלם ולאסטרואיד ירדו חיים
            if (NumOfShots <= max_shots && NumOfShots > 0)
            {
                //ציור היריות הזזתן
                shots.Draw1();
                shots.Move1();
                //בדיקה אם יריה כלשהי פגעה באסטרואיד
                for (int i = 0; i < shots.GetShots().Length; i++)
                {
                    if (shots.GetShots()[i] != null) //אם ישנה יריה
                    {
                        if (!slant_rl_dead1 && slantRightToLeft1.GotShot(shots.GetShots()[i].GetX(), shots.GetShots()[i].GetY())) //בדיקה אם היריה פגעה באסטרואיד שקיים
                        {
                            rl_life1 -= 50; //הורדה של 50 מהחיים של האסטרואיד
                            score += 50; //הוספה של 50 לניקוד
                            if (rl_life1 <= 0) //אם החיים של האסטרואיד הגיעו ל0 כלומר הוא הושמד
                            {
                                slant_rl_dead1 = true; //האסטרואיד התפוצץ
                                destroyedAsteroid.Start(); //סאונד של פיצוץ האסטרואיד מופעל
                            }
                            shots.GetShots()[i] = null; //היריה תעלם
                            break; //יציאה מהלולאה
                        }
                        else if (!slant_lr_dead1 && slantLeftToRight1.GotShot(shots.GetShots()[i].GetX(), shots.GetShots()[i].GetY()))//בדיקה אם היריה פגעה באסטרואיד שקיים
                        {
                            lr_life1 -= 50; //הורדה של 50 מהחיים של האסטרואיד
                            score += 50; //הוספה של 50 לניקוד
                            if (lr_life1 <= 0) //אם החיים של האסטרואיד הגיעו ל0 כלומר הוא הושמד
                            {
                                slant_lr_dead1 = true; //האסטרואיד התפוצץ
                                destroyedAsteroid.Start(); //סאונד של פיצוץ האסטרואיד מופעל
                            }
                            shots.GetShots()[i] = null; //היריה תעלם
                            break; //יציאה מהלולאה

                        }
                        else if (!regular_dead1 && regular1.GotShot(shots.GetShots()[i].GetX(), shots.GetShots()[i].GetY())) //בדיקה אם היריה פגעה באסטרואיד שקיים
                        {
                            regular_life1 -= 50; //הורדה של 50 מהחיים של האסטרואיד
                            score += 50; //הוספה של 50 לניקוד
                            if (regular_life1 <= 0) //אם החיים של האסטרואיד הגיעו ל0 כלומר הוא הושמד
                            {
                                regular_dead1 = true; //האסטרואיד התפוצץ
                                destroyedAsteroid.Start(); //סאונד של פיצוץ האסטרואיד מופעל
                            }
                            shots.GetShots()[i] = null; //היריה תעלם
                            break; //יציאה מהלולאה
                        }
                        //2
                        else if (!slant_rl_dead2 && slantRightToLeft2.GotShot(shots.GetShots()[i].GetX(), shots.GetShots()[i].GetY())) //בדיקה אם היריה פגעה באסטרואיד שקיים
                        {
                            rl_life2 -= 50; //הורדה של 50 מהחיים של האסטרואיד
                            score += 50; //הוספה של 50 לניקוד
                            if (rl_life2 <= 0) //אם החיים של האסטרואיד הגיעו ל0 כלומר הוא הושמד
                            {
                                slant_rl_dead2 = true; //האסטרואיד התפוצץ
                                destroyedAsteroid.Start(); //סאונד של פיצוץ האסטרואיד מופעל
                            }
                            shots.GetShots()[i] = null; //היריה תעלם
                            break; //יציאה מהלולאה
                        }
                        else if (!slant_lr_dead2 && slantLeftToRight2.GotShot(shots.GetShots()[i].GetX(), shots.GetShots()[i].GetY()))//בדיקה אם היריה פגעה באסטרואיד שקיים
                        {
                            lr_life2 -= 50; //הורדה של 50 מהחיים של האסטרואיד
                            score += 50; //הוספה של 50 לניקוד
                            if (lr_life2 <= 0)  //אם החיים של האסטרואיד הגיעו ל0 כלומר הוא הושמד
                            {
                                slant_lr_dead2 = true; //האסטרואיד התפוצץ
                                destroyedAsteroid.Start(); //סאונד של פיצוץ האסטרואיד מופעל
                            }
                            shots.GetShots()[i] = null; //היריה תעלם
                            break; //יציאה מהלולאה

                        }
                        else if (!regular_dead2 && regular2.GotShot(shots.GetShots()[i].GetX(), shots.GetShots()[i].GetY())) //בדיקה אם היריה פגעה באסטרואיד שקיים
                        {
                            regular_life2 -= 50; //הורדה של 50 מהחיים של האסטרואיד
                            score += 50; //הוספה של 50 לניקוד
                            if (regular_life2 <= 0) //אם החיים של האסטרואיד הגיעו ל0 כלומר הוא הושמד
                            {
                                regular_dead2 = true; //האסטרואיד התפוצץ
                                destroyedAsteroid.Start(); //סאונד של פיצוץ האסטרואיד מופעל
                            }
                            shots.GetShots()[i] = null; //היריה תעלם
                            break; //יציאה מהלולאה
                        }
                    }
                }
            }
            //פעולה המחכה 3 שניות ואז מורידה את שלושת האסטרואידים הנוספים, בשלב 3 מחכה 4 שניות
            if (level == 3 && myTimer.getTimer() <= myTimer.GetKeepTimer() - 4 && fall == false) // אם זהו השלב השלישי 
            {
                //האסטרואידים נופלים בפעה הראשונה ולכן הם עוד לא התפוצצו
                regular_dead2 = false;
                slant_lr_dead2 = false;
                slant_rl_dead2 = false;
                fall = true;
            }
            if (level != 3 && myTimer.getTimer() <= myTimer.GetKeepTimer() - 3 && fall == false) //אם זהו כל שלב אחר
            {
                //האסטרואידים נופלים בפעה הראשונה ולכן הם עוד לא התפוצצו
                regular_dead2 = false; 
                slant_lr_dead2 = false;
                slant_rl_dead2 = false;
                fall = true;
            }

            spaceship_GotHurt(); //קריאה לפעולה שבודקת האם אסטרואיד פגע בחללית
            //ציור הניקוד, השלב וכפתור העצירה על הקנבס
            canvas.DrawText("Score: " + score, 680, 150, scoreText);
            canvas.DrawText("Level " + level, 300, 150, levelText);
            canvas.DrawBitmap(PausePic, 900, 60, null);


        }
        //פעולה הבודקת אם החללית נפגעה על ידי האסטרואידים
        public void spaceship_GotHurt()
        {
            if (!slant_rl_dead1 && slantRightToLeft1.Crash(Xsc, spaceship.GetY(),SCpic)) //בדיקה אם היריה פגעה באסטרואיד שקיים
            {
                rl_life1 = 0; //איפוס חייו של האסטרואיד
                slant_rl_dead1 = true; //האסטרואיד התפוצץ
                score -= 50; //הניקוד יורד ב50
                if (score < 0) //איפוס הניקוד במקרה שקטן מ0
                    score = 0;
                destroyedAsteroid.Start(); //סאונד של אסטרואיד שהתפוצץ מופעל
            }
            else if (!slant_lr_dead1 && slantLeftToRight1.Crash(Xsc, spaceship.GetY(), SCpic)) //בדיקה אם היריה פגעה באסטרואיד שקיים
            {
                lr_life1 = 0; //איפוס חייו של האסטרואיד
                slant_lr_dead1 = true; //האסטרואיד התפוצץ
                score -= 50; //הניקוד יורד ב50
                if (score < 0) //איפוס הניקוד במקרה שקטן מ0
                    score = 0;
                destroyedAsteroid.Start(); //סאונד של אסטרואיד שהתפוצץ מופעל
            }
            else if (!regular_dead1 && regular1.Crash(Xsc, spaceship.GetY(), SCpic)) //בדיקה אם היריה פגעה באסטרואיד שקיים
            {
                regular_life1 = 0; //איפוס חייו של האסטרואיד
                regular_dead1 = true; //האסטרואיד התפוצץ
                score -= 50; //הניקוד יורד ב50
                if (score < 0) //איפוס הניקוד במקרה שקטן מ0
                    score = 0;
                destroyedAsteroid.Start(); //סאונד של אסטרואיד שהתפוצץ מופעל
            }
            else if (!slant_rl_dead2 && slantRightToLeft2.Crash(Xsc, spaceship.GetY(), SCpic)) //בדיקה אם היריה פגעה באסטרואיד שקיים
            {
                rl_life2 = 0; //איפוס חייו של האסטרואיד
                slant_rl_dead2 = true; //האסטרואיד התפוצץ
                score -= 50; //הניקוד יורד ב50
                if (score < 0) //איפוס הניקוד במקרה שקטן מ0
                    score = 0;
                destroyedAsteroid.Start(); //סאונד של אסטרואיד שהתפוצץ מופעל
            }
            else if (!slant_lr_dead2 && slantLeftToRight2.Crash(Xsc, spaceship.GetY(), SCpic)) //בדיקה אם היריה פגעה באסטרואיד שקיים
            {
                lr_life2 = 0; //איפוס חייו של האסטרואיד
                slant_lr_dead2 = true; //האסטרואיד התפוצץ
                score -= 50; //הניקוד יורד ב50
                if (score < 0) //איפוס הניקוד במקרה שקטן מ0
                    score = 0;
                destroyedAsteroid.Start(); //סאונד של אסטרואיד שהתפוצץ מופעל
            }
            else if (!regular_dead2 && regular2.Crash(Xsc, spaceship.GetY(), SCpic)) //בדיקה אם היריה פגעה באסטרואיד שקיים
            {
                regular_life2 = 0; //איפוס חייו של האסטרואיד
                regular_dead2 = true; //האסטרואיד התפוצץ
                score -= 50; //הניקוד יורד ב50
                if (score < 0) //איפוס הניקוד במקרה שקטן מ0
                    score = 0;
                destroyedAsteroid.Start(); //סאונד של אסטרואיד שהתפוצץ מופעל
            }
        }

        //פעולה הבודקת מגע על המסך
        public override bool OnTouchEvent(MotionEvent e)
        {
            if (e.GetX() <= Xsc + SCpic.Width && e.GetX() >= Xsc && e.GetY() >= spaceship.GetY()) //הנגיעה במסך היא על החללית
            {
                if (MotionEventActions.Move == e.Action) // אם החללית נגררה
                {
                    Xsc = e.GetX() - 200; //עדכון שיעור האיקס של החללית לפי המיקום שהמשתמש החליק אליו
                    spaceship.SetX(Xsc);
                    shot = true; //נורתה יריה
                    shotTaken.Start(); //סאונד של יריה

                }
                if (MotionEventActions.Up == e.Action) //אם הייתה נגיעה בחללית
                {
                    shot = true; //נורתה יריה
                    shotTaken.Start(); //סאונד של יריה
                }
            }
            if (e.GetX() <= 1100 && e.GetX() >= 900 && e.GetY() >= 0 && e.GetY() <= 200) //לחיצה על כפתור העצירה
            {
                if (MotionEventActions.Up == e.Action)
                {
                    pause(); //עצירה של המשחק
                    handler.Post(PauseDialog); //הופעת דיאלוג של עצירה
                }
            }
            if (first_shot && shot) //אם יצאה יריה והיא הראשונה
            {
                Shot s = new Shot(Xsc + SCpic.Width / 2, shotY, ShotPic, spaceship.GetCanvas()); //יצירה של יריה ואתחול המיקום
                shots.Add(s, ++NumOfShots);//הוספה של היריה למערך
                last = DateTime.Now; //שמירה של הזמן בה נורה
                first_shot = false; //יריה ראשונה כבר נורתה
                shot = false; //בינתיים עוד יריה
            }
            else if (!first_shot && shot) //אם יצאה יריה והיא לא הראשונה
            {
                timeDifference = DateTime.Now - last; //הזמן שעבר בין היריה האחרונה ליריה הזאת
                if ((int)timeDifference.Seconds >= 0.000001) //בדיקה אם עבר מספיק זמן כדי לשחרר את היריה השנייה
                {
                    Shot s = new Shot(Xsc + SCpic.Width / 2, shotY, ShotPic, spaceship.GetCanvas()); //יצירה של יריה ואתחול המיקום
                    shots.Add(s, ++NumOfShots);//הוספה של היריה למערך
                    last = DateTime.Now; //עדכון הזמן העדכני
                    shot = false; //בינתיים עוד יריה
                }
            }

            return true;
        }
        //פעולה המזיזה את מסך הרקע
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void MoveScreen(Canvas canvas, Bitmap bitmap, int x, int y, int width, int height)
        {
            Rect source = new Rect(0, 0, bitmap.Width, bitmap.Height); //המיקום שתמונת הרקע מתחילה בו
            Rect target = new Rect(x, y, x + width, y + height); //המיקום שאליו תגיע תמונת הרקע
            canvas.DrawBitmap(bitmap,source, target, null); //מצייר את התמונה ומזיז אותה באופן אוטומטי

        }
        //הפעולה שמריצה את כל המשחק
        public void Run()
        {

            while (threadRunning) //while the game thread is running
            {
                if (isRunning) //אם המשחק רץ
                {
                    if (!this.Holder.Surface.IsValid) //אם לא ניתן לצייר בקנבס מכיוון שהוא נעול, תמשיך בקוד
                        continue;
                    try
                    {
                        Draw(); //ציור כל האובייקטים
                        if (myTimer != null && myTimer.getTimer()>0) //אם הטיימר לא הסתיים תמשיך לצייר אותו, במידה ונשארו 5 שניות הזמן יצויר באדום במקום צהוב
                        {
                            if(myTimer.getTimer() <=5)
                                paintTimer.Color = Color.Red;
                            else
                                paintTimer.Color = Color.Yellow;

                            canvas.DrawText("" + myTimer.getTimer(), 50, 150, paintTimer);
                        }

                    }
                    catch (Exception e)
                    {

                    }
                    finally
                    {
                        if (canvas != null) //אם נוצר קנבס
                        {
                            this.Holder.UnlockCanvasAndPost(canvas); //תשחרר אותו ותצייר את הקנבס
                            if (myTimer.getTimer() == 0) //זמן המשחק הסתיים 
                            {
                                myTimer.isRunning = false; //הזמן של המשחק הופסק
                                isRunning = false; //המשחק לא רץ - הוא הסתיים
                                ScoreBoardActivity.Insert(score,myTimer.GetKeepTimer(),username,level); //שמירה של התוצאות
                                checkWinOrLose(); //בדיקה האם ניצח או הפסיד

                            }
                        }
                        
                    }
                }
                
            }
            
        }
        //פעולה הבודקת האם השחקן ניצח - אם התוצאה שלו גבוהה מהמינימום לניצחון
        public void checkWinOrLose()
        {
            gameMusic.Stop(); //עצירה של מוזיקת הרקע
            if (score >= scoreWin) //אם כן שולחת אותו לדיאלוג של הניצחון
            {
                handler.Post(WinDialog);
            }
            else //אם לא שולחת אותו לדיאלוג של ההפסד
            {
                handler.Post(LoseDialog);
            }
                
        }
        //דיאלוג של הניצחון
        public void WinDialog()
        {
            winSound.Start(); //סאונד של ניצחון
            //יצירה של הדיאלוג
            dWin = new Dialog(context);
            dWin.SetContentView(Resource.Layout.WinPop);
            dWin.SetTitle("Reset");
            dWin.SetCancelable(false);

            TextView score_tv = dWin.FindViewById<TextView>(Resource.Id.score_tv);
            TextView level_tv = dWin.FindViewById<TextView>(Resource.Id.level_tv);
            TextView time_tv = dWin.FindViewById<TextView>(Resource.Id.time_tv);
            TextView username_tv = dWin.FindViewById<TextView>(Resource.Id.username_tv);
            ImageButton star1 = dWin.FindViewById<ImageButton>(Resource.Id.star1);   
            ImageButton star2 = dWin.FindViewById<ImageButton>(Resource.Id.star2);
            ImageButton star3 = dWin.FindViewById<ImageButton>(Resource.Id.star3);
            ImageButton home = dWin.FindViewById<ImageButton>(Resource.Id.home);
            ImageButton restart = dWin.FindViewById<ImageButton>(Resource.Id.restart);
            ImageButton next = dWin.FindViewById<ImageButton>(Resource.Id.next);

            //אם זה השלב השלישי לא יופיע כפתור של מעבר לשלב הבא מכיוון שזהו השלב האחרון
            if (level == 3)
                next.Visibility = ViewStates.Invisible;
            else
                next.Visibility = ViewStates.Visible;

            home.Click += Home_Click;
            restart.Click += Restart_Click1;
            next.Click += Next_Click;

            //העברה של הניקוד, השלב, הזמן ושם המשתמש לדיאלוג
            score_tv.Text = "score: " + score;
            level_tv.Text = "Level " + level;
            time_tv.Text = "time: " + (myTimer.GetKeepTimer() - myTimer.getTimer());
            username_tv.Text = username + "!";

            if (score == MaxScore) //אם התוצאה היא מקסימלית יהיו שלושה כוכבים מלאים
            {
                star1.SetBackgroundResource(Resource.Drawable.fullstar);
                star2.SetBackgroundResource(Resource.Drawable.fullstar);
                star3.SetBackgroundResource(Resource.Drawable.fullstar);
            }
            else if(score >= (scoreWin+MaxScore)/2 && score<MaxScore) //אם התוצאה בין התוצאה המקסימלית לתוצאה לאמצע יהיו שני כוכבים מלאים ואחד ריק
            {
                star1.SetBackgroundResource(Resource.Drawable.fullstar);
                star2.SetBackgroundResource(Resource.Drawable.fullstar);
                star3.SetBackgroundResource(Resource.Drawable.emptystar);
            }
            else //אחרת יהיו שני כוכבים ריקים ואחד מלא
            {
                star1.SetBackgroundResource(Resource.Drawable.fullstar);
                star2.SetBackgroundResource(Resource.Drawable.emptystar);
                star3.SetBackgroundResource(Resource.Drawable.emptystar);
            }

            dWin.Show(); //הצגה של הדיאלוג של הניצחון
        }

        private void Next_Click(object sender, EventArgs e) //כפתור למעבר לשלב הבא
        {
            dWin.Dismiss(); //סיום הדיאלוג
            if (winSound != null) //עצירה של המוזיקה של הניצחון
                winSound.Stop();
            //שינוי הערכים לפי השלב הבא
            if (level == 1)
            {
                level = 2;
                scoreWin = 900;
                MaxScore = 1100;
                time = 30;
            }
            else if(level == 2)
            {
                level = 3;
                scoreWin = 1200;
                MaxScore = 1400;
                time = 40;
            }
            gameMusic.Start(); //התחלה של מוזיקת הרקע
            startGame(); //פעולה המתחילה את המשחק
            isRunning = true; //המשחק רץ
            myTimer.setTimer(time); //אתחול של זמן המשחק
            myTimer.isRunning = true; //הזמן רץ
            

        }

        private void Restart_Click1(object sender, EventArgs e) //כפתור להתחלת משחק חדש לאחר שניצח
        {
            if(winSound != null) //עצירה של המוזיקה של הניצחון
                winSound.Stop();
            dWin.Dismiss(); //סיום הדיאלוג
            gameMusic.Start(); //התחלה של מוזיקת הרקע
            startGame(); //פעולה המתחילה את המשחק
            isRunning = true; //המשחק רץ
            myTimer.setTimer(time); //אתחול של זמן המשחק
            myTimer.isRunning = true; //הזמן רץ
        }

        //דיאלוג של הפסד
        public void LoseDialog()
        {
            loseSound.Start();  //סאונד של הפסד
            //יצירה של הדיאלוג
            dLose = new Dialog(context);
            dLose.SetContentView(Resource.Layout.LosePop);
            dLose.SetTitle("Reset");
            dLose.SetCancelable(false);

            TextView score_tv = dLose.FindViewById<TextView>(Resource.Id.score_tv);
            TextView level_tv = dLose.FindViewById<TextView>(Resource.Id.level_tv);
            TextView time_tv = dLose.FindViewById<TextView>(Resource.Id.time_tv);
            TextView username_tv = dLose.FindViewById<TextView>(Resource.Id.username_tv);
            ImageButton star1 = dLose.FindViewById<ImageButton>(Resource.Id.star1);
            ImageButton star2 = dLose.FindViewById<ImageButton>(Resource.Id.star2);
            ImageButton star3 = dLose.FindViewById<ImageButton>(Resource.Id.star3);
            ImageButton home = dLose.FindViewById<ImageButton>(Resource.Id.home);
            ImageButton restart = dLose.FindViewById<ImageButton>(Resource.Id.restart);

            home.Click += Home_Click;
            restart.Click += Restart_Click2;

            //העברה של הניקוד, השלב, הזמן ושם המשתמש לדיאלוג
            score_tv.Text = "score: " + score;
            level_tv.Text = "Level " + level;
            time_tv.Text = "time: " + (myTimer.GetKeepTimer() - myTimer.getTimer());
            username_tv.Text = username + "!";

            //מכיוון שהשחקן הפסיד יש שלושה כוכבים ריקים
            star1.SetBackgroundResource(Resource.Drawable.emptystar);
            star2.SetBackgroundResource(Resource.Drawable.emptystar);
            star3.SetBackgroundResource(Resource.Drawable.emptystar);

            dLose.Show(); //הצגה של הדיאלוג
        }

        private void Restart_Click2(object sender, EventArgs e)//כפתור להתחלת משחק חדש לאחר שהפסיד
        {
            if(loseSound!=null) //עצירה של הסאונד של המוזיקה
                loseSound.Stop();
            dLose.Dismiss(); //סיום הדיאלוג
            gameMusic.Start(); //התחלת מוזיקת הרקע
            startGame(); //התחלת המשחק
            isRunning = true; //המשחק רץ
            myTimer.setTimer(time); //אתחול זמן המשחק
            myTimer.isRunning = true; //הזמן רץ
        }

        //דיאלוג של עצירה
        public void PauseDialog()
        {
            //יצירה של הדיאלוג
            dPause = new Dialog(context);
            dPause.SetContentView(Resource.Layout.PauseDialog);
            dPause.SetTitle("reset");
            dPause.SetCancelable(false);

            ImageButton resume = dPause.FindViewById<ImageButton>(Resource.Id.play);
            ImageButton home = dPause.FindViewById<ImageButton>(Resource.Id.home);
            ImageButton restart = dPause.FindViewById<ImageButton>(Resource.Id.restart);

            restart.Click += Restart_Click;
            home.Click += Home_Click;
            resume.Click += Resume_Click;
            dPause.Show(); //הצגה של הדיאלוג
            gameMusic.Pause(); //עצירה של מוזיקת הרקע
        }

        private void Resume_Click(object sender, EventArgs e) //כפתור הממשיך את המשחק לאחר שנעצר
        {
            gameMusic.Start(); //התחלת מוזיקת הרקע
            resume(); //פעולה הממשיכה את המשחק
            dPause.Dismiss(); //סיום של הדיאלוג
        }

        private void Home_Click(object sender, EventArgs e) //כפתור השולח למסך השלבים
        {
            destroy(); //פעולה המסיימת את המשחק

            //עצירה של הסאונד של הניצחון או ההפסד
            if(winSound!=null)
                winSound.Stop();
            if(loseSound!=null)
                loseSound.Stop();

        }

        private void Restart_Click(object sender, EventArgs e)//כפתור להתחלת משחק חדש באמצע המשחק
        {
            dPause.Dismiss(); //סיום של הדיאלוג
            startGame(); //התחלת המשחק
            isRunning = true; //המשחק רץ
            myTimer.setTimer(time); //אתחול זמן המשחק
            myTimer.isRunning = true; //הזמן רץ
        }
        //פעולה המסיימת את המשחק
        public void destroy()
        {
            gameMusic.Stop(); //עצירה של מוזיקת הרקע
            isRunning = false; //המשחק לא רץ
            myTimer.threadRunning = false; //הזמן לא רץ
            while (true)
            {
                try
                {
                    myTimer.thread.Join(); //Destroing the time thread
                }
                catch (Exception ex)
                {

                }
                break;
            }
            myTimer = null;
            ((BoardGameActivity)context).Finish(); //קריאה לפעולה שמשלימה את סיום המשחק
        }
        //פעולה העוצרת את המשחק
        public void pause()
        {
            isRunning = false; //המשחק לא רץ
            if (myTimer != null) //עצירה של זמן המשחק
                myTimer.StopTimer();
        }
        //פעולה הממשיכה את המשחק לאחר שנעצר או מתחילה אותו
        public void resume()
        {
            isRunning = true; //המשחק רץ
            if (myTimer != null) //המשכת הזמן
                myTimer.ResumeTimer();
            else if (myTimer == null) //אם לא נוצר זמן, נאתחל אותו ונתחיל אותו
            {
                myTimer = new Timer(time);
                myTimer.Start();

            }
            if (ts == null) //אם לא נוצר המשחק בעבר יש ליצור את הט'רד ולהתחיל אותו
            {
                this.ts = new ThreadStart(this.Run);
                this.gameThread = new Thread(this.ts);
                this.gameThread.Start();
            }
            
        }
        //פעולה המאפסת את הערכים כדי להתחיל את המשחק מחדש
        public void startGame()
        {
            slantRightToLeft1 = null; //אסטרואיד הנע באלכסון מהפינה הימינית העליונה לפינה השמאלית התחתונה
            slantLeftToRight1 = null; //אסטרואיד הנע באלכסון מהפינה השמאלית העליונה לפינה הימנית התחתונה
            regular1 = null; //אסטרואיד הנע כלפי מטה אך ורק בציר האנכי

            //האסטרואידים שמגיעים מאוחר יותר
            slantRightToLeft2 = null;
            slantLeftToRight2 = null;
            regular2 = null;

            spaceship = null; //יצירת עצם של החללית

            shots = null; //יצירת ירייה
            NumOfShots = 0; //מספר יריות שכבר נורו      
            shot = false; //משתנה הבודק אם יצאה יריה
            first_shot = true; //משתנה הבודק אם זאת יריה ראשונה
            first_enter = true; //משתנה הבודק אם זאת כניסה ראשונה למשחקx
            fall = false; //משתנה הגורם לאסטרואידים השניים לרדת
            score = 0; //אתחול הניקוד

        }
        //פעולה המשנה את הגודל של התמונה
        public Bitmap ResizeBitmap(Bitmap originalImage, int widthToScae, int heightToScale) // הפעולה מקבלת תמונה ואורך ורוחב רצוי שהתמונה תהיה בה
        {
            Bitmap resizedBitmap = Bitmap.CreateBitmap(widthToScae, heightToScale, Bitmap.Config.Argb8888); //גודל התמונה הרצוי

            float originalWidth = originalImage.Width; //רוחב התמונה הרצויה
            float originalHeight = originalImage.Height; //גובה התמונה הרצויה

            Canvas canvas = new Canvas(resizedBitmap);

            float scale = resizedBitmap.Width / originalWidth;

            float xTranslation = 0.0f;
            float yTranslation = (resizedBitmap.Height - originalHeight * scale) / 2.0f;

            Matrix transformation = new Matrix(); //מחלקה שיש לה מטריצה של 3 על 3
            transformation.PostTranslate(xTranslation, yTranslation); //התאמה של הערכים הרצויים לתמונה הנוכחית שקיבלנו
            transformation.PreScale(scale, scale);

            Paint paint = new Paint();
            paint.FilterBitmap = true;

            canvas.DrawBitmap(originalImage, transformation, paint);

            return resizedBitmap; //החזרת התמונה עם הגודל הרצוי
        }






    }
}
