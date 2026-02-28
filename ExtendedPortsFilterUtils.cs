using System;
using System.IO;

namespace libertypre
{
    public class ExtendedPortsFilterUtils
    {
        // Класс для переключение расширенного режима портов (фильтрация портов 1024-65535)
        // Class for toggling extended ports mode (filtering ports 1024-65535)

        private static string epModeFlagFileTCP = Path.Combine(MainClass.basePath, "extended_ports_tcp");
        private static string epModeFlagFileUDP = Path.Combine(MainClass.basePath, "extended_ports_udp");

        // Получить состояние TCP режима
        // Get TCP mode status
        public static bool GetTcpModeStatus() => File.Exists(epModeFlagFileTCP);

        // Получить состояние UDP режима
        // Get UDP mode status
        public static bool GetUdpModeStatus() => File.Exists(epModeFlagFileUDP);

        // Переключение TCP режима
        // Toggle TCP mode
        public static void ToggleTcpMode()
        {
            if (GetTcpModeStatus())
            {
                File.Delete(epModeFlagFileTCP);
                LocaleUtils.WriteTr("InfoDisablingExtendedPortsTcp");
            }
            else
            {
                File.WriteAllText(epModeFlagFileTCP, "");
                LocaleUtils.WriteTr("InfoEnablingExtendedPortsTcp");
            }
        }

        // Переключение UDP режима
        // Toggle UDP mode
        public static void ToggleUdpMode()
        {
            if (GetUdpModeStatus())
            {
                File.Delete(epModeFlagFileUDP);
                LocaleUtils.WriteTr("InfoDisablingExtendedPortsUdp");
            }
            else
            {
                File.WriteAllText(epModeFlagFileUDP, "");
                LocaleUtils.WriteTr("InfoEnablingExtendedPortsUdp");
            }
        }

        // Комбинированное переключение
        // Combined toggle
        public static void ToggleCombinedMode()
        {
            bool tcp = GetTcpModeStatus();
            bool udp = GetUdpModeStatus();

            if (tcp || udp)
            {
                // Если хоть один включён - выключаем оба
                // If at least one is on - turn both off
                if (tcp) File.Delete(epModeFlagFileTCP);
                if (udp) File.Delete(epModeFlagFileUDP);
                LocaleUtils.WriteTr("InfoDisablingExtendedPortsBoth");
            }
            else
            {
                // Если оба выключены - включаем оба
                // If both are off - turn both on
                File.WriteAllText(epModeFlagFileTCP, "");
                File.WriteAllText(epModeFlagFileUDP, "");
                LocaleUtils.WriteTr("InfoEnablingExtendedPortsBoth");
            }
        }
    }
}
