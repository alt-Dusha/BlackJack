using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack
{
    internal class Program
    {
        static int[] score = { 0, 0 };
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Что бы начать игру напишите: play\nСчёт игры за последний запуск(твой:диллера): {score[0]}:{score[1]}");
                string mess = Console.ReadLine();
                if (mess == "play")
                {
                    Console.Clear();
                    Menu();
                }
                else
                {
                    Console.Clear();
                    Main(args);
                }
            }
        }

        static void Menu()
        {
            string[,] cards =
             {
                { "A♣", "11" }, { "A♠", "11" }, { "A♥", "11" }, { "A♦", "11" },
                { "K♣", "10" }, { "K♠", "10" }, { "K♥", "10" }, { "K♦", "10" },
                { "Q♣", "10" }, { "Q♠", "10" }, { "Q♥", "10" }, { "Q♦", "10" },
                { "V♣", "10" }, { "V♠", "10" }, { "V♥", "10" }, { "V♦", "10" },
                { "10♣","10" }, { "10♠","10" }, { "10♥","10" }, { "10♦","10" },
                { "9♣", "9" },  { "9♠", "9" },  { "9♥", "9" },  { "9♦", "9" },
                { "8♣", "8" },  { "8♠", "8" },  { "8♥", "8" },  { "8♦", "8" },
                { "7♣", "7" },  { "7♠", "7" },  { "7♥", "7" },  { "7♦", "7" },
                { "6♣", "6" },  { "6♠", "6" },  { "6♥", "6" },  { "6♦", "6" },
                { "5♣", "5" },  { "5♠", "5" },  { "5♥", "5" },  { "5♦", "5" },
                { "4♣", "4" },  { "4♠", "4" },  { "4♥", "4" },  { "4♦", "4" },
                { "3♣", "3" },  { "3♠", "3" },  { "3♥", "3" },  { "3♦", "3" },
                { "2♣", "2" },  { "2♠", "2" },  { "2♥", "2" },  { "2♦", "2" },
            };
            string[] handPlayer = new string[5];
            string[] handDiller = new string[5];
            string[] indexCardInGame = new string[10];

            //Создание консольной надписи вверху

            First(cards, ref handPlayer, ref handDiller, ref indexCardInGame);
            string menu = "";
            for (int i = 0; i < 21; i++)
            {
                menu += " ";
                if (i == 10)
                {
                    menu += "Black Jack";
                }
            }
            string dillerConsole = "\nДиллер:\t";
            string playerConsole = "\nТвои:\t";
            foreach (string card in handPlayer)
            {
                if (card != null)
                {
                    playerConsole += card + " ";
                }
            }
            int c = 2;
            while (true)
            {
                Console.WriteLine(menu + dillerConsole + handDiller[0] + " *"
                    + playerConsole + "\nБерёшь карту?(+/-)");
                if (Console.ReadLine() == "+")
                {
                    Console.Clear();
                    if (c < handPlayer.Length)
                    {
                        GetCard(ref handPlayer, cards, ref indexCardInGame);
                        playerConsole += handPlayer[c] + " ";
                        c++;
                    }
                    if (ScoreBy(handPlayer, cards) == 21)
                    {
                        Console.Clear();
                        Console.WriteLine(menu + dillerConsole + DillerCards(handDiller) + playerConsole + "\nПобеда!");
                        score[0] += 1;
                        Vait();
                        break;
                    }
                    else if (ScoreBy(handPlayer, cards) > 21)
                    {
                        Console.Clear();
                        Console.WriteLine($"{menu}{dillerConsole + DillerCards(handDiller)}{playerConsole}\nУвы, но ты проиграл(\nТвой счёт: {ScoreBy(handPlayer, cards)}");
                        score[1] += 1;
                        Vait();
                        break;
                    }
                }
                else
                {
                    Console.Clear();
                    while (ScoreBy(handDiller, cards) < 17)
                    {
                        GetCard(ref handDiller, cards, ref indexCardInGame);
                        Console.WriteLine(menu + dillerConsole + DillerCards(handDiller) + playerConsole);
                    }
                    if (ScoreBy(handDiller, cards) > 21)
                    {
                        Console.Clear();
                        Console.WriteLine($"{menu}{dillerConsole + DillerCards(handDiller)}{playerConsole}" +
                            $"\nПобеда!(\nСчёт диллера: {ScoreBy(handDiller, cards)}");
                        Vait();
                        score[0] += 1;
                        break;
                    }
                    else
                    {
                        if (ScoreBy(handDiller, cards) > ScoreBy(handPlayer, cards))
                        {
                            Console.Clear();
                            Console.WriteLine(menu + dillerConsole + DillerCards(handDiller) + "\nЕго счёт: " + ScoreBy(handDiller, cards) +
                                " а твой: " + ScoreBy(handPlayer, cards) + playerConsole + "\nТы проиграл!");
                            Vait();
                            score[1] += 1;
                            break;
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine(menu + dillerConsole + DillerCards(handDiller) + "\nЕго счёт: " + ScoreBy(handDiller, cards) +
                                " а твой: " + ScoreBy(handPlayer, cards) + playerConsole + "\nТы победил!");
                            Vait();
                            score[0] += 1;
                            break;
                        }
                    }
                }
            }
        }

        static void Vait()
        {
            Console.ReadKey();
            Console.Clear();
        }

        static string DillerCards(string[] hand)
        {
            string inHand = "";
            foreach (string card in hand)
            {
                if (card != null)
                {
                    inHand += card + " ";
                }
                else
                {
                    break;
                }
            }
            return inHand;
        }

        static void GetCard(ref string[] handDiller, string[,] cards, ref string[] indexCardInGame)
        {
            int r = RandInt();
            if (indexCardInGame.Contains(r.ToString()))
            {
                while (indexCardInGame.Contains(r.ToString()))
                {
                    r = RandInt();
                }
            }
            if (!indexCardInGame.Contains(r.ToString()))
            {
                for (int j = 0; j < indexCardInGame.Length; j++)
                {
                    if (indexCardInGame[j] == null || indexCardInGame[j] == "")
                    {
                        indexCardInGame[j] = r.ToString();
                        break;
                    }
                }
                for (int j = 0; j < handDiller.Length; j++)
                {
                    if (handDiller[j] == null || handDiller[j] == "")
                    {
                        handDiller[j] = cards[r, 0];
                        break;
                    }
                }
            }
        }

        static int ScoreBy(string[] hand, string[,] cards)
        {
            int score = 0;
            for (int i = 0; i < 52; i++)
            {
                foreach (string card in hand)
                {
                    if (cards[i, 0] == card)
                    {
                        score += Convert.ToInt32(cards[i, 1]);
                    }
                }
            }
            return score;
        }

        static void First(string[,] cards, ref string[] handPlayer, ref string[] handDiller, ref string[] indexCardInGame)
        {
            for (int i = 0; i < 4; i++)
            {
                int r = RandInt();
                if (indexCardInGame.Contains(r.ToString()))
                {
                    while (indexCardInGame.Contains(r.ToString()))
                    {
                        r = RandInt();
                    }
                }
                if (!indexCardInGame.Contains(r.ToString()))
                {
                    for (int  j = 0; j < indexCardInGame.Length; j++)
                    {
                        if (indexCardInGame[j] == null || indexCardInGame[j] == "")
                        {
                            indexCardInGame[j] = r.ToString();
                            break;
                        }
                    }
                    if (i < 2)
                    {
                        for (int j = 0; j < handDiller.Length; j++)
                        {
                            if (handDiller[j] == null || handDiller[j] == "")
                            {
                                handDiller[j] = cards[r, 0];
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < handDiller.Length; j++)
                        {
                            if (handPlayer[j] == null || handPlayer[j] == "")
                            {
                                handPlayer[j] = cards[r, 0];
                                break;
                            }
                        }
                    }
                }
            }
        }

        static int RandInt()
        {
            Random rnd = new Random();
            int randomInt = rnd.Next(52);
            return randomInt;
        }
    }
}
