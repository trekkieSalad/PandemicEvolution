using UnityEngine;
using ABMU.Core;

using System.Collections.Generic;

public class Scientist : AbstractController 
{
    [Header("Simulation")]
    public Bounds Bounds;

    [Header("Agents")]
    public bool CreateSimulatedCitizens;
    public GameObject Cube;
    public static PopulationDensity DensityOfAgents = PopulationDensity.Low;

    [SerializeField] private List<Citizen> Citizens = new List<Citizen>();

    void Start(){
        CitizenFactory citizenFactory = new RealCitizenFactory();
        Citizens.AddRange(citizenFactory.createPopulation(Cube, Bounds));

        if (CreateSimulatedCitizens)
        {
            citizenFactory = new SimulatedCitizenFactory(Citizens);
            Citizens.AddRange(citizenFactory.createPopulation(Cube, Bounds));
        }
    }

    
}
