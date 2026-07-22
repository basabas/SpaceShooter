using Bas.Debug;
using Bas.Thoth;
using UnityEngine;

public class Bootstrap
{
    [RuntimeInitializeOnLoadMethod]
    private static void OnRuntimeMethodLoad()
    {
        ThothManager thoth = Singletons.GetInstanceOf<ThothManager>();
        thoth.StartGame();

        //if (Application.isEditor)
        {
            Singletons.Get<DebugText>();
        }
    }
}
