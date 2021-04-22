using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_VAN_EX1_283 : SimTemplate //* Frost Elemental
	{
        //Battlecry: Freeze a character.

		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.minionGetFrozen(target);
		}
	}
}