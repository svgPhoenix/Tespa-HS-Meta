﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;


namespace TespaMeta
{
    class Analyzer
    {
        static void PrintOptions()
        {
            Console.Out.WriteLine(
                            "Type the number that corresponds with the menu item:\n" +
                            "1: Deserialize a single code\n" +
                            "2: Analyze the meta of the entire tournament\n" +
                            "3: Analyze a specific opponent's lineup over all previous weeks (unimplemented)");
            char input = Convert.ToChar(Console.Read());
            Console.ReadLine();
            switch (input)
            {
                case '1':
                    DeserializeSingleDeck();
                    break;
                case '2':
                    SummarizeMeta();
                    break;
                case '3':
                    PreviewOpponent();
                    break;
                default:
                    Console.WriteLine("Please only type a number from the list.");
                    PrintOptions();
                    break;

            }
        }

        static void Main(string[] args)
        {
            Console.Clear();
            PrintOptions();
        }

        private static void DeserializeSingleDeck()
        {
            HearthDb.Deckstrings.Deck deck;
            Dictionary<HearthDb.Card, int> cards;

            Console.WriteLine("Paste a deckstring and press Enter.");
            string deckstring = Console.ReadLine();
            //deserialize the deck
            deck = HearthDb.Deckstrings.DeckSerializer.Deserialize(deckstring);
            cards = deck.GetCards();
            string toPrint = "#Deck Format: " + deck.Format;
            toPrint += "\n#Format Year: " + deck.Format;
            foreach (HearthDb.Card card in cards.Keys)
            {
                toPrint += "\n" + card.Name + ": " + card.DbfId;
            }
            Console.WriteLine(toPrint);
            Console.ReadLine();
        }

