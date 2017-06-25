using Protogame;

namespace Protogame.Editor.Api.Version1.Layout
{
    public interface IDockableContainer : IContainer
    {
        bool Visible { get; }
    }
}