using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace w4ndrv.Core
{
    public static class User 
    {
        //Name
        private static string _userName { get; set; }
        public static string UserName => _userName;

        //Score
        private static int _score { get; set; }
        public static int Score => _score;

        //Currency
        //Abilities 
        //.....

        public static void SetUserName(string name) => _userName = name;
        public static void UpdateScore(int score) => _score = score;


    }

}
