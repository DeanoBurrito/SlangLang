using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using SlangLang.Parsing;
using SlangLang.Binding;

namespace SlangLang
{
    public sealed class Repl
    {
        Dictionary<string, (MethodInfo method, string help)> metaCommands = new Dictionary<string, (MethodInfo, string)>();
        bool showParseTree = false;

        public Repl()
        {}

        public void Run()
        {
            LoadCommands();

            Console.Title = "SlangLang REPL - v" + SlangEnvironment.GetInterpreterVersion().ToString();
            Console.WriteLine();
            BannerGenerator.DrawRandomBanner();
            Console.WriteLine("Welcome to the  S l a n g L a n g  interpreter environment.");
            Console.WriteLine("All input is interpreted and run as code, unless prefixed with '#'.");
            Console.WriteLine("Use '#help' for a list of other repl commands.");
            
            while (true)
            {
                Console.Write(">>> ");
                string line = Console.ReadLine();
                if (line.StartsWith("#"))
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
                else
                {
                    SlangLang.Debug.Diagnostics diags = new SlangLang.Debug.Diagnostics();

                    Lexer lex = new Lexer(diags, line, "Interpreter");
                    Parser parser = new Parser(diags, lex.LexAll());
                    SlangLang.Binding.Binder binder = new SlangLang.Binding.Binder(diags, parser.ParseAll());
                    BoundExpression boundNode = binder.BindAll();
                    SlangLang.Evaluation.Evaluator eval = new Evaluation.Evaluator(diags, boundNode);
                    eval.Evaluate();
                    
                    if (showParseTree)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        PrettyPrintExpTree(boundNode);
                        Console.ResetColor();
                    }
                    diags.WriteToStandardOut();
                }
            }
        }

