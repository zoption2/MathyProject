using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace Randoms.DailyReward
{
    using Internals;

    public class DailyRewardManager: MonoBehaviour
    {
        #region FIELDS

        [Header("REWARD CONFIG:")]
        public RewardIntervalType RewardIntervalType = RewardIntervalType.Daily;
        public TMP_Text TimeCounterText;
        public int NextRewardInterval = 20;

        [Header("DEBUG:")]
        [SerializeField] private bool isDebugEnabled;
        [SerializeField] private TMP_Text debugTextLabel;

        public static DailyRewardManager Instance { get; private set; }
        private bool isInitialized = false;
        private bool canRefreshUI  = true;
        private DailyRewardBtn activeBtn;

        #endregion

        void Awake ()
        {
            if (Instance) Destroy (this);
            Instance = this;
        }
        
        void Start()
        {
            isInitialized = true;
            StartCoroutine (CountTimer());
            Init ();
        }

        void OnEnable ()
        {
            Log(PlayerPrefs.GetString ("RANDOMS_DAILYREWARD_STORE"));
            if (!isInitialized) return;
            Init ();
        }

        void OnDisable()
        {
            StopAllCoroutines ();
        }

        void OnDestroy()
        {
            StopAllCoroutines ();
            DailyRewardBtn.dailyRewardBtns.Clear (); 
        }

        public void ResetToDefault()
        {
            DailyRewardInternal.ResetToDefault();
            StopAllCoroutines();
            isInitialized = true;
            Init();
        }

        public void CollectReward ()
        {
            activeBtn.onRewardCollect?.Invoke ();
        }

        public void Collect2XReward ()
        {
            activeBtn.on2XRewardCollect?.Invoke ();
        }

        private void Log(string message)
        {
            if (isDebugEnabled)
            {
                Debug.Log(message);
                if (debugTextLabel != null)
                {
                    debugTextLabel.text += "\r\n" + message;
                }
            }
        }

        /// <summary>
        /// Invokes Action On Btns
        /// </summary>
        void Init ()
        {
            //Debug.Log($"dailyRewardBtns.Count = {DailyRewardBtn.dailyRewardBtns.Count}");
            foreach (var btn in DailyRewardBtn.dailyRewardBtns)
            {
                btn.Init ();
                var (canClaim, status) = DailyRewardInternal.GetDailyRewardStatus (btn.day);
                //Debug.Log($"Day {btn.day} button - canClaim = " + canClaim);
                switch (status)
                {
                    case DailyRewardStatus.CLAIMED:  btn.OnClaimedState?.Invoke (); break;
                    case DailyRewardStatus.UNCLAIMED_UNAVAILABLE:  btn.OnClaimUnAvailableState?.Invoke();  break;
                }
                
                // ative btn
                if (status == DailyRewardStatus.UNCLAIMED_AVAILABLE && canClaim)
                {
                    activeBtn = btn;
                    btn.OnClaimState?.Invoke ();
                    btn.btn.onClick.AddListener (()=> DailyRewardInternal.ClaimTodayReward (()=> {
                        Init ();
                        btn.onClick?.Invoke (); 
                    }));
                }
            }
        }   
        
        IEnumerator CountTimer ()
        {
            while (true)
            {
                yield return new WaitForSeconds (1f);
                if (TimeCounterText)
                {
                    TimeCounterText.text = "" + DailyRewardInternal.NextRewardTimer();
                }

                if (DailyRewardInternal.CanClaimTodayReward () && canRefreshUI)
                {
                    canRefreshUI = false;
                    Init ();
                }
                else
                {
                    canRefreshUI = true;
                }
            }
        }
    }
    /*   public class MyScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var myScript = target as DailyRewardManager;

            myScript.RewardIntervalType = (RewardIntervalType)EditorGUILayout.EnumPopup("Reward Interval Type:", myScript.RewardIntervalType);

            if (myScript.RewardIntervalType == RewardIntervalType.CustomInterval)
                myScript.NextRewardInterval = EditorGUILayout.IntField("Next Reward Interval:", myScript.NextRewardInterval);

            myScript.TimeCounterText = (TMP_Text)EditorGUILayout.ObjectField("Time Counter Text:", myScript.TimeCounterText, typeof(TMP_Text), true);
        }
    }*/

    public enum RewardIntervalType
    {
        Daily = 0,
        TwentyFourHours = 1,
        CustomInterval = 2
    }
}