using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using Mathy.Core.Tasks.DailyTasks;
using Zenject;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using RANDOM = UnityEngine.Random;
using Mathy.Core.Tasks;

namespace Mathy
{
    public interface ITaskFactory
    {
        UniTask<ITaskController> CreateTask(ScriptableTask taskSettings, Transform parent);
        UniTask<ITaskController> CreateTaskFromRange(List<ScriptableTask> taskSettings, Transform parent);
    }


    public class TaskFactory : ITaskFactory
    {
        private DiContainer container;
        private IAddressableRefsHolder refsHolder;

        public TaskFactory(DiContainer container, IAddressableRefsHolder refsHolder)
        {
            this.container = container;
            this.refsHolder = refsHolder;
        }

        public async UniTask<ITaskController> CreateTaskFromRange(List<ScriptableTask> taskSettings, Transform parent)
        {
            var selected = GetRandomSettingFromList(taskSettings);
            ITaskController controller = await CreateTaskInternal(selected, parent);
            return controller;
        }

        public async UniTask<ITaskController> CreateTask(ScriptableTask taskSettings, Transform parent)
        {
            var controller = await CreateTaskInternal(taskSettings, parent);
            return controller;
        }

        private async UniTask<ITaskController> CreateTaskInternal(ScriptableTask taskSettings, Transform viewParent)
        {
            switch (taskSettings.TaskType)
            {
                case TaskType.Addition:
                    var model = new AdditionTaskModel(taskSettings);
                    var controller = container.Resolve<AdditionTaskController>();
                    var view = await refsHolder.TaskViewProvider.InstantiateFromReference<IStandardTaskView>(TaskType.Addition, viewParent);
                    controller.Init(model, view);
                    return controller;

                case TaskType.Subtraction:

                case TaskType.Comparison:

                case TaskType.Multiplication:

                case TaskType.Division:

                case TaskType.ComplexAddSub:

                case TaskType.RandomArithmetic:

                case TaskType.MissingNumber:

                case TaskType.ImageOpening:

                case TaskType.PairsNumbers:

                case TaskType.PairsEquation:

                case TaskType.PairsOperands:

                case TaskType.ShapeGuessing:

                case TaskType.MissingSign:

                case TaskType.MissingMultipleSigns:

                case TaskType.IsThatTrue:

                case TaskType.ComparisonWithMissingNumber:

                case TaskType.ComparisonMissingElements:

                case TaskType.AddSubMissingNumber:

                case TaskType.ComparisonExpressions:

                case TaskType.SumOfNumbers:

                case TaskType.MissingExpression:

                default:
                    throw new ArgumentException(
                        string.Format("There is no implementation for task >>{0}<< creation", taskSettings.TaskType)
                        );
            }
        }

        //private async UniTask<ITaskView> InstantiateViewByRef(AssetReferenceGameObject reference, Transform parent)
        //{
        //    try
        //    {
        //        AsyncOperationHandle<GameObject> handler = reference.LoadAssetAsync<GameObject>();
        //        await handler;
        //        GameObject viewPrefab = handler.Result;
        //        var viewGO = container.InstantiatePrefab(viewPrefab, parent);
        //        Addressables.Release(handler);
        //        var view = viewGO.GetComponent<ITaskView>();
        //        return view;
        //    }
        //    catch (Exception)
        //    {
        //        throw new ArgumentNullException(
        //            string.Format("Can't load addressable reference")
        //            );
        //    }
        //}

        private ScriptableTask GetRandomSettingFromList(List<ScriptableTask> taskSettings)
        {
            return taskSettings[RANDOM.Range(0, taskSettings.Count)];
        }
    }
}



