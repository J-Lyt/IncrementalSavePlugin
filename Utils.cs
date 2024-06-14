using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Frosty.Core;
using FrostyEditor;
using Frosty.Core.Windows;
using Frosty.Core.Controls;

namespace IncrementalSavePlugin {
    public static class Utils {

        public static void SaveIncremental() {
            FrostyProject m_project = typeof(MainWindow).GetField("m_project", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Application.Current.MainWindow) as FrostyProject;
            string fileName = m_project.Filename;

            if (fileName == "") {
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

            FrostyTaskWindow.Show("Saving Project", m_project.Filename, (task) => m_project.Save("", true, true));

            ((MainWindow)Application.Current.MainWindow).AddRecentProject(m_project.Filename);

            dataExplorer.RefreshItems();
            legacyExplorer.RefreshItems();
            ((MainWindow)Application.Current.MainWindow).RefreshTabs();

            FrostyEditor.App.Logger.Log("Project saved to {0}", m_project.Filename);

            typeof(MainWindow).InvokeMember("UpdateWindowTitle", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, Application.Current.MainWindow, []);
            typeof(MainWindow).InvokeMember("UpdateDiscordState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, Application.Current.MainWindow, []);
        }


        public static Window MainWin {
            get {
                Window w = null;
                Application.Current.Dispatcher.Invoke(() => w = Application.Current.MainWindow);
                return w;
            }
        }

        public async static void ReorderMenuExtension() {
            await Task.Run(() => {
                while (MainWin is not MainWindow) {
                    Task.Delay(1000).Wait();
                }
            });

            Menu menu = typeof(MainWindow).GetField("menu", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(MainWin) as Menu;

            foreach (MenuItem topItem in menu.Items) {
                if (topItem.Header as string == "File") {
                    MenuItem saveIncrItem = topItem.Items[topItem.Items.Count - 1] as MenuItem;
                    topItem.Items.RemoveAt(topItem.Items.Count - 1);
                    topItem.Items.Insert(topItem.Items.Cast<object>().ToList().FindIndex(i => (i as MenuItem).Header.ToString().Contains("Save As")), saveIncrItem);
                }
            }
        }
    }
}
