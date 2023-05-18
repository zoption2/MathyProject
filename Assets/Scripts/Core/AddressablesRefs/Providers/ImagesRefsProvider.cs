using UnityEngine;
using System;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Linq;

namespace Mathy.Data.Addressables
{
    public interface IImagesRefsProvider
    {
        UniTask<Sprite> GetRandomSprite();
    }


    public abstract class ImagesRefsProvider<TType> : AddressableRefsProvider<TType, AssetReferenceSprite> where TType : Enum 
    {
        public async UniTask<Sprite> GetRandomSprite()
        {
            var random = new System.Random();
            var values = Enum.GetValues(typeof(TType));
            var type = (TType)values.GetValue(random.Next(values.Length));
            return await LoadAsync<Sprite>(type);
        }

        public async UniTask<Sprite> GetSpriteByType(TType type)
        {
            var random = new System.Random();
            var availableRefs = references.Where(x => x.Type.Equals(type)).Select(x => x.Reference).ToArray();
            var randomIndex = random.Next(0, availableRefs.Length);
            var persistRef = availableRefs[randomIndex];
            return await LoadByRefAsync<Sprite>(persistRef);
        }
    }
}



