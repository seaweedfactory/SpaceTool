using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Configuration;
using SpaceTools.Tools.Downloader;
using SpaceTools.Tools.Crawler;
using SpaceTools.Tools.ListDownloader;
using SpaceTools.Tools.LocationCrawl;
using System.Reflection;

namespace SpaceTool
{
    /// <summary>
    /// Command line interface to SpaceTools library.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                #region Tool modes
                String toolProfileDownload = "pd";
                String toolProfileCrawl = "pc";
                String toolListProfileDownload = "lpd";
                String toolLocationCrawl = "lc";
                #endregion

                #region Call timings and limits
                int minimumDelayBetweenPages = 200;
                int minimumDelayBetweenAPICalls = 250;
                int maximumCrawlDepth = 2;
                #endregion

                CommandLineArgument toolArg = new CommandLineArgument("t", "tool");
                CommandLineArgument userNameArg = new CommandLineArgument("u", "username");
                CommandLineArgument hashKeyArg = new CommandLineArgument("h", "hashkey");
                CommandLineArgument storeDirectoryArg = new CommandLineArgument("s", "store_directory");
                CommandLineArgument capturePhotosArg = new CommandLineArgument("c", "capture_photos");
                CommandLineArgument captureConnectionsArg = new CommandLineArgument("n", "capture_connections");
                CommandLineArgument delayBetweenPagesArg = new CommandLineArgument("dbp", "delay_between_pages");
                CommandLineArgument delayBetweenAPICallsArg = new CommandLineArgument("dba", "delay_between_api_calls");
                CommandLineArgument crawlDepthArg = new CommandLineArgument("d", "crawl_depth");
                CommandLineArgument usernameListPathArg = new CommandLineArgument("l", "username_list_path");
                CommandLineArgument locationCriteriaArg = new CommandLineArgument("o", "location_criteria");
                CommandLineArgument allowEmptyLocationsArg = new CommandLineArgument("e", "allow_empty_locations");

                List<CommandLineArgument> argumentList = new List<CommandLineArgument>()
                {
                    toolArg,
                    userNameArg,
                    hashKeyArg,
                    storeDirectoryArg,
                    capturePhotosArg,
                    captureConnectionsArg,
                    delayBetweenPagesArg,
                    delayBetweenAPICallsArg,
                    crawlDepthArg,
                    usernameListPathArg,
                    locationCriteriaArg,
                    allowEmptyLocationsArg
                };

                #region Print argument help if no arguments.
                if (args.Length == 0)
                {
                    ArgumentHelp();
                }
                #endregion

                #region Set default arguments

                //Default to profile download tool
                toolArg.Value = toolProfileDownload;

                //Blank hashkey           
                hashKeyArg.Value = "";

                //Get store directory from app file if available, otherwise use current working directory.
                String storeDirectoryApp = ConfigurationManager.AppSettings["store_directory"];
                storeDirectoryArg.Value = !String.IsNullOrEmpty(storeDirectoryApp) ? storeDirectoryApp
                    : Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                capturePhotosArg.Value = false.ToString();

                captureConnectionsArg.Value = false.ToString();

                //Keep delays to calls to minimum
                delayBetweenPagesArg.Value = minimumDelayBetweenPages.ToString();
                delayBetweenAPICallsArg.Value = minimumDelayBetweenAPICalls.ToString();

                crawlDepthArg.Value = 2.ToString(); //Keep default crawl depth to minimum

                //Default to no location criteria
                locationCriteriaArg.Value = null;

                //Default to allow empty locations
                allowEmptyLocationsArg.Value = true.ToString();

                #endregion

