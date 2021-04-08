using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Pen_BAR_076 : PenTemplate //* 前沿哨所 Far Watch Post
	{

//    spott/
        public override int getPlayPenalty(Playfield p, Minion m, Minion target, int choice, bool isLethal)
        {
            if(p.ownMaxMana <= 2) return -20;
			return 0;
        }

	}
}