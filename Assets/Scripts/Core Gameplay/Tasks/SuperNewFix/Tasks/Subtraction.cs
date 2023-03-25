using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using CustomRandom;
using UnityEngine;

namespace Mathy.Core.Tasks.DailyTasks
{
    public class Subtraction : ArithmeticTask
    {
        public Subtraction(int seed, ScriptableTask taskSettings)
        {
            this.variants = new List<Variant>();
            this.operators = new List<Operator>();

            this.TaskSettings = taskSettings;

            //Setting seed
            Random = new FastRandom(seed);
            this.Seed = seed;

            CreateTaskElementsAsync();
        }

        protected override async System.Threading.Tasks.Task CreateOperators()
        {
            this.operators.Add(new Operator(ArithmeticSigns.Minus));
            //last operator is always =
            this.operators.Add(new Operator(ArithmeticSigns.Equal));
        }

        protected async override System.Threading.Tasks.Task CreateElements()
        {
            int first = this.Random.Range(TaskSettings.BaseStats.MinNumber, TaskSettings.BaseStats.MaxNumber);
            int second = this.Random.Range(TaskSettings.BaseStats.MinNumber, first);

            this.Elements.Add(new TaskElement(first));
            this.Elements.Add(new TaskElement(second));
            this.Elements.Add(new TaskElement(ArithmeticSigns.QuestionMark));
        }

        protected override async UniTask InitializeElementsView()
        {
            await base.InitializeElementsView();
            unknownElement = this.Elements[Elements.Count - 1].ElementView;
        }
    }
}