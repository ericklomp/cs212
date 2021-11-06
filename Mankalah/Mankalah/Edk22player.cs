using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;

namespace Mankalah
{
    public class Edk22player : Player
    {
        Position us;
        public int timeLimit;

        int firstmove = 0;
        int lastmove = 0;
        int p2firstmove = 0;
        int p2lastmove = 0;

        public Edk22player(Position pos, int settimeLimit) : base(pos, "Edk22player", settimeLimit)
        {
            us = pos;
            timeLimit = settimeLimit;

            if (us == Position.Top)
            {
                firstmove = 7;
                lastmove = 12;
                p2firstmove = 0;
                p2lastmove = 5;
            }
            else
            {
                firstmove = 0;
                lastmove = 5;
                p2firstmove = 7;
                p2lastmove = 12;
            }
        }

        public override string gloat()
        {
            return "I WIN LOL";
        }

        public override int chooseMove(Board b)
        {
            Stopwatch t = new Stopwatch();
            t.Start();
            int i = 3;
            Result move = new Result(firstmove, -1);
            while(t.ElapsedMilliseconds < getTimePerMove())
            {
                move = minimaxVal(b, i, Int32.MinValue, Int32.MaxValue);
                i++;
            }
            return move.getmove();
        }

        private Result minimaxVal(Board b, int depth, int alpha, int beta)
        {
            int bestVal = 0;
            int bestMove = firstmove;

            if (b.gameOver() || depth == 0)
            {
                return new Result(0, evaluate(b));
            }
            if(b.whoseMove() == Position.Top)
            {
                bestVal = Int32.MinValue;
                for(int move = 7; move <= 12 && alpha < beta; move++)
                {
                    if (b.legalMove(move))
                    {
                        Board b1 = new Board(b);
                        b1.makeMove(move, false);
                        Result val = minimaxVal(b1, depth - 1, alpha, beta);
                        if (val.getscore() > bestVal)
                        {
                            bestVal = val.getscore();
                            bestMove = move;
                        }
                        if (bestVal > alpha) { alpha = bestVal; }
                    }
                }
                return new Result(bestMove, bestVal);
            }
            else
            {
                bestVal = int.MaxValue;
                for (int move = 0; move <= 5 && alpha < beta; move++)
                {
                    if (b.legalMove(move))
                    {
                        Board b1 = new Board(b);
                        b1.makeMove(move, false);
                        Result val = minimaxVal(b1, depth - 1, alpha, beta);
                        if (val.getscore() < bestVal)
                        {
                            bestVal = val.getscore();
                            bestMove = move;
                        }
                        if (bestVal < beta) { beta = bestVal; }
                    }
                }
                
            }
            return new Result(bestMove, bestVal);
        }

        public override int evaluate(Board b)
        {
            int score = b.stonesAt(13) - b.stonesAt(6);

            int totalStones = 0;
            int goAgainTotal = 0;
            int capturesTotal = 0;

            if (b.whoseMove() == Position.Top)
            {
                for (int i = firstmove; i <= 12; i++)
                {
                    totalStones += b.stonesAt(i);
                    if (b.stonesAt(i) - (13 - i) == 0)
                        goAgainTotal += 1;
                    int target = i + b.stonesAt(i);
                    if (target < 13)
                    {
                        int tStones = b.stonesAt(target);
                        if (b.whoseMove() == us)
                        {
                            if (tStones == 0 && b.stonesAt(13 - target - 1) != 0)
                                capturesTotal += b.stonesAt(13 - target - 1);
                        }
                    }
                }
            }
            else
            {
                for(int i = p2firstmove; i<= p2lastmove; i++)
                {
                    totalStones -= b.stonesAt(i);
                    if (b.stonesAt(i) - (6 - i) == 0)
                    {
                        goAgainTotal -= 1;
                    }
                    int target = i + b.stonesAt(i);
                    if (target < 6)
                    {
                        int tStones = b.stonesAt(target);
                        if (b.whoseMove() == Position.Bottom)
                        {
                            if (tStones == 0 && b.stonesAt(13 - target - 1) != 0)
                                capturesTotal -= b.stonesAt(13 - target - 1);
                        }
                    }
                }
            }
            capturesTotal = (b.whoseMove() == Position.Top) ? capturesTotal : -1 * capturesTotal;
            totalStones = (b.whoseMove() == Position.Top) ? totalStones : -1 * totalStones;
            goAgainTotal = (b.whoseMove() == Position.Top) ? goAgainTotal : -1 * goAgainTotal;


            score += totalStones + capturesTotal + goAgainTotal;
            return score;
        }
        public override String getImage() { return "file.jpg"; }
    }

    class Result
    {
        private int bestmove;
        private int bestscore;

        public Result(int move, int score)
        {
            bestmove = move;
            bestscore = score;
        }

        public int getmove() { return bestmove; }
        public int getscore() { return bestscore; }
        public void makemove(int move) { bestmove = move; }
        public void setscore(int score) { bestscore = score; }
    }
    
}
