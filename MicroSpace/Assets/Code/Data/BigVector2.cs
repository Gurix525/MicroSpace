using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Code.Data
{
    [Serializable]
    public struct BigVector2 : IEquatable<BigVector2>, IFormattable
    {
        #region Fields

        public double x;
        public double y;

        private static readonly BigVector2 zeroVector = new(0d, 0d);
        private static readonly BigVector2 oneVector = new(1d, 1d);
        private static readonly BigVector2 upVector = new(0d, 1d);
        private static readonly BigVector2 downVector = new(0d, -1d);
        private static readonly BigVector2 leftVector = new(-1d, 0d);
        private static readonly BigVector2 rightVector = new(1d, 0d);

        private static readonly BigVector2 positiveInfinityVector =
            new(double.PositiveInfinity, double.PositiveInfinity);

        private static readonly BigVector2 negativeInfinityVector =
            new(double.NegativeInfinity, double.NegativeInfinity);

        #endregion Fields

        #region Properties

        public double this[int index]
        {
            get => index switch
            {
                0 => x,
                1 => y,
                _ => throw new IndexOutOfRangeException("Invalid BigVector2 index!")
            };
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;

                    case 1:
                        y = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException("Invalid Vector2 index!");
                }
            }
        }

        public BigVector2 normalized
        {
            get
            {
                BigVector2 result = new BigVector2(x, y);
                result.Normalize();
                return result;
            }
        }

        public double magnitude =>
            (double)Math.Sqrt(x * x + y * y);

        public double sqrMagnitude =>
            x * x + y * y;

        public static BigVector2 zero =>
            zeroVector;

        public static BigVector2 one =>
            oneVector;

        public static BigVector2 up =>
            upVector;

        public static BigVector2 down =>
            downVector;

        public static BigVector2 left =>
            leftVector;

        public static BigVector2 right =>
            rightVector;

        public static BigVector2 positiveInfinity =>
            positiveInfinityVector;

        public static BigVector2 negativeInfinity =>
            negativeInfinityVector;

        #endregion Properties

        #region Constructors

        public BigVector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        #endregion Constructors

        #region Public methods

        public void Set(double newX, double newY)
        {
            x = newX;
            y = newY;
        }

        public static BigVector2 Lerp(BigVector2 a, BigVector2 b, double t)
        {
            t = t < 0 ? 0 : t > 1 ? 1 : t;
            return new(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }

        public static BigVector2 LerpUnclamped(BigVector2 a, BigVector2 b, double t) =>
            new(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);

        public static BigVector2 MoveTowards(
            BigVector2 current,
            BigVector2 target,
            double maxDistanceDelta
            )
        {
            double num = target.x - current.x;
            double num2 = target.y - current.y;
            double num3 = num * num + num2 * num2;
            if (
                num3 == 0d ||
                (maxDistanceDelta >= 0d && num3 <= maxDistanceDelta * maxDistanceDelta)
                )
                return target;

            double num4 = (double)Math.Sqrt(num3);
            return new(
                current.x + num / num4 * maxDistanceDelta,
                current.y + num2 / num4 * maxDistanceDelta
                );
        }

        public static BigVector2 Scale(BigVector2 a, BigVector2 b) =>
            new(a.x * b.x, a.y * b.y);

        public void Scale(BigVector2 scale)
        {
            x *= scale.x;
            y *= scale.y;
        }

        public void Normalize() =>
            this = magnitude > 1E-50d ? this /= magnitude : zero;

        public override string ToString() =>
            ToString(null, null);

        public string ToString(string format) =>
            ToString(format, null);

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "D2";

            if (formatProvider == null)
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;

            return string.Format(
                CultureInfo.InvariantCulture.NumberFormat,
                "({0}, {1})",
                x.ToString(format, formatProvider),
                y.ToString(format, formatProvider)
                );
        }

        public override int GetHashCode() =>
            x.GetHashCode() ^ (y.GetHashCode() << 3);

        public override bool Equals(object other)
        {
            if (other is not BigVector2)
                return false;
            return Equals((BigVector2)other);
        }

        public bool Equals(BigVector2 other) =>
            x == other.x && y == other.y;

        public static BigVector2 Reflect(BigVector2 inDirection, BigVector2 inNormal)
        {
            double num = -2f * Dot(inNormal, inDirection);
            return new(
                num * inNormal.x + inDirection.x,
                num * inNormal.y + inDirection.y
                );
        }

        public static BigVector2 Perpendicular(BigVector2 inDirection) =>
            new BigVector2(0d - inDirection.y, inDirection.x);

        public static double Dot(BigVector2 lhs, BigVector2 rhs) =>
            lhs.x * rhs.x + lhs.y * rhs.y;

        public static double Angle(BigVector2 from, BigVector2 to)
        {
            double num = (double)Math.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
            if (num < 1E-50d)
                return 0d;

            double num2 = Math.Clamp(Dot(from, to) / num, -1d, 1d);
            return (double)Math.Acos(num2) * 57.295779513082320876798154814105d;
        }

        public static double SignedAngle(BigVector2 from, BigVector2 to)
        {
            double num = Angle(from, to);
            double num2 = Math.Sign(from.x * to.y - from.y * to.x);
            return num * num2;
        }

        public static double Distance(BigVector2 a, BigVector2 b)
        {
            double num = a.x - b.x;
            double num2 = a.y - b.y;
            return (double)Math.Sqrt(num * num + num2 * num2);
        }

        public static BigVector2 ClampMagnitude(BigVector2 vector, double maxLength)
        {
            double sqrMagnitude = vector.sqrMagnitude;
            if (sqrMagnitude > maxLength * maxLength)
            {
                double num = (double)Math.Sqrt(sqrMagnitude);
                double num2 = vector.x / num;
                double num3 = vector.y / num;
                return new(num2 * maxLength, num3 * maxLength);
            }

            return vector;
        }

        public static double SqrMagnitude(BigVector2 a) =>
            a.x * a.x + a.y * a.y;

        public double SqrMagnitude() =>
            x * x + y * y;

        public static BigVector2 Min(BigVector2 lhs, BigVector2 rhs) =>
            new(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));

        public static BigVector2 Max(BigVector2 lhs, BigVector2 rhs) =>
            new(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));

        #endregion Public methods

        #region Operators

        public static BigVector2 operator +(BigVector2 a, BigVector2 b) =>
            new(a.x + b.x, a.y + b.y);

        public static BigVector2 operator -(BigVector2 a, BigVector2 b) =>
            new(a.x - b.x, a.y - b.y);

        public static BigVector2 operator *(BigVector2 a, BigVector2 b) =>
            new(a.x * b.x, a.y * b.y);

        public static BigVector2 operator /(BigVector2 a, BigVector2 b) =>
            new(a.x / b.x, a.y / b.y);

        public static BigVector2 operator -(BigVector2 a) =>
            new(-a.x, -a.y);

        public static BigVector2 operator *(BigVector2 a, double d) =>
            new(a.x * d, a.y * d);

        public static BigVector2 operator *(double d, BigVector2 a) =>
            a * d;

        public static BigVector2 operator /(BigVector2 a, double d) =>
            new(a.x / d, a.y / d);

        public static bool operator ==(BigVector2 a, BigVector2 b) =>
            a.Equals(b);

        public static bool operator !=(BigVector2 a, BigVector2 b) =>
            !a.Equals(b);

        public static implicit operator BigVector2(Vector2 v) =>
            new(v.x, v.y);

        public static explicit operator BigVector2(Vector3 v) =>
            new(v.x, v.y);

        public static explicit operator Vector2(BigVector2 v) =>
            new((float)v.x, (float)v.y);

        public static explicit operator Vector3(BigVector2 v) =>
            new((float)v.x, (float)v.y);

        #endregion Operators
    }
}