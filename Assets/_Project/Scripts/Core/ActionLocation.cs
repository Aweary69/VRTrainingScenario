namespace Project.Core
{
    /// <summary>
    /// Указывает, к какому шагу и какому действию внутри шага относится ActionKey.
    /// Используется GroupRuntimeIndex как значение словаря для быстрого поиска:
    /// "это действие ожидается сейчас или относится к другому шагу?".
    /// </summary>
    public readonly struct ActionLocation
    {
        public readonly int StepIndex;
        public readonly int ActionIndex;

        public ActionLocation(int stepIndex, int actionIndex)
        {
            StepIndex = stepIndex;
            ActionIndex = actionIndex;
        }

        public override string ToString() => $"Step {StepIndex} / Action {ActionIndex}";
    }
}
