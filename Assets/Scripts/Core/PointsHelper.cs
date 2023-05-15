using System.Collections.Generic;
using UnityEngine;

namespace Mathy
{
    public static class PointsHelper
    {
        public static Achievements GetDayReward(int rate)
        {
            rate = Mathf.Clamp(rate, 0, 100);
            switch (rate)
            {
                case int x when (x >= 90 && x <= 100):
                    return Achievements.GoldMedal;
                case int x when (x >= 80 && x <= 89):
                    return Achievements.SilverMedal;
                case int x when (x >= 70 && x <= 79):
                    return Achievements.BronzeMedal;
                default:
                    return Achievements.none;
            }
        }

        public static int GetExperiencePointsByRate(int rate)
        {
            rate = Mathf.Clamp(rate, 0, 100);
            switch (rate)
            {
                case int x when (x >= 96): return 200;
                case int x when (x >= 92 && x <= 95): return 150;
                case int x when (x >= 89 && x <= 91): return 120;
                case int x when (x >= 86 && x <= 88): return 100;
                case int x when (x >= 82 && x <= 85): return 90;
                case int x when (x >= 79 && x <= 81): return 80;
                case int x when (x >= 76 && x <= 78): return 70;
                case int x when (x >= 72 && x <= 75): return 60;
                case int x when (x >= 69 && x <= 71): return 50;
                case int x when (x >= 66 && x <= 68): return 40;
                case int x when (x >= 62 && x <= 65): return 30;
                case int x when (x >= 59 && x <= 61): return 20;
                case int x when (x >= 1 && x <= 68): return 10;
                default:
                    return 0;
            }
        }


        private static readonly List<int> levelUpValues = new List<int>
        { 500, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000,
        10000, 11000, 12000, 13000, 14000, 15000, 16000, 17000, 18000, 19000};

        public static int GetMaxExperienceOfRank(int rank)
        {
            var lastRankValue = levelUpValues[levelUpValues.Count - 1];
            rank = Mathf.Clamp(rank, 0, lastRankValue);
            return levelUpValues[rank];
        }

        public static int GetRankByExperience(int value)
        {
            int rank = 0;
            for (int i = 0, j = levelUpValues.Count; i < j; i++)
            {
                var xp = levelUpValues[i];
                if (value >= xp)
                {
                    rank = i + 1;
                }
                else break;
            }
            return rank;
        }
    }
}

