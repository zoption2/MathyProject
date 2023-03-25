//#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("DpSSlAqILFYm47iKUZ89xaCBA8mTEBEXGDuXWZfmcnUUECGQ4yE7FxlPIZMQABcSRAwxFZMQGSGTEBUhr");
        private static int[] order = new int[] { 49,34,33,55,15,55,27,53,35,10,54,43,18,21,47,37,43,30,58,54,50,37,30,53,44,34,59,27,29,59,41,53,38,48,49,49,38,42,47,57,47,41,53,50,59,51,57,53,51,53,57,56,59,59,55,57,57,59,58,59,60 };
        private static int key = 17;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
//#endif
