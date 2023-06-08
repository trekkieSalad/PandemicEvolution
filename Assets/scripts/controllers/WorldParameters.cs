
using UnityEngine;
using UnityEditor;
using Unity.Collections;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public enum PopulationDensity{
    Low = 1,
    Medium = 2,
    High = 4,
    VeryHigh = 8
}

public class WorldParameters
{

    private static WorldParameters instance;

    // Simulation parameters
    [Header("Simulation Variables")]
    public static double populationDensity = (double)Scientist.DensityOfAgents/100;
    [SerializeField] private double _pSe;
    [SerializeField] private double _pSeNonEssentialNodes;
    [SerializeField] private double _pSeAcceptingMeasures;
    [SerializeField] private double _pSeLockdown;
    [SerializeField] private double _pId;
    [SerializeField] private double _pIh;
    [SerializeField] private double _pHd;
    [SerializeField] private double _pHicu;
    [SerializeField] private double _pIcud;
    [SerializeField] private int _recoveredDays;
    [SerializeField] private int _infectiousDaysToDead;
    [SerializeField] private int _infectiousDaysToHospitalized;
    [SerializeField] private int _infectiousDaysToRecovered;
    [SerializeField] private int _daysToStartQuarantine;
    [SerializeField] private int _hospitalizedDaysToDead;
    [SerializeField] private int _hospitalizedDaysToIcu;
    [SerializeField] private int _hospitalizedDaysToRecovered;
    [SerializeField] private int _icuDaysToDead;
    [SerializeField] private int _icuDaysToRecovered;
    [SerializeField] private double _socialReach;
    [SerializeField] private int _numFriends;
    [SerializeField] private double _randomFriendProb;
    [SerializeField] private int _newFriends;
    [SerializeField] private double _probNotEssentialNode;
    [SerializeField] private Day _firstWorkingDay;
    [SerializeField] private Day _firstLeisureDay;
    [SerializeField] private int _initialInfected;

    public double pSe { get => _pSe; }
    public double pSeNonEssentialNodes { get => _pSeNonEssentialNodes; }
    public double pSeAcceptingMeasures { get => _pSeAcceptingMeasures; }
    public double pSeLockdown { get => _pSeLockdown; }
    public double pId { get => _pId; }
    public double pIh { get => _pIh; }
    public double pHd { get => _pHd; }
    public double pHicu { get => _pHicu; }
    public double pIcud { get => _pIcud; }
    public int recoveredDays { get => _recoveredDays; }
    public int infectiousDaysToDead { get => _infectiousDaysToDead; }
    public int infectiousDaysToHospitalized { get => _infectiousDaysToHospitalized; }
    public int infectiousDaysToRecovered { get => _infectiousDaysToRecovered; }
    public int daysToStartQuarantine { get => _daysToStartQuarantine; }
    public int hospitalizedDaysToDead { get => _hospitalizedDaysToDead; }
    public int hospitalizedDaysToIcu { get => _hospitalizedDaysToIcu; }
    public int hospitalizedDaysToRecovered { get => _hospitalizedDaysToRecovered; }
    public int icuDaysToDead { get => _icuDaysToDead; }
    public int icuDaysToRecovered { get => _icuDaysToRecovered; }
    public double socialReach { get => _socialReach; }
    public int numFriends { get => _numFriends; }
    public double randomFriendProb { get => _randomFriendProb; }
    public int newFriends { get => _newFriends; }
    public double probNotEssentialNode { get => _probNotEssentialNode; }
    public Day firstWorkingDay { get => _firstWorkingDay; }
    public Day firstLeisureDay { get => _firstLeisureDay; }
    public int initialInfected { get => _initialInfected; }


    private WorldParameters(){
        initParameters();
    }

    public static WorldParameters GetInstance() 
    {
        if (instance == null)
            instance = new WorldParameters();
        return instance;
    }
    
    private void initParameters(){
        Dictionary<string, object> parameters = new Dictionary<string, object>();


        TextAsset textAsset = Resources.Load<TextAsset>("WorldParams");

        string[] lines = textAsset.text.Split('\n');
        lines = lines.Skip(1).ToArray();

        string[] data;
        foreach (string line in lines)
        {
            if (line.Equals("") || line.Equals("\r")) continue;
            string finalLine = line.Replace("\r", "");
            data = Utils.CsvRowData(finalLine, ";");
            parameters.Add(data[0], data[1]);
        }

        _pSe = double.Parse(
            parameters["p-se"].ToString(), CultureInfo.InvariantCulture);
        _pSeNonEssentialNodes = 
            double.Parse(parameters["p-se-non-essential-nodes"].ToString(), 
            CultureInfo.InvariantCulture);
        _pSeAcceptingMeasures = 
            double.Parse(parameters["p-se-accepting-measures"].ToString(),
            CultureInfo.InvariantCulture);
        _pSeLockdown = 
            double.Parse(parameters["p-se-lockdown"].ToString(),
            CultureInfo.InvariantCulture);
        _pId = 
            double.Parse(parameters["p-id"].ToString(),
            CultureInfo.InvariantCulture);
        _pIh = 
            double.Parse(parameters["p-ih"].ToString(),
            CultureInfo.InvariantCulture);
        _pHd = 
            double.Parse(parameters["p-hd"].ToString(),
            CultureInfo.InvariantCulture);
        _pHicu = 
            double.Parse(parameters["p-hicu"].ToString(),
            CultureInfo.InvariantCulture);
        _pIcud = 
            double.Parse(parameters["p-icud"].ToString(),
            CultureInfo.InvariantCulture);

        _recoveredDays = int.Parse(parameters["recovered-days"].ToString());
        _infectiousDaysToDead = 
            int.Parse(parameters["infectious-days-to-dead"].ToString());
        _infectiousDaysToHospitalized = 
            int.Parse(parameters["infectious-days-to-hospitalized"].ToString());
        _infectiousDaysToRecovered =
            int.Parse(parameters["infectious-days-to-recovered"].ToString());
        _daysToStartQuarantine =
            int.Parse(parameters["days-to-start-quarantine"].ToString());
        _hospitalizedDaysToDead = 
            int.Parse(parameters["hospitalized-days-to-dead"].ToString());
        _hospitalizedDaysToIcu =
            int.Parse(parameters["hospitalized-days-to-icu"].ToString());
        _hospitalizedDaysToRecovered =
            int.Parse(parameters["hospitalized-days-to-recovered"].ToString());
        _icuDaysToDead = int.Parse(parameters["ICU-days-to-dead"].ToString());
        _icuDaysToRecovered =
            int.Parse(parameters["ICU-days-to-recovered"].ToString());

        _numFriends = int.Parse(parameters["num-friends"].ToString());  
        _newFriends = int.Parse(parameters["new-friends"].ToString());

        _randomFriendProb = 
            double.Parse(parameters["random-friend-prob"].ToString(),
            CultureInfo.InvariantCulture);  
        _probNotEssentialNode = 
            double.Parse(parameters["prob-not-essential-node"].ToString(),
            CultureInfo.InvariantCulture);        
        _socialReach = 
            double.Parse(parameters["social-reach"].ToString(), 
            CultureInfo.InvariantCulture);
            
        _firstWorkingDay = (Day)System.Enum.Parse(
            typeof(Day), parameters["first-working-day"].ToString());
        _firstLeisureDay = (Day)System.Enum.Parse(
            typeof(Day), parameters["first-leisure-day"].ToString());
        _initialInfected = int.Parse(parameters["initial-infected"].ToString());
    }

}