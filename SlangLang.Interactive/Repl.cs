using System;
using System.Collections.Generic;
using System.Reflection;
using SlangLang.Binding;
using SlangLang.Drivers;
using SlangLang.Debug;

namespace SlangLang.Interactive
{
    public sealed class Repl
    {
        Dictionary<string, (MethodInfo method, string help)> metaCommands = new Dictionary<string, (MethodInfo, string)>();
        bool showTree = false;
        int treeStage = 2; //default of 2 (bound tree)
        bool autoEval = true;

        Dictionary<VariableSymbol, object> variables = new Dictionary<VariableSymbol, object>();
        Compilation previousCompilation = null;

        public Repl()
        { }

        public void Run()
        {
            LoadCommands();

            Console.Title = "SlangLang REPL - v" + SlangEnvironment.GetInterpreterVersion().ToString();
            Console.WriteLine();
            BannerGenerator.DrawRandomBanner();
            Console.WriteLine("Welcome to the  S l a n g L a n g  interpreter environment.");
            Console.WriteLine("All input is interpreted and run as code, unless prefixed with '#'.");
            Console.WriteLine("Use '#help' for a list of other repl commands.");

            bool inMultilineMode = false;
            List<string> multilineTexts = new List<string>();
            while (true)
            {
                if (inMultilineMode)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("> ");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(">>> ");
                }
                
                string line = Console.ReadLine();
                if (inMultilineMode)
                {
                    if (line == "#exit")
                    {
                        DoCompilation(multilineTexts.ToArray());
                        inMultilineMode = false;
                        multilineTexts.Clear();
                    }
                    else
                    {
                        multilineTexts.Add(line);
                    }
                }
                else if (line.StartsWith("#"))
                {
                    if (line.Length == 1)
                        continue;
                    //try process meta command
                    line = line.Remove(0, 1);
                    string[] args = line.Split(' ');
                    if (metaCommands.ContainsKey(args[0]))
                    {
                        List<string> argList = new List<string>(args);
                        argList.RemoveRange(0, 1);
                        metaCommands[args[0]].method.Invoke(null, new object[] { this, argList.ToArray() });
                    }
                    else
                    {
                        Console.WriteLine("Command '" + args[0] + "' not recognised.");
                    }
                }
                else if (line.Length == 0)
                {
                    inMultilineMode = true;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("* [Multiline edit mode: to exit type '#exit']");
                    Console.ResetColor();
                }
                else
                {
                    DoCompilation(new string[] { line });
                }
            }
        }

        private void DoCompilation(string[] lines)
        {
            CompilationOptions options = new CompilationOptions();
            switch (treeStage)
            {
                case 0:
                    options.printLexerOutput = showTree;
                    break;
                case 1:
                    options.printParserOutput = showTree;
                    break;
                case 2:
                    options.printBinderOutput = showTree;
                    break;
            }

            Compilation currentCompilation;
            if (previousCompilation == null)
                currentCompilation = new Compilation(new TextStore("REPL", lines), options);
            else
                currentCompilation = previousCompilation.ContinueWith(new TextStore("REPL", lines), options);
            
            if (autoEval)
            {
                Evaluation.EvaluationResult result = currentCompilation.Evaluate(variables);
                if (result.diagnostics.HasErrors)
                {
                    result.diagnostics.WriteToStandardOut();
                }
                else
                {
                    Console.WriteLine("Evaluation result: " + result.value.ToString());
                    previousCompilation = currentCompilation;
                }
            }
        }

