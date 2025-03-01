using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using Frosty.Core;
using Frosty.Core.Windows;
using Frosty.Core.Controls;
using FrostyEditor.Windows;

namespace IncrementalSavePlugin
{
    public static class Utils
    {

        public static void SaveIncremental()
        {
            FrostyProject m_project = typeof(MainWindow).GetField("m_project", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Application.Current.MainWindow) as FrostyProject;
            string fileName = m_project.Filename;

            if (fileName == "")
            {
                FrostySaveFileDialog sfd = new("Save Project As", "*.fbproject (Frosty Project)|*.fbproject", "Project");
                if (!sfd.ShowDialog())
                    return;

                fileName = sfd.FileName;
            }

            Match numberMatch = Regex.Match(fileName.Replace(".fbproject", ""), @"\d{0,4}$");

            if (numberMatch.Success && int.TryParse(numberMatch.Value, out int num))
                fileName = Regex.Replace(fileName.Replace(".fbproject", ""), @"\d{0,4}$", "") + (num + 1) + ".fbproject";
            else
                fileName = fileName.Replace(".fbproject", "") + 2 + ".fbproject";

            m_project.Filename = fileName;

            FrostyDataExplorer dataExplorer = typeof(MainWindow).GetField("dataExplorer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Application.Current.MainWindow) as FrostyDataExplorer;
            FrostyDataExplorer legacyExplorer = typeof(MainWindow).GetField("legacyExplorer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Application.Current.MainWindow) as FrostyDataExplorer;

            FrostyTaskWindow.Show("Saving Project", m_project.Filename, (task) => m_project.Save("", true));

            dataExplorer.RefreshItems();
            legacyExplorer.RefreshItems();
            typeof(MainWindow).InvokeMember("RefreshTabs", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, Application.Current.MainWindow, []);

            FrostyEditor.App.Logger.Log("Project saved to {0}", m_project.Filename);

            typeof(MainWindow).InvokeMember("UpdateWindowTitle", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, Application.Current.MainWindow, []);
            typeof(MainWindow).InvokeMember("UpdateDiscordState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, Application.Current.MainWindow, []);
        }
    }
}
