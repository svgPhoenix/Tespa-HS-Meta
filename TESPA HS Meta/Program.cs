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

            int warlockZoo=0, warlockEven=0, warlockControl=0, warlockCube=0, druidToken=0;


            string filePath = "decks.txt";
            DeckstringReader reader = new DeckstringReader(filePath);

            HearthDb.Deckstrings.Deck deck;
            Dictionary<HearthDb.Card, int> cards;
            foreach (string deckCode in reader)
            {
                deck = HearthDb.Deckstrings.DeckSerializer.Deserialize(deckCode);
                cards = deck.GetCards();
                foreach(HearthDb.Card key in cards.Keys)
                {
                    if (key.Name == target)
                        Console.WriteLine("dbfID of " + target + ": " + key.DbfId);
                    //if (key.DbfId == dbfIDs.GetValueOrDefault("Wispering Woods"))
                        //druidToken++;
                }
                //Console.WriteLine("Token Druids" + druidToken);
                Console.Read();
            }
        }
    }

    /// <summary>
    /// reads a text file copy-paste of TESPA's google sheet to find deck codes (they all start with ([)AAECA
    /// </summary>
    class DeckstringReader : IEnumerable
    {
        ArrayList deckCodes = new ArrayList();
        public DeckstringReader(string filepath)
        {
            
        }

        public IEnumerator GetEnumerator()
        {
            yield return "AAECAf0GApziAo+CAw4whAH3BM4Hwgj3DJvLAp/OAvLQAtHhAofoAu/xAvT3AtP4AgA";
            yield break;
        }
    }

}