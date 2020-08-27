using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultProbeGenerator.SERVICES
{
    public class ProbeGenerator
    {
        // CONSTRUCTOR
        public ProbeGenerator()
        {
            // ...
        }

        // METHODS
        public void GenerateCPPResultProbes(List<NXOpen.CAE.SimSolution> solutions)
        {
            Logger.Write(Environment.NewLine +
                "--- GENERATE CPP RESULT PROBES ---");

            foreach (NXOpen.CAE.SimSolution solution in solutions)
            {
                Logger.Write("   SOLUTION:  " + solution.Name.ToUpper());
            }
        }
    }
}
