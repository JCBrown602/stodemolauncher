using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using StoDemoLauncher.Model;
using StoDemoLauncher.Helper;

namespace StoDemoLauncher.Parser
{
    public class DemoInfoParser : AbstractParser
    {
        // result
        DemoInfo result = new DemoInfo();

        // placeholders
        string zoneName = "";
        string zmInfoFilename = "";
        long startTime = -1;
        long endTime = -1;
        string character = "";
        long activePlayerRef = -1;
        long entityRef = -1;

        /// <summary>
        /// Is called before the first line is parsed.
        /// </summary>
        /// <param name="filename">Absolute path to the file being parsed</param>
        public override void StartParsing(string filename)
        {
            ConfigurationFile config = ConfigurationFile.GetInstance();

            result.FileName = filename.Substring(filename.LastIndexOf('\\') + 1).ToString();

            // Get creation and modification times from file system
            FileInfo fileInfo = new FileInfo(filename);
            result.Create = fileInfo.CreationTime;
            result.Modify = fileInfo.LastWriteTime;

            // determine server
            if (filename.Contains(config.GetStringValue(GameClient.GameClientIniGroup, GameClient.HolodeckPathIniKey))) result.Server = GameServer.HOLODECK;
            else if (filename.Contains(config.GetStringValue(GameClient.GameClientIniGroup, GameClient.TribblePathIniKey))) result.Server = GameServer.TRIBBLE;
            else if (filename.Contains(config.GetStringValue(GameClient.GameClientIniGroup, GameClient.RedshirtPathIniKey))) result.Server = GameServer.REDSHIRT;
            else result.Server = GameServer.EXTERN;
        }

        /// <summary>
        /// Is called by the parser when it has fetched a new line
        /// </summary>
        /// <param name="line">Content of the line</param>
        /// <param name="lineNumber">Current line number (0 index)</param>
        public override void NewLine(string line, int lineNumber)
        {
            // internal map name
            if (line.Contains("ZoneName"))
            {
                zoneName = line.Substring(line.LastIndexOf("/") + 1);
                zoneName = zoneName.Substring(0, zoneName.LastIndexOf(".Zone"));
            }
            // internal map name
            if (line.Contains("zmInfoFilename"))
            {
                zmInfoFilename = line.Substring(line.LastIndexOf("/") + 1);
                zmInfoFilename = zmInfoFilename.Substring(0, zmInfoFilename.LastIndexOf(".Zone"));
            }
            // start time in seconds
            if (line.Contains("StartTime"))
            {
                startTime = Convert.ToInt64(line.Substring(line.LastIndexOf("StartTime") + 10));
            }
            // end time in seconds
            if (line.Contains("EndTime"))
            {
                endTime = Convert.ToInt64(line.Substring(line.LastIndexOf("EndTime") + 8));
            }
            // parse character name only, if we are currently reading the correct entity
            if (line.Contains("savedName") && activePlayerRef == entityRef)
            {
                character = line.Substring(line.LastIndexOf("savedName") + 10);
                if (character.StartsWith("\"") && character.EndsWith("\""))
                {
                    character = character.Substring(1, character.Length - 2);
                }
            }
            // this is the entity reference of the player character
            if (line.Contains("activePlayerRef"))
            {
                activePlayerRef = Convert.ToInt64(line.Substring(line.LastIndexOf("activePlayerRef") + 16));
            }
            // keep track of the entity ref wer are currently parsing
            if (line.Contains("EntityRef"))
            {
                entityRef = Convert.ToInt64(line.Substring(line.LastIndexOf("EntityRef") + 10));
            }
        }

        /// <summary>
        /// Is called before each line is read. When you return true, parsing
        /// will stop prematurely.
        /// </summary>
        /// <returns>true if this plugin does not want to  process any more
        /// lines</returns>
        public override bool HasAllInformation()
        {
            return !((zoneName.Equals("") && zmInfoFilename.Equals("")) || startTime == -1 || endTime == -1 || character.Equals(""));
        }

        /// <summary>
        /// Is called, when all lines have been read by the parser.
        /// </summary>
        /// <param name="lineCount">Total number of lines in the demo file.</param>
        public override void StopParsing(long lineCount)
        {
            // copy info to return type
            if (!zoneName.Equals("")) result.MapName = zoneName;
            if (!zmInfoFilename.Equals("")) result.MapName = zmInfoFilename;
            result.Character = character;
            result.StartTime = startTime;
            result.EndTime = endTime;
        }

        public DemoInfo GetResult()
        {
            if(this.Complete()) return this.result;
            return null;
        }

        private bool Complete()
        {
            return (
                (!this.zoneName.Equals("") || !this.zmInfoFilename.Equals("")) &&
                this.startTime != -1 &&
                this.endTime != -1 &&
                !this.character.Equals("")
                );
        }
    }
}
