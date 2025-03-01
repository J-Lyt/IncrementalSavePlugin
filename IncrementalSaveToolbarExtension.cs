using Frosty.Core;

namespace IncrementalSavePlugin
{
    public class IncrementalSaveToolbarExtension : ToolbarExtension
    {
        public override string Name => "";
        public override string Tooltip => "Save Incremental";
        public override string Icon => "IncrementalSavePlugin;component/Images/SaveInc.png";

        public override RelayCommand ToolbarItemClicked => new(_ => Utils.SaveIncremental());
    }
}
