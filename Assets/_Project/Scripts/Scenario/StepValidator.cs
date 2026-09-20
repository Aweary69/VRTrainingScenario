using Project.Core;

namespace Project.Scenario
{
    /// <summary>
    /// Чистая логика сравнения входящего действия с текущим ожиданием группы.
    /// Не имеет побочных эффектов и не хранит состояние — только отвечает на вопрос
    /// "что это было?". Все переходы состояний (изменение статусов шагов, переход
    /// между группами) выполняет ScenarioController, а не валидатор — это разделение
    /// ответственности не даёт классу разрастись в God Object.
    /// </summary>
    public static class StepValidator
    {
        public static ValidationResult Validate(
            ActionKey incomingAction,
            GroupRuntimeIndex groupIndex,
            StepRuntimeState currentStep)
        {
            // Явный дистрактор группы — всегда ошибка, независимо от того, к какому
            // шагу он "привязан" физически в сцене.
            if (groupIndex.IsKnownDistractor(incomingAction))
                return ValidationResult.Mismatch;

            var location = groupIndex.Resolve(incomingAction);

            // Действие не встречается ни в шагах, ни в дистракторах текущей группы —
            // это случайный шум сцены (например, объект другой, ещё не активной группы,
            // или чисто декоративный интерактивный элемент).
            if (location == null)
                return ValidationResult.Ignore;

            var resolved = location.Value;

            // currentStep == null означает, что все шаги группы уже завершены —
            // в норме до этого события дойти не должно (группа уже была бы закрыта),
            // но на всякий случай трактуем как шум, а не как ошибку рантайма.
            if (currentStep == null)
                return ValidationResult.Ignore;

            if (resolved.StepIndex == currentStep.StepIndex)
            {
                // Тот же шаг: совпадает с текущим ожидаемым действием внутри шага?
                return resolved.ActionIndex == currentStep.CurrentActionIndex
                    ? ValidationResult.Success
                    : ValidationResult.Mismatch;
            }

            // Действие относится к другому шагу этой же группы.
            // Так как шаги линейны и мы уже проверили "текущий" шаг выше,
            // resolved.StepIndex тут всегда больше currentStep.StepIndex (шаг из будущего).
            return ValidationResult.SequenceBroken;
        }
    }
}
