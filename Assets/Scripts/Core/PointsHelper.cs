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
    }
}

