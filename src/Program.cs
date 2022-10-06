using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Timers;

class ReadProArenaData
{
     private static System.Timers.Timer aTimer = new System.Timers.Timer(1000);
     private static string outputPath = "";
     private static int interval = 1000;
     private static string dataFilePath = "";
    static void Main(string [] args)
    {
        // 아규먼트 길이 체크
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: pro-arena-data-extracter <interval millisecond> <data file path> <output path>  ");
            return;
        }

        interval = Int32.Parse(args[0]);
        dataFilePath = args[1];
        outputPath = args[2];


        SetTimer();

        Console.ReadLine();
        aTimer.Stop();
        aTimer.Dispose();
    }

    private static void SetTimer()
   {
        // Create a timer with a two second interval.
        aTimer = new System.Timers.Timer(interval);
        // Hook up the Elapsed event for the timer. 
        aTimer.Elapsed += OnTimedEvent;
        aTimer.AutoReset = true;
        aTimer.Enabled = true;
    }

    private static void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        Console.WriteLine("Start, load data file");
        readPlayersData();
        Console.WriteLine("End, load data file");
    }

    static void readPlayersData()
    {
        using(var db = new LiteDB.LiteDatabase(dataFilePath))
        {
            // Get a collection (or create, if doesn't exist)
            var col = db.GetCollection<SerializedStored>("player");
            
            // Use LINQ to query documents (filter, sort, transform)
            var results = col.FindAll().OrderByDescending(x => x._id);
            
            System.IO.Directory.CreateDirectory(outputPath);
            string outputFileName = String.Format("{0}.txt", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff"));

            string pathString = System.IO.Path.Combine(outputPath, outputFileName);
            
            using StreamWriter file = new(pathString);

            

            // print results
            foreach(var x in results)
            {
                //print x 
                string s = String.Format("{0}", x.SerializedStoredItem);

                PlayerInfo playerInfo = JsonConvert.DeserializeObject<PlayerInfo>(s);

                JObject o = JObject.Parse(s);
                //Console.WriteLine(s);
                string line = String.Format("Id:{6}, Player Name: {7}, Wins:{0}, Loses:{1}, Kills:{2}, Deaths:{3}, Shots:{4}, TotalGames:{5}", 
                    playerInfo.statistics.Wins, playerInfo.statistics.Loses, playerInfo.statistics.Kills, playerInfo.statistics.Deaths, playerInfo.statistics.Shots, playerInfo.statistics.TotalGames
                    , playerInfo.Id,  playerInfo.profile.FirstName);
                Console.WriteLine(line);
                file.WriteLineAsync(line);
            }
        }
    }



    // 아직은 사용 안함 
    static void readAreanaData()
    {
        using(var db = new LiteDB.LiteDatabase("./data/arena.data"))
        {
            // Get a collection (or create, if doesn't exist)
            var col = db.GetCollection<PlayerInfo>("game");
            
            // Use LINQ to query documents (filter, sort, transform)
            // find all limti 1
            var results = col.FindAll().Take(1);
            
            // print results
            foreach(var x in results)
            {
                //print x 
                string s = String.Format("!!!!!{0}!!!!!", x.SerializedStoredItem);
                Console.WriteLine(s);
                Console.WriteLine(" ");
                break;
            }
        }
    }
}
