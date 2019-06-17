namespace OracleProcedureManager
{
    public static class Settings
    {
        public static string isInterruptRequestedMessage = "Произошла отмена операции синхронизации";

        public static string ConstraintDisableProcName = "SYNC_CONSTRAINT_DISABLE";
        public static string ConstraintEnableProcName = "SYNC_CONSTRAINT_ENABLE";
        public static string IndexUnusableProcName = "SYNC_INDEX_UNUSABLE";
        public static string IndexRebuildProcName = "SYNC_INDEX_REBUILD";

    }
}