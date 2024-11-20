namespace Core
{
    public class Locator<T> where T : class
    {
        public static T Instance { get; private set; }
        public static void Set(T instance) => Instance = instance;
    }
}