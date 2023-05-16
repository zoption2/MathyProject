using UnityEngine;
using System;
using System.Collections.Generic;

namespace Mathy.UI
{
    public class ComplexAnimatorController : BaseViewAnimator
    {
        [SerializeField] private List<BaseViewAnimator> _showingAnimators;
        [SerializeField] private List<BaseViewAnimator> _hidingAnimators;

        public override void AnimateShowing(Action onComplete)
        {
            if (_showingAnimators.Contains(this))
            {
                _showingAnimators.Remove(this);
            }

            int animatorsInProgress = 0;
            for (int i = 0, j = _showingAnimators.Count; i < j; i++)
            {
                animatorsInProgress++;
                _showingAnimators[i].AnimateShowing(Complete);
            }

            void Complete()
            {
                animatorsInProgress--;
                CheckAnimatorsProgress();
            }

            void CheckAnimatorsProgress()
            {
                if (animatorsInProgress == 0)
                {
                    onComplete?.Invoke();
                }
            }
        }

        public override void AnimateHiding(Action onComplete)
        {
            if (_hidingAnimators.Contains(this))
            {
                _hidingAnimators.Remove(this);
            }

            int animatorsInProgress = 0;
            for (int i = 0, j = _hidingAnimators.Count; i < j; i++)
            {
                animatorsInProgress++;
                _hidingAnimators[i].AnimateHiding(Complete);
            }

            void Complete()
            {
                animatorsInProgress--;
                CheckAnimatorsProgress();
            }

            void CheckAnimatorsProgress()
            {
                if (animatorsInProgress == 0)
                {
                    onComplete?.Invoke();
                }
            }
        }
    }
}

