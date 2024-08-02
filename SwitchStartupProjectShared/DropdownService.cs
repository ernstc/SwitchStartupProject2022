using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell;

using Task = System.Threading.Tasks.Task;

namespace SwitchStartupProject
{
    public class DropdownService
    {
        private readonly OleMenuCommand dropdownCommand;
        private static readonly IDropdownEntry sentinel = new OtherDropdownEntry("");
        private static readonly IDropdownEntry configure = new OtherDropdownEntry("Configure...");
        private IList<IDropdownEntry> dropdownList;
        private IDropdownEntry currentDropdownValue = sentinel;

        public DropdownService(IMenuCommandService mcs)
        {
            // A DropDownCombo does not let the user type into the combo box; they can only pick from the list.
            // The string value of the element selected is returned.
            // A DropDownCombo box requires two commands:

            // One command is used to ask for the current value of the combo box and to set the new value when the user makes a choice in the combo box.
            var dropodownCommandId = new CommandID(GuidList.guidSwitchStartupProjectCmdSet, (int)PkgCmdIDList.cmdidSwitchStartupProjectCombo);
            dropdownCommand = new OleMenuCommand(_HandleDropdownCommand, dropodownCommandId);
            dropdownCommand.ParametersDescription = "$"; // accept any argument string
            dropdownCommand.Enabled = false;
            mcs.AddCommand(dropdownCommand);

            // The second command is used to retrieve the list of choices for the combo box.
            var dropdownListCommandId = new CommandID(GuidList.guidSwitchStartupProjectCmdSet, (int)PkgCmdIDList.cmdidSwitchStartupProjectComboGetList);
            var dropdownListCommand = new OleMenuCommand(_HandleDropdownListCommand, dropdownListCommandId);
            mcs.AddCommand(dropdownListCommand);

            DropdownList = null;
            dropdownCommand.Enabled = true;
        }

        public static IDropdownEntry OtherItem { get; } = new OtherDropdownEntry("<other>");

        public static IDropdownEntry SeparatorItem { get; } = new OtherDropdownEntry(" ");

        public IDropdownEntry CurrentDropdownValue
        {
            get { return currentDropdownValue != sentinel ? currentDropdownValue : null; }
            set { currentDropdownValue = value ?? sentinel; }
        }

        public IList<IDropdownEntry> DropdownList
        {
            get { return dropdownList; }
            set
            {
                dropdownList = value ?? new List<IDropdownEntry>();
                if (DropdownList.Contains(SeparatorItem))
                {
                    dropdownList.Add(SeparatorItem);
                }
                dropdownList.Add(configure);
            }
        }

        public bool DropdownEnabled
        {
            get { return dropdownCommand.Enabled; }
            set { dropdownCommand.Enabled = value; }
        }

        public Action OnConfigurationSelected { get; set; }

        public Func<IDropdownEntry, Task> OnListItemSelectedAsync { get; set; }

        public void Reset()
        {
            dropdownList = new List<IDropdownEntry>();
            CurrentDropdownValue = null;
            dropdownCommand.Enabled = false;
        }


        private void _HandleDropdownCommand(object sender, EventArgs e)
        {
            if ((null == e) || (e == EventArgs.Empty))
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentException("EventArgs are required"));
            }
            var eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs == null)
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentException("EventArgs are required"));
            }

            var newChoice = eventArgs.InValue as int?;
            var currentValueHolder = eventArgs.OutValue;

            if (currentValueHolder == IntPtr.Zero && newChoice == null)
            {
                throw (new ArgumentException("Both in and out parameters should not be null"));
            }
            if (currentValueHolder != IntPtr.Zero && newChoice != null)
            {
                throw (new ArgumentException("Both in and out parameters should not be specified"));
            }

            if (currentValueHolder != IntPtr.Zero)
            {
                // when currentValueHolder is non-NULL, the IDE is requesting the current value for the combo
                Marshal.GetNativeVariantForObject(currentDropdownValue.DisplayName, currentValueHolder);
            }
            else if (newChoice != null)
            {
                // when newChoice is non-NULL, the IDE is sending the new value that has been selected in the combo
                ThreadHelper.JoinableTaskFactory.Run(async () =>
                {
                    await _ChooseNewDropdownValueAsync(newChoice.Value);
                });
            }
        }

        private void _HandleDropdownListCommand(object sender, EventArgs e)
        {
            if (e == EventArgs.Empty)
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentException("EventArgs are required"));
            }
            var eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs == null)
            {
                // We should never get here; EventArgs are required.
                throw (new ArgumentException("EventArgs are required"));
            }

            var inParam = eventArgs.InValue;
            var listHolder = eventArgs.OutValue;

            if (inParam != null)
            {
                throw (new ArgumentException("In parameter may not be specified"));
            }
            if (listHolder == IntPtr.Zero)
            {
                throw (new ArgumentException("Out parameter can not be NULL"));
            }

            // the second command is used to retrieve the full list of choices as an array of strings
            Marshal.GetNativeVariantForObject(dropdownList.Select(item => item.DisplayName).ToArray(), listHolder);
        }

        private async Task _ChooseNewDropdownValueAsync(int index)
        {
            var item = dropdownList[index];

            // new value was selected
            // see if it is the configuration item
            if (item == SeparatorItem)
            {
                return;
            }

            // see if it is the configuration item
            if (item == configure)
            {
                if (OnConfigurationSelected != null) OnConfigurationSelected();
                return;
            }

            // see if it is the sentinel
            if (item == sentinel)   // TODO: this cannot happen because sentinel is not in list!
            {
                if (OnListItemSelectedAsync != null) await OnListItemSelectedAsync(null);
                return;
            }

            if (OnListItemSelectedAsync != null) await OnListItemSelectedAsync(item);
        }

    }
}
