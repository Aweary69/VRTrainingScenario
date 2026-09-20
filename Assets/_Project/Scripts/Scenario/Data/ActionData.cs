using System;
using Project.Core;
using UnityEngine;

namespace Project.Scenario.Data
{
    /// <summary>
    /// Одно ожидаемое действие внутри шага. Например: "MoveToPoint -> Zone_Documents".
    /// Обычный сериализуемый класс (не ScriptableObject), т.к. живёт как элемент
    /// массива внутри StepData и не нуждается в собственном asset-файле.
    /// </summary>
    [Serializable]
    public class ActionData
    {
        [SerializeField] private ActionType actionType;
        [SerializeField] private string targetId;

        public ActionType ActionType => actionType;
        public string TargetId => targetId;

        public ActionKey ToKey() => new ActionKey(actionType, targetId);
    }
}
