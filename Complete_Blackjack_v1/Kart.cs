using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Complete_Blackjack_v1
{
    public class Kart
    {
        private static int cardID = 0;

        public int ID = 0;

        public int cardID2
        {
            get
            {
                return cardID;
            }
            set
            {
            }
        }

        public string Type { get; set; }
        public int Mag { get; set; }

        public Kart(string type, int value)
        {
            ID = GetNextID();
            Type = type;
            Mag = value;
        }

        protected int GetNextID()
        {
            return ++cardID;
        }


    }

}
