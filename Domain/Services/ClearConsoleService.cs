using System.Runtime.InteropServices;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services
{
    public class ClearConsoleService : IClearConsoleService
    {
        private const int StdOutputHandle = -11;
        private const byte Empty = 32;

        [StructLayout(LayoutKind.Sequential)]
        struct Coord
        {
            public short x;
            public short y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct ConsoleScreenBufferInfo
        {
            public Coord dwSize;
            public Coord dwCursorPosition;
            public int wAttributes;
            public SmallRect srWindow;
            public Coord dwMaximumWindowSize;
        }

        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", EntryPoint = "FillConsoleOutputCharacter", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int FillConsoleOutputCharacter(int hConsoleOutput, byte cCharacter, int nLength, Coord dwWriteCoord, ref int lpNumberOfCharsWritten);

        [DllImport("kernel32.dll", EntryPoint = "GetConsoleScreenBufferInfo", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetConsoleScreenBufferInfo(int hConsoleOutput, ref ConsoleScreenBufferInfo lpConsoleScreenBufferInfo);

        [DllImport("kernel32.dll", EntryPoint = "SetConsoleCursorPosition", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int SetConsoleCursorPosition(int hConsoleOutput, Coord dwCursorPosition);

        private int hConsoleHandle;

        public ClearConsoleService()
        {
            // 
            // TODO: Add constructor logic here.
            // 
            hConsoleHandle = GetStdHandle(StdOutputHandle);
        }

        public void Clear()
        {
            var hWrittenChars = 0;
            var strConsoleInfo = new ConsoleScreenBufferInfo();
            Coord home;
            home.x = home.y = 0;
            GetConsoleScreenBufferInfo(hConsoleHandle, ref strConsoleInfo);
            FillConsoleOutputCharacter(hConsoleHandle, Empty, strConsoleInfo.dwSize.x * strConsoleInfo.dwSize.y, home, ref hWrittenChars);
            SetConsoleCursorPosition(hConsoleHandle, home);
        }
    }
}