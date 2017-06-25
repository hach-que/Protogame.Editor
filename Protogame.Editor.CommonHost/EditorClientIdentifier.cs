namespace Protogame.Editor.CommonHost
{
    public class EditorClientIdentifier : IClientIdentifier
    {
        public EditorClientIdentifier(long id)
        {
            ClientId = id;
        }

        public long ClientId { get; }
    }
}
