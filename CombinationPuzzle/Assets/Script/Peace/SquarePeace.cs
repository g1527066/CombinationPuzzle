using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquarePeace : Peace {

    public override PeaceForm GetPeaceForm
    {
        get { return PeaceForm.Square; }
    }
    public override PeaceColor GetPeaceColor
    {
        get { return PeaceColor.None; }
    }
        public override PeaceForm GetNextPeaceForm
    {
        get { return PeaceForm.Pentagon; }
    }
}
