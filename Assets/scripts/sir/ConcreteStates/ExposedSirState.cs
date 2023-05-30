using UnityEngine;

public class ExposedSirState : SirState
{
    public ExposedSirState(Citizen citizen) : base(citizen)
    {
        Utils.ChangeColor(_citizen, StateColor.Yellow);
        _citizen.typeOfState = TypeOfState.Exposed;
        _citizen.transform.SetParent(GameObject.Find("Exposeds").transform);
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