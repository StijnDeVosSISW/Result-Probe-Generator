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
        public void GenerateCPPResultProbes(List<NXOpen.CAE.SimSolution> solutions, NXOpen.CAE.SimPart mySIM)
        {
            Logger.Write(Environment.NewLine +
                "--- GENERATE CPP RESULT PROBES ---");

            foreach (NXOpen.CAE.SimSolution solution in solutions)
            {
                Logger.Write("   SOLUTION:  " + solution.Name.ToUpper());

                try
                {
                    // CHECK IF RESULT PROBE EXISTS ALREADY
                    NXOpen.CAE.ResultProbe[] existingResultProbes;
                    solution.GetAllResultProbes(out existingResultProbes);

                    if (existingResultProbes.Any(x => x.Name.Contains("True Bolt Shear")))
                    {
                        Logger.Write("      Already contains CPP Result Probe --> SKIPPED");
                        continue;
                    }

                    // MAKE SOLUTION ACTIVE
                    mySIM.Simulation.ActiveSolution = solution;
                    Logger.Write("      Made active solution");

                    // GENERATE CPP RESULT PROBE
                    // Initialize
                    NXOpen.CAE.ResultProbe _resultProbe = null;
                    NXOpen.CAE.ResultProbeBuilder resultProbeBuilder = solution.CreateResultProbeBuilder(_resultProbe);

                    // Name
                    resultProbeBuilder.ProbeName = "True Bolt Shear";
                    // Formula
                    resultProbeBuilder.Formula = "sqrt(bxy^2+bxz^2)";
                    // Iteration Definition
                    resultProbeBuilder.Loadcase = NXOpen.CAE.ResultProbeBuilder.LoadcaseSelection.All;
                    resultProbeBuilder.IterationTypeOption = NXOpen.CAE.ResultProbeBuilder.IterationType.All;
                    resultProbeBuilder.Iteration = NXOpen.CAE.ResultProbeBuilder.IterationSelection.First;
                    resultProbeBuilder.CombineAcrossIteration = false;
                    // Selection and Averaging
                    resultProbeBuilder.ModelSelectionType = NXOpen.CAE.ResultProbeBuilder.SelectionType.EntireModel;
                    resultProbeBuilder.CombineAcross = false;
                    // Output
                    resultProbeBuilder.ErrorHndl = NXOpen.CAE.ResultProbeBuilder.ErrorHandling.Fillzero;
                    resultProbeBuilder.ResultType = NXOpen.CAE.Result.Quantity.AppliedForce;
                    resultProbeBuilder.Unit = (NXOpen.Unit)mySIM.UnitCollection.FindObject("Newton");
                    resultProbeBuilder.ResultReferenceType = NXOpen.CAE.SimResultReference.Type.Structural;

                    // Create target Result Probe
                    resultProbeBuilder.Commit();

                    // Clean up
                    resultProbeBuilder.Destroy();

                    Logger.Write("      CPP Result Probe generated!");
                }
                catch (Exception e)
                {
                    Logger.Write(Environment.NewLine + 
                        "!ERROR occurred: " + Environment.NewLine + 
                        e.ToString() + Environment.NewLine);
                }
            }
        }
    }
}
