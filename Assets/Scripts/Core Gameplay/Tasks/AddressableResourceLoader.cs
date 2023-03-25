using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Mathy.Core
{
    //ToDO: Make this class responsible for all Adressable Loading stuff
    public class AddressableResourceLoader<T> : IDisposable
    {
        public async System.Threading.Tasks.Task<List<T>> LoadListByAssetLable(string assetLabel)
        {
            var locations = await Addressables.LoadResourceLocationsAsync(assetLabel, typeof(T)).Task;
            List<System.Threading.Tasks.Task<T>> tasks = new List<System.Threading.Tasks.Task<T>>();
            List<T> result = new List<T>();
 
            foreach (var location in locations)
            {
                tasks.Add(Addressables.LoadAssetAsync<T>(location).Task);
            }

            var loadedObj = await System.Threading.Tasks.Task.WhenAll(tasks);
            result.AddRange(loadedObj);

            return result;
        }

        //Todo: huge optimization, just load the sprite names as a list, and then return it without loading 
        //all Elements that are not needed now
        
        public async System.Threading.Tasks.Task<T> LoadSingle(string assetLabel)
        {
            return await Addressables.LoadAssetAsync<T>(assetLabel).Task;
        }


        // Loading and instantiation prefab in parent location
        public async System.Threading.Tasks.Task<GameObject> 
            LoadAndInstantiateSingle(string assetLabel, Transform parent)
        {
            return await Addressables.InstantiateAsync(assetLabel, parent).Task;
        }

        public void Dispose()
        {
            //Here is nothing to dispose right now
        }
    }
}