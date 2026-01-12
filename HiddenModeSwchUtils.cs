using System;
using System.IO;

namespace libertypre
{
    public class HiddenModeSwchUtils
    {
        private static string hiddenModeFile = Path.Combine(MainClass.basePath, "hidden_mode");

        // Переключение скрытого / невидимого режима (Windows)
        // Toggle hidden / invisible mode (Windows)
        public static void ToggleHiddenMode()
        {
            if (File.Exists(hiddenModeFile))
            {
                File.Delete(hiddenModeFile);
                LocaleUtils.WriteTr("InfoDisablingHiddenMode");
            }
            else
            {
                File.WriteAllText(hiddenModeFile, "");
                LocaleUtils.WriteTr("InfoEnablingHiddenMode");
            }
        }

        // Получение текущего состояния скрытого режима
        // Get current hidden mode state
        public static bool GetHiddenModeStatus()
        {
            return File.Exists(hiddenModeFile);
        }
    }
}
