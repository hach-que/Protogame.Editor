using System;

namespace Protogame.Editor.Api.Version1.Core
{
    [Serializable]
    public class SignalData
    {
        public static readonly SignalData Empty = new SignalData();

        public T Get<T>(string name) where T : class
        {
            return GetType().GetProperty(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).GetGetMethod().Invoke(this, null) as T;
        }
    }
}