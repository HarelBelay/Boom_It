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

namespace Boom_It
{
    [Activity(Label = "Instructions")]
    public class Instructions : Activity
    {
        TextView tv; //טקסט עם הוראות המשחק
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.instructions);

            tv = FindViewById<TextView>(Resource.Id.tv);
            tv.Text = "Hi and welcome to my game 'Boom It'!\n" +
                "In this game you are the spaceship and your mission is to destroy the asteroids that are coming towards you.\n" +
                "There are 6 asteroids in total, on each level they have different amounts of points. They are falling from top to bottom and then appear again at the top. The spaceship can only move left and right." +
                " You can destroy the asteroids by shooting the shots you have in your spaceship, there are 100 shots but don't worry it can't be over." +
                " The shooting can be done by moving the spaceship left and right or by touching it.\n" +
                "Each shot that hits the asteroid deducts from it 50 points and adds it to the score. When an asteroid hits the spaceship it deducts from the score 50 points.\n" +
                "In order to win the game and to get one star you need a certain amount of points, to get three stars you need to destroy all asteroids, and to get two stars you need the score between them. You can see more in the chart.";
        }

    }
}