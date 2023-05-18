using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Mathy.Data.Addressables
{
    public interface IImagesRefsPlayerRankProvider : IImagesRefsProvider
    {
        UniTask<Sprite> GetSpriteByType(PlayerRankImageType type);
    }

    [Serializable]
    public class ImagesRefsPlayerRankProvider : ImagesRefsProvider<PlayerRankImageType>, IImagesRefsPlayerRankProvider
    {

    }
}



