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

    #region Life Cycle Methods

    public override void Awake()
    {
        AbstractController controller =
            FindObjectOfType<AbstractController>();

        // Añadimos el agente al mundo y obtenemos su cola de comportamientos
        if (controller == null) Debug.LogError("No controller found");
        else
        {
            base.Awake();
            base.Init();
            CreateStepper(BehaviourQueue);
        }
    }

    private void InquireBehaviour()
    {
        if (dissonanceStrength > 0 &&
            (needADilemma || needBDilemma))
        {
            //Debug.Log("InquireBehaviour " + this.name);
            InquireComm();
        }
    }

    private void SignalBehaviour()
    {
        if (dissonanceStrength > 0 &&
            membershipDilemma)
            SignalComm();
    }

    public void BehaviourQueue()
    {
        inquiring = false;
        signaling = false;
        randomConversation = false;

        InquireBehaviour();
        SignalBehaviour();

        resetComms();
    }

    #endregion

    #region Communication Methods

    private void InquireComm()
    {
        List<Relationship> sortRelationships = Relationships.
            OrderBy(f => f.inquired).
            ThenByDescending(f => f.sameBehavior).
            ThenByDescending(f => f.persuasion).ToList();

        Citizen inquiredFriend = sortRelationships[0].receptor;
        inquiring = true;

        // Todo esto se puede actualizar al usar diccionarios

        double similarityNeedAImportanceA = needSimilarity(
            needAEvaluationA, inquiredFriend.needAEvaluationA,
            needAImportance, inquiredFriend.needAImportance);

        double similarityNeedBImportanceA = needSimilarity(
            needBEvaluationA, inquiredFriend.needBEvaluationA,
            needBImportance, inquiredFriend.needBImportance);

        double similarityNeedAImportanceB = needSimilarity(
            needAEvaluationB, inquiredFriend.needAEvaluationB,
            needAImportance, inquiredFriend.needAImportance);

        double similarityNeedBImportanceB = needSimilarity(
            needBEvaluationB, inquiredFriend.needBEvaluationB,
            needBImportance, inquiredFriend.needBImportance);

        double persuasionNeedAA =
            sortRelationships[0].trust * similarityNeedAImportanceA;
        double persuasionNeedBA =
            sortRelationships[0].trust * similarityNeedBImportanceA;
        double persuasionNeedAB =
            sortRelationships[0].trust * similarityNeedAImportanceB;
        double persuasionNeedBB =
            sortRelationships[0].trust * similarityNeedBImportanceB;

        needASatisfactionA = newNeedSatisfaction(needASatisfactionA,
            persuasionNeedAA, inquiredFriend.needASatisfactionA);
        needBSatisfactionA = newNeedSatisfaction(needBSatisfactionA,
            persuasionNeedBA, inquiredFriend.needBSatisfactionA);
        needASatisfactionB = newNeedSatisfaction(needASatisfactionB,
            persuasionNeedAB, inquiredFriend.needASatisfactionB);
        needBSatisfactionB = newNeedSatisfaction(needBSatisfactionB,
            persuasionNeedBB, inquiredFriend.needBSatisfactionB);

        updateEvaluations();
        updateDissonances();
        calculateBehavior();
        updateEvaluations();
        updateDissonances();

        sortRelationships[0].persuasion = persuasionNeedAA + persuasionNeedBA +
            persuasionNeedAB + persuasionNeedBB;
        sortRelationships[0].inquired = true;
    }

    private void SignalComm()
    {
        List<Relationship> sortRelationships = Relationships.
            OrderBy(f => f.signaled).
            ThenByDescending(f => f.sameBehavior).
            ThenByDescending(f => f.gullibility).ToList();

        Relationship relationship = sortRelationships[0];
        signaling = true;

        updateRelationshipReceptor(relationship, true);

        updateEvaluations();
        updateDissonances();
        calculateBehavior();
        updateEvaluations();
        updateDissonances();

    }

    private void updateRelationshipReceptor(Relationship relationship, bool signalComm = false)
    {

        Citizen receptor = relationship.receptor;

        double similarityNeedAImportanceA = needSimilarity(
            needAEvaluationA, receptor.needAEvaluationA,
            needAImportance, receptor.needAImportance);

        double similarityNeedBImportanceA = needSimilarity(
            needBEvaluationA, receptor.needBEvaluationA,
            needBImportance, receptor.needBImportance);

        double similarityNeedAImportanceB = needSimilarity(
            needAEvaluationB, receptor.needAEvaluationB,
            needAImportance, receptor.needAImportance);

        double similarityNeedBImportanceB = needSimilarity(
            needBEvaluationB, receptor.needBEvaluationB,
            needBImportance, receptor.needBImportance);



        List<Relationship> friendFriendships = receptor.Relationships;
        Relationship reverseLink = null;
        foreach (Relationship friendship in friendFriendships)
        {
            if (friendship.receptor == this)
            {
                reverseLink = friendship;
                break;
            }
        }

        double sigTrust = reverseLink.trust;
        double persuasionNeedAA = sigTrust * similarityNeedAImportanceA;
        double persuasionNeedBA = sigTrust * similarityNeedBImportanceA;
        double persuasionNeedAB = sigTrust * similarityNeedAImportanceB;
        double persuasionNeedBB = sigTrust * similarityNeedBImportanceB;

        receptor.needASatisfactionA = newNeedSatisfaction(
            receptor.needASatisfactionA, persuasionNeedAA,
            needASatisfactionA);
        receptor.needBSatisfactionA = newNeedSatisfaction(
            receptor.needBSatisfactionA, persuasionNeedBA,
            needBSatisfactionA);
        receptor.needASatisfactionB = newNeedSatisfaction(
            receptor.needASatisfactionB, persuasionNeedAB,
            needASatisfactionB);
        receptor.needBSatisfactionB = newNeedSatisfaction(
            receptor.needBSatisfactionB, persuasionNeedBB,
            needBSatisfactionB);

        receptor.updateEvaluations();
        receptor.updateDissonances();
        receptor.calculateBehavior();
        receptor.updateEvaluations();
        receptor.updateDissonances();

        if (signalComm)
        {
            relationship.gullibility = persuasionNeedAA + persuasionNeedBB;
            relationship.signaled = true;
        }

    }

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

    private double needSimilarity(
        double needEvaluation, double friendNeedEvaluation,
        double needImportance, double friendNeedImportance)
    {
        if (needEvaluation * friendNeedEvaluation > 0)
            return 0.4 * (1 - Math.Abs(needImportance - friendNeedImportance));
        else
            return 0;
    }

    private double newNeedSatisfaction(double needSatisfaction,
        double needPersuasion, double friendNeedSatisfaction)
    {
        return (1 - needPersuasion) * needSatisfaction +
            needPersuasion * friendNeedSatisfaction;
    }

    private void resetComms()
    {
        if (friendships.All(f => f.inquired))
        {
            foreach (Relationship friendship in friendships)
                friendship.inquired = false;
        }


        if (friendships.All(f => f.signaled))
        {
            foreach (Relationship friendship in friendships)
                friendship.signaled = false;
        }
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