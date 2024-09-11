namespace couchspacesShared.Helpers
{
    public static class NullHelper
    {
        public static void AssignIfNotNull<T>(ref T target, T? value) where T : class
        {
            if (value != null)
            {
                target = value;
            }
        }

        public static void AssignIfNotNull<T>(ref T target, T? value, T defaultValue) where T : struct
        {
            target = value ?? defaultValue;
        }
    }
}