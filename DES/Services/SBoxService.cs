﻿using System;
using System.Collections.Generic;
using System.Linq;
using DES.Interfaces;
using DES.Util;

namespace DES.Services
{
    internal class SBoxService : ISBoxService
    {
        private readonly int[,] sBox1 =
        {
            {14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7},
            {0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8},
            {4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0},
            {15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13}
        };

        private readonly int[,] sBox2 =
        {
            {15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10},
            {3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5},
            {0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15},
            {13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9}
        };

        private readonly int[,] sBox3 =
        {
            {10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8},
            {13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1},
            {13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7},
            {1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12}
        };

        private readonly int[,] sBox4 =
        {
            {7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15},
            {13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9},
            {10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4},
            {3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14}
        };

        private readonly int[,] sBox5 =
        {
            {2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9},
            {14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6},
            {4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14},
            {11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3}
        };

        private readonly int[,] sBox6 =
        {
            {12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11},
            {10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8},
            {9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6},
            {4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13}
        };

        private readonly int[,] sBox7 =
        {
            {4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1},
            {13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6},
            {1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2},
            {6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12}
        };

        private readonly int[,] sBox8 =
        {
            {13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7},
            {1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2},
            {7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8},
            {2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11}
        };

        private readonly Dictionary<int, int[,]> sBoxTable;

        public SBoxService()
        {
            sBoxTable = new Dictionary<int, int[,]>
            {
                {0, sBox1},
                {1, sBox2},
                {2, sBox3},
                {3, sBox4},
                {4, sBox5},
                {5, sBox6},
                {6, sBox7},
                {7, sBox8},
            };
        }

        public List<bool> ReplaceWithSBoxes(IList<bool> bits48)
        {
            if (bits48.Count != Constants.EXPANDED_SEMIBLOCK)
            {
                throw new Exception("bits length = " + bits48.Count);
            }

            int vectorLength = 6;
            int isTrue = 1;

            List<string> bits6StrList = new List<string>();
            for (int i = 0; i < bits48.Count; i += vectorLength)
            {
                bits6StrList.Add(string.Concat(bits48.Skip(i).Take(vectorLength).Select(x => x ? "1" : "0")));
            }

            List<bool> fullBoxedList = new List<bool>();
            for (int i = 0; i < bits6StrList.Count; i++)
            {
                string bits6Str = bits6StrList[i];
                IEnumerable<bool> sBoxValueList = GetSBoxValue(bits6Str, i).Select(x => x == isTrue);
                fullBoxedList.AddRange(sBoxValueList);
            }

            return fullBoxedList;
        }

        private List<int> GetSBoxValue(string bits6Str, int boxIndex)
        {
            int scaleOfNotation = 2;
            int bitBlockSize = 4;

            string mStr = string.Empty + bits6Str[0] + bits6Str[bits6Str.Length - 1];
            string lStr = string.Empty + bits6Str[1] + bits6Str[2] + bits6Str[3] + bits6Str[4];

            int m = Convert.ToInt32(mStr, scaleOfNotation);
            int l = Convert.ToInt32(lStr, scaleOfNotation);

            int sBoxValue = sBoxTable[boxIndex][m, l];

            string shortBinaryValue = Convert.ToString(sBoxValue, scaleOfNotation);
            string fullBinaryValue = new string('0', bitBlockSize - shortBinaryValue.Length) + shortBinaryValue;
            return fullBinaryValue.Select(x => int.Parse(x.ToString())).ToList();
        }
    }
}