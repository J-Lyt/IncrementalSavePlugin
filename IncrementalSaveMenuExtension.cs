using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Frosty.Core;

namespace IncrementalSavePlugin {
    public class IncrementalSaveMenuExtension : MenuExtension {
        internal static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/Images/Save.png") as ImageSource;

        public override string TopLevelMenuName => "File";
        public override string SubLevelMenuName => null;

        public override string MenuItemName => "Save Incremental";
        public override ImageSource Icon => imageSource;

        public override RelayCommand MenuItemClicked => new(_ => Utils.SaveIncremental());

        public IncrementalSaveMenuExtension() {
            try {
                Application.Current.Dispatcher.Invoke(Utils.ReorderMenuExtension, DispatcherPriority.Normal);
            }
            catch (Exception e) {
                App.Logger.LogWarning("Unable to reorder Incremental Save menu extension: " + e.Message);
            }
        }
    }
}
