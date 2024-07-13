using System.Xml.Linq;

namespace PackageUpdater;

class Program
{
    static int Main(string[] args)
    {
        // See https://aka.ms/new-console-template for more information

        // Handle package File Watch to auto-deploy
        // Increment Build Version
        if (args.Length == 0)
        {
            DisplayHelp();
            return 1;
        }

        try
        {
            if (args[0].Equals("Increment", StringComparison.OrdinalIgnoreCase))
            {
                var projectPath = GetArgument(args, "--project", "-p") ?? throw new Exception("Missing --project (alias -p) argument.");
                var doc = XDocument.Load(projectPath);
                var versionElement = doc.Descendants("FileVersion").FirstOrDefault();

                if (versionElement != null)
                {
                    var versionParts = versionElement.Value.Split('.');
                    var newVersion = $"{versionParts[0]}.{versionParts[1]}.{versionParts[2]}.{int.Parse(versionParts[3]) + 1}";
                    Console.WriteLine($"Updating Version from {versionElement.Value} to {newVersion}.");
                    versionElement.Value = newVersion;
                    doc.Save(projectPath);
                }
                else
                {
                    Console.WriteLine("FileVersion element not found in the XML file.");
                    return 1;
                }
                return 0;

            }
            else
            {
                Console.WriteLine("Unrecognized command: " + args[0]);
                Console.WriteLine("");
                DisplayHelp();
            }
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error has occured!");
            Console.WriteLine(ex);

        }

        return 0;
    }

    private static string? GetArgument(string[] args, string fullName, string shortName)
    {
        var found = false;
        foreach (var arg in args.Skip(1))
        {
            if (found)
            {
                return arg;
            }

            found = arg.Equals(fullName, StringComparison.OrdinalIgnoreCase)
                    || arg.Equals(shortName, StringComparison.OrdinalIgnoreCase);
        }
        return null;
    }

    public static void DisplayHelp()
    {
        Console.WriteLine("Usage: PackagerUtil Increment [--project].");
        Console.WriteLine("    Increment: Increments the Revision portion of the Build Version in the project file.");
        Console.WriteLine("    --project The Project file path to increment the revision for. (alias -p)");
        Console.WriteLine("");
    }
}