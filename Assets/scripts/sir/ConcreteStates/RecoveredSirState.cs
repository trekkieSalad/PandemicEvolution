using UnityEngine;

public class RecoveredSirState : SirState
{
    public RecoveredSirState(Citizen citizen) : base(citizen)
    {
        Utils.ChangeColor(citizen, StateColor.Blue);
        _citizen.typeOfState = TypeOfState.Recovered;
        _citizen.transform.SetParent(GameObject.Find("Recovereds").transform);
    }

    protected override void CalculateNextState()
    {
        _nextState = TypeOfState.Susceptible;
        SetTimeToStateUpdate(_worldParameters.recoveredDays);
    }

    protected override void ChangeState()
    {
        _citizen.actualState = new SusceptibleSirState(_citizen);
    }
}