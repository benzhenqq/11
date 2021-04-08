using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Pen_CS2_013 : PenTemplate //wildgrowth
	{

//    erhaltet einen leeren manakristall.
		public override int getPlayPenalty(Playfield p, Minion m, Minion target, int choice, bool isLethal)
		{
			if(p.ownMaxMana <= 3) return -40;
			else if(p.ownMaxMana <= 5) return -10;
			return 0;
		}

	}
}