using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DesktopLauncher
{
    /// <summary>
    /// http://web.archive.org/web/20111225141637/http://huddledmasses.org:80/how-to-get-the-character-and-virtualkey-from-a-wpf-keydown-event/
    /// </summary>
    static class HotKeyConverter
    { 
        private enum MapType : uint
        {
            MAPVK_VK_TO_VSC = 0x0,
            MAPVK_VSC_TO_VK = 0x1,
            MAPVK_VK_TO_CHAR = 0x2,
            MAPVK_VSC_TO_VK_EX = 0x3,
        }

        [DllImport("user32.dll")]
        private static extern int ToUnicode(
            uint wVirtKey,
            uint wScanCode,
            byte[] lpKeyState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
            StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags);

        [DllImport("user32.dll")]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, MapType uMapType);

        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        public static string ConvertToString(Key key)
        {
            string str = new KeyConverter().ConvertToString(key);

            int virtualKey = KeyInterop.VirtualKeyFromKey(key);
            byte[] keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            uint scanCode = MapVirtualKey((uint)virtualKey, MapType.MAPVK_VK_TO_VSC);
            StringBuilder stringBuilder = new StringBuilder(2);

            int result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            if (result > 0 && !Char.IsControl(stringBuilder[0]) && !Char.IsWhiteSpace(stringBuilder[0]))
            {
                str = stringBuilder.ToString();
            }
            return str;
        }

        public static Key ConvertFromString(string str)
        {
            try
            {
                return (Key)new KeyConverter().ConvertFromString(str);
            }
            catch
            {
                return KeyInterop.KeyFromVirtualKey(VkKeyScan(str[0]));
            }
        }
    }
}
