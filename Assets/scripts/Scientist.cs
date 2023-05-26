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

    void Start(){
        CitizenFactory citizenFactory = new RealCitizenFactory();
        citizenFactory.createPopulation(Cube, Bounds);
    }

    
}
