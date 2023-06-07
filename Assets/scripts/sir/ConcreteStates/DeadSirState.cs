using UnityEngine;

public class DeadSirState : SirState
{
    public DeadSirState(Citizen citizen) : base(citizen)
    {
        Utils.ChangeColor(_citizen, StateColor.Grey);
        Debug.Log("Dead " + _citizen.name);
        _citizen.typeOfState = TypeOfState.Dead;
        _citizen.Dead();
    }

    protected override void CalculateNextState()
    {
        return;
    }

    protected override void ChangeState()
    {
        return;
    }
}