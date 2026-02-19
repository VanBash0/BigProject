using System;

namespace BigProject.Utilities
{
    public static class ExceptionUtilities
    {
        private const string AUTHOR_EXCEPTION_MSG = "{0}: {1}.";

        public static void ThrowIfNull(object arg, string msg = "Null reference exception.")
        {
            if (arg == null)
            {
                throw new NullReferenceException(msg);
            }
        }

        public static void ThrowIfNull(object arg, string author, string msg = "Null reference exception.")
        {
            if (arg == null)
            {
                throw new NullReferenceException(string.Format(AUTHOR_EXCEPTION_MSG, author, msg));
            }
        }
    }
}