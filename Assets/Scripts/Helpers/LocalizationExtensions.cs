public static class LocalizationExtensions
{
    public static string TryLocalizeTaskVariant(this string key)
    {
        switch (key)
        {
            case "True":
            case "False":
                return LocalizationManager.GetLocalizedString("GUI Elements", key);

            default:
                return key;
        }
    }
}
