using System;

namespace Project.Core
{
    /// <summary>
    /// Композитный ключ действия: тип действия + идентификатор цели.
    /// Композитность важна, потому что один и тот же объект в сцене
    /// может участвовать в разных шагах с разными типами действий
    /// (например, сначала Click по объекту, а позже, в другом шаге, Grab того же объекта).
    /// Реализует IEquatable, чтобы корректно и быстро работать как ключ Dictionary/HashSet.
    /// </summary>
    public readonly struct ActionKey : IEquatable<ActionKey>
    {
        public readonly ActionType Type;
        public readonly string TargetId;

        public ActionKey(ActionType type, string targetId)
        {
            Type = type;
            TargetId = targetId;
        }

        public bool Equals(ActionKey other)
        {
            return Type == other.Type && string.Equals(TargetId, other.TargetId, StringComparison.Ordinal);
        }

        public override bool Equals(object obj) => obj is ActionKey other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + (int)Type;
                hash = hash * 31 + (TargetId != null ? TargetId.GetHashCode() : 0);
                return hash;
            }
        }

        public override string ToString() => $"{Type}:{TargetId}";
    }
}
