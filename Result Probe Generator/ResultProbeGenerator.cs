using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;
using ResultProbeGenerator.SERVICES;

namespace ResultProbeGenerator
{
    public class ResultProbeGenerator
    {
        //class members
        private static Session theSession = null;
        private static UI theUI = null;
        private string theDlxFileName;
        private NXOpen.BlockStyler.BlockDialog theDialog;
        private NXOpen.BlockStyler.Group group0;// Block type: Group
        private NXOpen.BlockStyler.ListBox LB_Solutions;// Block type: List Box
        private NXOpen.BlockStyler.Button button_SelectAll;// Block type: Button
        private NXOpen.BlockStyler.Separator separator0;// Block type: Separator
        private NXOpen.BlockStyler.Button BTN_Generate;// Block type: Button

        // Custom members
        private static NXOpen.CAE.SimPart mySIM = null;
        private static List<NXOpen.CAE.SimSolution> mySolutions = null;
        private static List<NXOpen.CAE.SimSolution> mySelectedSolutions = new List<NXOpen.CAE.SimSolution>();
        private static bool allSelected = false;

        //------------------------------------------------------------------------------
        //Constructor for NX Styler class
        //------------------------------------------------------------------------------
        public ResultProbeGenerator()
        {
            try
            {
                theSession = Session.GetSession();
                theUI = UI.GetUI();
                // Set path to GUI .dlx file
                if (Environment.GetEnvironmentVariable("UCCREATOR_ENV") == "SIEMENS")
                {
                    theDlxFileName = "ResultProbeGenerator.dlx";
                }
                else
                {
                    //theDlxFileName = @"D:\NX\CAE\UBC\ABC\ResultProbeGenerator\ResultProbeGenerator.dlx";  // IN CPP TC environment as Production tool

                    string PLMHOST = Environment.GetEnvironmentVariable("PLMHOST");
                    string UGII_VERSION = Environment.GetEnvironmentVariable("UGII_VERSION");

                    if (PLMHOST == "")
                    {
                        theUI.NXMessageBox.Show("PLMHOST NOT FOUND", NXMessageBox.DialogType.Error, "Could not find PLMHOST environment variable!");
                    }
                    if (UGII_VERSION == "")
                    {
                        theUI.NXMessageBox.Show("UGII_VERSION NOT FOUND", NXMessageBox.DialogType.Error, "Could not find UGII_VERSION environment variable!");
                    }

                    theDlxFileName = PLMHOST + @"\plmshare\config\nxcustom\NX-" + UGII_VERSION + @"\site\application\ResultProbeGenerator.dlx";
                }
                theDialog = theUI.CreateDialog(theDlxFileName);
                theDialog.AddUpdateHandler(new NXOpen.BlockStyler.BlockDialog.Update(update_cb));
                theDialog.AddInitializeHandler(new NXOpen.BlockStyler.BlockDialog.Initialize(initialize_cb));
                theDialog.AddDialogShownHandler(new NXOpen.BlockStyler.BlockDialog.DialogShown(dialogShown_cb));
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                throw ex;
            }
        }
        //------------------------------- DIALOG LAUNCHING ---------------------------------
        //
        //    Before invoking this application one needs to open any part/empty part in NX
        //    because of the behavior of the blocks.
        //
        //    Make sure the dlx file is in one of the following locations:
        //        1.) From where NX session is launched
        //        2.) $UGII_USER_DIR/application
        //        3.) For released applications, using UGII_CUSTOM_DIRECTORY_FILE is highly
        //            recommended. This variable is set to a full directory path to a file 
        //            containing a list of root directories for all custom applications.
        //            e.g., UGII_CUSTOM_DIRECTORY_FILE=$UGII_BASE_DIR\ugii\menus\custom_dirs.dat
        //
        //    You can create the dialog using one of the following way:
        //
        //    1. Journal Replay
        //
        //        1) Replay this file through Tool->Journal->Play Menu.
        //
        //    2. USER EXIT
        //
        //        1) Create the Shared Library -- Refer "Block UI Styler programmer's guide"
        //        2) Invoke the Shared Library through File->Execute->NX Open menu.
        //
        //------------------------------------------------------------------------------
        public static void Main()
        {
            ResultProbeGenerator theResultProbeGenerator = null;
            try
            {
                theResultProbeGenerator = new ResultProbeGenerator();
                // The following method shows the dialog immediately
                theResultProbeGenerator.Show();
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            finally
            {
                if (theResultProbeGenerator != null)
                    theResultProbeGenerator.Dispose();
                theResultProbeGenerator = null;
            }
        }
        //------------------------------------------------------------------------------
        // This method specifies how a shared image is unloaded from memory
        // within NX. This method gives you the capability to unload an
        // internal NX Open application or user  exit from NX. Specify any
        // one of the three constants as a return value to determine the type
        // of unload to perform:
        //
        //
        //    Immediately : unload the library as soon as the automation program has completed
        //    Explicitly  : unload the library from the "Unload Shared Image" dialog
        //    AtTermination : unload the library when the NX session terminates
        //
        //
        // NOTE:  A program which associates NX Open applications with the menubar
        // MUST NOT use this option since it will UNLOAD your NX Open application image
        // from the menubar.
        //------------------------------------------------------------------------------
        public static int GetUnloadOption(string arg)
        {
            //return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);
            return System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);
            // return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
        }

