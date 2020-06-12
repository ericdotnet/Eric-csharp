﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FftSharp
{
    public static class Pad
    {
        /// <summary>
        /// Test if a number is an even power of 2
        /// </summary>
        public static bool IsPowerOfTwo(int x) => ((x & (x - 1)) == 0) && (x > 0);

        /// <summary>
        /// Return the input array (or a new zero-padded new one) ensuring length is a power of 2
        /// </summary>
        /// <param name="input">array of any length</param>
        /// <returns>the input array or a zero-padded copy</returns>
        public static Complex[] ZeroPad(Complex[] input)
        {
            if (IsPowerOfTwo(input.Length))
                return input;

            int targetLength = 1;
            while (targetLength < input.Length)
                targetLength *= 2;

            int difference = targetLength - input.Length;
            Complex[] padded = new Complex[targetLength];
            Array.Copy(input, 0, padded, difference / 2, input.Length);

            return padded;
        }

        /// <summary>
        /// Return the input array (or a new zero-padded new one) ensuring length is a power of 2
        /// </summary>
        /// <param name="input">array of any length</param>
        /// <returns>the input array or a zero-padded copy</returns>
        public static double[] ZeroPad(double[] input)
        {
            if (IsPowerOfTwo(input.Length))
                return input;

            int targetLength = 1;
            while (targetLength < input.Length)
                targetLength *= 2;

            int difference = targetLength - input.Length;
            double[] padded = new double[targetLength];
            Array.Copy(input, 0, padded, difference / 2, input.Length);

            return padded;
        }

        /// <summary>
        /// Return the input array zero-padded to reach a final length
        /// </summary>
        /// <param name="input">array of any length</param>
        /// <returns>a zero-padded copy of the input array</returns>
        public static Complex[] ZeroPad(Complex[] input, int finalLength)
        {
            int difference = finalLength - input.Length;
            Complex[] padded = new Complex[finalLength];
            Array.Copy(input, 0, padded, difference / 2, input.Length);
            return padded;
        }

        /// <summary>
        /// Return the input array zero-padded to reach a final length
        /// </summary>
        /// <param name="input">array of any length</param>
        /// <returns>a zero-padded copy of the input array</returns>
        public static double[] ZeroPad(double[] input, int finalLength)
        {
            int difference = finalLength - input.Length;
            double[] padded = new double[finalLength];
            Array.Copy(input, 0, padded, difference / 2, input.Length);
            return padded;
        }
    }
}
