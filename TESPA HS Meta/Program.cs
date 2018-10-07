using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

namespace TespaMeta
{
    class Analyzer
    {
        static void Main(string[] args)
        {
            string target = "Flame Imp";

            Dictionary<string, int> dbfIDs = new Dictionary<string, int>();
            dbfIDs.Add("Wispering Woods", 47063);
            dbfIDs.Add("Baku the Mooneater", 48158);

            int warlockZoo=0, warlockEven=0, warlockControl=0, warlockCube=0, 
                druidToken=0, 
                paladinOdd=0;


            string filePath = "decks.txt";
            DeckstringReader reader = new DeckstringReader(filePath);

            HearthDb.Deckstrings.Deck deck;
            Dictionary<HearthDb.Card, int> cards;
            //foreach (string deckCode in reader)
            //{
                deck = HearthDb.Deckstrings.DeckSerializer.Deserialize("AAECAaIHBKICsgKvBJ74Ag2MAssD1AX1Bd0IgcICn8IC68IC0eECi+UCpu8Cx/gC4vgCAA==");
                cards = deck.GetCards();
                Console.WriteLine("Class: " + deck.GetHero().Class + "\n");
                foreach(HearthDb.Card key in cards.Keys)
                {
                    Console.WriteLine(key.Name + ": " + key.DbfId);
                    //if (key.DbfId == dbfIDs.GetValueOrDefault("Wispering Woods"))
                        //druidToken++;
                }
                //Console.WriteLine("Token Druids" + druidToken);
                Console.Read();
            //}
        }
    }

    /// <summary>
    /// reads a text file copy-paste of TESPA's google sheet to find deck codes (they all start with ([)AAECA
    /// </summary>
    class DeckstringReader : IEnumerable
    {
        //ArrayList deckCodes = new ArrayList();
        public DeckstringReader(string filepath)
        {
            
        }

        public IEnumerator GetEnumerator()
        {
            yield return "AAECAQcIogLPxwKa7gLN7wKd8AKS+AKe+AKggAMLS6IE3gX/B5vCAsrnAuL4AoP7Ao77Ap77ArP8AgA";
            yield break;
        }
    }

}