        //------------------------------------------------------------------------------
        // Following method cleanup any housekeeping chores that may be needed.
        // This method is automatically called by NX.
        //------------------------------------------------------------------------------
        public static void UnloadLibrary(string arg)
        {
            try
            {
                //---- Enter your code here -----
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
        }

        //------------------------------------------------------------------------------
        //This method shows the dialog on the screen
        //------------------------------------------------------------------------------
        public NXOpen.UIStyler.DialogResponse Show()
        {
            try
            {
                //CHECK WORKING OBJECT
                Logger.Write("--- WORKING OBJECT CHECK ---");

                NXOpen.NXObject workObj = theSession.Parts.BaseWork;
                if (workObj.GetType().ToString() != "NXOpen.CAE.SimPart")
                {
                    Logger.Write(workObj.GetType().ToString() + "  --> EXPECTED A SIM OBJECT TO BE THE WORKING OBJECT:  ABORT");
                    Logger.Show();
                }
                else
                {
                    // SHOW GUI
                    theDialog.Show();
                }
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return 0;
        }

        //------------------------------------------------------------------------------
        //Method Name: Dispose
        //------------------------------------------------------------------------------
        public void Dispose()
        {
            if (theDialog != null)
            {
                theDialog.Dispose();
                theDialog = null;
            }
        }

        //------------------------------------------------------------------------------
        //---------------------Block UI Styler Callback Functions--------------------------
        //------------------------------------------------------------------------------

        //------------------------------------------------------------------------------
        //Callback Name: initialize_cb
        //------------------------------------------------------------------------------
        public void initialize_cb()
        {
            try
            {
                group0 = (NXOpen.BlockStyler.Group)theDialog.TopBlock.FindBlock("group0");
                LB_Solutions = (NXOpen.BlockStyler.ListBox)theDialog.TopBlock.FindBlock("LB_Solutions");
                button_SelectAll = (NXOpen.BlockStyler.Button)theDialog.TopBlock.FindBlock("button_SelectAll");
                separator0 = (NXOpen.BlockStyler.Separator)theDialog.TopBlock.FindBlock("separator0");
                BTN_Generate = (NXOpen.BlockStyler.Button)theDialog.TopBlock.FindBlock("BTN_Generate");
                //------------------------------------------------------------------------------
                //Registration of ListBox specific callbacks
                //------------------------------------------------------------------------------
                //LB_Solutions.SetAddHandler(new NXOpen.BlockStyler.ListBox.AddCallback(AddCallback));

                //LB_Solutions.SetDeleteHandler(new NXOpen.BlockStyler.ListBox.DeleteCallback(DeleteCallback));

                //------------------------------------------------------------------------------
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
        }

        //------------------------------------------------------------------------------
        //Callback Name: dialogShown_cb
        //This callback is executed just before the dialog launch. Thus any value set 
        //here will take precedence and dialog will be launched showing that value. 
        //------------------------------------------------------------------------------
        public void dialogShown_cb()
        {
            try
            {
                Logger.Write(Environment.NewLine +
                    "--- PREPARE GUI ---");

                // GET WORKING SIM
                mySIM = (NXOpen.CAE.SimPart)theSession.Parts.BaseWork;

                Logger.Write("Working SIM :  " + mySIM.Name);

                // GET ALL SOLUTION OBJECTS
                mySolutions = mySIM.Simulation.Solutions.ToArray().ToList();

                Logger.Write("# Solutions =  " + mySolutions.Count.ToString());
                foreach (NXOpen.CAE.SimSolution solution in mySolutions)
                {
                    Logger.Write("   " + solution.Name);
                }

                // ADD SOLUTION OBJECTS TO GUI LISTBOX
                LB_Solutions.SetListItems(mySolutions.Select(x => x.Name).ToArray());

                Logger.Write("Added solutions to GUI");
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
        }

        //------------------------------------------------------------------------------
        //Callback Name: update_cb
        //------------------------------------------------------------------------------
        public int update_cb(NXOpen.BlockStyler.UIBlock block)
        {
            try
            {
                if (block == LB_Solutions)
                {
                   // ...
                }
                else if (block == button_SelectAll)
                {
                    if (!allSelected)
                    {
                        // SELECT ALL
                        LB_Solutions.SetSelectedItems(Enumerable.Range(0, mySolutions.Count).ToArray());

                        // Adjust button label
                        button_SelectAll.Label = "Deselect all";
                        allSelected = true;
                    }
                    else
                    {
                        // DESELECT ALL
                        LB_Solutions.SetSelectedItems(new List<int>().ToArray());

                        // Adjust button label
                        button_SelectAll.Label = "Select all";
                        allSelected = false;
                    }
                }
                else if (block == separator0)
                {
                    //---------Enter your code here-----------
                }
                else if (block == BTN_Generate)
                {
                    // GET SELECTED SOLUTIONS
                    mySelectedSolutions.Clear();

                    foreach (int index in LB_Solutions.GetSelectedItems())
                    {
                        mySelectedSolutions.Add(mySolutions[index]);
                    }

                    // GENERATE CPP RESULT PROBES
                    ProbeGenerator myProbeGenerator = new ProbeGenerator();
                    myProbeGenerator.GenerateCPPResultProbes(mySelectedSolutions, mySIM);

                    Logger.Show();
                }
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return 0;
        }
        //------------------------------------------------------------------------------
        //ListBox specific callbacks
        //------------------------------------------------------------------------------
        //public int  AddCallback (NXOpen.BlockStyler.ListBox list_box)
        //{
        //}

        //public int  DeleteCallback(NXOpen.BlockStyler.ListBox list_box)
        //{
        //}

        //------------------------------------------------------------------------------

        //------------------------------------------------------------------------------
        //Function Name: GetBlockProperties
        //Returns the propertylist of the specified BlockID
        //------------------------------------------------------------------------------
        public PropertyList GetBlockProperties(string blockID)
        {
            PropertyList plist = null;
            try
            {
                plist = theDialog.GetBlockProperties(blockID);
            }
            catch (Exception ex)
            {
                //---- Enter your exception handling code here -----
                theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
            }
            return plist;
        }
    }
}
