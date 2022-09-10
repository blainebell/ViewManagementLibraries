
using UnityEngine;

public class DPActionVector2Func : DPActionVector2
{
    public DPVector2Action actionPerformedFunc;
    override public Vector2 call(float fU, float fV)
    {
        return actionPerformedFunc.Invoke(fU, fV);
    }
}