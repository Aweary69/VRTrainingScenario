using System.Collections.Generic;
using Project.Core;
using Project.Scenario.Data;

namespace Project.Scenario
{
    /// <summary>
    /// Runtime-индекс группы шагов. Строится один раз при активации группы
    /// (не хранится в ScriptableObject-данных — см. обсуждение архитектуры:
    /// данные должны оставаться чистыми, без runtime-кэша).
    ///
    /// Отвечает на два вопроса:
    /// 1. К какому (шаг, действие) относится входящий ActionKey?
    /// 2. Является ли входящий ActionKey заведомым дистрактором группы?
    /// </summary>
    public class GroupRuntimeIndex
    {
        private readonly Dictionary<ActionKey, ActionLocation> _expectedActions = new Dictionary<ActionKey, ActionLocation>();
        private readonly HashSet<ActionKey> _distractors = new HashSet<ActionKey>();

        public GroupRuntimeIndex(StepGroupData groupData)
        {
            for (int stepIndex = 0; stepIndex < groupData.Steps.Length; stepIndex++)
            {
                var actions = groupData.Steps[stepIndex].Actions;
                for (int actionIndex = 0; actionIndex < actions.Length; actionIndex++)
                {
                    var key = actions[actionIndex].ToKey();
                    _expectedActions[key] = new ActionLocation(stepIndex, actionIndex);
                }
            }

            if (groupData.KnownDistractors != null)
            {
                foreach (var distractor in groupData.KnownDistractors)
                {
                    _distractors.Add(distractor.ToKey());
                }
            }
        }

        /// <summary>Пытается найти, к какому шагу/действию относится ключ. Null, если это не ожидаемое действие группы.</summary>
        public ActionLocation? Resolve(ActionKey key)
        {
            return _expectedActions.TryGetValue(key, out var location) ? location : (ActionLocation?)null;
        }

        /// <summary>Является ли ключ заведомым дистрактором (намеренно неправильным объектом группы).</summary>
        public bool IsKnownDistractor(ActionKey key) => _distractors.Contains(key);
    }
}
