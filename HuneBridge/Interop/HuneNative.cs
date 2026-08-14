using System.Runtime.InteropServices;
using System.Text;

namespace HuneBridge
{
    public static class HuneNative
    {
        private const string DllName = "hunerf.dll";

        [DllImport(DllName, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern int ReadCardSN(int com, StringBuilder cardSN);

        [DllImport(DllName, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern int ReadMessage(
            int com,
            int nBlock,
            int encrypt,
            ref int cardNumber,
            ref int cardType,
            ref int passLevel,
            StringBuilder cardPass,
            StringBuilder systemCode,
            StringBuilder address,
            StringBuilder sDateTime);

        [DllImport(DllName, SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern int KeyCard(
            int com,
            int cardNo,
            int nBlock,
            int encrypt,
            string cardPass,
            string systemCode,
            string hotelCode,
            string pass,
            string address,
            string sdIn,
            string stIn,
            string sdOut,
            string stOut,
            int levelPass,
            int passMode,
            int addressMode,
            int addressQty,
            int timeMode,
            int v8,
            int v16,
            int v24,
            int alwaysOpen,
            int openBolt,
            int terminateOld,
            int validTimes);
    }
}
