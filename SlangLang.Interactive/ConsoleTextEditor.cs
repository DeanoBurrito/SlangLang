using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SlangLang.Interactive
{
    public sealed class ConsoleTextEditor
    {
        List<string> lines = new List<string>() { "" };
        int cursorLine = 0;
        int cursorIdx = 0;

        int renderTop = 0;
        int renderLeft = 2;
        int renderWidth = 0;
        char[] partialReadBuff;

        bool showingHelp = false;
        int tabSpacesWidth = 4;

        internal bool readyToExecute = false;
        internal bool readyToExit = false;

        public ConsoleTextEditor()
        {
            Console.WriteLine("In multi-line editor: Ctrl-H to show help, Ctrl-Q to quit, and Ctrl-X to execute.");

            Reinitialize();
        }

        public void Reinitialize()
        {
            renderWidth = Console.WindowWidth;
            renderTop = Console.CursorTop;
            partialReadBuff = new char[renderWidth];

            RenderAll();
        }

        public void HandleConsoleKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
            {
                switch (keyInfo.Key)
                {
                    case ConsoleKey.X:
                        HandleCtrlX(keyInfo);
                        break;
                    case ConsoleKey.Q:
                        HandleCtrlQ(keyInfo);
                        break;
                    case ConsoleKey.H:
                        HandleCtrlH(keyInfo);
                        break;
                }
            }
            else
            {
                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        HandleEnter(keyInfo);
                        break;
                    case ConsoleKey.Home:
                        HandleHome(keyInfo);
                        break;
                    case ConsoleKey.End:
                        HandleEnd(keyInfo);
                        break;
                    case ConsoleKey.UpArrow:
                        HandleUpArrow(keyInfo);
                        break;
                    case ConsoleKey.DownArrow:
                        HandleDownArrow(keyInfo);
                        break;
                    case ConsoleKey.LeftArrow:
                        HandleLeftArrow(keyInfo);
                        break;
                    case ConsoleKey.RightArrow:
                        HandleRightArrow(keyInfo);
                        break;
                    case ConsoleKey.Escape:
                        HandleEscape(keyInfo);
                        break;
                    case ConsoleKey.Backspace:
                        HandleBackspace(keyInfo);
                        break;
                    case ConsoleKey.Tab:
                        HandleTab(keyInfo);
                        break;
                    default:
                        HandleText(keyInfo.KeyChar);
                        break;
                }
            }

            if (showingHelp)
                RenderHelp();
            else
                RenderAll();
        }

        public string[] GetText()
        {
            return lines.ToArray();
        }

        public void RenderAll(bool setupCursor = true)
        {
            if (setupCursor)
                Console.CursorVisible = false;

            for (int y = 0; y < lines.Count; y++)
            {
                RenderLine(y, false);
            }

            if (setupCursor)
            {
                Console.SetCursorPosition(renderLeft + cursorIdx, renderTop + cursorLine);
                Console.CursorVisible = true;
            }
        }

        private void RenderLine(int index, bool setupCursor = true)
        {
            if (index < 0 || index >= lines.Count)
                return;

            List<(int index, ConsoleColor color)> colorChanges = new List<(int index, ConsoleColor color)>();
            StringBuilder lineBuilder = new StringBuilder(renderWidth);
            lineBuilder.Append(new string(' ', renderLeft)); //pad to left-most area we want to draw in
            if (renderLeft > 0)
            {
                colorChanges.Add((0, ConsoleColor.Cyan));
                lineBuilder[0] = '>';
                colorChanges.Add((1, ConsoleColor.Gray));
            }

            string lineText = "";
            if (index < lines.Count)
                lineText = lines[index];

            for (int x = renderLeft; x < renderWidth; x++)
            {
                int textIdx = x - renderLeft;
                if (textIdx < lineText.Length)
                    lineBuilder.Append(lineText[textIdx]);
                else
                    lineBuilder.Append(' ');
            }

            if (setupCursor)
                Console.CursorVisible = false;

            RenderColoredLine(lineBuilder.ToString(), 0, renderTop + index, colorChanges);

            if (setupCursor)
            {
                Console.SetCursorPosition(renderLeft + cursorIdx, renderTop + cursorLine);
                Console.CursorVisible = true;
            }
        }

        private void RenderHelp()
        {
            RenderColoredLine("--- SlangLang Repl text editor ---", 4, renderTop, new List<(int index, ConsoleColor color)>());
            RenderColoredLine("Ctrl-H to show help.", 4, renderTop + 2, new List<(int index, ConsoleColor color)>()
                {
                    (0, ConsoleColor.Gray),
                    (15, ConsoleColor.Yellow),
                    (19, ConsoleColor.Gray),
                });
            RenderColoredLine("Ctrl-Q to quit, without executing any code.", 4, renderTop + 3, new List<(int index, ConsoleColor color)>()
                {
                    (0, ConsoleColor.Gray),
                    (10, ConsoleColor.Red),
                    (14, ConsoleColor.Gray),
                });
            RenderColoredLine("Ctrl-X to execute the current code.", 4, renderTop + 4, new List<(int index, ConsoleColor color)>()
                {
                    (0, ConsoleColor.Gray),
                    (10, ConsoleColor.Green),
                    (17, ConsoleColor.Gray),
                });
        }

        private void ClearLines(int start, int count)
        {
            Console.CursorVisible = false;
            string clearStr = new string(' ', renderWidth);

            for (int i = 0; i < count; i++)
            {
                Console.SetCursorPosition(0, start + i);
                Console.Write(clearStr);
            }

            Console.CursorVisible = true;
        }

        private void RenderColoredLine(string text, int left, int top, List<(int index, ConsoleColor color)> colorChanges)
        {
            Console.SetCursorPosition(left, top);
            if (colorChanges.Count > 0)
            {
                using (StringReader reader = new StringReader(text))
                {
                    for (int i = 0; i < colorChanges.Count; i++)
                    {
                        int readLen;
                        if (i < colorChanges.Count - 1)
                            readLen = colorChanges[i + 1].index - colorChanges[i].index;
                        else
                            readLen = text.Length - colorChanges[i].index;
                        reader.ReadBlock(partialReadBuff, 0, readLen);

                        Console.ForegroundColor = colorChanges[i].color;
                        Console.Write(partialReadBuff, 0, readLen);
                    }
                }
                Console.ResetColor();
            }
            else
            {
                Console.Write(text);
            }
        }

        private string[] GetLineSegments(int line, int splitIndex)
        {
            if (line < 0 || line >= lines.Count)
            {
                return new string[] { string.Empty, string.Empty };
            }

            string lineStr = lines[line];
            if (splitIndex == 0)
            {
                return new string[] { string.Empty, lineStr };
            }
            else if (splitIndex >= lineStr.Length)
            {
                return new string[] { lineStr, string.Empty };
            }
            else
            {
                return new string[]
                {
                        lineStr.Remove(splitIndex),
                        lineStr.Remove(0, splitIndex),
                };
            }
        }

        private void HandleCtrlX(ConsoleKeyInfo info)
        {
            readyToExecute = true;
        }

        private void HandleCtrlQ(ConsoleKeyInfo info)
        {
            if (showingHelp)
            {
                showingHelp = false;
            }
            else
            {
                readyToExecute = false;
                readyToExit = true;
            }
        }

        private void HandleCtrlH(ConsoleKeyInfo info)
        {
            showingHelp = !showingHelp;
            if (showingHelp)
            {
                ClearLines(renderTop, lines.Count);
            }
            else
            {
                ClearLines(renderTop, 5);
            }
        }

        private void HandleEnter(ConsoleKeyInfo info)
        {
            string[] segments = GetLineSegments(cursorLine, cursorIdx);
            lines[cursorLine] = segments[1];
            lines.Insert(cursorLine, segments[0]);

            cursorLine++;
            cursorIdx = 0;
            RenderLine(cursorLine - 1);
        }

        private void HandleHome(ConsoleKeyInfo info)
        {
            cursorIdx = 0;
        }

        private void HandleEnd(ConsoleKeyInfo info)
        {
            cursorIdx = lines[cursorLine].Length;
        }

        private void HandleUpArrow(ConsoleKeyInfo info)
        {
            if (cursorLine > 0)
            {
                cursorLine--;
                if (cursorIdx > lines[cursorLine].Length)
                    cursorIdx = lines[cursorLine].Length;
            }
        }

        private void HandleDownArrow(ConsoleKeyInfo info)
        {
            if (cursorLine < lines.Count - 1)
            {
                cursorLine++;
                if (cursorIdx > lines[cursorLine].Length)
                    cursorIdx = lines[cursorLine].Length;
            }
        }

        private void HandleLeftArrow(ConsoleKeyInfo info)
        {
            if (cursorIdx > 0)
                cursorIdx--;
        }

        private void HandleRightArrow(ConsoleKeyInfo info)
        {
            if (cursorIdx < lines[cursorLine].Length)
                cursorIdx++;
        }

        private void HandleEscape(ConsoleKeyInfo info)
        {
            readyToExecute = false;
            readyToExit = true;
        }

        private void HandleBackspace(ConsoleKeyInfo info)
        {
            if (cursorLine < lines.Count)
            {
                if (cursorIdx <= lines[cursorLine].Length && cursorIdx > 0)
                {
                    lines[cursorLine] = lines[cursorLine].Remove(cursorIdx - 1, 1);
                    cursorIdx--;
                }
            }
        }

        private void HandleTab(ConsoleKeyInfo info)
        {
            if (info.Modifiers.HasFlag(ConsoleModifiers.Shift))
            {
                int removalLen = 0;
                while (removalLen < tabSpacesWidth)
                {
                    if (cursorIdx - removalLen <= 0 || !char.IsWhiteSpace(lines[cursorLine], cursorIdx - removalLen - 1))
                        break;
                    
                    removalLen++;
                }

                lines[cursorLine] = lines[cursorLine].Remove(cursorIdx - removalLen, removalLen);
                cursorIdx -= removalLen;
            }
            else
            {
                string[] segments = GetLineSegments(cursorLine, cursorIdx);
                string processedLine = string.Join(new string(' ', tabSpacesWidth), segments);
                lines[cursorLine] = processedLine;
                cursorIdx += tabSpacesWidth;
            }
        }

        private void HandleText(char input)
        {
            if (cursorIdx >= lines[cursorLine].Length)
                lines[cursorLine] += input.ToString();
            else
                lines[cursorLine] = lines[cursorLine].Insert(cursorIdx, input.ToString());
            cursorIdx++;
        }
    }
}