using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Pen_BAR_074 : PenTemplate //* 莫尔杉哨所 Mor'shan Watch Post
	{

//    spott/
        public override int getPlayPenalty(Playfield p, Minion m, Minion target, int choice, bool isLethal)
        {
            if(p.ownMaxMana <= 3) return -10;
			return 0;
        }

	}
}