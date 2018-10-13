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

namespace TespaMeta
{
    class Analyzer
    {
        static void Main(string[] args)
        {
            //these are the cards that are used to identify archetypes.
            Dictionary<string, int> dbfIDs = new Dictionary<string, int>();
            dbfIDs.Add("Wispering Woods", 47063);
            dbfIDs.Add("Baku the Mooneater", 48158);
            dbfIDs.Add("Malygos", 436);
            dbfIDs.Add("Druid Quest", 41099);
            dbfIDs.Add("King Togwaggle", 46589);
            dbfIDs.Add("Azalina Soulthief", 46874);
            dbfIDs.Add("Hadronox", 43439);
            dbfIDs.Add("Mecha'thun", 48625);
            dbfIDs.Add("Spiteful Summoner", 46551);

            int decksScanned=0, invalidDecks=0;
            int warlockZoo = 0, warlockEven = 0, warlockControl = 0, warlockCube = 0,
                druidToken = 0, druidMalygos = 0, druidTogwaggle = 0, druidQuest = 0, druidTaunt = 0, druidMechathun = 0, druidSpiteful = 0, druidOther = 0,
                paladinOdd=0;

            DeckstringReader reader = new DeckstringReader();

            HearthDb.Deckstrings.Deck deck;
            Dictionary<int, int> cardDBFIDs;
            Dictionary<HearthDb.Card, int> cardsAsCards;
            foreach (string deckCode in reader)
            {
                if(deckCode.Length > 1)
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
                    }
                    catch (ArgumentException)
                    {
                        invalidDecks++;
                    }
                    //Console.WriteLine("Class: " + deck.GetHero().Class + "\n");
                    //foreach (HearthDb.Card card in cards.Keys)
                    //{
                    //    if (card.DbfId == dbfIDs.GetValueOrDefault("Wispering Woods"))
                    //        druidToken++;
                    //}
                }
            }

            Console.WriteLine("Meta Analysis Done. Unrecognized Decks Above. Press ENTER to continue to overview.");
            Console.ReadLine();

            Console.Clear();
            Console.WriteLine("Invalid Decks: " + invalidDecks);
            Console.WriteLine("\nDruid Archetypes: " + 
                "\n Token Druids: \t\t" + druidToken +
                "\n Malygos Druids: \t" + druidMalygos +
                "\n Togwaggle Druids: \t" + druidTogwaggle +
                "\n Taunt Druids: \t\t" + druidTaunt +
                "\n Mecha'thun Druids: \t" + druidMechathun + 
                "\n Spiteful Druids: \t" + druidSpiteful + 
                "\n Quest Druids: \t\t" + druidQuest +
                "\n Other Druids: \t\t" + druidOther);
            Console.ReadLine();
        }
    }

    /// <summary>
    /// reads a text file copy-paste of TESPA's google sheet to find deck codes (they all start with ([)AAECA)
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

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
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
                    // Print columns A and E, which correspond to indices 0 and 4.
                    //Console.WriteLine("{0}, {1}", row[0], row[4]);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
        }
    }

}