using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Windows;
using Microsoft.Win32;

namespace CopyToClipboard
{
    class Program
    {
        const string IconPath = @"%SystemRoot%\system32\shell32.dll,-148";
        const string CommandLabel = "Copy file path";

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                // install as explorer shortcut
                try
                {
                    InstallExplorerShortcut();
                }
                catch (ObjectDisposedException e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (SecurityException e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (UnauthorizedAccessException e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // copy first argument to clipboard
                // other arguments will be ignored
                string resPath = FormatPath(args[0]);
                Clipboard.SetData(DataFormats.UnicodeText, resPath);
            }
        }

        static string FormatPath(string path)
        {
            // replace '\' with '/' so the user does not have to escape the path
            return path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        static void InstallExplorerShortcut()
        {
            RegistryKey root = Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true);
            RegistryKey directory = root.OpenSubKey("Directory", true).OpenSubKey("shell", true);
            RegistryKey directorySidebar = root.OpenSubKey("Directory", true).OpenSubKey("Background", true).OpenSubKey("shell", true);
            RegistryKey file = root.OpenSubKey("*", true).OpenSubKey("shell", true);
            RegistryKey drive = root.OpenSubKey("Drive", true).OpenSubKey("shell", true);

            InstallExplorerShortcut(directory);
            InstallExplorerShortcut(directorySidebar);
            InstallExplorerShortcut(file);
            InstallExplorerShortcut(drive);
        }
        static void InstallExplorerShortcut(RegistryKey root)
        {
            RegistryKey location = root.CreateSubKey("CopyPath");
            location.SetValue(null, CommandLabel);
            location.SetValue("Icon", IconPath);
            RegistryKey command = location.CreateSubKey("command");
            command.SetValue(null, $"{Assembly.GetExecutingAssembly().Location} \"%1\"");
        }
    }
}
