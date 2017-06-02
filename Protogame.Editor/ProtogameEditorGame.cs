using Protoinject;

using Protogame;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using Protogame.Editor.LoadedGame;

namespace Protogame.Editor
{
    public class ProtogameEditorGame : CoreGame<ProtogameEditorWorld>
    {
        private ILoadedGame _loadedGame;
        private IKernel _kernel;

        public ProtogameEditorGame(IKernel kernel)
            : base(kernel)
        {
            _kernel = kernel;
        }

        public override void PrepareGameWindow(IGameWindow window)
        {
            IsMouseVisible = true;
            window.AllowUserResizing = true;
            window.Title = "Please wait, Protogame is loading...";
            window.Maximize();
        }

        protected override void ConfigureRenderPipeline(IRenderPipeline pipeline, IKernel kernel)
        {
            pipeline.AddFixedRenderPass(kernel.Get<ICanvasRenderPass>());
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (_loadedGame != null)
            {
                _loadedGame.Render(GameContext, RenderContext);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_loadedGame != null)
            {
                _loadedGame.Update(GameContext, UpdateContext);
            }

            base.Update(gameTime);
        }

        protected override async Task LoadContentAsync()
        {
            await base.LoadContentAsync();

            _loadedGame = _kernel.Get<ILoadedGame>();
        }
    }
}
