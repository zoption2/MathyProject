namespace Mathy.UI
{
    public class ResultScreenRewardModel : IModel
    { 
        public string LocalizedTitle { get; set; }
        public int RewardValue { get; set; }
        public int PreviousValue { get; set; }
        public bool NeedAnimation { get; set; }
    }
}


