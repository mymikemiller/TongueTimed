//using Google.GData.Spreadsheets;
//using Google.GData.Documents;
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Logging;
using Google.Apis.Services;
using Google.Apis.Upload;

using Google.GData.Client;
using Google.GData.Spreadsheets;

namespace TTLib
{
    public class Translator_Google
    {
        private DriveService mDriveService;
        private SpreadsheetsService mSpreadsheetsService;

        private const string cFolderName = "TongueTimed";

        public Translator()
        {
        }

        public async Task Authenticate()
        {
            Task<DriveService> authenticateTask = GoogleAuthHelper.AuthenticateDrive();
            mDriveService = await authenticateTask;
        }

        public void Translate(string title, string text, bool overwrite = false)
        {
            List<string> words = SplitUnique(text);
            PostToGoogleSpreadsheet(title, words);
        }

        private File GetOrCreateFolder()
        {
            FilesResource.ListRequest request = mDriveService.Files.List();
            request.Q = "mimeType='application/vnd.google-apps.folder' and trashed=false and title='TongueTimed' and 'root' in parents";
            FileList files = request.Execute();

            File file;
            if (files.Items.Count == 0)
            {
                // Create the folder

                File body = new File();
                body.Title = cFolderName;
                body.Description = "document description";
                body.MimeType = "application/vnd.google-apps.folder";

                // service is an authorized Drive API service instance
                file = mDriveService.Files.Insert(body).Execute();// .Fetch();
            }
            else
            {
                // Choose the first one that matches
                file = files.Items[0];
            }

            return file;
        }

        private File GetOrCreateSpreadsheet(File folder, string name)
        {
            FilesResource.ListRequest request = mDriveService.Files.List();
            request.Q = "mimeType='application/vnd.google-apps.spreadsheet' and trashed=false and title='" + name + "' and '" + folder.Id + "' in parents"; //"mimeType='application/vnd.google-apps.folder' and trashed=false and title='TongueTimed' and 'root' in parents";
            FileList files = request.Execute();

            File file;
            if (files.Items.Count == 0)
            {
                // Create the file

                //
                //File body = new File();
                //body.Title = name;
                //body.Description = "Translation for " + name;
                //body.MimeType = "imeType='application/vnd.google-apps.spreadsheet";
                

                File body = new File();
                body.Title = name;
                body.Description = "Test";
                body.MimeType = "text/csv";
                ParentReference parentFolder = new ParentReference();
                parentFolder.Id = folder.Id;
                body.Parents = new List<ParentReference>{ parentFolder };

                byte[] byteArray = System.IO.File.ReadAllBytes("test.csv");
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);


                FilesResource.InsertMediaUpload insertReq = mDriveService.Files.Insert(body, stream, "application/vnd.google-apps.spreadsheet");
                insertReq.Convert = true;
                insertReq.Upload();

                file = insertReq.ResponseBody;
                

                // service is an authorized Drive API service instance
                //file = mService.Files.Insert(body).Execute();// .Fetch();
            }
            else
            {
                // Choose the first one that matches
                file = files.Items[0];
            }

            return file;
        }

        public static SpreadsheetEntry GetSpreadsheet(File folder, string name)
        {
            SpreadsheetsService service = new SpreadsheetsService("MySpreadsheetIntegration-v1");

            // TODO: Authorize the service object for a specific user (see other sections)

            // Instantiate a SpreadsheetQuery object to retrieve spreadsheets.
            SpreadsheetQuery query = new SpreadsheetQuery();

            // Make a request to the API and get all spreadsheets.
            SpreadsheetFeed feed = service.Query(query);

            if (feed.Entries.Count == 0)
            {
                // TODO: There were no spreadsheets, act accordingly.
            }

            // TODO: Choose a spreadsheet more intelligently based on your
            // app's needs.
            SpreadsheetEntry spreadsheet = (SpreadsheetEntry)feed.Entries[0];
            Console.WriteLine(spreadsheet.Title.Text);

            // Get the first worksheet of the first spreadsheet.
            // TODO: Choose a worksheet more intelligently based on your
            // app's needs.
            WorksheetFeed wsFeed = spreadsheet.Worksheets;
            WorksheetEntry worksheet = (WorksheetEntry)wsFeed.Entries[0];

            // Define the URL to request the list feed of the worksheet.
            AtomLink listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);

            // Fetch the list feed of the worksheet.
            ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
            ListFeed listFeed = service.Query(listQuery);

            // TODO: Choose a row more intelligently based on your app's needs.
            ListEntry row = (ListEntry)listFeed.Entries[0];

            // Delete the row using the API.
            row.Delete();
        }

        public static List<string> SplitUnique(string text)
        {
            // Remove punctuation except for single quotes (to allow contractions)
            string textSansPunc = text.Where(c => !char.IsPunctuation(c) || c == '\'').Aggregate("", (current, c) => current + c);
            string[] split = textSansPunc.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            var unique = new HashSet<string>(split);
            return unique.ToList();
        }

        public void PostToGoogleSpreadsheet(String title, List<string> words)
        {
            File folder = GetOrCreateFolder();
            File spreadsheet = GetOrCreateSpreadsheet(folder, title);


            //SpreadsheetsService service = GoogleAuthHelper.AuthenticateSpreadsheet(); // = new SpreadsheetsService("MySpreadsheetIntegration-v1");

            // TODO: Authorize the service object for a specific user (see other sections)

            
            // Instantiate a SpreadsheetQuery object to retrieve spreadsheets.
            SpreadsheetQuery query = new SpreadsheetQuery();

            // Make a request to the API and get all spreadsheets.
            SpreadsheetFeed feed = service.Query(query);

            if (feed.Entries.Count == 0)
            {
            // TODO: There were no spreadsheets, act accordingly.
            }

            // TODO: Choose a spreadsheet more intelligently based on your
            // app's needs.
            SpreadsheetEntry spreadsheet = (SpreadsheetEntry)feed.Entries[0];
            Console.WriteLine(spreadsheet.Title.Text);

            // Get the first worksheet of the first spreadsheet.
            // TODO: Choose a worksheet more intelligently based on your
            // app's needs.
            WorksheetFeed wsFeed = spreadsheet.Worksheets;
            WorksheetEntry worksheet = (WorksheetEntry)wsFeed.Entries[0];

            // Fetch the cell feed of the worksheet.
            CellQuery cellQuery = new CellQuery(worksheet.CellFeedLink);
            
            CellFeed cellFeed = service.Query(cellQuery); 

            // Iterate through each cell, updating its value if necessary.
            foreach (CellEntry cell in cellFeed.Entries)
            {
                if (cell.Title.Text == "A1")
                {
                    cell.InputValue = "200";
                    cell.Update();
                }
                else if (cell.Title.Text == "B1")
                {
                    cell.InputValue = "=SUM(A1, 200)";
                    cell.Update();
                }
            }
        }
    }
}*/