using System;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Activities.Statements;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CSharp.Activities;
using NumberGuessWorkflowActivities;

namespace NumberGuessWorkflowHost
{
    public partial class WorkflowHostForm : Form
    {
        const string connectionString =
            "Server=(local);Initial Catalog=WF45GettingStartedTutorial;Integrated Security=SSPI";

        SqlWorkflowInstanceStore store;

        bool WorkflowStarting;

        public WorkflowHostForm()
        {
            InitializeComponent();
        }

        public Guid WorkflowInstanceId => InstanceId.SelectedIndex == -1 ? Guid.Empty : (Guid) InstanceId.SelectedItem;

        private void WorkflowHostForm_Load(object sender, EventArgs e)
        {
            // Initialize the store and configure it so that it can be used for  
// multiple WorkflowApplication instances.  
            store = new SqlWorkflowInstanceStore(connectionString);
            WorkflowApplication.CreateDefaultInstanceOwner(store, null, WorkflowIdentityFilter.Any);
            
// Set default ComboBox selections.  
            NumberRange.SelectedIndex = 0;
            WorkflowType.SelectedIndex = 0;

            ListPersistedWorkflows();
        }

        private void InstanceId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InstanceId.SelectedIndex == -1) {
                return;
            }

// Clear the status window.  
            WorkflowStatus.Clear();

// Get the workflow version and display it.  
// If the workflow is just starting then this info will not  
// be available in the persistence store so do not try and retrieve it.  
            if (!WorkflowStarting) {
                var instance =
                    WorkflowApplication.GetInstance(this.WorkflowInstanceId, store);
                WorkflowVersion.Text =
                    WorkflowVersionMap.GetIdentityDescription(instance.DefinitionIdentity);

                // Unload the instance.  
                instance.Abandon();
            }
        }

        private void ListPersistedWorkflows()
        {
            using (var localCon = new SqlConnection(connectionString)) {
                var localCmd =
                    "Select [InstanceId] from [System.Activities.DurableInstancing].[Instances] Order By [CreationTime]";
                var cmd = localCon.CreateCommand();
                cmd.CommandText = localCmd;
                localCon.Open();
                using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection)) {
                    while (reader.Read()) {
                        // Get the InstanceId of the persisted Workflow  
                        var id = Guid.Parse(reader[0].ToString());
                        InstanceId.Items.Add(id);
                    }
                }
            }
        }

        private delegate void UpdateStatusDelegate(string msg);

        public void UpdateStatus(string msg)
        {
            // We may be on a different thread so we need to  
            // make this call using BeginInvoke.  
            if (InvokeRequired) {
                BeginInvoke(new UpdateStatusDelegate(UpdateStatus), msg);
            }
            else {
                if (!msg.EndsWith("\r\n")) {
                    msg += "\r\n";
                }

                WorkflowStatus.AppendText(msg);

                WorkflowStatus.SelectionStart = WorkflowStatus.Text.Length;
                WorkflowStatus.ScrollToCaret();
            }
        }

        private delegate void GameOverDelegate();

        private void GameOver()
        {
            if (InvokeRequired) {
                BeginInvoke(new GameOverDelegate(GameOver));
            }
            else {
                // Remove this instance from the combo box  
                InstanceId.Items.Remove(InstanceId.SelectedItem);
                InstanceId.SelectedIndex = -1;
            }
        }

        private void ConfigureWorkflowApplication(WorkflowApplication wfApp)
        {
            // Configure the persistence store.  
            wfApp.InstanceStore = store;
            // Add a StringWriter to the extensions. This captures the output  
            // from the WriteLine activities so we can display it in the form.  
            var sw = new StringWriter();
            wfApp.Extensions.Add(sw);
            wfApp.Completed = delegate(WorkflowApplicationCompletedEventArgs e){
                switch (e.CompletionState) {
                    case ActivityInstanceState.Faulted:
                        UpdateStatus(
                            $"Workflow Terminated. Exception: {e.TerminationException.GetType().FullName}\r\n{e.TerminationException.Message}");
                        break;
                    case ActivityInstanceState.Canceled:
                        UpdateStatus("Workflow Canceled.");
                        break;
                    default: {
                        var Turns = Convert.ToInt32(e.Outputs["Turns"]);
                        UpdateStatus($"Congratulations, you guessed the number in {Turns} turns.");
                        break;
                    }
                }

                GameOver();
            };
            wfApp.Aborted = delegate(WorkflowApplicationAbortedEventArgs e){
                UpdateStatus($"Workflow Aborted. Exception: {e.Reason.GetType().FullName}\r\n{e.Reason.Message}");
            };

            wfApp.OnUnhandledException = delegate(WorkflowApplicationUnhandledExceptionEventArgs e){
                UpdateStatus(
                    $"Unhandled Exception: {e.UnhandledException.GetType().FullName}\r\n{e.UnhandledException.Message}");
                GameOver();
                return UnhandledExceptionAction.Terminate;
            };

            wfApp.PersistableIdle = delegate(WorkflowApplicationIdleEventArgs e){
                // Send the current WriteLine outputs to the status window.  
                var writers = e.GetInstanceExtensions<StringWriter>();
                foreach (var writer in writers) {
                    UpdateStatus(writer.ToString());
                }

                return PersistableIdleAction.Unload;
            };
        }

        private void NewGame_Click(object sender, EventArgs e)
        {
            var inputs = new Dictionary<string, object> {{"MaxNumber", Convert.ToInt32(NumberRange.SelectedItem)}};
            WorkflowIdentity identity = null;
            switch (WorkflowType.SelectedItem.ToString()) {
                case "SequentialNumberGuessWorkflow":
                    identity = WorkflowVersionMap.SequentialNumberGuessIdentity;
                    break;

                case "StateMachineNumberGuessWorkflow":
                    identity = WorkflowVersionMap.StateMachineNumberGuessIdentity;
                    break;

                case "FlowchartNumberGuessWorkflow":
                    identity = WorkflowVersionMap.FlowchartNumberGuessIdentity;
                    break;
            }
            var wf = WorkflowVersionMap.GetWorkflowDefinition(identity);
            var wfApp = new WorkflowApplication(wf, inputs, identity);
            // Add the workflow to the list and display the version information.  
            WorkflowStarting = true;
            var wfAppId = wfApp.Id;
            InstanceId.SelectedIndex = InstanceId.Items.Add(wfAppId);
            WorkflowVersion.Text = identity.ToString();
            WorkflowStarting = false;
            // Configure the instance store, extensions, and   
// workflow lifecycle handlers.  
            ConfigureWorkflowApplication(wfApp);
            // Start the workflow.  
            wfApp.Run();

            
            //test(wfAppId);

            //wfApp.Unload();
        }

        void test(Guid id)
        {
            var instance =
                WorkflowApplication.GetInstance(id, store);

            // Use the persisted WorkflowIdentity to retrieve the correct workflow  
            // definition from the dictionary.  

            var wf =
                WorkflowVersionMap.GetWorkflowDefinition(instance.DefinitionIdentity);

            // Associate the WorkflowApplication with the correct definition  
            var wfApp =
                new WorkflowApplication(wf, instance.DefinitionIdentity);

            // Configure the extensions and lifecycle handlers.  
            // Do this before the instance is loaded. Once the instance is  
            // loaded it is too late to add extensions.  
            ConfigureWorkflowApplication(wfApp);

            // Load the workflow.  
            wfApp.Load(instance);
            var sequence = new Sequence();
            
            // Resume the workflow.  
            wfApp.ResumeBookmark("EnterGuess", 100);

           
        }
        
        private void EnterGuess_Click(object sender, EventArgs e)
        {
            if (WorkflowInstanceId == Guid.Empty) {
                MessageBox.Show("Please select a workflow.");
                return;
            }

            int guess;
            if (!Int32.TryParse(Guess.Text, out guess)) {
                MessageBox.Show("Please enter an integer.");
                Guess.SelectAll();
                Guess.Focus();
                return;
            }

            var instance =
                WorkflowApplication.GetInstance(WorkflowInstanceId, store);

            // Use the persisted WorkflowIdentity to retrieve the correct workflow  
            // definition from the dictionary.  

            var wf =
                WorkflowVersionMap.GetWorkflowDefinition(instance.DefinitionIdentity);

            // Associate the WorkflowApplication with the correct definition  
            var wfApp =
                new WorkflowApplication(wf, instance.DefinitionIdentity);

            // Configure the extensions and lifecycle handlers.  
            // Do this before the instance is loaded. Once the instance is  
            // loaded it is too late to add extensions.  
            ConfigureWorkflowApplication(wfApp);

            // Load the workflow.  
            wfApp.Load(instance);
            

            // Resume the workflow.  
            wfApp.ResumeBookmark("EnterGuess", guess);
            
            // Clear the Guess textbox.  
            Guess.Clear();
            Guess.Focus();
        }

        private void QuitGame_Click(object sender, EventArgs e)
        {
            if (WorkflowInstanceId == Guid.Empty) {
                MessageBox.Show("Please select a workflow.");
                return;
            }

            var instance =
                WorkflowApplication.GetInstance(WorkflowInstanceId, store);

// Use the persisted WorkflowIdentity to retrieve the correct workflow  
// definition from the dictionary.  
            var wf = WorkflowVersionMap.GetWorkflowDefinition(instance.DefinitionIdentity);

// Associate the WorkflowApplication with the correct definition  
            var wfApp =
                new WorkflowApplication(wf, instance.DefinitionIdentity);

// Configure the extensions and lifecycle handlers  
            ConfigureWorkflowApplication(wfApp);

// Load the workflow.  
            wfApp.Load(instance);

// Terminate the workflow.  
            wfApp.Terminate("User resigns.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (WorkflowInstanceId == Guid.Empty) {
                MessageBox.Show("Please select a workflow.");
                return;
            }          
            var instance = WorkflowApplication.GetInstance(WorkflowInstanceId, store);
            

            
        }
    }

    public static class WorkflowVersionMap
    {
        private static readonly Dictionary<WorkflowIdentity, Activity> Map;

        // Current version identities.  
        public static WorkflowIdentity StateMachineNumberGuessIdentity;

        public static WorkflowIdentity FlowchartNumberGuessIdentity;

        public static WorkflowIdentity SequentialNumberGuessIdentity;

        static WorkflowVersionMap()
        {
            Map = new Dictionary<WorkflowIdentity, Activity>();

            // Add the current workflow version identities.  
            StateMachineNumberGuessIdentity = new WorkflowIdentity {
                Name = "StateMachineNumberGuessWorkflow",
                Version = new Version(1, 0, 0, 0)
            };

            FlowchartNumberGuessIdentity = new WorkflowIdentity {
                Name = "FlowchartNumberGuessWorkflow",
                Version = new Version(1, 0, 0, 0)
            };

            SequentialNumberGuessIdentity = new WorkflowIdentity {
                Name = "SequentialNumberGuessWorkflow",
                Version = new Version(1, 0, 0, 0)
            };

            Map.Add(StateMachineNumberGuessIdentity, new StateMachineNumberGuessWorkflow());  
//            map.Add(FlowchartNumberGuessIdentity, new FlowchartNumberGuessWorkflow());
//            map.Add(SequentialNumberGuessIdentity, new SequentialNumberGuessWorkflow());  
        }

        public static Activity GetWorkflowDefinition(WorkflowIdentity identity)
        {
            return Map[identity];
        }

        public static string GetIdentityDescription(WorkflowIdentity identity)
        {
            return identity.ToString();
        }
    }
}