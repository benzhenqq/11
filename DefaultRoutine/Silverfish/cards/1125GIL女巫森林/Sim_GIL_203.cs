using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_GIL_203 : SimTemplate //* 责难 Rebuke
	{
		//Enemy spells cost (5) more next turn.
		//下个回合敌方法术的法力值消耗增加（5）点。
		// 别介意，我只是试下责难能不能用出去
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			p.allMinionOfASideGetBuffed(true, 0, 3);
		}
		
	}
}
