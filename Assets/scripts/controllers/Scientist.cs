using UnityEngine;

using System.Collections.Generic;


public class Scientist : MonoBehaviour 
{
    [Header("Simulation")]
    public Bounds Bounds;
    public WorldController World;

    [Header("Agents")]
    public bool CreateSimulatedCitizens;
    public GameObject Cube;
    public static PopulationDensity DensityOfAgents = PopulationDensity.Low;

    [SerializeField] private List<Citizen> Citizens = new List<Citizen>();

    void Start(){
        Debug.Log("Scientist is creating the world");
        
        if(Bounds == null){
            Bounds = new Bounds(Vector3.zero, new Vector3(100f,0,100f));
        }

        // Creamos los ciudadanos
        createCitizens();

        // Habilitamos el script del controlador de la simulacion
        World.enabled = true;

        Debug.LogWarning("Added " + World.agents.Count + " citizens to the world");
    }

    void Update()
    {
        World.createSocialNetworks();
        World.createSocialCircle();

        foreach (Citizen citizen in Citizens)
        {
            Debug.Log(citizen.name);
            citizen.updateEvaluations(true);
            citizen.updateDissonances();
            citizen.calculateBehavior();
            citizen.updateEvaluations(true);
            citizen.updateDissonances();
        }

        this.enabled = false;
        //Destroy(gameObject);

    }

    private void createCitizens()
    {

        // Creamos los ciudadanos reales
        CitizenFactory citizenFactory = new RealCitizenFactory();
        Citizens.AddRange(citizenFactory.createPopulation(Cube, Bounds));

        // Creamos los ciudadanos simulados
        if(CreateSimulatedCitizens){
            citizenFactory =  new SimulatedCitizenFactory(Citizens);
            Citizens.AddRange(citizenFactory.createPopulation(Cube, Bounds));
        }

    }

    
}
