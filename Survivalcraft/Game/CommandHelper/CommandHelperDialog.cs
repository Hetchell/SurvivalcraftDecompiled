using System;
using System.Collections.Generic;
using System.Xml.Linq;
namespace Game
{
    public class CommandHelperDialog : Dialog
    {
        TextBoxWidget textBoxAnimal;
        TextBoxWidget textBoxBlock;

        ButtonWidget animals;
        ButtonWidget blocks;
        ButtonWidget back;

        SubsystemCommandEngine commandEngine;

        public CommandHelperDialog(SubsystemCommandEngine engine)
        {
            commandEngine = engine;

            XElement node = ContentManager.Get<XElement>("Dialogs/CommandHelperDialog");
            WidgetsManager.LoadWidgetContents(this, this, node);
            animals = Children.Find<ButtonWidget>("CommandHelperDialog.ShowAnimals");
            blocks = Children.Find<ButtonWidget>("CommandHelperDialog.ShowBlocks");
            back = Children.Find<ButtonWidget>("CommandHelperDialog.Back");

            textBoxAnimal = Children.Find<TextBoxWidget>("CommandHelperDialog.AnimalName");
            textBoxBlock = Children.Find<TextBoxWidget>("CommandHelperDialog.BlockName");
        }

        IEnumerable<string> SearchForKeyWord(string str, IEnumerable<string> bank)
        {
            if (str == string.Empty)
            {
                return bank;
            }
            var result = new List<string>();
            foreach (string s in bank)
            {
                if (s.Contains(str))
                {
                    result.Add(s);
                }
            }
            return result;
        }

        public override void Update()
        {
            if (animals.IsChecked)
            {
                DialogsManager.ShowDialog(ParentWidget,
                                          new ListSelectionDialog("select a animal name to copy",
                                                                  SearchForKeyWord(textBoxAnimal.Text, commandEngine.creatureTemplateNames.Keys),
                                                                  64f,
                                                                  arg => arg.ToString(),
                                                                  obj =>
                                                                  {
                                                                      string result = obj.ToString();
                                                                      ClipboardManager.ClipboardString = result;
                                                                  }));
            }
            if (blocks.IsChecked)
            {
                DialogsManager.ShowDialog(ParentWidget,
                                          new ListSelectionDialog("select a block to copy id",
                                                                  SearchForKeyWord(textBoxBlock.Text, commandEngine.blockIds.Keys),
                                                                  64f,
                                                                  arg => arg.ToString(),
                                                                  obj =>
                                                                  {
                                                                      int result = commandEngine.blockIds[obj.ToString()];
                                                                      ClipboardManager.ClipboardString = result.ToString();
                                                                  }));
            }
            if (Input.Cancel || back.IsClicked)
            {
                DialogsManager.HideDialog(this);
            }
        }
    }
}
