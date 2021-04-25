using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DAL_049 : SimTemplate //下水道渔人
	{
        //
        public override void onCardIsGoingToBePlayed(Playfield p, Handmanager.Handcard hc, bool ownplay, Minion m)
        {
            if (m.own == ownplay && (TAG_RACE)m.handcard.card.race == TAG_RACE.MURLOC) {
                p.evaluatePenality -= 5;
                p.drawACard(CardDB.cardName.primalfinchampion, m.own);
            }
        }
	}
}