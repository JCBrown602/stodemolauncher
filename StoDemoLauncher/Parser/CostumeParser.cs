using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StoDemoLauncher.Model;

namespace StoDemoLauncher.Parser
{
    /// <summary>
    /// Fetches all 
    /// </summary>
    class CostumeParser : AbstractSectionsParser
    {
        /// <summary>
        /// The list of parsed resources
        /// </summary>
        List<DemoSection> result = new List<DemoSection>();

        // placeholders
        int braceLevel = 0;
        int lastCostumeV5BraceLevel = 0;
        int lastCostumeV5SectionStart = 0;
        bool inCostumeV5Section = false;
        string costumeType = "";
        string costumeName = "";

        /// <summary>
        /// Is called by the parser when it has fetched a new line
        /// </summary>
        /// <param name="line">Content of the line</param>
        /// <param name="lineNumber">Current line number (0 index)</param>
        public override void NewLine(string line, int lineNumber)
        {
            if (line.Trim().StartsWith("CostumeV5"))
            {
                lastCostumeV5SectionStart = lineNumber;
                lastCostumeV5BraceLevel = braceLevel;
                inCostumeV5Section = true;
            }
            if (inCostumeV5Section && costumeType.Equals("") && line.Trim().StartsWith("hReferencedCostume"))
            {
                this.costumeType = "hReferencedCostume";
                this.costumeName = line.Trim().Substring(19);
            }
            if (inCostumeV5Section && costumeType.Equals("") && line.Trim().StartsWith("pSubstituteCostume"))
            {
                this.costumeType = "pSubstituteCostume";
                this.costumeName = line.Trim().Substring(19);
            }
            if (inCostumeV5Section && costumeType.Equals("") && line.Trim().StartsWith("pStoredCostume"))
            {
                this.costumeType = "pStoredCostume";
                this.costumeName = line.Trim().Substring(15);
            }
            if (inCostumeV5Section && costumeType.Equals("") && line.Trim().StartsWith("pcDestructibleObjectCostume"))
            {
                this.costumeType = "pcDestructibleObjectCostume";
                this.costumeName = line.Trim().Substring(15);
            }
            if (inCostumeV5Section && line.Trim().StartsWith("}") && braceLevel - 1 == lastCostumeV5BraceLevel)
            {
                inCostumeV5Section = false;
                DemoCostume resource = new DemoCostume();
                resource.StartLine = lastCostumeV5SectionStart;
                resource.EndLine = lineNumber;
                resource.Name = this.costumeName;
                resource.Type = this.costumeType;
                result.Add(resource);
                costumeName = "";
                costumeType = "";
            }
            if (line.EndsWith("{")) braceLevel++;
            if (line.EndsWith("}")) braceLevel--;
        }

        /// <summary>
        /// Returns the list with the found resources.
        /// </summary>
        /// <returns></returns>
        override public List<DemoSection> GetResult()
        {
            return result;
        }

    }
}
