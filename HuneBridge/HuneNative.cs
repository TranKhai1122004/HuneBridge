using System;
using System.Runtime.InteropServices;

namespace HuneBridge
{
    public static class HuneNative
    {
        // ⚠️ Đổi "MainDLL.dll" thành đúng tên file DLL thực tế bạn đang có nếu khác tên!
        private const string DLL_NAME = "HUNERF.DLL";

        // 1. Hàm Đọc Thẻ Hune
        [DllImport(DLL_NAME, EntryPoint = "ReadCard", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int ReadCard(
            int com,
            ref int retSerial,
            ref int retCardNo,
            byte[] cardSN,
            ref int cardNoOld);

        // 2. Hàm Ghi Thẻ Hune (Tạo chìa phòng)
        [DllImport(DLL_NAME, EntryPoint = "WriteKeyCard", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int WriteKeyCard(
            int cardNo,
            int nBlock,
            int encrypt,
            string cardPass,
            string systemCode,
            string hotelCode,
            string roomPass,
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
            int validTimes,
            int com);
    }
}
