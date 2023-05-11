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
    }
}

