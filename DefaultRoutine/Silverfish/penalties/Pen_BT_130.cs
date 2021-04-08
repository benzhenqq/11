using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Pen_BT_130 : PenTemplate //过度生长
	{

//    erhaltet einen leeren manakristall.
		public override int getPlayPenalty(Playfield p, Minion m, Minion target, int choice, bool isLethal)
		{
			if(p.ownMaxMana <= 4) return -60;
			else if(p.ownMaxMana <= 6) return -10;
			return 0;
		}

	}
}