﻿using System;
using System.Numerics;

namespace FftSharp
{
    public static class Transform
    {
        /// <summary>
        /// Test if a number is an even power of 2
        /// </summary>
        public static bool IsPowerOfTwo(int x)
        {
            return ((x & (x - 1)) == 0) && (x > 0);
        }

        /// <summary>
        /// Return the input array ensuring length is a power of 2 (zero-padding if needed)
        /// </summary>
        /// <param name="input">complex array of any length</param>
        /// <returns>the original array or a padded version</returns>
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
        /// Create an array of Complex data given the real component
        /// </summary>
        private static Complex[] Complex(double[] real)
        {
            Complex[] com = new Complex[real.Length];
            for (int i = 0; i < real.Length; i++)
                com[i] = new Complex(real[i], 0);
            return com;
        }

        /// <summary>
        /// Compute the 1D discrete Fourier Transform using the Fast Fourier Transform (FFT) algorithm
        /// </summary>
        /// <param name="input">real input</param>
        /// <returns>transformed input</returns>
        public static Complex[] FFT(double[] input)
        {
            return FFT(Complex(input));
        }

        /// <summary>
        /// Compute the 1D discrete Fourier Transform using the Fast Fourier Transform (FFT) algorithm
        /// </summary>
        /// <param name="input">complex input</param>
        /// <returns>transformed input</returns>
        public static Complex[] FFT(Complex[] input, bool checkLength = true)
        {
            if (checkLength)
                if (IsPowerOfTwo(input.Length) == false)
                    throw new ArgumentException($"FFT input length ({input.Length}) must be an even power of two. Use the ZeroPad method to achieve this.");

            int N = input.Length;
            Complex[] output = new Complex[N];
            Complex[] d, D, e, E;

            if (N == 1)
            {
                output[0] = input[0];
                return output;
            }

            int k;
            e = new Complex[N / 2];
            d = new Complex[N / 2];

            for (k = 0; k < N / 2; k++)
            {
                e[k] = input[2 * k];
                d[k] = input[2 * k + 1];
            }

            D = FFT(d, false);
            E = FFT(e, false);

            for (k = 0; k < N / 2; k++)
            {
                double radians = -2 * Math.PI * k / N;
                D[k] *= new Complex(Math.Cos(radians), Math.Sin(radians));
            }

            for (k = 0; k < N / 2; k++)
            {
                output[k] = E[k] + D[k];
                output[k + N / 2] = E[k] - D[k];
            }

            return output;
        }

        /// <summary>
        /// Compute the 1D discrete Fourier Transform (not using the fast FFT algorithm)
        /// </summary>
        /// <param name="input">real input</param>
        /// <returns>transformed input</returns>
        public static Complex[] DFT(double[] input)
        {
            return DFT(Complex(input));
        }

        /// <summary>
        /// Compute the 1D discrete Fourier Transform (not using the fast FFT algorithm)
        /// </summary>
        /// <param name="real">complex input</param>
        /// <returns>transformed input</returns>
        public static Complex[] DFT(Complex[] input, bool inverse = false)
        {
            int N = input.Length;
            double mult1 = (inverse) ? 2 * Math.PI / N : -2 * Math.PI / N;
            double mult2 = (inverse) ? 1.0 / N : 1.0;
            Console.WriteLine($"REAL {mult1} {mult2}");

            Complex[] output = new Complex[N];
            for (int k = 0; k < N; k++)
            {
                output[k] = new Complex(0, 0);
                for (int n = 0; n < N; n++)
                {
                    double radians = n * k * mult1;
                    Complex temp = new Complex(Math.Cos(radians), Math.Sin(radians));
                    temp *= input[n];
                    output[k] += temp * mult2;
                }
            }

            return output;
        }

        /// <summary>
        /// Calculte FFT and return the power
        /// </summary>
        /// <param name="input">real input</param>
        /// <param name="singleSided">combine positive and negative power (useful when symmetrical)</param>
        /// <returns>Power (dB)</returns>
        public static double[] FFTpower(double[] input, bool singleSided = true, bool decibels = true)
        {
            // first calculate the FFT
            Complex[] fft = FFT(Complex(input));

            // create an array of the complex magnitudes
            double[] output;
            if (singleSided)
            {
                output = new double[fft.Length / 2];

                // double to account for negative power
                for (int i = 0; i < output.Length; i++)
                    output[i] = fft[i].Magnitude * 2;

                // first point (DC component) is not doubled
                output[0] = fft[0].Magnitude;
            }
            else
            {
                output = new double[fft.Length];
                for (int i = 0; i < output.Length; i++)
                    output[i] = fft[i].Magnitude;
            }

            // convert to dB (the 2 comes from the conversion from RMS)
            if (decibels)
                for (int i = 0; i < output.Length; i++)
                    output[i] = 2 * 10 * Math.Log10(output[i]);

            return output;
        }

        /// <summary>
        /// Return frequencies for each point in an FFT
        /// </summary>
        public static double[] FFTfreq(int sampleRate, int pointCount, bool oneSided = true)
        {
            double[] freqs = new double[pointCount];

            if (oneSided)
            {
                double fftPeriodHz = (double)sampleRate / pointCount / 2;

                // freqs start at 0 and approach maxFreq
                for (int i = 0; i < pointCount; i++)
                    freqs[i] = i * fftPeriodHz;
                return freqs;
            }
            else
            {
                double fftPeriodHz = (double)sampleRate / pointCount;

                // first half: freqs start a 0 and approach maxFreq
                int halfIndex = pointCount / 2;
                for (int i = 0; i < halfIndex; i++)
                    freqs[i] = i * fftPeriodHz;

                // second half: then start at -maxFreq and approach 0
                for (int i = halfIndex; i < pointCount; i++)
                    freqs[i] = -(pointCount - i) * fftPeriodHz;
                return freqs;
            }
        }
    }
}
