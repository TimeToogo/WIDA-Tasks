using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WIDA.Tasks.Triggers;
using WIDA.Tasks.Conditions;
using WIDA.Storage;

namespace WIDA
{
    public partial class definingForm : Form
    {
        public bool IsFinished = false;
        public object ReturnObj = null;
        private string DisplayLabelText;
        private bool IsCodeValid;
        private bool IsFormValid;
        private Source Source;
        private Source Form;
        private bool IsEditing = false;
        private object OriginalObj = null;
        private Source DefaultSource = new Source();
        private Source DefaultForm = new Source();
        private Criticals Criticals = null;
        private Conf.Definition Definition;

        public definingForm(string DisplayLabelText, Conf.Definition Definition, Criticals Criticals = null, object OriginalObj = null)
        {
            this.DisplayLabelText = DisplayLabelText;
            this.Criticals = Criticals;
            this.IsEditing = (OriginalObj != null);
            this.OriginalObj = OriginalObj;
            this.Definition = Definition;
            InitializeComponent();
        }

        private void definingForm_Load(object sender, EventArgs e)
        {
            displayLabel.Text = DisplayLabelText;
            DefaultForm.Files.AddRange(Conf.EmptyFormDefaultCode);
            if (Definition == Conf.Definition.Trigger)
            {
                DefaultSource.Files.Add(new File(Conf.DefaultFileName, Conf.TriggerDefaultCode, true));
            }
            else if (Definition == Conf.Definition.Condition)
            {
                DefaultSource.Files.Add(new File(Conf.DefaultFileName, Conf.ConditionDefaultCode, true));
            }
            else if (Definition == Conf.Definition.Action)
            {
                DefaultSource.Files.Add(new File(Conf.DefaultFileName, Conf.ActionDefaultCode, true));
            }
            if (IsEditing)
            {
                IsCodeValid = true;
                IsFormValid = true;
                if (Definition == Conf.Definition.Trigger)
                {
                    Trigger Trigger = (Trigger)OriginalObj;
                    nameTextBox.Text = Trigger.Name;
                    groupNameTextBox.Text = Trigger.GroupName;
                    descriptionTextBox.Text = Trigger.Description;
                    needParamsCheckBox.Checked = Trigger.NeedsParams;
                    Form = Trigger.FormSource;
                    Source = Trigger.Source;
                }
                else if (Definition == Conf.Definition.Condition)
                {
                    Condition Condition = (Condition)OriginalObj;
                    nameTextBox.Text = Condition.Name;
                    groupNameTextBox.Text = Condition.GroupName;
                    descriptionTextBox.Text = Condition.Description;
                    needParamsCheckBox.Checked = Condition.NeedsParams;
                    Form = Condition.FormSource;
                    Source = Condition.Source;
                }
                else if (Definition == Conf.Definition.Action)
                {
                    WIDA.Tasks.Actions.Action Action = (WIDA.Tasks.Actions.Action)OriginalObj;
                    nameTextBox.Text = Action.Name;
                    groupNameTextBox.Text = Action.GroupName;
                    descriptionTextBox.Text = Action.Description;
                    needParamsCheckBox.Checked = Action.NeedsParams;
                    Form = Action.FormSource;
                    Source = Action.Source;
                }
            }
        }

        private string validateInput()
        {
            string Valid = null;
            if (nameTextBox.Text.Length == 0)
                Valid = "Please enter a valid name";
            else if (groupNameTextBox.Text.Length == 0)
                Valid = "Please enter a valid group name";
            else if (descriptionTextBox.Text.Length == 0)
                Valid = "Please enter a valid description";
            else if (!IsCodeValid)
                Valid = "Please write a valid class";
            else if (!IsFormValid && needParamsCheckBox.Checked)
                Valid = "Please write a valid form";

            return Valid;
        }

        private void editCodeButton_Click(object sender, EventArgs e)
        {
            compilerForm Form = new compilerForm(DefaultSource, Criticals);
            if(this.Source != null)
                Form = new compilerForm(this.Source, Criticals);
            Form.ShowDialog();
            if (Form.IsFinished)
                this.Source = Form.Source;
            IsCodeValid = (this.Source != null);
        }

        private void needParamsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            editFormButton.Enabled = needParamsCheckBox.Checked;
        }

        private void editFormButton_Click(object sender, EventArgs e)
        {
            compilerForm Form = new compilerForm(DefaultForm, Conf.FormCriticals);
            if (this.Form != null)
                Form = new compilerForm(this.Form, Conf.FormCriticals);
            Form.ShowDialog();
            if (Form.IsFinished)
                this.Form = Form.Source;
            IsFormValid = (this.Form != null);
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            if (validateInput() != null)
            {
                MessageBox.Show(validateInput());
                return;
            }

            string Name = nameTextBox.Text;
            string GroupName = groupNameTextBox.Text;
            string Description = descriptionTextBox.Text;
            if (Definition == Conf.Definition.Trigger)
            {
                Trigger Trigger = new Trigger(Name, GroupName, Description, Source, needParamsCheckBox.Checked, Form);
                this.ReturnObj = (object)Trigger;
            }
            else if (Definition == Conf.Definition.Condition)
            {
                Condition Condition = new Condition(Name, GroupName, Description, Source, needParamsCheckBox.Checked, Form);
                this.ReturnObj = (object)Condition;
            }
            else if (Definition == Conf.Definition.Action)
            {
                WIDA.Tasks.Actions.Action Action = new Tasks.Actions.Action(Name, GroupName, Description, Source, needParamsCheckBox.Checked, Form);
                this.ReturnObj = (object)Action;
            }
            IsFinished = true;
            Close();
        }
    }
}
