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

        public double X;
        public double Y;

        private static readonly BigVector2 _zeroVector = new(0d, 0d);
        private static readonly BigVector2 _oneVector = new(1d, 1d);
        private static readonly BigVector2 _upVector = new(0d, 1d);
        private static readonly BigVector2 _downVector = new(0d, -1d);
        private static readonly BigVector2 _leftVector = new(-1d, 0d);
        private static readonly BigVector2 _rightVector = new(1d, 0d);

        private static readonly BigVector2 _positiveInfinityVector =
            new(double.PositiveInfinity, double.PositiveInfinity);

        private static readonly BigVector2 _negativeInfinityVector =
            new(double.NegativeInfinity, double.NegativeInfinity);

        #endregion Fields

        #region Properties

        public double this[int index]
        {
            get => index switch
            {
                0 => X,
                1 => Y,
                _ => throw new IndexOutOfRangeException("Invalid BigVector2 index!")
            };
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;

                    case 1:
                        Y = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException("Invalid Vector2 index!");
                }
            }
        }

        public BigVector2 Normalized
        {
            get
            {
                BigVector2 result = new BigVector2(X, Y);
                result.Normalize();
                return result;
            }
        }

        public double Magnitude =>
            (double)Math.Sqrt(X * X + Y * Y);

        public double sqrMagnitude =>
            X * X + Y * Y;

        public static BigVector2 Zero =>
            _zeroVector;

        public static BigVector2 One =>
            _oneVector;

        public static BigVector2 Up =>
            _upVector;

        public static BigVector2 Down =>
            _downVector;

        public static BigVector2 Left =>
            _leftVector;

        public static BigVector2 Right =>
            _rightVector;

        public static BigVector2 PositiveInfinity =>
            _positiveInfinityVector;

        public static BigVector2 NegativeInfinity =>
            _negativeInfinityVector;

        #endregion Properties

        #region Constructors

        public BigVector2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        #endregion Constructors

        #region Public methods

        public void Set(double newX, double newY)
        {
            X = newX;
            Y = newY;
        }

        public static BigVector2 Lerp(BigVector2 a, BigVector2 b, double t)
        {
            t = t < 0 ? 0 : t > 1 ? 1 : t;
            return new(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
        }

        public static BigVector2 LerpUnclamped(BigVector2 a, BigVector2 b, double t) =>
            new(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);

        public static BigVector2 MoveTowards(
            BigVector2 current,
            BigVector2 target,
            double maxDistanceDelta
            )
        {
            double num = target.X - current.X;
            double num2 = target.Y - current.Y;
            double num3 = num * num + num2 * num2;
            if (
                num3 == 0d ||
                (maxDistanceDelta >= 0d && num3 <= maxDistanceDelta * maxDistanceDelta)
                )
                return target;

            double num4 = (double)Math.Sqrt(num3);
            return new(
                current.X + num / num4 * maxDistanceDelta,
                current.Y + num2 / num4 * maxDistanceDelta
                );
        }

        public static BigVector2 Scale(BigVector2 a, BigVector2 b) =>
            new(a.X * b.X, a.Y * b.Y);

        public void Scale(BigVector2 scale)
        {
            X *= scale.X;
            Y *= scale.Y;
        }

        public void Normalize() =>
            this = Magnitude > 1E-50d ? this /= Magnitude : Zero;

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
                X.ToString(format, formatProvider),
                Y.ToString(format, formatProvider)
                );
        }

        public override int GetHashCode() =>
            X.GetHashCode() ^ (Y.GetHashCode() << 3);

        public override bool Equals(object other)
        {
            if (other is not BigVector2)
                return false;
            return Equals((BigVector2)other);
        }

        public bool Equals(BigVector2 other) =>
            X == other.X && Y == other.Y;

        public static BigVector2 Reflect(BigVector2 inDirection, BigVector2 inNormal)
        {
            double num = -2f * Dot(inNormal, inDirection);
            return new(
                num * inNormal.X + inDirection.X,
                num * inNormal.Y + inDirection.Y
                );
        }

        public static BigVector2 Perpendicular(BigVector2 inDirection) =>
            new BigVector2(0d - inDirection.Y, inDirection.X);

        public static double Dot(BigVector2 lhs, BigVector2 rhs) =>
            lhs.X * rhs.X + lhs.Y * rhs.Y;

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
            double num2 = Math.Sign(from.X * to.Y - from.Y * to.X);
            return num * num2;
        }

        public static double Distance(BigVector2 a, BigVector2 b)
        {
            double num = a.X - b.X;
            double num2 = a.Y - b.Y;
            return (double)Math.Sqrt(num * num + num2 * num2);
        }

        public static BigVector2 ClampMagnitude(BigVector2 vector, double maxLength)
        {
            double sqrMagnitude = vector.sqrMagnitude;
            if (sqrMagnitude > maxLength * maxLength)
            {
                double num = (double)Math.Sqrt(sqrMagnitude);
                double num2 = vector.X / num;
                double num3 = vector.Y / num;
                return new(num2 * maxLength, num3 * maxLength);
            }

            return vector;
        }

        public static double SqrMagnitude(BigVector2 a) =>
            a.X * a.X + a.Y * a.Y;

        public double SqrMagnitude() =>
            X * X + Y * Y;

        public static BigVector2 Min(BigVector2 lhs, BigVector2 rhs) =>
            new(Math.Min(lhs.X, rhs.X), Math.Min(lhs.Y, rhs.Y));

        public static BigVector2 Max(BigVector2 lhs, BigVector2 rhs) =>
            new(Math.Max(lhs.X, rhs.X), Math.Max(lhs.Y, rhs.Y));

        #endregion Public methods

        #region Operators

        public static BigVector2 operator +(BigVector2 a, BigVector2 b) =>
            new(a.X + b.X, a.Y + b.Y);

        public static BigVector2 operator -(BigVector2 a, BigVector2 b) =>
            new(a.X - b.X, a.Y - b.Y);

        public static BigVector2 operator *(BigVector2 a, BigVector2 b) =>
            new(a.X * b.X, a.Y * b.Y);

        public static BigVector2 operator /(BigVector2 a, BigVector2 b) =>
            new(a.X / b.X, a.Y / b.Y);

        public static BigVector2 operator -(BigVector2 a) =>
            new(-a.X, -a.Y);

        public static BigVector2 operator *(BigVector2 a, double d) =>
            new(a.X * d, a.Y * d);

        public static BigVector2 operator *(double d, BigVector2 a) =>
            a * d;

        public static BigVector2 operator /(BigVector2 a, double d) =>
            new(a.X / d, a.Y / d);

        public static bool operator ==(BigVector2 a, BigVector2 b) =>
            a.Equals(b);

        public static bool operator !=(BigVector2 a, BigVector2 b) =>
            !a.Equals(b);

        public static implicit operator BigVector2(Vector2 v) =>
            new(v.x, v.y);

        public static explicit operator BigVector2(Vector3 v) =>
            new(v.x, v.y);

        public static explicit operator Vector2(BigVector2 v) =>
            new((float)v.X, (float)v.Y);

        public static explicit operator Vector3(BigVector2 v) =>
            new((float)v.X, (float)v.Y);

        #endregion Operators
    }
}