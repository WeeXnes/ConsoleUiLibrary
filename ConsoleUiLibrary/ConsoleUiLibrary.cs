using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Console = ConsoleUiLibrary.WXNConsole;
using VanillaConsole = System.Console;

namespace ConsoleUiLibrary
{
    public class CUI
    {
        public static void SetupGui(Action mainMethod)
        {
            Console.Utils.DisableCursor();
            VanillaConsole.Clear();
            VanillaConsole.Title = "ConsoleMenu";
            Console.Utils.SetupConsoleKeepOpen();
            SetupBackgroundWorkers();
            CurrentMenu.ValueChanged += () => VanillaConsole.Clear();
            mainMethod.Invoke();
            Console.Utils.KeepConsoleOpen();
        }
        public class MenuItem
        {
            public string displayText { get; set; }
            public Action onClick { get; set; }

            public MenuItem(string displayText, Action method)
            {
                this.displayText = displayText;
                this.onClick = method;
            }
        }
        public class CmdMenu
        {
            public int selectedIndex { 
                get; 
                set; 
            }
            public string Name { get; set; }
            public List<MenuItem> MenuItems { get; set; }
            public CmdMenu(string Name, List<MenuItem> MenuItems)
            {
                this.MenuItems = MenuItems;
                this.Name = Name;
                this.selectedIndex = 0;
            }
        }
        
        private static bool pauseWorkers = false;
        private static string filler = "                                ";
        private static BackgroundWorker InputWorker = new BackgroundWorker();
        private static BackgroundWorker RenderWorker = new BackgroundWorker();
        /// <summary>
        /// Holds the Current Menu as a CmdMenu Object
        /// Access by GUI.CurrentMenu.Value 
        /// </summary>
        public static UpdateVar<CUI.CmdMenu> CurrentMenu = new UpdateVar<CUI.CmdMenu>();
        /// <summary>
        /// Refresh Delay of the Background Threads
        /// </summary>
        public static int WorkerDelay = 20;
        

        private static void SetupBackgroundWorkers()
        {
            InputWorker.DoWork += InputWorkerOnDoWork;
            RenderWorker.DoWork += RenderWorkerOnDoWork;
            InputWorker.RunWorkerAsync();
            RenderWorker.RunWorkerAsync();
        }

        private static void RenderWorkerOnDoWork(object? sender, DoWorkEventArgs e)
        {
            while (true)
            {
                while (pauseWorkers)
                {
                    Thread.Sleep(WorkerDelay);
                }
                System.Console.CursorTop = 0;
                System.Console.CursorLeft = 0;
                if(CurrentMenu.Value != null)
                    drawMenu(CurrentMenu.Value);
                Thread.Sleep(WorkerDelay);
                //System.Console.Clear();
            }
        }

        private static void drawMenu(CUI.CmdMenu menu)
        {
            foreach (CUI.MenuItem menuItem in menu.MenuItems)
            {
                if (menu.MenuItems.IndexOf(menuItem) == menu.selectedIndex)
                {
                    Console.WriteLine("> " + menuItem.displayText + filler);
                }
                else
                {
                        
                    Console.WriteLine("  " + menuItem.displayText + filler);
                }
            }
        }

        private static void InputWorkerOnDoWork(object? sender, DoWorkEventArgs e)
        {
            while (true)
            {
                while (pauseWorkers)
                {
                    Thread.Sleep(WorkerDelay);
                }
                ConsoleKey pressedKey = System.Console.ReadKey().Key;
                if (pressedKey == ConsoleKey.UpArrow)
                {
                    if (!(CurrentMenu.Value.selectedIndex - 1 < 0))
                    {
                        CurrentMenu.Value.selectedIndex = CurrentMenu.Value.selectedIndex-1;
                    }
                    else
                    {
                        CurrentMenu.Value.selectedIndex = CurrentMenu.Value.MenuItems.Count - 1;
                    }
                    //debugWrite("Menu up");
                }

                if (pressedKey == ConsoleKey.DownArrow)
                {
                    //debugWrite("Menu down");
                    if (!(CurrentMenu.Value.selectedIndex + 1 > CurrentMenu.Value.MenuItems.Count-1))
                    {
                        CurrentMenu.Value.selectedIndex = CurrentMenu.Value.selectedIndex+1;
                    }
                    else
                    {
                        CurrentMenu.Value.selectedIndex = 0;
                    }
                }

                if (pressedKey == ConsoleKey.Enter)
                {
                    CurrentMenu.Value.MenuItems[CurrentMenu.Value.selectedIndex].onClick.Invoke();
                }
                Thread.Sleep(WorkerDelay);
            }
        }
        /// <summary>
        /// Pauses Ui Workers and executes some command
        /// </summary>
        /// <param name="method"></param>
        public static void InvokeAction(Action method)
        {
            pauseWorkers = true;
            System.Console.Clear();
            method.Invoke();
            System.Console.Clear();
            pauseWorkers = false;
        }
    }


