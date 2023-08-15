// Authorization: illyum
// License: MIT
// You're welcome to use this script! If you
// find any bugs, please report this in the issues
// tab and make sure you specify this script! (RareDropFollower)

// Version: 0.1.0


using System.Text.RegularExpressions;

class RareDropFollower {
    private const string ChancePattern = @"\d+% Chance\)";
    private const string LogFormattingPattern = @"\[(\d+:\d+:\d+)\] \[.+?\]: \[CHAT\] (.+)";

    static void Main(string[] args) {
        String path = PromptLogFile();
        if (path == null) {
            throw new IOException("Log file not located!");
        }
        Console.Clear();
        Console.Out.WriteLine("RareDropFollower is now active!\n");
        Console.Out.WriteLine("    Time     |    Drop");
        Console.Out.WriteLine("----------------------------------------");
        TailLogFile(path);
    }

    private static String PromptLogFile() {
        String Question = "Please select your log file (Or Minecraft Installation): ";
        Console.WriteLine(Question);
        String[] Options = { "1) Vanilla Minecraft", "2) Lunar Client", "3) Other (Manually Enter Log File)" };
        foreach (String option in Options) {
            Console.WriteLine(option);
        }
        Console.Write("Option: ");
        int selectedOption = -1;
        int.TryParse(Console.ReadLine(), result: out selectedOption);
        if (selectedOption == -1) {
            throw new ArgumentOutOfRangeException($"Option out of range. Please select 1, 2, or 3");
        }

        String userProfileDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        switch (selectedOption) {
            case 1:  // Vanilla
                return Path.Join(userProfileDir, "AppData", "Roaming", ".minecraft", "logs", "latest.log");
            case 2:  // Lunar Client
                return Path.Join(userProfileDir, ".lunarclient", "offline", "multiver", "logs", "latest.log");
            case 3: // Other (Manually Enter Log File)
                Console.Out.WriteLine("Please Enter your latest.log file path: ");
                String CustomPath = Console.ReadLine();
                return CustomPath;
        }
        return null;
    }

    private static void TailLogFile(string filePath, double delay = 0.1) {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (StreamReader reader = new StreamReader(fileStream)) {
            reader.BaseStream.Seek(0, SeekOrigin.End);
            while (true) {
                string line = reader.ReadLine();
                if (line == null) {
                    Thread.Sleep((int)(delay * 1000));
                    continue;
                }
                OnLogEmit(line.Trim());
            }
        }
    }

    private static string FormatLogLine(string logline) {
        Match match = Regex.Match(logline, LogFormattingPattern);
        if (match.Success) {
            string time = match.Groups[1].Value;
            string message = match.Groups[2].Value;
            return $"[ {time} ] : {message}";
        }

        return logline;
    }

    private static void OnLogEmit(String logline) {
        String substring = FormatLogLine(logline);
        if (Regex.IsMatch(substring, ChancePattern)) {
            Console.WriteLine(substring);
        }
    }
}