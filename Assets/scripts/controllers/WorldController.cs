using UnityEngine;

using ABMU.Core;

using System.Linq;
using TMPro;
using System.Threading;
using System.Collections.Generic;


public class WorldController : AbstractController
{
    private WorldParameters _parameters;
    public WorldParameters parameters { get => _parameters; }
    public List<Place> Places = new List<Place>();
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
            day.ToString() + "\nDay " + currentTick;

        foreach (Place place in Places) place.EmptyNode();
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

    public void SetPlacesToMove()
    {
        foreach (Citizen citizen in agents.Cast<Citizen>())
        {
            foreach (PlaceType type in System.Enum.GetValues(typeof(PlaceType)))
            {
                citizen.AddPlace(type, GetNearPlace(citizen, type));
            }
        }
    }

    public Citizen GetRandomCitizen()
    {
        return agents[Random.Range(0, agents.Count)] as Citizen;
    }

    public Place GetNearPlace(Citizen citizen, PlaceType type)
    {
        GameObject[] possibleNodes =
            GameObject.FindGameObjectsWithTag(type.ToString());

        GameObject nearObject = null;
        float distanciaMasCercana = Mathf.Infinity;

        foreach (GameObject objeto in possibleNodes)
        {
            float distancia = Vector3.Distance(objeto.transform.position, citizen.gameObject.transform.position);

            if (distancia < distanciaMasCercana)
            {
                distanciaMasCercana = distancia;
                nearObject = objeto;
            }
        }

        return nearObject.GetComponent<Place>();
    }

    public Place GetRandomPlace(PlaceType type)
    {
        List<Place> places = Places.Where(p => p.type == type).ToList();
        return places[Random.Range(0, places.Count)];
    }
}
