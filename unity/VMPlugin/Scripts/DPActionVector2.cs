using UnityEngine;
using System.Collections;

public abstract class DPActionVector2
{
    public delegate Vector2 DPVector2Action(float fU, float fV);
    public abstract Vector2 call(float fU, float fV);
}
