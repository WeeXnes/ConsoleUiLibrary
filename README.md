<div align="center">
    <img width="100" height="100" src="https://cdn.discordapp.com/attachments/741123537582162020/965619554426437732/wicon.png">
    
</div>
<h1 align="center">ConsoleUiLibrary</h1>


Install via <a href="https://www.nuget.org/packages/ConsoleUiLibrary">nuget</a>: 

```cmd
PM> Install-Package ConsoleUiLibrary
```

<h2>Setup Menu:</h2>

```cs
CUI.SetupGui(menuLoop);
```
the in the brackets defined method acts like a "render loop" for the menu

<h2>Setup menuLoop method:</h2>

First we create a MenuItem
```cs
CUI.MenuItem item1 = new CUI.MenuItem("Test Button 1", () =>
{
                
});
```
Then we assign it a method inside the curly brackets that fires when the button is clicked, using the CUI.InvokeAction method we can pause 
the render worker and Invoke the method thats defined in the curly brackets. It automatically resumes the renderer when the Action has executed

```cs
CUI.MenuItem item1 = new CUI.MenuItem("Test Button 1", () =>
{
         CUI.InvokeAction(() =>
         {
                    Console.WriteLine("Hello from Button 1");
                    Console.ReadKey();
         });
});
```

If you created all your Items/Buttons, create a List and add your Items:

```cs
List<CUI.MenuItem> Items = new List<CUI.MenuItem>();
Items.Add(item1);
```

after that, create a CmdMenu with a name and the List we just created and set it as the current menu

```cs
CUI.CmdMenu MainMenu = new CUI.CmdMenu("Main Menu", Items);
CUI.CurrentMenu.Value = MainMenu;
```

<h1>Example Menu (Full source) in compact form:</h1>


```cs
        public static void Main(string[] args)
        {
            CUI.SetupGui(menuLoop);
        }

        static void menuLoop()
        {
            CUI.CmdMenu MainMenu = new CUI.CmdMenu("Main Menu", new List<CUI.MenuItem>()
            {
                new CUI.MenuItem("Example Item 1", () =>
                {
                    //Example Action 
                    CUI.InvokeAction(() =>
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            Console.WriteLine(i);
                            Thread.Sleep(200);
                        }
                    });
                }),
                new CUI.MenuItem("Open Sub Menu", () =>
                {
                    //Example Sub Menu
                    CUI.CmdMenu cache = CUI.CurrentMenu.Value;
                    CUI.CurrentMenu.Value = new CUI.CmdMenu("Sub Menu", new List<CUI.MenuItem>()
                    {
                        new CUI.MenuItem("Example Sub Item 1", () =>
                        {
                            //Some Code
                        }),
                        new CUI.MenuItem("Example Sub Item 2", () =>
                        {
                            //Some Code
                        }),
                        new CUI.MenuItem("Back", () =>
                        {
                            CUI.CurrentMenu.Value = cache;
                        })
                    });
                }),
                new CUI.MenuItem("Example Item 3", () =>
                {
                    //Some Code
                })
            });
            CUI.CurrentMenu.Value = MainMenu;
        }
    }

```