    public static class WXNConsole
        {
            public static class Utils
            {
                static ManualResetEvent _quitEvent = new ManualResetEvent(false);
                public static void SetupConsoleKeepOpen()
                {
                    VanillaConsole.CancelKeyPress += (sender, eArgs) => {
                        _quitEvent.Set();
                        eArgs.Cancel = true;
                    };
                }

                public static void KeepConsoleOpen()
                {
                    _quitEvent.WaitOne();
                }
                
                private const int ATTACH_PARENT_PROCESS = -1;
                [DllImport("kernel32.dll")]
                private static extern bool AttachConsole(int dwProcessId);

                 /// <summary>
                /// /// Attach Programm to its own console (For JetBrains Rider)
                /// </summary>
                public static void AttachToConsole()
                {
                    AttachConsole(ATTACH_PARENT_PROCESS);
                }

                 public static void DisableCursor()
                 {
                     VanillaConsole.CursorVisible = false;
                 }
            }
            public static class Data
            {
                public static class Colors
                {
                    public static bool colored_output = true;
                    public static ConsoleColor int_color = ConsoleColor.Blue;
                    public static ConsoleColor double_color = ConsoleColor.Cyan;
                    public static ConsoleColor float_color = ConsoleColor.DarkCyan;
                }

                public static class Formatting
                {
                    
                }
            }
            #region AllDifferentWriteLineTypes

            public static void WriteLine(string text)
            {
                VanillaConsole.WriteLine(text);
            }

            #endregion
            private static void ConfiguredWriteline(string text, ConsoleColor color, ConsoleColor foregroundColor = ConsoleColor.White)
            {
                VanillaConsole.OutputEncoding = Encoding.UTF8;
                ConsoleColor prevColor = VanillaConsole.BackgroundColor;
                ConsoleColor prevForeColor = VanillaConsole.ForegroundColor;
                if (Data.Colors.colored_output)
                {
                    VanillaConsole.BackgroundColor = color;
                    VanillaConsole.ForegroundColor = foregroundColor;
                }
                VanillaConsole.WriteLine(text + " ");
                if (Data.Colors.colored_output)
                {
                    
                    VanillaConsole.BackgroundColor = prevColor;
                    VanillaConsole.ForegroundColor = prevForeColor;
                }
            }

            
            public static void Success(string text,
                [CallerLineNumber] int lineNumber = 0,
                [CallerMemberName] string caller = null)
            {
                ConfiguredWriteline(" ✓ (" + lineNumber + "|" + caller + ") " + text, ConsoleColor.Green, ConsoleColor.Black);
            }
            public static void Info(string text,
                [CallerLineNumber] int lineNumber = 0,
                [CallerMemberName] string caller = null)
            {
                ConfiguredWriteline(" ◈ (" + lineNumber + "|" + caller + ") " + text, ConsoleColor.Blue, ConsoleColor.Black);
            }
            public static void Error(string text,
                [CallerLineNumber] int lineNumber = 0,
                [CallerMemberName] string caller = null)
            {
                ConfiguredWriteline(" ☓ (" + lineNumber + "|" + caller + ") " + text, ConsoleColor.DarkRed, ConsoleColor.Black);
            }
            public static void Warning(string text,
                [CallerLineNumber] int lineNumber = 0,
                [CallerMemberName] string caller = null)
            {
                ConfiguredWriteline(" ⌬ (" + lineNumber + "|" + caller + ") " + text, ConsoleColor.DarkYellow, ConsoleColor.Black);
            }

            public static void WriteLine<T>(List<T> List, bool verbose = true)
            {
                ConfiguredWriteline("List contains " + typeof(T) + "(" + List.Count + ")", ConsoleColor.DarkMagenta, ConsoleColor.Black);
                if(!verbose)
                    return;

                for (int i = 0; i < List.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        ConfiguredWriteline("ListIdx(" + i + "): " + List[i], ConsoleColor.DarkGray);
                    }
                    else
                    {
                        ConfiguredWriteline("ListIdx(" + i + "): " + List[i], ConsoleColor.Black);
                    }
                }
            }

            public static void WriteLine(int IntNumber)
            {
                ConfiguredWriteline("Int: " + IntNumber, Data.Colors.int_color, ConsoleColor.Black);
            }
            public static void WriteLine(double DoubleNumber)
            {
                ConfiguredWriteline("Double: " + DoubleNumber, Data.Colors.double_color, ConsoleColor.Black);
            }
            public static void WriteLine(float FloatNumber)
            {
                ConfiguredWriteline("Float: " + FloatNumber, Data.Colors.float_color, ConsoleColor.Black);
            }
        }
        
    public class UpdateVar<T>
    {
        private T _value;

        public Action ValueChanged;

        public T Value
        {
            get => _value;

            set
            {
                _value = value;
                OnValueChanged();
            }
        }

        protected virtual void OnValueChanged() => ValueChanged?.Invoke();
    }
}