        private void LoadCommands()
        {
            Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type t in allTypes)
            {
                foreach (MethodInfo info in t.GetMethods())
                {
                    ReplCommandAttribute attrib = info.GetCustomAttribute<ReplCommandAttribute>();
                    if (attrib == null)
                        continue;
                    if (metaCommands.ContainsKey(attrib.identifier))
                        continue;
                    metaCommands.Add(attrib.identifier, (info, attrib.helptext));
                }
            }
        }

        static class ReplCommands
        {
            [ReplCommand("exit", "Exits this CLI.")]
            public static void Exit(Repl repl, string[] args)
            {
                Console.WriteLine("Exiting REPL...");
                Environment.Exit(0);
            }

            [ReplCommand("help", "Prints this dialogue.")]
            public static void Help(Repl repl, string[] args)
            {
                Console.WriteLine(repl.metaCommands.Count + " commands currently exist.");
                foreach (KeyValuePair<string, (MethodInfo, string)> pair in repl.metaCommands)
                {
                    Console.WriteLine(pair.Key.PadRight(30) + "- " + pair.Value.Item2);
                }
            }

            [ReplCommand("version", "Displays the various version numbers of the environment, compiler, repl.")]
            public static void Version(Repl repl, string[] args)
            {
                Console.WriteLine("     Enviroment Ver : " + SlangEnvironment.GetEnvironmentVersion().ToString());
                Console.WriteLine("    Interpreter Ver : " + SlangEnvironment.GetInterpreterVersion().ToString());
            }

            [ReplCommand("banner", "Displays a random startup banner.")]
            public static void Banner(Repl repl, string[] args)
            {
                BannerGenerator.DrawRandomBanner();
            }

            [ReplCommand("showtree", "Toggles showing the expression tree after parsing/lexing.")]
            public static void ToggleShowTree(Repl repl, string[] args)
            {
                if (args.Length > 0 && int.TryParse(args[0], out int stage))
                {
                    repl.showTree = true;
                    if (stage < 0 || stage > 2)
                        repl.treeStage = 0;
                    else
                        repl.treeStage = stage;
                }
                else
                    repl.showTree = !repl.showTree;
                Console.WriteLine("Displaying expression trees: " + repl.showTree);
            }

            [ReplCommand("cls", "Clears the current console output.")]
            public static void Cls(Repl repl, string[] args)
            {
                Console.Clear();
            }

            [ReplCommand("autoeval", "Sets whether or not to automatically evaluate the tree.")]
            public static void AutoEval(Repl repl, string[] args)
            {
                if (args.Length > 0 && bool.TryParse(args[0], out bool autoEval))
                {
                    repl.autoEval = autoEval;
                }
                else
                    repl.autoEval = !repl.autoEval;
                Console.WriteLine("Automatically evaluating parsed trees: " + repl.autoEval);
            }

            [ReplCommand("lsvars", "Lists all current variables.")]
            public static void ListVars(Repl repl, string[] args)
            {
                foreach (KeyValuePair<VariableSymbol, object> varPair in repl.variables)
                {
                    Console.WriteLine(varPair.Key.type + " " + varPair.Key.name + " = " + varPair.Value);
                }
            }

            [ReplCommand("reset", "Forgets any previous compilations (variables and errors included).")]
            public static void Reset(Repl repl, string[] args)
            {
                repl.previousCompilation = null;
            }
        }

        //I'm sorry, I couldnt help myself.
        private static class BannerGenerator
        {
            public static void DrawRandomBanner()
            {
                int idx = new Random().Next(12);
                string s = "Hmm, no banner today ;-;";
                switch (idx)
                {
                    case 0:
                        s =
@"(`-').->          (`-')  _ <-. (`-')_                         (`-')  _ <-. (`-')_            
 ( OO)_     <-.    (OO ).-/    \( OO) )    .->          <-.    (OO ).-/    \( OO) )    .->    
(_)--\_)  ,--. )   / ,---.  ,--./ ,--/  ,---(`-')     ,--. )   / ,---.  ,--./ ,--/  ,---(`-') 
/    _ /  |  (`-') | \ /`.\ |   \ |  | '  .-(OO )     |  (`-') | \ /`.\ |   \ |  | '  .-(OO ) 
\_..`--.  |  |OO ) '-'|_.' ||  . '|  |)|  | .-, \     |  |OO ) '-'|_.' ||  . '|  |)|  | .-, \ 
.-._)   \(|  '__ |(|  .-.  ||  |\    | |  | '.(_/    (|  '__ |(|  .-.  ||  |\    | |  | '.(_/ 
\       / |     |' |  | |  ||  | \   | |  '-'  |      |     |' |  | |  ||  | \   | |  '-'  |  
 `-----'  `-----'  `--' `--'`--'  `--'  `-----'       `-----'  `--' `--'`--'  `--'  `-----'   ";
                        break;
                    case 1:
                        s =
@"                                                                ,---.'|                                       
  .--.--.     ,--,                                              |   | :                                       
 /  /    '. ,--.'|                                              :   : |                                       
|  :  /`. / |  | :                     ,---,                    |   ' :                     ,---,             
;  |  |--`  :  : '                 ,-+-. /  |  ,----._,.        ;   ; '                 ,-+-. /  |  ,----._,. 
|  :  ;_    |  ' |     ,--.--.    ,--.'|'   | /   /  ' /        '   | |__   ,--.--.    ,--.'|'   | /   /  ' / 
 \  \    `. '  | |    /       \  |   |  ,'' ||   :     |        |   | :.'| /       \  |   |  ,'' ||   :     | 
  `----.   \|  | :   .--.  .-. | |   | /  | ||   | .\  .        '   :    ;.--.  .-. | |   | /  | ||   | .\  . 
  __ \  \  |'  : |__  \__\/: . . |   | |  | |.   ; ';  |        |   |  ./  \__\/: . . |   | |  | |.   ; ';  | 
 /  /`--'  /|  | '.'| ,' .--.; | |   | |  |/ '   .   . |        ;   : ;    ,' .--.; | |   | |  |/ '   .   . | 
'--'.     / ;  :    ;/  /  ,.  | |   | |--'   `---`-'| |        |   ,/    /  /  ,.  | |   | |--'   `---`-'| | 
  `--'---'  |  ,   /;  :   .'   \|   |/       .'__/\_: |        '---'    ;  :   .'   \|   |/       .'__/\_: | 
             ---`-' |  ,     .-./'---'        |   :    :                 |  ,     .-./'---'        |   :    : 
                     `--`---'                  \   \  /                   `--`---'                  \   \  /  
                                                `--`-'                                               `--`-'   ";
                        break;
                    case 2:
                        s =
@".------..------..------..------..------.     .------..------..------..------.
|S.--. ||L.--. ||A.--. ||N.--. ||G.--. |.-.  |L.--. ||A.--. ||N.--. ||G.--. |
| :/\: || :/\: || (\/) || :(): || :/\: ((5)) | :/\: || (\/) || :(): || :/\: |
| :\/: || (__) || :\/: || ()() || :\/: |'-.-.| (__) || :\/: || ()() || :\/: |
| '--'S|| '--'L|| '--'A|| '--'N|| '--'G| ((1)) '--'L|| '--'A|| '--'N|| '--'G|
`------'`------'`------'`------'`------'  '-'`------'`------'`------'`------'";
                        break;
                    case 3:
                        s =
@"         .---.                                         .---.                                
           |   |             _..._                       |   |             _..._              
           |   |           .'     '.   .--./)            |   |           .'     '.   .--./)   
           |   |          .   .-.   . /.''\\             |   |          .   .-.   . /.''\\    
           |   |    __    |  '   '  || |  | |            |   |    __    |  '   '  || |  | |   
       _   |   | .:--.'.  |  |   |  | \`-' /             |   | .:--.'.  |  |   |  | \`-' /    
     .' |  |   |/ |   \ | |  |   |  | /(''`              |   |/ |   \ | |  |   |  | /(''`     
    .   | /|   |`' __ | | |  |   |  | \ '---.            |   |`' __ | | |  |   |  | \ '---.   
  .'.'| |//|   | .'.''| | |  |   |  |  /'""'.\           |   | .'.''| | |  |   |  |  /'""'.\  
.'.'.-'  / '---'/ /   | |_|  |   |  | ||     ||          '---'/ /   | |_|  |   |  | ||     || 
.'   \_.'       \ \._,\ '/|  |   |  | \'. __//                \ \._,\ '/|  |   |  | \'. __//  
                 `--'  `' '--'   '--'  `'---'                  `--'  `' '--'   '--'  `'---'  ";
                        break;
                    case 4:
                        s =
@" (                             (                         
 )\ )  (                       )\ )                      
