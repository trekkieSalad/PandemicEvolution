using UnityEngine;

using ABMU.Core;

using System.Linq;

public class WorldController : AbstractController
{
    private WorldParameters _parameters;
    public WorldParameters parameters { get => _parameters; }

    public override void Init()
    {
        Debug.Log("Initializing world");
        base.Init();
        _parameters = WorldParameters.GetInstance();
    }
}