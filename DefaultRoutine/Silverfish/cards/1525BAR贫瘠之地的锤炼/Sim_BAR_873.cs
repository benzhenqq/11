using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_BAR_873 : SimTemplate //* 圣礼骑士 Knight of Anointment
	{
		//<b>Battlecry:</b> Draw aHoly spell.
		//<b>战吼：</b>抽一张神圣法术牌。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardIDEnum.BT_025, own.own, true);			
        }
		
	}
}
