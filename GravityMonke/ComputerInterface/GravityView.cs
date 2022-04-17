using System;
using Utilla;
using ComputerInterface;
using ComputerInterface.ViewLib;

namespace GravityMonke.ComputerInterface
{
    class GravityView : ComputerView
    {
        public static GravityView instance;
        private readonly UISelectionHandler selectionHandler;
        const string highlightColour = "336BFF";
        public float gravity = Plugin.gravity;
        public bool enabled = Plugin.Allowed;

        public GravityView()
        {
            instance = this;

            selectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down, EKeyboardKey.Enter);

            selectionHandler.MaxIdx = 1;

            selectionHandler.OnSelected += OnEntrySelected;

            selectionHandler.ConfigureSelectionIndicator($"<color=#{highlightColour}>></color> ", "", "  ", "");
        }

        public override void OnShow(object[] args)
        {
            base.OnShow(args);
            // changing the Text property will fire an PropertyChanged event
            // which lets the computer know the text has changed and update it
            UpdateScreen();
        }

        public void UpdateScreen()
        {
            SetText(str =>
            {
                str.BeginCenter();
                str.MakeBar('-', SCREEN_WIDTH, 0, "ffffff10");
                str.AppendClr("Gravity Monke", highlightColour).EndColor().AppendLine();
                str.AppendLine("By Ragingg");
                str.MakeBar('-', SCREEN_WIDTH, 0, "ffffff10");
                str.EndAlign().AppendLines(1);
;
                str.AppendLine(selectionHandler.GetIndicatedText(0, $"Enabled: <color=#{highlightColour}>{enabled}</color>"));
                str.AppendLine(selectionHandler.GetIndicatedText(1, $"Gravity: <color=#{highlightColour}>{gravity}</color>"));

                if (!Plugin.inRoom)
                {
                    str.AppendLines(2);
                    str.AppendClr("Please join a modded room!", "A01515").EndColor().AppendLine();
                }
            });
        }

        private void OnEntrySelected(int index)
        {
            try
            {
                switch (index)
                {
                    case 0:
                        Plugin.Allowed = !Plugin.Allowed;
                        enabled = !enabled;
                        UpdateScreen();
                        break;
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
        }

        private void OnEntryAdjusted(int index, bool increase)
        {
            try
            {
                float offset = increase ? -1f : 1f;
                switch (index)
                {
                    case 1:
                        Plugin.gravity = Plugin.gravity + offset;
                        gravity = gravity + offset;
                        UpdateScreen();
                        break;
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
        }

        public override void OnKeyPressed(EKeyboardKey key)
        {
            if (selectionHandler.HandleKeypress(key))
            {
                UpdateScreen();
                return;
            }

            if (key == EKeyboardKey.Left || key == EKeyboardKey.Right)
            {
                OnEntryAdjusted(selectionHandler.CurrentSelectionIndex, key == EKeyboardKey.Right);
                UpdateScreen();
            }

            switch (key)
            {
                case EKeyboardKey.Back:
                    ReturnToMainMenu();
                    break;
            }
        }
    }
}