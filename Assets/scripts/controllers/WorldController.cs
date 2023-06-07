using UnityEngine;

using ABMU.Core;

using System.Linq;
using TMPro;
using System.Threading;

public class WorldController : AbstractController
{
    private WorldParameters _parameters;
    public WorldParameters parameters { get => _parameters; }
    public Day day;

    public override void Init()
    {
        Debug.Log("Initializing world");
        base.Init();
        _parameters = WorldParameters.GetInstance();
    }

    private void Update()
    {
        day = (Day)(currentTick % 7);
        GameObject.Find("DayText").GetComponent<TextMeshProUGUI>().text = 
            EnumHelper.GetDescription(day) + "\nDay " + currentTick;
        //Thread.Sleep(1000);
    }

    public void createSocialNetworks()
    {
        foreach (Citizen citizen in agents)
        {
            citizen.CreateSocialNetwork();
        }
    }

    public void createSocialCircle()
    {
        RelationshipFactory socialCircleFactory = new SocialCircleFactory();
        foreach (Citizen citizen in agents.Cast<Citizen>())
        {
            socialCircleFactory.createNetwork(citizen);
        }
    }

    public Citizen GetRandomCitizen()
    {
        return agents[Random.Range(0, agents.Count)] as Citizen;
    }
}
