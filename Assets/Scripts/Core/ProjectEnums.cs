namespace Mathy
{
    public enum UIComponentType
    {
        DefaultElement = 0,
        ImageWithColliderElement = 1,
        WideDefaultElement = 2,
        SimpleImageElement = 3,

        DefaultVariant = 10,
        WideDefaultVariant = 11,

        DefaultOperator = 20,
    }

    public enum TaskFeatures
    {
        DefaultCounter,
        CounterVariantOne,
    }

    public enum Popups
    {
        ParentGate = 1,
    }

    public enum TaskElementType
    {
        Value,
        Operator,
    }

    public enum TaskStatus
    {
        Pending = 0,
        InProgress = 1,
        Right = 2,
        Wrong = 3
    }

    #region Backgrounds
    public enum BackgroundType
    {
        none = 0,
        Colliseum = 1,
        ChichenItza = 2,
        GoldenGates = 3,
        Pantheon = 4,
        Pyramids = 5,
        Stonehendge = 6,
        DailyRewards = 7,

        BlueDots = 100,
        BlueLines = 101,
        BlueStars = 102,
        BlueZigzag = 103,
        GreenDots = 110,
        GreenLines = 111,
        GreenStars = 112,
        GreenZigzag = 113,
        IndigoDots = 120,
        IndigoLines = 121,
        IndigoStars = 122,
        IndigoZigzag = 123,
        OrangeDots = 130,
        OrangeLines = 131,
        OrangeStars = 132,
        OrangeZigzag = 133,
        RedDots = 140,
        RedLines = 141,
        RedStars = 142,
        RedZigzag = 143,
        VioletDots = 150,
        VioletLines = 151,
        VioletStars = 152,
        VioletZigzag = 153,
        YellowDots = 160,
        YellowLines = 161,
        YellowStars = 162,
        YellowZigzag = 163,
    }

    public enum StandardBackgroundType
    {
        Colliseum = 1,
        ChichenItza = 2,
        GoldenGates = 3,
        Pantheon = 4,
        Pyramids = 5,
        Stonehendge = 6,
        DailyRewards = 7,
    }

    public enum VariantOneBackgroundType
    {
        BlueDots = 100,
        BlueLines = 101,
        BlueStars = 102,
        BlueZigzag = 103,
        GreenDots = 110,
        GreenLines = 111,
        GreenStars = 112,
        GreenZigzag = 113,
        IndigoDots = 120,
        IndigoLines = 121,
        IndigoStars = 122,
        IndigoZigzag = 123,
        OrangeDots = 130,
        OrangeLines = 131,
        OrangeStars = 132,
        OrangeZigzag = 133,
        RedDots = 140,
        RedLines = 141,
        RedStars = 142,
        RedZigzag = 143,
        VioletDots = 150,
        VioletLines = 151,
        VioletStars = 152,
        VioletZigzag = 153,
        YellowDots = 160,
        YellowLines = 161,
        YellowStars = 162,
        YellowZigzag = 163,
    }
    #endregion

    #region CountedImageTypes
    public enum CountedImageType
    {
        Bean = 1,
        Cheese = 2,
        Coin = 3,
        Flower = 4,
        Leaf = 5,
        Candy = 6,
        Strawberry = 7,

        BottleVariant_1 = 10,
        BottleVariant_2 = 11,
        BottleVariant_3 = 12,
        Apple = 13,
        RoundCandy = 14,
        Carom = 15,
        EsterEgg = 16,
        Ellipse = 17,
        Grape = 18,
        IceCream = 19,
        Lemon = 20,
        Square = 21,
        Star = 22,
        Starfish = 23,
        Triangle = 24,
        Pie = 25,

        Carrot = 30,

        Watermelon = 40,
    }

    public enum CountedElementImageType
    {
        Bean = 1,
        Cheese = 2,
        Coin = 3,
        Flower = 4,
        Leaf = 5,
        Candy = 6,
        Strawberry = 7,
    }

    public enum SelectFromThreeImageType
    {
        BottleVariant_1 = 10,
        BottleVariant_2 = 11,
        BottleVariant_3 = 12,
        Apple = 13,
        RoundCandy = 14,
        Carom = 15,
        EsterEgg = 16,
        Ellipse = 17,
        Grape = 18,
        IceCream = 19,
        Lemon = 20,
        Square =21,
        Star = 22,
        Starfish = 23,
        Triangle = 24,
        Pie = 25
    }

    public enum CountedElementFrame10Type
    { 
        Carrot = 30,
    }

    public enum CountedElementFrame20Type
    {
        Watermelon = 40,
    }

    #endregion

    public enum KeyValuePairKeys
    {
        none = 0,
        UniqueTaskIndex = 1,
        TotalCorrectAnswers = 2,
        TotalWrongAnswers = 3,

        Count = 10,
        Addition = 11,
        Subtraction = 12,
        Comparison = 13,
        Shapes = 14,
        Sorting = 15,
        Complex = 16
    }
}

