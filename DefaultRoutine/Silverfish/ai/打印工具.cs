namespace HREngine.Bots
{
    using System;
    using System.IO;
    public class printUtils
    {
        public static bool printNextMove = false;

        public static bool printPentity = false;
        

        // type 取值 0 （打印下一步）， 1（打印自定义惩罚）
        public static void printDebuggerInfo(CardDB.Card card, string content, int pen, int type){
            if(pen == 0) return;
            if(printNextMove && type == 0 || printPentity && type == 1 ){
                Helpfunctions.Instance.ErrorLog(card.卡名 + content + pen+"");
            }
        }
    }
}