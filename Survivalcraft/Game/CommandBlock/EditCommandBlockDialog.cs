using System;
using System.Collections.Generic;
using System.Xml.Linq;
namespace Game
{
    public class EditCommandBlockDialog : Dialog, AutoCompleteStream.IAutoCompleteReciver
    {
        TextBoxWidget textBox;
        ListPanelWidget list;

        ButtonWidget okButton;
        ButtonWidget cancelButton;

        CommandData commandData;
        string command;

        bool ignoreTextChange;

        SubsystemCommandEngine engine;

        public EditCommandBlockDialog(CommandData data)
        {
            XElement node = ContentManager.Get<XElement>("Dialogs/EditCommandBlockDialog");
            WidgetsManager.LoadWidgetContents(this, this, node);
            textBox = Children.Find<TextBoxWidget>("EditCommandBlockDialog.TextBox");
            list = Children.Find<ListPanelWidget>("EditCommandBlockDialog.List");
            okButton = Children.Find<ButtonWidget>("EditCommandBlockDIalog.OkButton");
            cancelButton = Children.Find<ButtonWidget>("EditCommandBlockDIalog.CancelButton");

            commandData = data;
            command = data.Command;
            textBox.Text = command;
            textBox.TextChanged += TextChanged;

            list.ItemClicked += ListSelected;

            engine = GameManager.Project.FindSubsystem<SubsystemCommandEngine>(true);
        }

        public override void Update()
        {
            if (okButton.IsClicked)
            {
                commandData.Command = textBox.Text;
                DialogsManager.HideDialog(this);
            }
            else if (cancelButton.IsClicked || Input.Cancel)
            {
                DialogsManager.HideDialog(this);
            }
        }

        void TextChanged(TextBoxWidget widget)
        {
            command = widget.Text;
            engine.AutoCompleteCommand(command, this);
        }

        void ListSelected(object obj)
        {
            if (selecting)
            {
                selecting = false;
                var label = (LabelWidget)obj;
                int i = command.LastIndexOf(' ') + 1;
                if (i != command.Length)
                    command.Remove(i);
                command += selectingValues(label.Text);
                textBox.Text = command;
            }
        }

        void ShowList(IEnumerable<string> strs)
        {
            var children = list.Items;
            int index = 0;
            foreach (string s in strs)
            {
                if (index < children.Count)
                {
                    ((LabelWidget)children[index]).Text = s;
                }
                else
                {
                    list.AddItem(new LabelWidget
                    {
                        Text = s,
                        HorizontalAlignment = WidgetAlignment.Center,
                        VerticalAlignment = WidgetAlignment.Center
                    });
                }
                index++;
            }

            int count = children.Count;
            if (index < count)
            {
                for (int i = index; i < count; i++)
                {
                    list.RemoveItemAt(i);
                }
            }
        }

        void ShowList(params string[] str)
        {
            ShowList(str);
        }

        public void CompleteEnum(string str, IEnumerable<string> enums)
        {
            var l = new List<string>();
            foreach (string s in enums)
            {
                if (s.Contains(str))
                    l.Add(s);
            }
            ShowList(l);
        }

        public void Error(string msg)
        {
            ShowList(msg);
        }

        public void ProvideAny(Type t)
        {
            ShowList(string.Format("enter a {0}", t.Name));
        }

        public void ProvideBool()
        {
            ShowList("enter a bool");
        }

        public void ProvideEnum(IEnumerable<string> enums)
        {
            selecting = true;
            selectingValues = (s) => s;
            ShowList(enums);
        }

        public void ProvideEnumDiscription(Dictionary<string, string> discriptionToName)
        {
            selecting = true;
            selectingValues = (s) => discriptionToName[s];
            ShowList(discriptionToName.Keys);
        }

        public void ProvideFloat()
        {
            ShowList("enter a float");
        }

        public void ProvideInt()
        {
            ShowList("enter a int");
        }

        public void ProvideString()
        {
            ShowList("enter a string");
        }

        bool selecting;
        Func<string, string> selectingValues;
    }
}
