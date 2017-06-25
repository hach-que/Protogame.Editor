using Protogame.Editor.Layout;
using Protogame.Editor.Api.Version1.Window;

namespace Protogame.Editor.Window
{
    public class ConsoleEditorWindow : EditorWindow
    {
        private readonly IConsole _console;
        private readonly ScrollableContainer _scrollableContainer;

        public ConsoleEditorWindow(
            IAssetManager assetManager,
            IConsole console)
        {
            _console = console;
            
            Icon = assetManager.Get<TextureAsset>(IconName);

            var consoleContainer = new ConsoleContainer { Console = console as EditorConsole };

            _scrollableContainer = new ScrollableContainer();
            _scrollableContainer.SetChild(consoleContainer);

            SetChild(_scrollableContainer);
        }

        public override string Title => "Console";

        public override string IconName => "texture.IconTerminal";

        public override void OnFocus()
        {
            // Switch focus to the scrollable container.
            _scrollableContainer.Focus();
        }
    }
}