        private static void PrettyPrintExpTree(BoundExpression node, string indent = "", bool isLast = true)
        {
            string marker = isLast ? "└──" : "├──";
            Console.Write(indent);
            Console.Write(marker);
            Console.WriteLine(node.ToString());

            indent += isLast ? "   " : "│  ";
            BoundExpression lastChild = node.GetChildren().LastOrDefault();
            foreach (BoundExpression child in node.GetChildren())
            {
                PrettyPrintExpTree(child, indent, child == lastChild);
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
                if (args.Length > 0 && bool.TryParse(args[0], out bool showTree))
                {
                    repl.showParseTree = showTree;
                }
                else
                    repl.showParseTree = !repl.showParseTree;
                Console.WriteLine("Displaying expression trees: " + repl.showParseTree);
            }

            [ReplCommand("cls", "Clears the current console output.")]
            public static void Cls(Repl repl, string[] args)
            {
                Console.Clear();
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
                    case 0: s =
@"(`-').->          (`-')  _ <-. (`-')_                         (`-')  _ <-. (`-')_            
 ( OO)_     <-.    (OO ).-/    \( OO) )    .->          <-.    (OO ).-/    \( OO) )    .->    
(_)--\_)  ,--. )   / ,---.  ,--./ ,--/  ,---(`-')     ,--. )   / ,---.  ,--./ ,--/  ,---(`-') 
/    _ /  |  (`-') | \ /`.\ |   \ |  | '  .-(OO )     |  (`-') | \ /`.\ |   \ |  | '  .-(OO ) 
\_..`--.  |  |OO ) '-'|_.' ||  . '|  |)|  | .-, \     |  |OO ) '-'|_.' ||  . '|  |)|  | .-, \ 
.-._)   \(|  '__ |(|  .-.  ||  |\    | |  | '.(_/    (|  '__ |(|  .-.  ||  |\    | |  | '.(_/ 
\       / |     |' |  | |  ||  | \   | |  '-'  |      |     |' |  | |  ||  | \   | |  '-'  |  
 `-----'  `-----'  `--' `--'`--'  `--'  `-----'       `-----'  `--' `--'`--'  `--'  `-----'   ";
                        break;
                    case 1: s =
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
                    case 2: s =
@".------..------..------..------..------.     .------..------..------..------.
|S.--. ||L.--. ||A.--. ||N.--. ||G.--. |.-.  |L.--. ||A.--. ||N.--. ||G.--. |
| :/\: || :/\: || (\/) || :(): || :/\: ((5)) | :/\: || (\/) || :(): || :/\: |
| :\/: || (__) || :\/: || ()() || :\/: |'-.-.| (__) || :\/: || ()() || :\/: |
| '--'S|| '--'L|| '--'A|| '--'N|| '--'G| ((1)) '--'L|| '--'A|| '--'N|| '--'G|
`------'`------'`------'`------'`------'  '-'`------'`------'`------'`------'";
                        break;
                    case 3: s =
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
                    case 4: s =
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
                    case 5: s = 
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
                    case 6: s =
@"   _____ __                     __                     
  / ___// /___ _____  ____ _   / /   ____ _____  ____ _
  \__ \/ / __ `/ __ \/ __ `/  / /   / __ `/ __ \/ __ `/
 ___/ / / /_/ / / / / /_/ /  / /___/ /_/ / / / / /_/ / 
/____/_/\__,_/_/ /_/\__, /  /_____/\__,_/_/ /_/\__, /  
                   /____/                     /____/   ";
                        break;
                    case 7: s =
@"    ___       ___       ___       ___       ___            ___       ___       ___       ___   
   /\  \     /\__\     /\  \     /\__\     /\  \          /\__\     /\  \     /\__\     /\  \  
  /::\  \   /:/  /    /::\  \   /:| _|_   /::\  \        /:/  /    /::\  \   /:| _|_   /::\  \ 
 /\:\:\__\ /:/__/    /::\:\__\ /::|/\__\ /:/\:\__\      /:/__/    /::\:\__\ /::|/\__\ /:/\:\__\
 \:\:\/__/ \:\  \    \/\::/  / \/|::/  / \:\:\/__/      \:\  \    \/\::/  / \/|::/  / \:\:\/__/
  \::/  /   \:\__\     /:/  /    |:/  /   \::/  /        \:\__\     /:/  /    |:/  /   \::/  / 
   \/__/     \/__/     \/__/     \/__/     \/__/          \/__/     \/__/     \/__/     \/__/  ";
                        break;
                    case 8: s =
@"███████╗██╗      █████╗ ███╗   ██╗ ██████╗     ██╗      █████╗ ███╗   ██╗ ██████╗ 
██╔════╝██║     ██╔══██╗████╗  ██║██╔════╝     ██║     ██╔══██╗████╗  ██║██╔════╝ 
███████╗██║     ███████║██╔██╗ ██║██║  ███╗    ██║     ███████║██╔██╗ ██║██║  ███╗
╚════██║██║     ██╔══██║██║╚██╗██║██║   ██║    ██║     ██╔══██║██║╚██╗██║██║   ██║
███████║███████╗██║  ██║██║ ╚████║╚██████╔╝    ███████╗██║  ██║██║ ╚████║╚██████╔╝
╚══════╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝ ╚═════╝     ╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝ ╚═════╝ ";

                        break;
                    case 9: s =
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
                    case 10: s =
@"_       __     ________  _____     ___    ___       ______     ________  _____     ___    ___       ___
 )  ____) \   |       /  \    |    \  |  |   )  ____)     \   |       /  \    |    \  |  |   )  ____)  
(  (___    |  |      /    \   |  |\ \ |  |  /  /  __       |  |      /    \   |  |\ \ |  |  /  /  __   
 \___  \   |  |     /  ()  \  |  | \ \|  | (  (  (  \      |  |     /  ()  \  |  | \ \|  | (  (  (  \  
 ____)  )  |  |__  |   __   | |  |  \    |  \  \__)  )     |  |__  |   __   | |  |  \    |  \  \__)  ) 
(      (__/      )_|  (__)  |_|  |___\   |___)      (_____/      )_|  (__)  |_|  |___\   |___)      (__";
                        break;
                    case 11: s =
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