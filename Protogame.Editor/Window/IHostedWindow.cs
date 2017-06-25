namespace Protogame.Editor.Window
{
    public interface IHostedWindow
    {
        string Title { get; }

        string IconName { get; }

        bool Visible { get; }

        bool MustDestroyTextures { get; set; }

        void SendTextures(string mmapSyncFilename, long[] texturePointers, int textureWidth, int textureHeight);

        void QueueEvent(Event @event);

        void CheckLiveness(HostedEditorWindow hostedWindow);
    }
}
