using UnityEngine;

using System.Collections.Generic;
using System.Linq;

public class Scientist : MonoBehaviour 
{
    [Header("Simulation")]
    public Bounds Bounds;
    public WorldController World;

    [Header("Agents")]
    public bool CreateSimulatedCitizens;
    public GameObject CitizenObject;
    public GameObject NodeObject;
    public static PopulationDensity DensityOfAgents = PopulationDensity.Low;

    [SerializeField] private List<Citizen> Citizens = new List<Citizen>();

    void Start()
    {
        Debug.Log("Scientist is creating the world");

        if (Bounds == null)
        {
            Bounds = new Bounds(Vector3.zero, new Vector3(100f, 0, 100f));
        }

        // Habilitamos el script del controlador de la simulacion
        World.enabled = true;

        // Creamos los ciudadanos
        createCitizens();

        // Creamos los nodos
        createPlaces();

        Debug.LogWarning("Added " + World.agents.Count + " citizens to the world");
    }

    void Update()
    {
        World.createSocialNetworks();
        World.createSocialCircle();
        World.SetPlacesToMove();

        foreach (Citizen citizen in Citizens)
        {
            citizen.UpdateCitizen(true);
        }

        this.enabled = false;
        //Destroy(gameObject);

    }

    private void createCitizens()
    {

        // Creamos los ciudadanos reales
        CitizenFactory citizenFactory = new RealCitizenFactory();
        Citizens.AddRange(citizenFactory.createPopulation(CitizenObject, Bounds));

        // Creamos los ciudadanos simulados
        if(CreateSimulatedCitizens){
            citizenFactory =  new SimulatedCitizenFactory(Citizens);
            Citizens.AddRange(citizenFactory.createPopulation(CitizenObject, Bounds));
        }

        SetInitialState();
    }

    private void createPlaces()
    {
        PlaceFactory placeFactory = new PlaceFactory();
        World.Places = placeFactory.CreatePlaces(NodeObject, Bounds);
    }

    private void SetInitialState()
    {
        List<Citizen> susceptibleCitizens = new List<Citizen>(Citizens);
        int initialInfected = WorldParameters.GetInstance().initialInfected;

        for (int i = 0; i < initialInfected; i++)
        {
            int index = Random.Range(0, susceptibleCitizens.Count);
            Citizen citizen = susceptibleCitizens[index];
            citizen.ActualState = new InfectedSirState(citizen);
            susceptibleCitizens.RemoveAt(index);
        }

        foreach (Citizen citizen in susceptibleCitizens)
        {
            citizen.ActualState = new SusceptibleSirState(citizen);
        }

    }

    
}