                //Parse arugments
                if (args.Length > 0)
                {
                    int argumentIndex = 0;
                    while (argumentIndex < args.Length)
                    {
                        CommandLineArgument matchedArguments = argumentList
                            .Where(x => x.ShortName.Equals(args[argumentIndex]) || x.LongName.Equals(args[argumentIndex]))
                            .FirstOrDefault();
                        if (matchedArguments != null)
                        {
                            if (argumentIndex < args.Length - 1)
                            {
                                //Found an argument, get the value and skip to next position
                                matchedArguments.Value = args[argumentIndex + 1];
                                argumentIndex = argumentIndex + 2;
                            }
                            else
                            {
                                //Argument with no value
                                Console.WriteLine(String.Format("Argument with no value: {0}", args[argumentIndex]));
                                Environment.Exit(1);
                            }
                        }
                        else
                        {
                            //Invalid argument, ignore for now
                            Console.WriteLine(String.Format("Invalid argument: {0}", args[argumentIndex]));
                            if (argumentIndex < args.Length)
                            {
                                argumentIndex++;
                            }
                        }
                    }
                }

                #region Process defaults that may be in the .app file.
                if (String.IsNullOrEmpty(usernameListPathArg.Value))
                {
                    //Default to list.txt in store directory for list path
                    usernameListPathArg.Value = Path.Combine(storeDirectoryArg.Value, "list.txt");
                }

                if (String.IsNullOrEmpty(hashKeyArg.Value))
                {
                    //Get hashkey from app file if available.
                    hashKeyArg.Value = ConfigurationManager.AppSettings["hashkey"];
                }
                #endregion

                //Verify arguments and launch tools
                if (String.IsNullOrEmpty(toolArg.Value) || toolArg.Value.Equals(toolProfileDownload)) //Default to profile download
                {
                    #region Profile Download
                    if (String.IsNullOrEmpty(userNameArg.Value))
                    {
                        ArgumentError(userNameArg);
                        return;
                    }

                    ProfileDownloader pd = new ProfileDownloader(
                        userNameArg.Value,
                        storeDirectoryArg.Value,
                        hashKeyArg.Value,
                        capturePhotosArg.AsBool != null ? (bool)capturePhotosArg.AsBool : false,
                        captureConnectionsArg.AsBool != null ? (bool)captureConnectionsArg.AsBool : false);

                    pd.Download();
                    #endregion
                }
                else if (toolArg.Value.Equals(toolProfileCrawl))
                {
                    #region Profile Crawl
                    if (String.IsNullOrEmpty(userNameArg.Value))
                    {
                        ArgumentError(userNameArg);
                        return;
                    }

                    ProfileCrawler pc = new ProfileCrawler(
                        storeDirectoryArg.Value,
                        hashKeyArg.Value,
                        userNameArg.Value,
                        delayBetweenPagesArg.AsInt != null ? Math.Max((int)delayBetweenPagesArg.AsInt, minimumDelayBetweenPages) : minimumDelayBetweenPages,
                        delayBetweenAPICallsArg.AsInt != null ? Math.Max((int)delayBetweenAPICallsArg.AsInt, minimumDelayBetweenAPICalls) : minimumDelayBetweenAPICalls,
                        crawlDepthArg.AsInt != null ? Math.Min((int)crawlDepthArg.AsInt, maximumCrawlDepth) : 2,
                        locationCriteriaArg.Value,
                        allowEmptyLocationsArg.AsBool != null ? (bool)allowEmptyLocationsArg.AsBool : true,
                        capturePhotosArg.AsBool != null ? (bool)capturePhotosArg.AsBool : false
                        );

                    pc.Crawl();
                    #endregion
                }
                else if (toolArg.Value.Equals(toolListProfileDownload))
                {
                    #region Download Profile List
                    if (String.IsNullOrEmpty(usernameListPathArg.Value))
                    {
                        ArgumentError(usernameListPathArg);
                        return;
                    }

                    ListDownloader ld = new ListDownloader(
                        usernameListPathArg.Value,
                        storeDirectoryArg.Value,
                        hashKeyArg.Value,
                        capturePhotosArg.AsBool != null ? (bool)capturePhotosArg.AsBool : false,
                        captureConnectionsArg.AsBool != null ? (bool)captureConnectionsArg.AsBool : false
                        );

                    ld.Download();
                    #endregion
                }
                else if (toolArg.Value.Equals(toolLocationCrawl))
                {
                    #region Crawl for locations in connections from directory of existing profiles
                    LocationCrawler lc = new LocationCrawler(
                        storeDirectoryArg.Value,
                        hashKeyArg.Value
                        );

                    lc.Crawl();
                    #endregion
                }
                else
                {
                    //Not a recognized tool
                    ArgumentError(toolArg);
                    return;
                }

