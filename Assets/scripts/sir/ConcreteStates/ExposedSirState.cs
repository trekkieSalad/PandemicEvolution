using UnityEngine;

public class ExposedSirState : SirState
{
    public ExposedSirState(Citizen citizen) : base(citizen)
    {
        Utils.ChangeColor(_citizen, StateColor.Yellow);
        _citizen.typeOfState = TypeOfState.Exposed;
    }

    protected override void CalculateNextState()
    {
        _nextState = TypeOfState.Infected;
        SetTimeToStateUpdate(Utils.logNormal());
    }

    protected override void ChangeState()
    {
        _citizen.actualState = new InfectedSirState(_citizen);
        _citizen.asintomatic = Random.value < 0.4;
    }
}