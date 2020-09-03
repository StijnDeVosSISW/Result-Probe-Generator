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
        // -------

        /// <summary>
        /// Generates target CPP Result Probe in each Solution, which does not have this Result Probe yet
        /// </summary>
        /// <param name="solutions"></param>
        public void GenerateCPPResultProbes(List<NXOpen.CAE.SimSolution> solutions)
        {
            Logger.Write(Environment.NewLine +
                "--- GENERATE CPP RESULT PROBES ---");

            foreach (NXOpen.CAE.SimSolution solution in solutions)
            {
                Logger.Write("   SOLUTION:  " + solution.Name.ToUpper());

                // CHECK IF RESULT PROBE EXISTS ALREADY
                NXOpen.CAE.ResultProbe[] existingResultProbes;
                solution.GetAllResultProbes(out existingResultProbes);

                if (existingResultProbes.Any(x => x.Name == "True Bolt Shear"))
                {
                    Logger.Write("      Already contain CPP Result Probe --> SKIPPED");
                    continue;
                }

                // ...

            }
        }
    }
}
