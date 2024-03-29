﻿using System;
using System.IO;
using System.Reflection;
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
        public float resetGravity = Plugin.resetGravity;
        public bool enabled = Plugin.Allowed;

        private string fileLocation = string.Format("{0}/SaveData", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        private string[] fileArray = new string[3];


        public GravityView()
        {
            instance = this;

            selectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down, EKeyboardKey.Enter);

            selectionHandler.MaxIdx = 2;

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
                str.AppendLine(selectionHandler.GetIndicatedText(2, $"Default Gravity: <color=#{highlightColour}>{resetGravity}</color>"));

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
                        fileArray[0] = enabled.ToString();
                        File.WriteAllText(fileLocation, string.Join(",", fileArray));
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
                        GravityMonke.WD.WristDisplay.instance.UpdateText();
                        fileArray[1] = gravity.ToString();
                        File.WriteAllText(fileLocation, string.Join(",", fileArray));
                        break;

                    case 2:
                        Plugin.resetGravity = Plugin.resetGravity + offset;
                        resetGravity = resetGravity + offset;
                        UpdateScreen();
                        fileArray[2] = resetGravity.ToString();
                        File.WriteAllText(fileLocation, string.Join(",", fileArray));
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