(()/(  )\    )         (  (   (()/(     )         (  (   
 /(_))((_)( /(   (     )\))(   /(_)) ( /(   (     )\))(  
(_))   _  )(_))  )\ ) ((_))\  (_))   )(_))  )\ ) ((_))\  
/ __| | |((_)_  _(_/(  (()(_) | |   ((_)_  _(_/(  (()(_) 
\__ \ | |/ _` || ' \))/ _` |  | |__ / _` || ' \))/ _` |  
|___/ |_|\__,_||_||_| \__, |  |____|\__,_||_||_| \__, |  
                      |___/                      |___/   ";
                        break;
                    case 5:
                        s =
@"   .-'''-.   .---.        ____    ,---.   .--.  .-_'''-.             .---.        ____    ,---.   .--.  .-_'''-.    
  / _     \  | ,_|      .'  __ `. |    \  |  | '_( )_   \            | ,_|      .'  __ `. |    \  |  | '_( )_   \   
 (`' )/`--',-./  )     /   '  \  \|  ,  \ |  ||(_ o _)|  '         ,-./  )     /   '  \  \|  ,  \ |  ||(_ o _)|  '  
(_ o _).   \  '_ '`)   |___|  /  ||  |\_ \|  |. (_,_)/___|         \  '_ '`)   |___|  /  ||  |\_ \|  |. (_,_)/___|  
 (_,_). '.  > (_)  )      _.-`   ||  _( )_\  ||  |  .-----.         > (_)  )      _.-`   ||  _( )_\  ||  |  .-----. 
.---.  \  :(  .  .-'   .'   _    || (_ o _)  |'  \  '-   .'        (  .  .-'   .'   _    || (_ o _)  |'  \  '-   .' 
\    `-'  | `-'`-'|___ |  _( )_  ||  (_,_)\  | \  `-'`   |          `-'`-'|___ |  _( )_  ||  (_,_)\  | \  `-'`   |  
 \       /   |        \\ (_ o _) /|  |    |  |  \        /           |        \\ (_ o _) /|  |    |  |  \        /  
  `-...-'    `--------` '.(_,_).' '--'    '--'   `'-...-'            `--------` '.(_,_).' '--'    '--'   `'-...-'   ";
                        break;
                    case 6:
                        s =
@"   _____ __                     __                     
  / ___// /___ _____  ____ _   / /   ____ _____  ____ _
  \__ \/ / __ `/ __ \/ __ `/  / /   / __ `/ __ \/ __ `/
 ___/ / / /_/ / / / / /_/ /  / /___/ /_/ / / / / /_/ / 
/____/_/\__,_/_/ /_/\__, /  /_____/\__,_/_/ /_/\__, /  
                   /____/                     /____/   ";
                        break;
                    case 7:
                        s =
@"    ___       ___       ___       ___       ___            ___       ___       ___       ___   
   /\  \     /\__\     /\  \     /\__\     /\  \          /\__\     /\  \     /\__\     /\  \  
  /::\  \   /:/  /    /::\  \   /:| _|_   /::\  \        /:/  /    /::\  \   /:| _|_   /::\  \ 
 /\:\:\__\ /:/__/    /::\:\__\ /::|/\__\ /:/\:\__\      /:/__/    /::\:\__\ /::|/\__\ /:/\:\__\
 \:\:\/__/ \:\  \    \/\::/  / \/|::/  / \:\:\/__/      \:\  \    \/\::/  / \/|::/  / \:\:\/__/
  \::/  /   \:\__\     /:/  /    |:/  /   \::/  /        \:\__\     /:/  /    |:/  /   \::/  / 
   \/__/     \/__/     \/__/     \/__/     \/__/          \/__/     \/__/     \/__/     \/__/  ";
                        break;
                    case 8:
                        s =
@"███████╗██╗      █████╗ ███╗   ██╗ ██████╗     ██╗      █████╗ ███╗   ██╗ ██████╗ 
██╔════╝██║     ██╔══██╗████╗  ██║██╔════╝     ██║     ██╔══██╗████╗  ██║██╔════╝ 
███████╗██║     ███████║██╔██╗ ██║██║  ███╗    ██║     ███████║██╔██╗ ██║██║  ███╗
╚════██║██║     ██╔══██║██║╚██╗██║██║   ██║    ██║     ██╔══██║██║╚██╗██║██║   ██║
███████║███████╗██║  ██║██║ ╚████║╚██████╔╝    ███████╗██║  ██║██║ ╚████║╚██████╔╝
╚══════╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝ ╚═════╝     ╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝ ╚═════╝ ";

                        break;
                    case 9:
                        s =
@"  ██████  ██▓    ▄▄▄       ███▄    █   ▄████     ██▓    ▄▄▄       ███▄    █   ▄████ 
▒██    ▒ ▓██▒   ▒████▄     ██ ▀█   █  ██▒ ▀█▒   ▓██▒   ▒████▄     ██ ▀█   █  ██▒ ▀█▒
░ ▓██▄   ▒██░   ▒██  ▀█▄  ▓██  ▀█ ██▒▒██░▄▄▄░   ▒██░   ▒██  ▀█▄  ▓██  ▀█ ██▒▒██░▄▄▄░
  ▒   ██▒▒██░   ░██▄▄▄▄██ ▓██▒  ▐▌██▒░▓█  ██▓   ▒██░   ░██▄▄▄▄██ ▓██▒  ▐▌██▒░▓█  ██▓
▒██████▒▒░██████▒▓█   ▓██▒▒██░   ▓██░░▒▓███▀▒   ░██████▒▓█   ▓██▒▒██░   ▓██░░▒▓███▀▒
▒ ▒▓▒ ▒ ░░ ▒░▓  ░▒▒   ▓▒█░░ ▒░   ▒ ▒  ░▒   ▒    ░ ▒░▓  ░▒▒   ▓▒█░░ ▒░   ▒ ▒  ░▒   ▒ 
░ ░▒  ░ ░░ ░ ▒  ░ ▒   ▒▒ ░░ ░░   ░ ▒░  ░   ░    ░ ░ ▒  ░ ▒   ▒▒ ░░ ░░   ░ ▒░  ░   ░ 
░  ░  ░    ░ ░    ░   ▒      ░   ░ ░ ░ ░   ░      ░ ░    ░   ▒      ░   ░ ░ ░ ░   ░ 
      ░      ░  ░     ░  ░         ░       ░        ░  ░     ░  ░         ░       ░ ";
                        break;
                    case 10:
                        s =
@"_       __     ________  _____     ___    ___       ______     ________  _____     ___    ___       ___
 )  ____) \   |       /  \    |    \  |  |   )  ____)     \   |       /  \    |    \  |  |   )  ____)  
(  (___    |  |      /    \   |  |\ \ |  |  /  /  __       |  |      /    \   |  |\ \ |  |  /  /  __   
 \___  \   |  |     /  ()  \  |  | \ \|  | (  (  (  \      |  |     /  ()  \  |  | \ \|  | (  (  (  \  
 ____)  )  |  |__  |   __   | |  |  \    |  \  \__)  )     |  |__  |   __   | |  |  \    |  \  \__)  ) 
(      (__/      )_|  (__)  |_|  |___\   |___)      (_____/      )_|  (__)  |_|  |___\   |___)      (__";
                        break;
                    case 11:
                        s =
@"     ___    |¯¯¯|__'   )¯¯,¯\ ° |\¯¯¯\)¯¯\    /¯¯¯/\__\‘           |¯¯¯|__'   )¯¯,¯\ ° |\¯¯¯\)¯¯\    /¯¯¯/\__\‘  
  _(   __\  |_____'|  /__/'\__\ |/__/\____\°|\___\)¯¯¯)°         |_____'|  /__/'\__\ |/__/\____\°|\___\)¯¯¯)°
/____)__| |_____'| |__ |/\|__|'|__|/\|____|  \|     |¯¯¯|           |_____'| |__ |/\|__|'|__|/\|____|  \|     |¯¯¯|  
|____|‘     ‘           '                    '            ¯¯¯¯¯¯            ‘           '                    '            ¯¯¯¯¯¯   ";
                        break;
                }
                Console.WriteLine(s);
            }
        }
    }
}