using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mathy.UI
{
    public class CalendarCellButton : MonoBehaviour
    {
        #region FIELDS

        [Header("COMPONENTS:")]
        [SerializeField] protected TMP_Text dateText;
        [SerializeField] protected Button button;
        [SerializeField] protected Image image;

        protected CalendarManager calendarManager;  //Calendar Script (attached to UI text for selected month
        public int OriginalNumber;                  //Original number of each button

        public int CurrentNumber
        {
            get => OriginalNumber - calendarManager.FirstOfMonthDay() + 1;
        }

        #endregion

        #region INITIALIZATION

        public virtual void Initialize()
        {
            calendarManager = CalendarManager.Instance;
            //Init();
        }

        #endregion

        #region PUBLIC

        //Change the text in the button to the day of selected month
        //Blank out buttons that are not numbered
        public virtual void Init(DateTime date)
        {
            if (CurrentNumber <= 0 || CurrentNumber > calendarManager.DaysInMonth)
            {
                image.enabled = false;
                button.enabled = false;
                dateText.text = "";
            }
            else
            {
                dateText.text = CurrentNumber.ToString();
                button.enabled = true;
                image.enabled = true;
            }
        }

        #endregion
    }
}
