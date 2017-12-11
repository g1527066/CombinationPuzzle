using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrianglePeace : Peace {

    public override PeaceForm GetPeaceForm
    {
        get { return PeaceForm.triangle; }
    }
    public override PeaceColor GetPeaceColor
    {
        get { return peaceColor; }
    }


}
