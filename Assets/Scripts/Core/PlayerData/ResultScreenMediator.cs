using Mathy.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mathy.Services
{
    public interface IResultScreenMediator : IView
    {

    }


    public class ResultScreenMediator : IResultScreenMediator
    {
        private const string kResultScreenTable = "ResultScreen";


        private readonly IAddressableRefsHolder _refsHolder;
        private readonly IResultScreenSkillsController _skillResults;
        private IResultScreenView _view;

        public ResultScreenMediator(IAddressableRefsHolder refsHolder, IResultScreenSkillsController skillResults)
        {
            _refsHolder = refsHolder;
            _skillResults = skillResults;
        }

        public async void Show(Action onShow)
        {
            _view = await _refsHolder.PopupsProvider.InstantiateFromReference<IResultScreenView>(Popups.ResultScreen, null);
            InitSkillResults();
        }

        public void InitSkillResults()
        {
            IResultScreenSkillResultsView view = _view.SkillResults;
            _skillResults.Init(view);
        }



        public void Hide(Action onHide)
        {
            throw new NotImplementedException();
        }

        public void Release()
        {
            throw new NotImplementedException();
        }
    }

    public class SkillResultPopupController
    {

    }
}


