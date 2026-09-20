using System.Collections.Generic;
using Project.Core;
using Project.Scenario.Data;
using UnityEngine;

namespace Project.Scenario
{
    /// <summary>
    /// Верхнеуровневая стейт-машина сценария тренировки.
    ///
    /// Отвечает за:
    ///  - хранение текущей позиции прохождения (группа/шаг/действие внутри шага);
    ///  - построение GroupRuntimeIndex при входе в новую группу;
    ///  - применение таблицы решений StepValidator к каждому входящему действию;
    ///  - оповещение остальной системы (UI, Feedback) через GameEvents.
    ///
    /// Контроллер намеренно не содержит логику звука/подсветки/UI — только оркестрацию
    /// переходов состояний (SRP). Скрипт не использует DI: единственная связь с внешним
    /// миром — подписка на статическую событийную шину GameEvents.
    /// </summary>
    public class ScenarioController : MonoBehaviour
    {
        [SerializeField] private ScenarioData scenarioData;

        private readonly List<GroupRuntimeState> _groups = new List<GroupRuntimeState>();
        private int _currentGroupIndex;

        private GroupRuntimeState CurrentGroup =>
            _currentGroupIndex < _groups.Count ? _groups[_currentGroupIndex] : null;

        private void OnEnable()
        {
            GameEvents.OnPlayerAction += HandlePlayerAction;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerAction -= HandlePlayerAction;
        }

        private void Start()
        {
            StartScenario();
        }

        /// <summary>Инициализирует прохождение с первой группы. Используется также для Restart.</summary>
        public void StartScenario()
        {
            _groups.Clear();
            for (int i = 0; i < scenarioData.Groups.Length; i++)
            {
                _groups.Add(new GroupRuntimeState(scenarioData.Groups[i], i));
            }

            _currentGroupIndex = 0;
            EnterCurrentGroup();
        }

        private void EnterCurrentGroup()
        {
            var group = CurrentGroup;
            if (group == null)
            {
                FinishScenario();
                return;
            }

            // GroupRuntimeIndex уже построен в конструкторе GroupRuntimeState —
            // здесь только оповещаем систему, что группа стала активной
            // (повод для UI показать подсказку по порядку действий).
            GameEvents.RaiseGroupChanged(group);
        }

        private void HandlePlayerAction(ActionType actionType, string targetId)
        {
            var group = CurrentGroup;
            if (group == null) return; // сценарий уже завершён — игнорируем поздние события

            var currentStep = group.CurrentStep;
            var key = new ActionKey(actionType, targetId);

            var result = StepValidator.Validate(key, group.Index, currentStep);

            switch (result)
            {
                case ValidationResult.Ignore:
                    // Намеренно ничего не делаем — случайный шум сцены не должен влиять на прогресс.
                    break;

                case ValidationResult.Success:
                    HandleSuccess(group, currentStep);
                    break;

                case ValidationResult.Mismatch:
                    HandleMismatch(group, currentStep);
                    break;

                case ValidationResult.SequenceBroken:
                    HandleSequenceBroken(group);
                    break;
            }
        }

        private void HandleSuccess(GroupRuntimeState group, StepRuntimeState step)
        {
            if (step.IsLastAction)
            {
                step.MarkSuccess();
                GameEvents.RaiseStepCompleted(step);
                AdvanceToNextStepOrGroup(group);
            }
            else
            {
                // Многодействийный шаг ещё не окончен — просто сдвигаем указатель
                // на следующее ожидаемое действие внутри того же шага.
                step.AdvanceAction();
            }
        }

        private void HandleMismatch(GroupRuntimeState group, StepRuntimeState step)
        {
            step.MarkFailed();
            GameEvents.RaiseStepCompleted(step);
            AdvanceToNextStepOrGroup(group);
        }

        private void HandleSequenceBroken(GroupRuntimeState group)
        {
            // Игрок перепрыгнул на действие будущего шага. Текущий шаг не был завершён,
            // поэтому по ТЗ он тоже получает статус "пропущенный", а не "с ошибкой".
            group.SkipRemainingSteps();

            foreach (var step in group.Steps)
            {
                if (step.Status == StepStatus.Skipped)
                    GameEvents.RaiseStepCompleted(step);
            }

            AdvanceToNextGroup();
        }

        private void AdvanceToNextStepOrGroup(GroupRuntimeState group)
        {
            if (group.CurrentStep == null)
            {
                AdvanceToNextGroup();
            }
        }

        private void AdvanceToNextGroup()
        {
            _currentGroupIndex++;
            EnterCurrentGroup();
        }

        private void FinishScenario()
        {
            GameEvents.RaiseScenarioFinished(_groups);
        }
    }
}
