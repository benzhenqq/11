using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_BAR_040 : SimTemplate //* 南海岸酋长 South Coast Chieftain
	{
		//<b>Battlecry:</b> If you control another Murloc, deal 2_damage.
		//<b>战吼：</b>如果你控制另一个鱼人，则造成2点伤害。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            List<Minion> temp = (own.own) ? p.ownMinions : p.enemyMinions;
            foreach (Minion m in temp)
            {
                if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MURLOC)
                {
					p.minionGetDamageOrHeal(target, 2, false);
                    break;
                }
            }
        }
		
		
	}
}