        private static void SummarizeMeta()
        {
            //these are the cards that are used to identify archetypes.
            Dictionary<string, int> dbfIDs = new Dictionary<string, int>
            {
                { "Wispering Woods", 47063 },
                { "Baku", 48158 },
                { "Malygos", 436 },
                { "Druid Quest", 41099 },
                { "King Togwaggle", 46589 },
                { "Azalina Soulthief", 46874 },
                { "Hadronox", 43439 },
                { "Mecha'thun", 48625 },
                { "Spiteful Summoner", 46551 },
                { "Rogue Quest", 41222 },
                { "Kingsbane", 47035 },
                { "Devilsaur Egg", 41259 },
                { "Academic Espionage", 48040 },
                { "Pogo-Hopper", 48471 },
                { "Leeroy Jenkins", 559 },
                { "Genn Greymane", 47693 },
                { "Soul Infusion", 48211 },
                { "Carnivorous Cube", 45195 },
                { "Voidlord", 46056 },
                { "Zola", 46403 },
                { "Kangor's Army", 49009 },
                { "Murloc Warleader", 1063 },
                { "Warrior Quest", 41427 },
                { "Aluneth", 43426 },
                { "Book of Specters", 47054 },
                { "Dragoncaller Alanna", 46499 },
                { "Archmage Antonidas", 1080 },
                { "Alexstraza", 581 },
                { "Zerek's Cloning Gallery", 49421 },
                { "Divine Spirit", 1361 },
                { "Al'Akir the Windlord", 32},
                { "The Lich King", 42818},
                { "Shudderwock", 48111},
                { "Rhok'delar", 43369},
                { "Subject 9", 49447}
            };

            int decksScanned = 0, invalidDecks = 0;
            int warlockZoo = 0, warlockEven = 0, warlockControl = 0, warlockCube = 0, warlockMechathun = 0, warlockOther = 0,
                druidToken = 0, druidMalygos = 0, druidTogwaggle = 0, druidQuest = 0, druidTaunt = 0, druidMechathun = 0, druidSpiteful = 0, druidOther = 0,
                paladinOdd = 0, paladinEven = 0, paladinOther = 0, paladinOTK = 0, paladinMech = 0, paladinMurloc = 0,
                rogueOdd = 0, rogueOther = 0, rogueQuest = 0, rogueMaly = 0, rogueKingsbane = 0, rogueDeathrattle = 0, rogueThief = 0, roguePogo = 0, rogueAggro = 0,
                warriorOdd = 0, warriorOddQuest = 0, warriorQuest = 0, warriorMechathun = 0, warriorOther = 0,
                mageTempo = 0, mageHand = 0, mageBigSpell = 0, mageExodia = 0, mageOther = 0,
                priestRez = 0, priestControl = 0, priestDivineSpirit = 0, priestMechathun = 0, priestMechathunQuest = 0, priestOther = 0,
                shamanEven = 0, shamanOther = 0, shamanMidrange = 0, shamanShudderwok = 0,
                hunterDeathrattle = 0, hunterSecret = 0, hunterSpell = 0, hunterOther = 0;

            DeckstringReader reader = new DeckstringReader();
            HearthDb.Deckstrings.Deck deck;
            Dictionary<int, int> cardDBFIDs;
            string toPrint = "";
            foreach (string deckCode in reader)
            {
                if (deckCode.Length > 1)
                {
                    decksScanned++;
                    try
                    {
                        deck = HearthDb.Deckstrings.DeckSerializer.Deserialize(deckCode);
                        cardDBFIDs = deck.CardDbfIds;
                        if (deck.GetHero().Class.ToString() == "HUNTER")
                        {
                            if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Devilsaur Egg")))
                                hunterDeathrattle++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Subject 9")))
                                hunterSecret++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Rhok'delar")))
                                hunterSecret++;
                            else
                            {
                                hunterOther++;
                            }
                        }
                        else if (deck.GetHero().Class.ToString() == "WARLOCK")
                        {
                            if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Genn Greymane")))
                                warlockEven++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Soul Infusion")))
                                warlockZoo++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Carnivorous Cube")))
                                warlockCube++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Voidlord")))
                                warlockControl++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Mecha'thun")))
                                warlockMechathun++;
                            else
                            {
                                warlockOther++;
                            }
                        }
                        else if (deck.GetHero().Class.ToString() == "MAGE")
                        {
                            if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Aluneth")))
                                mageTempo++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Book of Specters")))
                                mageHand++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Dragoncaller Alanna")))
                                mageBigSpell++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Archmage Antonidas")))
                                mageExodia++;
                            else
                            {
                                mageOther++;
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
                        else if (deck.GetHero().Class.ToString() == "DRUID")
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
                        else if (deck.GetHero().Class.ToString() == "SHAMAN")
                        {
                            if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Genn Greymane")))
                                shamanEven++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Al'Akir the Windlord")) || cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("The Lich King")))
                                shamanMidrange++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Shudderwock")))
                                shamanShudderwok++;
                            else
                            {
                                shamanOther++;
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
                        else if (deck.GetHero().Class.ToString() == "PRIEST")
                        {
                            if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Zerek's Cloning Gallery")))
                                priestRez++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Alexstraza")))
                                priestControl++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Mecha'thun")))
                            {
                                if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Priest Quest")))
                                    priestMechathunQuest++;
                                else
                                    priestMechathun++;
                            }
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Divine Spirit")))
                                priestDivineSpirit++;
                            else
                            {
                                priestOther++;
                            }
                        }
                        else if (deck.GetHero().Class.ToString() == "PALADIN")
                        {
                            if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Baku")))
                                paladinOdd++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Zola")))
                                paladinOTK++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Genn Greymane")))
                                paladinEven++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Kangor's Army")))
                                paladinMech++;
                            else if (cardDBFIDs.ContainsKey(dbfIDs.GetValueOrDefault("Murloc Warleader")))
                                paladinMurloc++;
                            else
                            {
                                paladinOther++;
                            }
                        }
                    }
                    catch (ArgumentException)
                    {
                        invalidDecks++;
                    }
                }
            }
            //nullify vars after use to save some memory
            /*
            dbfIDs = null;
            reader = null;
            deck = null;
            cardDBFIDs = null;
            cardsAsCards = null;
            */

            Console.WriteLine(toPrint + "\n\nMeta Analysis Done. Unrecognized Decks Above. Press ENTER to continue to overview.");
            toPrint = "";
            Console.ReadLine();

            Console.Clear();
            toPrint += "Invalid Decks: " + invalidDecks + "\n";
            //Druid data
            toPrint += "\nDruid Archetypes: " +
                "\n Token Druids: \t\t" + druidToken +
                "\n Malygos Druids: \t" + druidMalygos +
                "\n Togwaggle Druids: \t" + druidTogwaggle +
                "\n Taunt Druids: \t\t" + druidTaunt +
                "\n Mecha'thun Druids: \t" + druidMechathun +
                "\n Spiteful Druids: \t" + druidSpiteful +
                "\n Quest Druids: \t\t" + druidQuest +
                "\n Other Druids: \t\t" + druidOther + "\n";
            //Rogue data
            toPrint += "\n\nRogue Archetypes: " +
                "\n Odd Rogues: \t\t" + rogueOdd +
                "\n Quest Rogue: \t\t" + rogueQuest +
                "\n Kingsbane Rogues: \t" + rogueKingsbane +
                "\n Thief Rogues: \t\t" + rogueThief +
                "\n Malygos Rogue: \t" + rogueMaly +
                "\n Deathrattle Rogues: \t" + rogueDeathrattle +
                "\n Aggro Rogues: \t\t" + rogueAggro +
                "\n Pogo-Hopper Rogues: \t" + roguePogo +
                "\n Other Rogues: \t\t" + rogueOther + "\n";
            //Warlock Data
            toPrint += "\n\nWarlock Archetypes: " +
                "\n Zoo Warlocks: \t\t" + warlockZoo +
                "\n Even Warlocks: \t" + warlockEven +
                "\n Control Warlocks: \t" + warlockControl +
                "\n Cube Warlocks: \t" + warlockCube +
                "\n Mecha'thun Warlocks: \t" + warlockMechathun +
                "\n Other Warlocks: \t" + warlockOther + "\n";
            //Paladin Data
            toPrint += "\n\nPaladin Archetypes: " +
                "\n Odd Paladin: \t\t" + paladinOdd +
                "\n Even Paladin: \t\t" + paladinEven +
                "\n OTK Paladin: \t\t" + paladinOTK +
                "\n Mech Paladin: \t\t" + paladinMech +
                "\n Murloc Paladin: \t" + paladinMurloc +
                "\n Other Paladin: \t" + paladinOther + "\n";
            //Warrior Data
            toPrint += "\n\nWarrior Archetypes: " +
                "\n Odd Warriors: \t\t" + warriorOdd +
                "\n Odd Quest Warriors: \t" + warriorOddQuest +
                "\n Quest Warriors: \t" + warriorQuest +
                "\n Mecha'thun Warriors: \t" + warriorMechathun +
                "\n Other Warriors: \t" + warriorOther + "\n";
            //Mage data
            toPrint += "\n\nMage Archetypes: " +
                "\nTempo Mages: \t" + mageTempo +
                "\nBig Spell Mages: \t\t" + mageBigSpell +
                "\nExodia Mages: \t\t" + mageExodia +
                "\nHand Mages: \t\t" + mageHand +
                "\nOther Mages: \t\t" + mageOther + "\n";
            //Priest Data
            toPrint += "\n\nPriest Archetypes: " +
                "\nControl Priests: \t" + priestControl +
                "\nRez Priests: \t\t" + priestRez +
                "\nDS Priests: \t\t" + priestDivineSpirit +
                "\nMecha'thun Priests: \t" + priestMechathun +
                "\nMecha'thun Quest Priests: \t" + priestMechathunQuest +
                "\nOther Priests: \t" + priestOther + "\n";
            //Shaman Data
            toPrint += "\n\nShaman Archetypes:" +
                "\nEven Shamans: \t\t" + shamanEven +
                "\nMidrange Shamans: \t" + shamanMidrange +
                "\nShudderwok Shaman \t\t" + shamanShudderwok +
                "\nOther Shamans" + shamanOther + "\n";
            toPrint += "\nHunter Archetypes: " +
                "\nDeathrattle Hunters: \t" + hunterDeathrattle +
                "\nSecret Hunters: \t\t" + hunterSecret +
                "\nSpell Hunters: \t\t" + hunterSpell +
                "\nOther Hunters: \t\t" + hunterOther + "\n";
            Console.WriteLine(toPrint);
            toPrint = "";
            Console.ReadLine();
        }

        private static void PreviewOpponent()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// returns deck codes from a Google Sheet (one cell at a time;
    /// we're just assuming everything is a deck code without checking because I have exclusive control of the sheet.)
    /// </summary>
    class DeckstringReader : IEnumerable
    {
        private static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private static readonly string ApplicationName = "TESPA HS Meta";
        UserCredential credential;
        private readonly int NUM_SHEETS = 3; //number of sheets in the target document (number of weeks of the tournament)

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
            string sheetID = "1-0fhWItkpb_bG5y0ZHnao-EuFYa7tbFqKM6MOts1IT8";

            for (int sheet = 1; sheet <= NUM_SHEETS; sheet++)
            {
                //query Google Sheets for the deckstrings
                string range = "Sheet" + sheet + "!C:F";
                SpreadsheetsResource.ValuesResource.GetRequest request =
                        service.Spreadsheets.Values.Get(sheetID, range);
                ValueRange response = request.Execute();
                IList<IList<Object>> deckStrings = response.Values;

                if (deckStrings != null && deckStrings.Count > 0)
                {
                    foreach (var row in deckStrings)
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

            //nullify some vars to maybe save memory while reading unrecognized deck codes
            service = null;
            credential = null;
        }
    }

}
