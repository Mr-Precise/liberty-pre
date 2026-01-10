using System;
using System.IO;

namespace libertypre
{
    public class ExtendedPortsFilterUtils
    {
        private static string extendedPortsModeFile = Path.Combine(MainClass.basePath, "extended_ports_mode");

        // Переключение расширенного режима портов (фильтрация портов 1024-65535)
        // Toggle extended ports mode (port filtering 1024-65535)
        public static void ToggleExtendedPortsMode()
        {
            if (File.Exists(extendedPortsModeFile))
            {
                File.Delete(extendedPortsModeFile);
                LocaleUtils.WriteTr("InfoDisablingExtendedPortsMode");
            }
            else
            {
                File.WriteAllText(extendedPortsModeFile, "");
                LocaleUtils.WriteTr("InfoEnablingExtendedPortsMode");
            }
        }

        // Получение текущего состояния расширенного режима портов
        // Get current extended ports mode state
        public static bool GetExtendedPortsModeStatus()
        {
            return File.Exists(extendedPortsModeFile);
        }
    }
}
