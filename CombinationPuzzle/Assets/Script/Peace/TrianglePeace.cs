using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrianglePeace : Peace
{
    public override PeaceForm GetPeaceForm
    {
        get { return PeaceForm.Triangle; }
    }
    public override PeaceColor GetPeaceColor
    {
        get { return peaceColor; }
    }
    public override PeaceForm GetNextPeaceForm
    {
        get { return PeaceForm.Square; }
    }
}
