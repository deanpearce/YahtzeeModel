using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.Modeling;

namespace YahtzeeModel
{

    /// <summary>
    /// The game scoring enumeration specified by the assignment.
    /// </summary>
    public enum ScoreType
    {
        One, Two, Three, Four, Five, Six, ThreeOfAKind, FourOfAKind, FullHouse,
        SmallStraight, LargeStraight, Chance, Yahtzee
    }

    /// <summary>
    /// Basic Yahtzee model to start a game, roll dice, hold dice
    /// and score the resulting throws
    /// </summary>
    /// 
    static class YahtzeeModelProgram
    {
        static int[] dice = new int[5];
        static bool[] dice_held = new bool[5];
        static bool[] filled_categories = new bool[13];

        static bool upper_bonus_applied = false;

        static int lowerScore;
        static int upperScore;
        static int currentRoll;

        [Rule]
        static void NewGame()
        {
            for (int i = 0; i < dice_held.Length; i++)
            {
                dice_held[i] = false;
            }

            for (int i = 0; i < filled_categories.Length; i++)
            {
                filled_categories[i] = false;
            }

            currentRoll = 0;
            upperScore = 0;
            lowerScore = 0;
        }

        [Rule]
        static void RollAll(int i1, int i2, int i3, int i4, int i5)
        {

            int[] incoming_dice = new int[5] { i1, i2, i3, i4, i5 };

            Condition.IsTrue(currentRoll < 3);

            bool can_roll = false;
            for (int i = 0; i < filled_categories.Length; i++)
            {
                if (!filled_categories[i]) can_roll = true;
            }

            Condition.IsTrue(can_roll);

            for (int i = 0; i < incoming_dice.Length; i++)
            {
                Condition.IsTrue(incoming_dice[i] >= 1 && incoming_dice[i] <= 6);
                if (dice_held[i])
                {
                    Condition.IsTrue(dice[i] == incoming_dice[i]);
                }
                else
                {
                    dice[i] = incoming_dice[i];
                }
            }

            currentRoll++;
        }

        [Rule]
        static void hold(int die)
        {
            Condition.IsTrue(currentRoll < 3);
            Condition.IsFalse(dice_held[die - 1]);
            dice_held[die - 1] = true;
        }

        [Rule]
        static int Score(ScoreType score) 
        {
            int Ones = 0;
            int Twos = 0;
            int Threes = 0;
            int Fours = 0;
            int Fives = 0;
            int Sixes = 0;

            for(int i = 0; i < dice.Length; i++)
            {
                if (dice[i] == 1) Ones++;
                if (dice[i] == 2) Twos++;
                if (dice[i] == 3) Threes++;
                if (dice[i] == 4) Fours++;
                if (dice[i] == 5) Fives++;
                if (dice[i] == 6) Sixes++;
            }

            switch (score)
            {
                case ScoreType.One:
                    Condition.IsFalse(filled_categories[0]);
                    filled_categories[0] = true;
                    upperScore += Ones;
                    break;
                case ScoreType.Two:
                    Condition.IsFalse(filled_categories[1]);
                    filled_categories[1] = true;
                    upperScore += Twos * 2;
                    break;
                case ScoreType.Three:
                    Condition.IsFalse(filled_categories[2]);
                    filled_categories[2] = true;
                    upperScore += Threes * 3;
                    break;
                case ScoreType.Four:
                    Condition.IsFalse(filled_categories[3]);
                    filled_categories[3] = true;
                    upperScore += Fours * 4;
                    break;
                case ScoreType.Five:
                    Condition.IsFalse(filled_categories[4]);
                    filled_categories[4] = true;
                    upperScore += Fives * 5;
                    break;
                case ScoreType.Six:
                    Condition.IsFalse(filled_categories[5]);
                    filled_categories[5] = true;
                    upperScore += Sixes * 6;
                    break;
                case ScoreType.ThreeOfAKind:
                    Condition.IsFalse(filled_categories[6]);
                    filled_categories[6] = true;
                    if((Ones >= 3) || (Twos >= 3) || (Threes >= 3) || (Fours >= 3) || (Fives >= 3) || (Sixes >= 3))
                        lowerScore += Ones + 2 * Twos + 3 * Threes + 4 * Fours + 5 * Fives + 6 * Sixes;                
                    break;
                case ScoreType.FourOfAKind:
                    Condition.IsFalse(filled_categories[7]);
                    filled_categories[7] = true;
                    if((Ones >= 4) || (Twos >= 4) || (Threes >= 4) || (Fours >= 4) || (Fives >= 4) || (Sixes >= 4))
                        lowerScore += Ones + 2 * Twos + 3 * Threes + 4 * Fours + 5 * Fives + 6 * Sixes;
                    break;
                case ScoreType.FullHouse:
                    Condition.IsFalse(filled_categories[8]);
                    filled_categories[8] = true;
                    if(((Ones == 3) || (Twos == 3) || (Threes == 3) || (Fours == 3) || (Fives == 3) || (Sixes == 3)) && ((Ones == 2) || (Twos == 2) || (Threes == 2) || (Fours == 2) || (Fives == 2) || (Sixes == 2)))
                        lowerScore += 25;
                    break;
                case ScoreType.SmallStraight:
                    Condition.IsFalse(filled_categories[9]);
                    filled_categories[9] = true;
                    if(((Ones >= 1) && (Twos >= 1) && (Threes >= 1) && (Fours >= 1)) || ((Twos >= 1) && (Threes >= 1) && (Fours >= 1) && (Fives >= 1)) || ((Threes >= 1) && (Fours >= 1) && (Fives >= 1) && (Sixes >= 1)))
                        lowerScore += 30;
                    break;
                case ScoreType.LargeStraight:
                    Condition.IsFalse(filled_categories[10]);
                    filled_categories[10] = true;
                    if(((Ones == 1) && (Twos == 1) && (Threes == 1) && (Fours == 1) && (Fives == 1)) || ((Twos == 1) && (Threes == 1) && (Fours == 1) && (Fives == 1) && (Sixes == 1)))
                        lowerScore += 40;
                    break;
                case ScoreType.Yahtzee:
                    Condition.IsFalse(filled_categories[11]);
                    filled_categories[11] = true;
                    if((Ones == 5) || (Twos == 5) || (Threes == 5) || (Fours == 5) || (Fives == 5) || (Sixes == 5))
                        lowerScore += 50;
                    break;
                case ScoreType.Chance:
                    Condition.IsFalse(filled_categories[12]);
                    filled_categories[12] = true;
                    lowerScore += Ones + 2 * Twos + 3 * Threes + 4 * Fours + 5 * Fives + 6 * Sixes;
                    break;
            }

            if (upperScore >= 63 && !upper_bonus_applied)
            {
                upperScore += 35;
                upper_bonus_applied = true;
            }

            for (int i = 0; i < dice_held.Length; i++)
            {
                dice_held[i] = false;
            }
            currentRoll = 0;
            return lowerScore + upperScore;
        }

    }
}
