using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Utilities
{
    public struct Vector3Area : IEquatable<Vector3Area>
    {
        public Vector3 from;
        public Vector3 to;

        public Vector3Area(Vector3 from, Vector3 to)
        {
            this.from = from;
            this.to = to;
        }

        public bool Contains(Vector3 position)
        {
            return from.x <= position.x && from.y <= position.y && from.z <= position.z
                && position.x <= to.x && position.y <= to.y && position.z <= to.z;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3Area area && Equals(area);
        }

        public bool Equals(Vector3Area other)
        {
            return from.Equals(other.from) &&
                   to.Equals(other.to);
        }

        public override int GetHashCode()
        {
            var hashCode = -1951484959;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(from);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(to);
            return hashCode;
        }

        public static bool operator ==(Vector3Area left, Vector3Area right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3Area left, Vector3Area right)
        {
            return !(left == right);
        }
    }

    public struct Vector2Area : IEquatable<Vector2Area>
    {
        public Vector2 from;
        public Vector2 to;

        public Vector2Area(Vector2 from, Vector2 to)
        {
            this.from = from;
            this.to = to;
        }

        public bool Contains(Vector2 position)
        {
            return from.x <= position.x && from.y <= position.y && position.x <= to.x && position.y <= to.y;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2Area area && Equals(area);
        }

        public bool Equals(Vector2Area other)
        {
            return from.Equals(other.from) &&
                   to.Equals(other.to);
        }

        public override int GetHashCode()
        {
            var hashCode = -1951484959;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(from);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(to);
            return hashCode;
        }

        public static bool operator ==(Vector2Area left, Vector2Area right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2Area left, Vector2Area right)
        {
            return !(left == right);
        }
    }
}
