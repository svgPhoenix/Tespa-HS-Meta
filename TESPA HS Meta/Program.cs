using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace TespaMeta
{
    class Analyzer
    {
        static readonly int NUM_SHEETS = 7;

        static void Main(string[] args)
        {
            Console.Clear();
            PrintOptions();
        }

        /// <summary>
        /// Print a basic menu and take different actions depending on input
        /// </summary>
        static void PrintOptions()
        {
            Console.Out.WriteLine(
                            "Type the number that corresponds with the menu item:\n" +
                            "1: Deserialize some deck codes\n" +
                            "2: Analyze the meta of the entire tournament\n" +
                            "3: Analyze a specific opponent's lineup over all previous weeks");
            char input = Convert.ToChar(Console.Read());
            Console.ReadLine();
            switch (input)
            {
                case '1':
                    //loop until the user doesn't want to check any more decks
                    while (DeserializeSingleDeck()) ;
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

        /// <summary>
        /// This was built to help me find the DbfId's of individual cards to help with "archetype detection"
        /// </summary>
        /// <returns>whether the user wants to deserialize another deck (calling function loops while(DeserializeSingleDeck);)</returns>
        private static bool DeserializeSingleDeck()
        {
            HearthDb.Deckstrings.Deck deck;
            Dictionary<HearthDb.Card, int> cards;
            string deckstring;
            string toPrint;

            Console.WriteLine("Paste a deckstring and press Enter.");
            deckstring = Console.ReadLine();
            //deserialize the deck and store it in 'deck' and 'cards' representations for later manipulation
            deck = HearthDb.Deckstrings.DeckSerializer.Deserialize(deckstring);
            cards = deck.GetCards();
            //some basic header stuff for the output
            toPrint = "#Deck Format: " + deck.Format;
            toPrint += "\n#Format Year: " + deck.Format;
            //add each card and its ID to the print queue
            foreach (HearthDb.Card card in cards.Keys)
            {
                toPrint += "\n" + card.Name + ": " + card.DbfId;
            }
            Console.WriteLine(toPrint);
            Console.ReadLine();

            //maybe the user wanted to check more decks
            string doContinue = "";
            while (doContinue != null)
            {
                Console.WriteLine("Would you like to analyze another deck? (Y/N)\t\t");
                doContinue = Console.ReadLine();
                switch (doContinue.ToLower())
                {
                    case "Y":
                    case "y":
                        return true;
                    case "F":
                    case "f":
                        return false;
                    default:
                        Console.WriteLine("Please input either 'y' or 'f'");
                        doContinue = null;
                        break;
                }
            }
            //NoT aLl CoDe PaThS rEtuRn A vAlUe
            return false;
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

            //TODO decksScanned is currently assigned but unused. this is for implementing percentages rather than raw numbers when I feel like it
            int decksScanned = 0, invalidDecks = 0,
                //counter vars for all the different deck archetypes tracked
                warlockZoo = 0, warlockEven = 0, warlockControl = 0, warlockCube = 0, warlockMechathun = 0, warlockOther = 0,
                druidToken = 0, druidMalygos = 0, druidTogwaggle = 0, druidQuest = 0, druidTaunt = 0, druidMechathun = 0, druidSpiteful = 0, druidOther = 0,
                paladinOdd = 0, paladinEven = 0, paladinOther = 0, paladinOTK = 0, paladinMech = 0, paladinMurloc = 0,
                rogueOdd = 0, rogueOther = 0, rogueQuest = 0, rogueMaly = 0, rogueKingsbane = 0, rogueDeathrattle = 0, rogueThief = 0, roguePogo = 0, rogueAggro = 0,
                warriorOdd = 0, warriorOddQuest = 0, warriorQuest = 0, warriorMechathun = 0, warriorOther = 0,
                mageTempo = 0, mageHand = 0, mageBigSpell = 0, mageExodia = 0, mageOther = 0,
                priestRez = 0, priestControl = 0, priestDivineSpirit = 0, priestMechathun = 0, priestMechathunQuest = 0, priestOther = 0,
                shamanEven = 0, shamanOther = 0, shamanMidrange = 0, shamanShudderwok = 0,
                hunterDeathrattle = 0, hunterSecret = 0, hunterSpell = 0, hunterOther = 0;

            HearthDb.Deckstrings.Deck deck;
            Dictionary<int, int> cardDBFIDs;
            string toPrint = "";

            //loop through each sheet of my google doc (week of the tournament)
            for (int i = 1; i <= NUM_SHEETS; i++)
            {
                GoogleSheetReader reader = new GoogleSheetReader("Sheet" + i + "!C:F");
                foreach (string deckCode in reader)
                {
                    //ignore empty cells from no-show / disqualified teams
                    if (deckCode.Length > 1)
                    {
                        decksScanned++;
                        try
                        {
                            /*
                             * I highly advise minimizing all these if/else statements. 
                             * I literally check for one card and then assume that the deck is one of a few archetypes
                             * It is ugly and I hate it and you should too but I don't have access to HearthSim's archetype analysis
                             * If you have a better solution, please fork and implement ;)
                             */
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
                        //I don't know why some decks cause errors. Bug in HearthDB??
                        catch (ArgumentException)
                        {
                            invalidDecks++;
                        }
                    }
                }
            }
            //nullify vars after use to save some memory(?)
            dbfIDs = null;
            deck = null;
            cardDBFIDs = null;

            //I wish I could collapse the following massive print
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
                "\nMecha Quest Priests: \t" + priestMechathunQuest +
                "\nOther Priests: \t" + priestOther + "\n";
            //Shaman Data
            toPrint += "\n\nShaman Archetypes:" +
                "\nEven Shamans: \t\t" + shamanEven +
                "\nMidrange Shamans: \t" + shamanMidrange +
                "\nShudderwok Shamans: \t" + shamanShudderwok +
                "\nOther Shamans: \t\t" + shamanOther + "\n";
            toPrint += "\nHunter Archetypes: " +
                "\nDeathrattle Hunters: \t" + hunterDeathrattle +
                "\nSecret Hunters: \t\t" + hunterSecret +
                "\nSpell Hunters: \t\t" + hunterSpell +
                "\nOther Hunters: \t\t" + hunterOther + "\n";
            Console.WriteLine(toPrint);
            toPrint = "";
            Console.ReadLine();
        }

        /// <summary>
        /// Take a team name and college name from the user and find all decks brought by any matching teams in the database
        /// </summary>
        private static void PreviewOpponent()
        {
            /* We need team name & college in case one of the two isn't unique. 
             * Could also use the team ID field from TESPA but I didn't import those to my sheet when I started,
             *     and haven't felt like adding them.
             */
            Console.Write("Enter the team name of your opponent: ");
            string teamName = Console.ReadLine();
            teamName = teamName.Trim();
            Console.WriteLine("Entered name: " + teamName);

            Console.Write("Enter the name of your opponents college: ");
            string collegeName = Console.ReadLine();
            collegeName = collegeName.Trim();
            Console.WriteLine("Entered College name: " + collegeName);

            //save time by searching for the opponent and saving their deck codes first
            //then deserializing the decks later
            string[] deckCodes = new string[4 * NUM_SHEETS];
            int deckNum = 0;
            //search for the opponent and then add all their decks to the array
            for (int i = 1; i <= NUM_SHEETS; i++)
            {
                //loop through each cell in the column of team names
                GoogleSheetReader nameReader = new GoogleSheetReader("Sheet" + i + "!A1:A");
                int row = 0;
                foreach (string team in nameReader)
                {
                    row++;
                    if (team == teamName)
                    {
                        //check that the team of matching name is the team from the correct college
                        //'foreach' should only execute once since we only ask the Sheets API for a single cell
                        GoogleSheetReader possibleMatch = new GoogleSheetReader("Sheet" + i + "!B" + row + ":B" + row);
                        foreach (string college in possibleMatch)
                        {
                            if (college == collegeName)
                            {
                                GoogleSheetReader opponentDecksForWeekI = new GoogleSheetReader("Sheet" + i + "!C" + row + ":F" + row);
                                foreach (string deckCode in opponentDecksForWeekI)
                                {
                                    deckCodes[deckNum++] = deckCode;
                                }
                            }
                        }
                    }
                }
            }

            //deserialize all the decks we just found
            HearthDb.Deckstrings.Deck deck;
            int index = -1; //this is for delimiting between weeks so that we don't dump 28 decks on our user at once
            string toPrint = ""; //this is faster than lots of Console.WriteLine() calls
            foreach (string code in deckCodes)
            {
                //teams bring 4 decks per week. delimiting the output by week keeps the user sane.
                if (++index % 4 == 0)
                {
                    toPrint += "\n----Week Number: " + index / 4 + " done. press Enter to continue with next week.";
                    Console.WriteLine(toPrint);
                    toPrint = "";
                    Console.ReadLine();
                }
                try
                {
                    deck = HearthDb.Deckstrings.DeckSerializer.Deserialize(code);
                    toPrint += HearthDb.Deckstrings.DeckSerializer.Serialize(deck, true) + "\n";
                }
                //return the raw code in case we can't read it. this helps the user take it elsewhere for a more reliable deserialize without finding it manually
                catch (ArgumentException)
                {
                    toPrint += "\n---------------------\n" +
                        "Deserializer couldn't read: " + code +
                        "\n---------------------\n";
                }
            }
            toPrint += "All weeks done.\n";
            Console.WriteLine(toPrint); //I think this might be unnecessary based off memory but I don't feel like testing that theory rn
            Console.ReadLine();
        }
    }

    /// <summary>
    /// enumerates through individual cells of a given range of a Google Sheet
    /// This code is very close to the example boilerplate from Google's own API docs. 
    /// I only modified it to take cell range as an argument. all else is theirs.
    /// </summary>
    internal class GoogleSheetReader : IEnumerable
    {
        public GoogleSheetReader(string cellRange)
        {
            this.cellRange = cellRange;
        }

        private readonly string cellRange;
        private static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private static readonly string ApplicationName = "TESPA HS Meta";
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
                //Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            string sheetID = "1-0fhWItkpb_bG5y0ZHnao-EuFYa7tbFqKM6MOts1IT8";

            //query Google Sheets for the deckstrings
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(sheetID, cellRange);
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

            //nullify some vars to maybe save memory while reading unrecognized deck codes
            service = null;
            credential = null;
        }
    }

}
