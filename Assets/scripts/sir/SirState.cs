using System.Collections;
using System.Collections.Generic;

using Unity.IO.LowLevel.Unsafe;

using UnityEngine;

public enum TypeOfState
{
    Susceptible,
    Exposed,
    Infected,
    Hospitalized,
    ICU,
    Dead,
    Recovered,
}

[System.Serializable]
public abstract class SirState
{
    protected Citizen _citizen;
    protected TypeOfState _nextState;
    protected WorldParameters _worldParameters;
    protected int _timeToStateUpdate;

    public SirState(Citizen citizen)
    {
        this._citizen = citizen;
        this._worldParameters = WorldParameters.GetInstance();
        CalculateNextState();
    }

    protected abstract void CalculateNextState();
    protected abstract void ChangeState();

    public virtual void UpdateState()
    {
        if (_timeToStateUpdate == _citizen.CurrentTick)
        {
            ChangeState();
        }
    }

    protected void SetTimeToStateUpdate(int timeToStateUpdate)
    {
        this._timeToStateUpdate = _citizen.CurrentTick + timeToStateUpdate;
    }


}
