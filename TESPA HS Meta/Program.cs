using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;


/*Console.WriteLine("\n\nDeck not recognized: ");
                                cardsAsCards = deck.GetCards();
                                foreach (HearthDb.Card card in cardsAsCards.Keys)
                                {
                                    Console.WriteLine(card.Name + ": " + card.DbfId);
                                }
                                */
namespace TespaMeta
{
    class Analyzer
    {
        static void Main(string[] args)
        {
            //these are the cards that are used to identify archetypes.
            Dictionary<string, int> dbfIDs = new Dictionary<string, int>();
            dbfIDs.Add("Wispering Woods", 47063);
            dbfIDs.Add("Baku", 48158);
            dbfIDs.Add("Malygos", 436);
            dbfIDs.Add("Druid Quest", 41099);
            dbfIDs.Add("King Togwaggle", 46589);
            dbfIDs.Add("Azalina Soulthief", 46874);
            dbfIDs.Add("Hadronox", 43439);
            dbfIDs.Add("Mecha'thun", 48625);
            dbfIDs.Add("Spiteful Summoner", 46551);
            dbfIDs.Add("Rogue Quest", 41222);
            dbfIDs.Add("Kingsbane", 47035);
            dbfIDs.Add("Devilsaur Egg", 41259);
            dbfIDs.Add("Academic Espionage", 48040);
            dbfIDs.Add("Pogo-Hopper", 48471);
            dbfIDs.Add("Leeroy Jenkins", 559);
            dbfIDs.Add("Genn Greymane", 47693);
            dbfIDs.Add("Flame Imp", 1090);
            dbfIDs.Add("Carnivorous Cube", 45195);
            dbfIDs.Add("Voidlord", 46056);
            dbfIDs.Add("Zola", 46403);
            dbfIDs.Add("Kangor's Army", 49009);
            dbfIDs.Add("Murloc Warleader", 1063);
            dbfIDs.Add("Warrior Quest", 41427);

            int decksScanned = 0, invalidDecks = 0;
            int warlockZoo = 0, warlockEven = 0, warlockControl = 0, warlockCube = 0, warlockMechathun = 0, warlockOther = 0,
                druidToken = 0, druidMalygos = 0, druidTogwaggle = 0, druidQuest = 0, druidTaunt = 0, druidMechathun = 0, druidSpiteful = 0, druidOther = 0,
                paladinOdd = 0, paladinEven = 0, paladinOther = 0, paladinOTK = 0, paladinMech = 0, paladinMurloc = 0,
                rogueOdd = 0, rogueOther = 0, rogueQuest = 0, rogueMaly = 0, rogueKingsbane = 0, rogueDeathrattle = 0, rogueThief = 0, roguePogo = 0, rogueAggro = 0,
                warriorOdd = 0, warriorOddQuest = 0, warriorQuest = 0, warriorMechathun = 0, warriorOther = 0;

            DeckstringReader reader = new DeckstringReader();

            HearthDb.Deckstrings.Deck deck;
            Dictionary<int, int> cardDBFIDs;
            Dictionary<HearthDb.Card, int> cardsAsCards;
            foreach (string deckCode in reader)
            {
                if (deckCode.Length > 1)
                {
                    decksScanned++;
                    try
                    {
                        deck = HearthDb.Deckstrings.DeckSerializer.Deserialize(deckCode);
                        cardDBFIDs = deck.CardDbfIds;
                        if (deck.GetHero().Class.ToString() == "DRUID")
                        {
                            if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Wispering Woods")))
                                druidToken++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Malygos")))
                                druidMalygos++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("King Togwaggle")))
                                druidTogwaggle++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Hadronox")))
                                druidTaunt++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Druid Quest")))
                                druidQuest++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Mecha'thun")))
                                druidMechathun++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Spiteful Summoner")))
                                druidSpiteful++;
                            else
                            {
                                druidOther++;
                            }
                        }
                        else if (deck.GetHero().Class.ToString() == "ROGUE")
                        {
                            if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Baku")))
                                rogueOdd++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Rogue Quest")))
                                rogueQuest++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Malygos")))
                                rogueMaly++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Kingsbane")))
                                rogueKingsbane++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Devilsaur Egg")))
                                rogueDeathrattle++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Academic Espionage")))
                                rogueThief++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Pogo-Hopper")))
                                roguePogo++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Leeroy Jenkins"))) //but NOT Baku
                                rogueAggro++;
                            else
                            {
                                rogueOther++;
                            }
                        }
                        else if (deck.GetHero().Class.ToString() == "WARLOCK")
                        {
                            if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Mecha'thun")))
                                warlockMechathun++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Flame Imp")))
                                warlockZoo++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Genn Greymane")))
                                warlockEven++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Carnivorous Cube")))
                                warlockCube++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Voidlord")))
                                warlockControl++;
                            else
                            {
                                warlockOther++;
                            }
                        }
                        else if (deck.GetHero().Class.ToString() == "PALADIN")
                        {
                            if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Baku")))
                                paladinOdd++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Genn Greymane")))
                                paladinEven++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Zola")))
                                paladinOTK++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Kangor's Army")))
                                paladinMech++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Murloc Warleader")))
                                paladinMurloc++;
                            else
                            {
                                paladinOther++;
                            }
                        }
                        else if (deck.GetHero().Class.ToString() == "WARRIOR")
                        {
                            if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Baku")))
                            {
                                if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Warrior Quest")))
                                    warriorOddQuest++;
                                else
                                    warriorOdd++;
                            }
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Warrior Quest")))
                                warriorQuest++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Mecha'thun")))
                                warriorMechathun++;
                            else
                            {
                                warriorOther++;
                            }

                        }
                    }
                    catch (ArgumentException)
                    {
                        invalidDecks++;
                    }
                }
            }

