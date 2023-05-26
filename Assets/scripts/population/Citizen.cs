using System.Collections.Generic;
using System.Linq;
using System;

using UnityEngine;
using ABMU.Core;

public enum StateColor
{
    Red,
    Green,
    Blue,
    Yellow,
    Grey,
}

public enum CitizenGender {    
    Male,
    Female,
}

public enum EconomicActivity {
    Employed = 1,
    Unemployed = 2,
    Freelance = 3,
    CivilServant = 4,
    Executive = 5,
    Student = 6,
    Inactive = 7,
}


public enum Behavior {
    Accept,
    Reject,
}

public class Citizen : AbstractAgent {

    // ATTRIBUTES

    #region Simulation Attributes
    [Header("Simulation")]
    public bool simulated;

    #endregion

    #region Sociodemographic Attributes
    [Header("Sociodemographic")]
    public CitizenGender gender;
    public int age;
    public int family;
    public bool ruralHouse; //preguntar si debería ser booleano
    public EconomicActivity economicActivity;
    public bool essentialJob; //preguntar si debería ser booleano
    public int netIncome;

    #endregion

    #region Behavior Attributes
    [Header("Behavior")]
    public double needAImportance;
    public double needBImportance;
    public double membershipImportance;

    public double needASatisfactionA;
    public double needBSatisfactionA;
    public double needBSatisfactionB;
    public double needASatisfactionB;
    public double membershipSatisfactionA;
    public double membershipSatisfactionB;
    
    public double needAEvaluationA;
    public double needBEvaluationA;
    public double needBEvaluationB;
    public double needAEvaluationB;
    public double membershipEvaluationA;
    public double membershipEvaluationB;

    public double satisfactionA;
    public double satisfactionB;
    public double satisfaction;

    public bool needADilemma;
    public bool needBDilemma;
    public bool membershipDilemma;

    public Behavior behavior;
    public double dissonanceTolerance;
    public double dissonanceA;
    public double dissonanceB;
    public double dissonanceStrength;
    public string section;

    #endregion

    #region SIR Attributes
    [Header("SIR")]
    public StateColor color;
    //public SirState actualState;
    public bool quarantine;
    public bool asintomatic;

    #endregion

    #region Critical Nodes Attributes
    [Header("Critical Nodes")]
    public double cityCouncilTrust;
    public double politicalOppositionTrust;
    public double localMediaTrust;
    public double localMediaOppositionTrust;

    #endregion

    #region Interaction Attributes
    [Header("Interaction")]
    [SerializeField]
    public List<Relationship> friendships =
        new List<Relationship>();
    [SerializeField]
    public List<Relationship> neighbors =
        new List<Relationship>();

    public List<Relationship> Relationships
    {
        get
        {
            List<Relationship> relationships = new List<Relationship>();
            relationships.AddRange(neighbors);
            relationships.AddRange(friendships);
            return relationships;
        }
    }
    public bool inquiring;
    public bool signaling;
    public bool randomConversation;

    #endregion

    #region Public Agent Methods

    public void createSocialNetwork()
    {
        // Creamos las redes sociales
        RelationshipFactory friendshipFactory = new FriendshipFactory(controller);
        friendshipFactory.createNetwork(this);
    }
    public void updateEvaluations(bool initialization = false)
    {
        needAEvaluationA = needAImportance * needASatisfactionA;
        needBEvaluationA = needBImportance * needBSatisfactionA;
        needAEvaluationB = needAImportance * needASatisfactionB;
        needBEvaluationB = needBImportance * needBSatisfactionB;

        double similar = 0;
        double different = 0;
        foreach (Relationship friendship in friendships)
        {
            if (friendship.sameBehavior)
                similar++;
            else
                different++;
        }
        similar = similar / friendsNumber;
        different = different / friendsNumber;

        membershipSatisfactionA = 0;
        membershipSatisfactionB = 0;

        if (behavior.Equals(Behavior.Accept))
        {
            membershipSatisfactionA =
                Utils.maxMinNormalize(similar, 0, 1, -1, 1);
            membershipSatisfactionB =
                Utils.maxMinNormalize(different, 0, 1, -1, 1);
        }
        else if (initialization)
        {
            membershipSatisfactionA =
                Utils.maxMinNormalize(different, 0, 1, -1, 1);
            membershipSatisfactionB =
                Utils.maxMinNormalize(similar, 0, 1, -1, 1);
        }
        else
        {
            membershipSatisfactionA = different;
            membershipSatisfactionB = similar;
        }

        membershipEvaluationA = membershipImportance * membershipSatisfactionA;
        membershipEvaluationB = membershipImportance * membershipSatisfactionB;

        satisfactionA =
            (needAEvaluationA + membershipEvaluationA + needBEvaluationA) / 3;
        satisfactionB =
            (needAEvaluationB + membershipEvaluationB + needBEvaluationB) / 3;
    }

