using UnityEngine;


namespace Mathy.Data.Addressables
{
    public interface IImagesAddressableRefsHolder
    {
        IImagesRefsPlayerRankProvider PlayerRank { get; }
    }

    [CreateAssetMenu(fileName = "ImagesAddressableRefsHolder", menuName = "ScriptableObjects/AddressableRefsHolders/Images")]
    public class ImagesAddressableRefsHolder : ScriptableObject, IImagesAddressableRefsHolder
    {
        [SerializeField] private ImagesRefsPlayerRankProvider _playerRank;

        public IImagesRefsPlayerRankProvider PlayerRank => _playerRank;
    }
}



