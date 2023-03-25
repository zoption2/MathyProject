public static class BasicTypesConverter
{
    public static int ToInt(this bool value)
    {
        if (value)
            return 1;
        else
            return 0;
    }

    public static bool ToBool(this int value)
    {
        if (value != 0)
            return true;
        else
            return false;
    }
}