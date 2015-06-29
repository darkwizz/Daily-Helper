using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyHelperLibrary
{
    public enum UserInterfaceType { Console, WinForms, WPF }

    public static class ErrorNotifier
    {
        public static void NotifyUser(string message)
        {
            UserInterfaceType type = UserInterfaceType.Console;
            switch (type)
            {
                case UserInterfaceType.Console:
                    Console.WriteLine(message);
                    break;
                case UserInterfaceType.WinForms:
                    System.Windows.Forms.MessageBox.Show(message);
                    break;
                case UserInterfaceType.WPF:
                    System.Windows.MessageBox.Show(message);
                    break;
            }
        }
    }
}
