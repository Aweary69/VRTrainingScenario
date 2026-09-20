using System;
using Project.Scenario;

namespace Project.Core
{
    /// <summary>
    /// Центральная событийная шина проекта.
    ///
    /// Архитектурная роль: заменяет DI-контейнер (который запрещён по ТЗ) для связи
    /// между слоями системы. Правило простое — эмиттеры (Interactable-компоненты)
    /// ничего не знают о подписчиках (ScenarioController, FeedbackManager, UI).
    ///
    /// Все события статические намеренно: в проекте существует ровно один активный
    /// сценарий за раз (одна VR-сцена тренировки), поэтому синглтон-шина оправдана
    /// и не требует передачи ссылок через инспектор или конструкторы.
    ///
    /// ВАЖНО: подписчики обязаны отписываться в OnDisable/OnDestroy, иначе при
    /// перезагрузке сцены (Restart/Return to Lobby) возможны утечки и "призрачные"
    /// обработчики от предыдущей сессии.
    /// </summary>
    public static class GameEvents
    {
        /// <summary>
        /// Игрок выполнил низкоуровневое действие в мире (подошёл, взял, кликнул, нажал кнопку).
        /// Эмитируется Interactable-компонентами, слушается ScenarioController.
        /// </summary>
        public static event Action<ActionType, string> OnPlayerAction;

        /// <summary>Шаг завершён (успешно, с ошибкой или пропущен). Слушают UI и Feedback.</summary>
        public static event Action<StepRuntimeState> OnStepCompleted;

        /// <summary>Активна новая группа шагов — повод показать подсказку по порядку действий.</summary>
        public static event Action<GroupRuntimeState> OnGroupChanged;

        /// <summary>Сценарий полностью завершён — повод показать экран результатов.</summary>
        public static event Action<System.Collections.Generic.IReadOnlyList<GroupRuntimeState>> OnScenarioFinished;

        public static void RaisePlayerAction(ActionType type, string targetId)
            => OnPlayerAction?.Invoke(type, targetId);

        public static void RaiseStepCompleted(StepRuntimeState state)
            => OnStepCompleted?.Invoke(state);

        public static void RaiseGroupChanged(GroupRuntimeState state)
            => OnGroupChanged?.Invoke(state);

        public static void RaiseScenarioFinished(System.Collections.Generic.IReadOnlyList<GroupRuntimeState> groups)
            => OnScenarioFinished?.Invoke(groups);

        /// <summary>
        /// Полный сброс всех подписок. Вызывать при возврате в Лобби, чтобы гарантированно
        /// не тащить обработчики от предыдущего прохождения сценария в новую сессию.
        /// </summary>
        public static void ClearAllSubscriptions()
        {
            OnPlayerAction = null;
            OnStepCompleted = null;
            OnGroupChanged = null;
            OnScenarioFinished = null;
        }
    }
}