            Console.WriteLine("\n\nMeta Analysis Done. Unrecognized Decks Above. Press ENTER to continue to overview.");
            Console.ReadLine();

            Console.Clear();
            Console.WriteLine("Invalid Decks: " + invalidDecks);
            //Druid data
            Console.WriteLine("\nDruid Archetypes: " +
                "\n Token Druids: \t\t" + druidToken +
                "\n Malygos Druids: \t" + druidMalygos +
                "\n Togwaggle Druids: \t" + druidTogwaggle +
                "\n Taunt Druids: \t\t" + druidTaunt +
                "\n Mecha'thun Druids: \t" + druidMechathun +
                "\n Spiteful Druids: \t" + druidSpiteful +
                "\n Quest Druids: \t\t" + druidQuest +
                "\n Other Druids: \t\t" + druidOther);
            //Rogue data
            Console.WriteLine("\n\nRogue Archetypes: " +
                "\n Odd Rogues: \t\t" + rogueOdd +
                "\n Quest Rogue: \t\t" + rogueQuest +
                "\n Kingsbane Rogues: \t" + rogueKingsbane +
                "\n Thief Rogues: \t\t" + rogueThief +
                "\n Malygos Rogue: \t" + rogueMaly +
                "\n Deathrattle Rogues: \t" + rogueDeathrattle +
                "\n Aggro Rogues: \t\t" + rogueAggro +
                "\n Pogo-Hopper Rogues: \t" + roguePogo +
                "\n Other Rogues: \t\t" + rogueOther);
            //Warlock Data
            Console.WriteLine("\n\nWarlock Archetypes: " +
                "\n Zoo Warlocks: \t\t" + warlockZoo + 
                "\n Even Warlocks: \t" + warlockEven + 
                "\n Control Warlocks: \t" + warlockControl + 
                "\n Cube Warlocks: \t" + warlockCube +
                "\n Mecha'thun Warlocks: \t" + warlockMechathun +
                "\n Other Warlocks: \t" + warlockOther);
            Console.WriteLine("\n\nPaladin Archetypes: " +
                "\n Odd Paladin: \t\t" + paladinOdd + 
                "\n Even Paladin: \t\t" + paladinEven + 
                "\n OTK Paladin: \t\t" + paladinOTK + 
                "\n Mech Paladin: \t\t" + paladinMech + 
                "\n Murloc Paladin: \t" + paladinMurloc +
                "\n Other Paladin: \t" + paladinOther);
            Console.WriteLine("\n\nWarrior Archetypes: " +
                "\n Odd Warriors: \t\t" + warriorOdd + 
                "\n Odd Quest Warriors: \t" + warriorOddQuest + 
                "\n Quest Warriors: \t" + warriorQuest + 
                "\n Mecha'thun Warriors: \t" + warriorMechathun + 
                "\n Other Warriors: \t" + warriorOther);
            Console.ReadLine();
        }
    }

    /// <summary>
    /// returns deck codes from a Google Sheet (one cell at a time;
    /// we're just assuming everything is a deck code without checking because I have exclusive control of the sheet.)
    /// </summary>
    class DeckstringReader : IEnumerable
    {
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "TESPA HS Meta";
        UserCredential credential;

        public IEnumerator GetEnumerator()
        {

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(
                        "1-0fhWItkpb_bG5y0ZHnao-EuFYa7tbFqKM6MOts1IT8", //the ID of the spreadsheet
                        "A:D"); //the range of cells we want from the spreadsheet


            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    foreach (var cell in row)
                    {
                        yield return cell;
                    }
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
        }
    }

}
