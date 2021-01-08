using System;

namespace Backend.Data
{
    /// <summary>
    /// A class representing a complex number, with simple operations
    /// </summary>
    public class Complex
    {
        /// <summary>
        /// The real component of the complex number
        /// </summary>
        public double Real;

        /// <summary>
        /// The imaginary component of the complex number
        /// </summary>
        public double Imaginary;

        /// <summary>
        /// Constructs a complex number with blank values
        /// </summary>
        public Complex()
        {
            Real = 0;
            Imaginary = 0;
        }

        /// <summary>
        /// Constructs a complex number using euler's formula (e^ix = cos(x) + isin(x))
        /// </summary>
        /// <param name="angle">Angle to use (x in above formula)</param>
        public Complex(double angle)
        {
            Real = Math.Cos(angle);
            Imaginary = Math.Sin(angle);
        }

        /// <summary>
        /// Returns the magnitude of the complex number
        /// </summary>
        public double Magnitude
        {
            get => Math.Sqrt(Math.Pow(Real, 2) + Math.Pow(Imaginary, 2));
        }

        /// <summary>
        /// Adds two complex numbers together
        /// </summary>
        /// <param name="one">First complex number</param>
        /// <param name="two">Second complex number</param>
        /// <returns><paramref name="one"/> + <paramref name="two"/></returns>
        public static Complex operator +(Complex one, Complex two) =>
            new Complex()
            {
                Real = one.Real + two.Real,
                Imaginary = one.Imaginary + two.Imaginary
            };


        /// <summary>
        /// Multiplies a complex number by a double
        /// </summary>
        /// <param name="one">Double</param>
        /// <param name="two">Complex number</param>
        /// <returns><paramref name="one"/> * <paramref name="two"/></returns>
        public static Complex operator *(double one, Complex two) =>
            new Complex()
            {
                Real = one * two.Real,
                Imaginary = one * two.Imaginary
            };
    }
}
