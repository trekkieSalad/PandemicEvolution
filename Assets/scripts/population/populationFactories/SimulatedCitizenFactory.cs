using ABMU;
using UnityEngine;

using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class SimulatedCitizenFactory : CitizenFactory
{
    private List<Citizen> realCitizens;

    public SimulatedCitizenFactory(List<Citizen> _realCitizens)
    {
        realCitizens = _realCitizens;
    }

    public List<Citizen> createPopulation(
        GameObject _citizenObject, Bounds bounds, int numCitizens = -1)
    {
        Debug.Log("Creating simulated citizens");
        List<Citizen> citizens = new List<Citizen>();
        bool citizenLimit = numCitizens != -1;
        int counter = realCitizens.Count;

        TextAsset textAsset = Resources.Load<TextAsset>("CensusCoruña");

        string[] lines = textAsset.text.Split('\n');
        lines = lines.Skip(1).ToArray();

        string[] data;

        foreach (string line in lines)
        {
            if (line.Equals("") || line.Equals("\r")) continue;
            double reductionFactor = WorldParameters.populationDensity;
            string finalLine = line.Replace("\r", "");
            data = Utils.CsvRowData(finalLine, ";");


            string section = data[0];
            for (int n = 1; n < 5; n++)
            {
                int citizensNumber = (int)(int.Parse(data[n]) * reductionFactor);
                CitizenGender gender = (n < 3) ?
                    CitizenGender.Male : CitizenGender.Female;
                bool old = (n % 2 == 0);
                for (int i = 0; i < citizensNumber; i++)
                {
                    Vector3 pos = Utilities.RandomPointInBounds(bounds);
                    GameObject citizenObject =
                        MonoBehaviour.Instantiate(_citizenObject);
                    citizenObject.transform.position = pos;
                    citizenObject.transform.position += Vector3.up * 0.5f;
                    citizenObject.name = "Citizen " + counter;
                    citizenObject.tag = "Citizen";

                    Citizen citizen = citizenObject.GetComponent<Citizen>();
                    MakeSimulatedPopulation(citizen, section, old, gender);
                    citizens.Add(citizen);
                    counter++;
                }
            }

            if (citizenLimit)
            {
                numCitizens--;
                if (numCitizens == 0) break;
            }

        }
        Debug.Log("Simulated population created: " + counter + " citizens");
        return citizens;
    }


    private void MakeSimulatedPopulation(
        Citizen citizen, string section, bool old, CitizenGender gender)
    {
        Dictionary<CitizenGender, double[]> famProb =
            new Dictionary<CitizenGender, double[]>(){
                {CitizenGender.Male, new double[]{0.388, 0.033}},
                {CitizenGender.Female, new double[]{0.354, 0.086}}
            };


        Dictionary<CitizenGender, double[]> netProb =
            new Dictionary<CitizenGender, double[]>(){
                {CitizenGender.Male, new double[]{0.708, 0.9}},
                {CitizenGender.Female, new double[]{0.636, 0.62}}
            };


        Dictionary<CitizenGender, double[]> rurProb =
            new Dictionary<CitizenGender, double[]>(){
                {CitizenGender.Male, new double[]{0.219, 0.167}},
                {CitizenGender.Female, new double[]{0.315, 0.114}}
            };


        Dictionary<CitizenGender, double[]> essentProb =
            new Dictionary<CitizenGender, double[]>(){
                {CitizenGender.Male, new double[]{0.169, 0.1}},
                {CitizenGender.Female, new double[]{0.197, 0.086}}
            };

        Dictionary<CitizenGender, double[]> empProb =
            new Dictionary<CitizenGender, double[]>(){
                {CitizenGender.Male, new double[]{0.284, 0.033}},
                {CitizenGender.Female, new double[]{0.238, 0.0}}
            };

        // Establecemos los atributos del ciudadano en función de su edad y sexo
        citizen.simulated = true;
        citizen.section = section;
        citizen.age = old ?
            UnityEngine.Random.Range(65, 101) :
            UnityEngine.Random.Range(18, 65);
        citizen.gender = gender;

        citizen.family =
            (UnityEngine.Random.value <= famProb[gender][old ? 1 : 0]) ? 4 : 1;
        citizen.netIncome =
            (UnityEngine.Random.value <= netProb[gender][old ? 1 : 0]) ? 3 : 1;
        citizen.ruralHouse =
            (UnityEngine.Random.value <= rurProb[gender][old ? 1 : 0]) ?
            true : false;
        citizen.essentialJob =
            (UnityEngine.Random.value <= netProb[gender][old ? 1 : 0]) ?
            true : false;
        citizen.economicActivity =
            (UnityEngine.Random.value <= empProb[gender][old ? 1 : 0]) ?
            EconomicActivity.Employed :
            (EconomicActivity)UnityEngine.Random.Range(2, 8);
        citizen.dissonanceTolerance =
            Utils.randomNormalTruncated(0.5, 0.14, 0, 1);

        // Copiamos las necesidades de un ciudadano real que cumpla las mismas
        // características que el ciudadano simulado
        Citizen toCopy = getCitizenToCopy(citizen);
        citizen.needAImportance = toCopy.needAImportance;
        citizen.needBImportance = toCopy.needBImportance;
        citizen.membershipImportance = toCopy.membershipImportance;

        citizen.needASatisfactionA = toCopy.needASatisfactionA;
        citizen.needBSatisfactionA = toCopy.needBSatisfactionA;
        citizen.needASatisfactionB = toCopy.needASatisfactionB;
        citizen.needBSatisfactionB = toCopy.needBSatisfactionB;

        citizen.needAEvaluationA = toCopy.needAEvaluationA;
        citizen.needBEvaluationA = toCopy.needBEvaluationA;
        citizen.needAEvaluationB = toCopy.needAEvaluationB;
        citizen.needBEvaluationB = toCopy.needBEvaluationB;

        citizen.satisfactionA = toCopy.satisfactionA;
        citizen.satisfactionB = toCopy.satisfactionB;
        citizen.satisfaction =
            Math.Max(citizen.satisfactionA, citizen.satisfactionB);

        citizen.behavior = (citizen.satisfaction == citizen.satisfactionA) ?
            Behavior.Accept : Behavior.Reject;

        // Establecemos el nivel de confianza del ciudadano simulado
        citizen.cityCouncilTrust = UnityEngine.Random.value;
        citizen.politicalOppositionTrust = UnityEngine.Random.value;
        citizen.localMediaTrust = UnityEngine.Random.value;
        citizen.localMediaOppositionTrust = UnityEngine.Random.value;

        // Establecemos la situacion epidemiologica del ciudadano simulado
        citizen.quarantine = false;
        citizen.asintomatic = false;

    }

    private Citizen getCitizenToCopy(Citizen citizen)
    {
        List<Citizen> filterRealCitizens = realCitizens;
        if (citizen.age >= 45)
        {
            filterRealCitizens = 
                filterRealCitizens.Where(a => a.age >= 45).ToList();
        }
        else if (citizen.age >= 25)
        {
            if (citizen.ruralHouse)
                filterRealCitizens = filterRealCitizens.Where(a =>
                        a.ruralHouse &&
                        25 <= a.age &&
                        a.age < 45
                    ).ToList();
            else if (citizen.essentialJob)
                filterRealCitizens = filterRealCitizens.Where(a =>
                        !a.ruralHouse &&
                        a.essentialJob &&
                        25 <= a.age &&
                        a.age < 45
                    ).ToList();
            else if (citizen.economicActivity.Equals(EconomicActivity.Employed))
                filterRealCitizens = filterRealCitizens.Where(a =>
                        !a.ruralHouse &&
                        !a.essentialJob &&
                        25 <= a.age &&
                        a.age < 45 &&
                        a.economicActivity.Equals(EconomicActivity.Employed)
                    ).ToList();
            else
                filterRealCitizens = filterRealCitizens.Where(a =>
                        !a.ruralHouse &&
                        !a.essentialJob &&
                        25 <= a.age &&
                        a.age < 45 &&
                        !a.economicActivity.Equals(EconomicActivity.Employed)
                    ).ToList();
        }
        else
        {
            if (citizen.family < 3)
                filterRealCitizens = filterRealCitizens.Where(a =>
                        a.age < 25 &&
                        a.family < 3
                    ).ToList();
            else if (citizen.netIncome > 2)
                filterRealCitizens = filterRealCitizens.Where(a =>
                        a.netIncome > 2 &&
                        a.age < 25 &&
                        a.family > 2
                    ).ToList();
            else
                filterRealCitizens = filterRealCitizens.Where(a =>
                        a.netIncome < 3 &&
                        a.age < 25 &&
                        a.family > 2
                    ).ToList();
        }

        return filterRealCitizens[
            UnityEngine.Random.Range(0, filterRealCitizens.Count)];
    }
}
