using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mathy.Core
{
    public class VibrationManager : StaticInstance<VibrationManager>
    {
        #region FIELDS

        [SerializeField] int inputTime = 50;

        #endregion

        void Start()
        {
            Vibration.Init();
        }

        public void TapVibrate()
        {
            if (GameSettingsManager.Instance.isVibrationEnabled)
            {
                Vibration.Vibrate();
            }   
        }

        public void TapVibrateCustom()
        {
            if (GameSettingsManager.Instance.isVibrationEnabled)
            {
                Vibration.Vibrate(inputTime);
            } 
        }

        /*public void TapVibratePattern()
        {
            string[] patterns = inputPattern.text.Replace(" ", "").Split(',');
            long[] longs = Array.ConvertAll<string, long>(patterns, long.Parse);

            Debug.Log(longs.Length);
            //Vibration.Vibrate ( longs, int.Parse ( inputRepeat.text ) );

            Vibration.Vibrate(longs, int.Parse(inputRepeat.text));
        }*/

        public void TapCancelVibrate()
        {
            Vibration.Cancel();
        }

        public void TapPopVibrate()
        {
            if (GameSettingsManager.Instance.isVibrationEnabled)
            {
                Vibration.VibratePop();
            }
        }

        public void TapPeekVibrate()
        {
            if (GameSettingsManager.Instance.isVibrationEnabled)
            {
                Vibration.VibratePeek();
            }
        }

        public void TapNopeVibrate()
        {
            if (GameSettingsManager.Instance.isVibrationEnabled)
            {
                Vibration.VibrateNope();
            } 
        }
    }
}
