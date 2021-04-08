using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Pen_CORE_CS2_013 : PenTemplate //* 野性成长
	{

//    spott/
        public override int getPlayPenalty(Playfield p, Minion m, Minion target, int choice, bool isLethal)
        {
            if(p.ownMaxMana <= 4) return -40;
			return 0;
        }

	}
}