using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoDemoLauncher.Model
{
    /// <summary>
    /// A "CostumeV5" section from a demo file
    /// </summary>
    class DemoCostume : DemoSection
    {
        /// <summary>
        /// hReferencedCostume, pSubstituteCostume, pStoredCostume, or pcDestructibleObjectCostume
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Name of the costume
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Human readable message
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = this.Name;
            if (this.Type.Equals("hReferencedCostume")) result += " (stock costume reference)";
            if (this.Type.Equals("pSubstituteCostume")) result += " (overridden stock costume)";
            if (this.Type.Equals("pStoredCostume")) result += " (player costume)";
            if (this.Type.Equals("pcDestructibleObjectCostume")) result = "(destructable object)";
            return result;
        }
    }
}
