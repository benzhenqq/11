using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_ULD_726 : SimTemplate //远古谜团
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			p.drawACard(CardDB.cardIDEnum.TRL_400, ownplay, true);
			Handmanager.Handcard newHc = p.owncards[p.owncards.Count - 1];
			newHc.manacost = 0;
			p.evaluatePenality += 15;
		}
	}
}