namespace convert2TraceSM
{
    internal class AppArgumentsManager(string[] passedArgs)
    {
        private const string hyphen = "-";
        private readonly string[] _args = passedArgs;

        public bool DoesOptionExistInArgumentList(string option)
        {
            bool result = false;
            if (String.IsNullOrEmpty(option))
            {
                return result;
            }

            if (_args == null)
            {
                return result;
            }

            int argsLength = _args.Length;
            for (int index = 0; index < argsLength; index++)
            {
                string arg = _args[index];
                if (arg == option)
                {
                    result = true;
                }
            }
            return result;
        }


        public string ExtractOptionWithParameter(string option)
        {
            string parameterValue = string.Empty;

            int argsLength = _args.Length;
            for (int index = 0; index < argsLength; index++)
            {
                string arg = _args[index];
                if (arg == option)
                {
                    if ((index + 1) < argsLength)
                    {
                        parameterValue = _args[index + 1];
                        if (String.IsNullOrEmpty(parameterValue) || parameterValue.StartsWith(hyphen))
                        {
                            parameterValue = string.Empty;
                        }
                    }
                }
            }
            return parameterValue;
        }
    }
}
