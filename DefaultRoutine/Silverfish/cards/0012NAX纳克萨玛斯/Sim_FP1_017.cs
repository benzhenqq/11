using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_FP1_017 : SimTemplate //nerubarweblord
	{

//    diener mit kampfschrei/ kosten (2) mehr.
        // 战吼随从 +2 费
        public override void onAuraStarts(Playfield p, Minion own)
		{
            foreach(Handmanager.Handcard hc in p.owncards)
            {
                CardDB.Card m = hc.card;
                if (m.battlecry) {
                    m.cost = m.cost + 2;
                }
            }
		}

        public override void onAuraEnds(Playfield p, Minion own)
        {
            foreach(Handmanager.Handcard hc in p.owncards)
            {
                CardDB.Card m = hc.card;
                if (m.battlecry) {
                    m.cost = m.cost - 2;
                }
            }
        }


	}
}