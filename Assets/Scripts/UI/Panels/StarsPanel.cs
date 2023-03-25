using Cysharp.Threading.Tasks;

public class StarsPanel : HeaderBar
{
    /*public override void UpdateText()
    {
        int stars = PlayerDataManager.Instance.PlayerStars;
        SetText(stars);
    }*/

    protected override async UniTask AsyncUpdateText()
    {
        await UniTask.WaitUntil(() => PlayerDataManager.Instance != null);
        int stars = PlayerDataManager.Instance.PlayerStars;
        SetText(stars);
    }
}
