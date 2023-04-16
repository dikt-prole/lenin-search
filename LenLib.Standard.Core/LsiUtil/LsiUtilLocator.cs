namespace LenLib.Standard.Core.LsiUtil
{
    public static class LsiUtilLocator
    {
        public static ILsiUtil GetLsiUtil(byte lsiVersion)
        {
            switch (lsiVersion)
            {
                case 1:
                    return new V1LsiUtil();
                case 2:
                    return new V2LsiUtil();
            }

            return null;
        }
    }
}