    public void updateDissonances()
    {
        needADilemma = false;
        needBDilemma = false;
        membershipDilemma = false;

        List<double> evaluationsList = new List<double>()
            {needAEvaluationA, needBEvaluationA, membershipEvaluationA};

        dissonanceA = dissonanceStatus(evaluationsList);
        double dissonanceStrengthA =
            (dissonanceA - dissonanceTolerance) / (1 - dissonanceTolerance);

        evaluationsList = new List<double>()
            {needAEvaluationB, needBEvaluationB, membershipEvaluationB};

        dissonanceB = dissonanceStatus(evaluationsList);
        double dissonanceStrengthB =
            (dissonanceB - dissonanceTolerance) / (1 - dissonanceTolerance);

        double evaluation1;
        double evaluation2;
        double membershipEvaluation;

        if (behavior.Equals(Behavior.Accept))
        {
            dissonanceStrength = Math.Max(0, dissonanceStrengthA);

            evaluation1 = needAEvaluationA;
            evaluation2 = needBEvaluationA;
            membershipEvaluation = membershipEvaluationA;

        }
        else
        {
            dissonanceStrength = Math.Max(0, dissonanceStrengthB);

            evaluation1 = needAEvaluationB;
            evaluation2 = needBEvaluationB;
            membershipEvaluation = membershipEvaluationB;
        }

        if ((evaluation1 > 0 && membershipEvaluation < 0 && evaluation2 < 0) ||
            (evaluation1 < 0 && membershipEvaluation > 0 && evaluation2 > 0))
            needADilemma = true;

        if ((evaluation1 < 0 && membershipEvaluation > 0 && evaluation2 < 0) ||
            (evaluation1 > 0 && membershipEvaluation < 0 && evaluation2 > 0))
            membershipDilemma = true;

        if ((evaluation1 < 0 && membershipEvaluation < 0 && evaluation2 > 0) ||
            (evaluation1 > 0 && membershipEvaluation > 0 && evaluation2 < 0))
            needBDilemma = true;
    }

    public void calculateBehavior()
    {
        bool random = UnityEngine.Random.value < 0.5;
        bool fSatisfaction =
            furtherComparisonNeeded(satisfactionA, satisfactionB, 2);
        bool fDissonance =
            furtherComparisonNeeded(dissonanceA, dissonanceB, 1);
        bool fEvaluation =
            furtherComparisonNeeded(needAEvaluationA, needAEvaluationB, 2);
        bool satisfaction = satisfactionA > satisfactionB;
        bool dissonance = dissonanceA < dissonanceB;
        bool evaluation = needAEvaluationA > needAEvaluationB;

        if (fSatisfaction && fDissonance && fEvaluation && random
            || fSatisfaction && fDissonance && !fEvaluation && evaluation
            || fSatisfaction && !fDissonance && dissonance
            || !fSatisfaction && satisfaction)
        {
            behavior = Behavior.Accept;
            this.satisfaction = satisfactionA;
        }
        else
        {
            behavior = Behavior.Reject;
            this.satisfaction = satisfactionB;
        }
    }

    #endregion

    #region Private Agent Methods

    private double dissonanceStatus(List<double> evaluationsList)
    {
        double satisfying = evaluationsList.Where(i => i > 0).ToList().Sum();
        double dissatisfying = evaluationsList.Where(i => i < 0).ToList().Sum();
        double dissonant = Math.Min(Math.Abs(satisfying), Math.Abs(dissatisfying));
        double consonant = Math.Max(Math.Abs(satisfying), Math.Abs(dissatisfying));
        double dissonance = ((dissonant + consonant) != 0) ?
            (2 * dissonant) / (dissonant + consonant) : 0;

        return dissonance;
    }

    private bool furtherComparisonNeeded(
            double dimensionA, double dimensionB, double theoricalRange)
    {
        return (dimensionA > dimensionB - 0.1 * theoricalRange &&
                dimensionA < dimensionB + 0.1 * theoricalRange);
    }

    #endregion

    #region Friendship methods

    public int friendsNumber
    {
        get { return friendships.Count; }
    }

    public void addFriendship(Citizen friend)
    {
        friendships.Add(new Relationship(this, friend));
    }

    public List<Citizen> getFriends()
    {
        List<Citizen> friends = new List<Citizen>();
        foreach (Relationship friendship in friendships)
        {
            friends.Add(friendship.receptor);
        }
        return friends;
    }

    public bool IsFriend(Citizen friend)
    {
        return getFriends().Contains(friend);
    }
    #endregion

    #region Social Circle methods

    public int neighborsNumber
    {
        get { return neighbors.Count; }
    }

    public void addNeighbor(Citizen neighbor)
    {
        neighbors.Add(new Relationship(this, neighbor));
    }

    public List<Citizen> getNeighbors()
    {
        List<Citizen> neighbors = new List<Citizen>();
        foreach (Relationship neighbor in this.neighbors)
        {
            neighbors.Add(neighbor.receptor);
        }
        return neighbors;
    }
    #endregion

    #region Citizen Utils
    public int currentTick
    {
        get { return controller.currentTick; }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireSphere(transform.position, 10f);
    }
    #endregion
}
