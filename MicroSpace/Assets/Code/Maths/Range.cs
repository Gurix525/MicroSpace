using System;

namespace Maths
{
    public struct Range : IEquatable<Range>
    {
        private float _start;
        private float _end;

        public float Start => _start;
        public float End => _end;

        public float Length => _end - _start;

        public Range(float start, float end)
        {
            if (start < end)
            {
                _start = start;
                _end = end;
            }
            else
            {
                _start = end;
                _end = start;
            }
        }

        public override string ToString()
        {
            return $"{_start}:{_end}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_start, _end);
        }

        public override bool Equals(object obj)
        {
            if (obj is not Range)
                return false;
            return Equals((Range)obj);
        }

        public bool Equals(Range other)
        {
            return GetHashCode() == other.GetHashCode();
        }

        public bool IsCovered(Range other)
        {
            if (_start > other.Start && _end < other.End)
                return true;
            return false;
        }

        public bool IsOverlapping(Range other)
        {
            if ((_start <= other.Start && _end >= other.Start)
                || (_end >= other.End && _start <= other.End))
                return true;
            return false;
        }

        public bool IsOverlapping(Range other, out Range combinedRange)
        {
            if (IsOverlapping(other))
            {
                combinedRange = new(
                    _start < other.Start ? _start : other.Start,
                    _end > other.End ? _end : other.End);
                return true;
            }
            combinedRange = this;
            return false;
        }
    }
}