                Environment.Exit(0);
            }
            catch (Exception appExc)
            {
                Console.WriteLine(String.Format("Error: {0}{1}{2}", appExc.Message, Environment.NewLine, appExc.StackTrace));
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Print argument help page.
        /// </summary>
        private static void ArgumentHelp()
        {
            Console.WriteLine("SpaceTool: Download profile information to JSON files.");
            Console.WriteLine("Arguments (-argument parameter):");
            Console.WriteLine(String.Format("-{0}, -{1}:\n{2}\n", "t", "tool", "[pd|lpd|lc|pc] Tool to use, defaults to profile download."));
            Console.WriteLine("Tools:");
            Console.WriteLine(String.Format("{0}\t {1}", "pd", "Profile downloader, downloads a single profile."));
            Console.WriteLine(String.Format("{0}\t {1}", "lpd", "List profile downloader, downloads a list of profiles."));
            Console.WriteLine(String.Format("{0}\t {1}", "lc", "Extracts location info from connections in profiles in directory."));
            Console.WriteLine(String.Format("{0}\t {1}", "pc", "Profile crawler, downloads a profile and then crawls its connections."));
            Console.WriteLine();
            Console.WriteLine(String.Format("-{0}, -{1}:\n{2}\n", "u", "username", "Profile name to download."));
            Console.WriteLine(String.Format("-{0}, -{1}:\n{2}\n", "h", "hashkey", "Hashkey for API calls."));
            Console.WriteLine(String.Format("-{0}, -{1}:\n{2}\n", "s", "store_directory", "Directory to store information to."));
            Console.WriteLine(String.Format("-{0}, -{1}:\n{2}\n", "c", "capture_photos", "[true|false] to capture photos."));
            Console.WriteLine(String.Format("-{0}, -{1}:\n{2}\n", "n", "capture_connections", "[true|false] to parse connections."));
            Console.WriteLine(String.Format("-{0}, -{1}:\n{2}\n", "dbp", "delay_between_pages", "ms between page requests."));
            Console.WriteLine(String.Format("-{0}, -{1}:\n{2}\n", "dba", "delay_between_api_calls", "ms between API calls."));
            Console.WriteLine(String.Format("-{0}, -{1}:\n{2}\n", "d", "crawl_depth", "Depth to crawl. Add 1 to process leaves."));
            Console.WriteLine(String.Format("-{0}, -{1}:\n{2}\n", "l", "username_list_path", "Path to list file for list downloader."));
            Console.WriteLine(String.Format("-{0}, -{1}:\n{2}\n", "o", "location_criteria", "Profile location must contain this string to be downloaded."));
            Console.WriteLine(String.Format("-{0}, -{1}:\n{2}\n", "e", "allow_empty_locations", "[true|false] to process profiles with no location information."));
            Console.WriteLine();
            Console.WriteLine("hashkey and store_directory can also be configured in the App.config file.");
            Console.WriteLine();
            Console.WriteLine("Get hashkey:");
            Console.WriteLine("1.Open profile page.");
            Console.WriteLine("2.Inspect root element.");
            Console.WriteLine("3.Go to network tab.");
            Console.WriteLine("4.Find POST call.");
            Console.WriteLine("5.Inspect header on call.");
            Console.WriteLine("6.Click Edit and Resend.");
            Console.WriteLine("7.Copy value of Hash parameter.");
            Console.WriteLine();
        }

        /// <summary>
        /// Display a message when there is an argument error.
        /// </summary>
        private static void ArgumentError(CommandLineArgument argument)
        {
            Console.WriteLine(String.Format("Argument error: -{0}, -{1} {2}", argument.ShortName, argument.LongName, argument.Value));
            ArgumentHelp();
            Environment.Exit(1);
        }
    }
}
