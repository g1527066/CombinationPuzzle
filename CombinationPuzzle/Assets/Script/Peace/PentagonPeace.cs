using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PentagonPeace : Peace
{
    public override PeaceForm GetPeaceForm
    {
        get { return PeaceForm.Pentagon; }
    }
    public override PeaceColor GetPeaceColor
    {
        get { return PeaceColor.None; }
    }

        public override PeaceForm GetNextPeaceForm
    {
        get { return PeaceForm.Square; }
    }
}
