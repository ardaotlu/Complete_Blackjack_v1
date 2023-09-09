using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Complete_Blackjack_v1
{
     public class Player
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string Id { get; set; }

        public string Name { get; set; }
        public string Password { get; set; }
        public string Score { get; set; }



    }
}
