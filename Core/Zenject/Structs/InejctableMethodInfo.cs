using System.Reflection;
using UTIRLib.Extensions;

#nullable enable

namespace UTIRLib.Zenject
{
    internal readonly struct InejctableMethodInfo
    {
        private readonly ParameterInfo[] parameters;
        private readonly object[] dependencies;
        public readonly MethodInfo method;

        public readonly bool IsResolved {
            get {
                if (dependencies.IsNullOrEmpty())
                {
                    return false;
                }

                for (int i = 0; i < dependencies.Length; i++)
                {
                    if (dependencies[i] == null)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public InejctableMethodInfo(MethodInfo method)
        {
            parameters = method.GetParameters();
            dependencies = new object[parameters.Length];
            this.method = method;
        }

        public bool TryAddDependency(object obj)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType.IsInstanceOfType(obj))
                {
                    dependencies[i] = obj;
                    return true;
                }
            }

            return false;
        }

        public void AddDependency(object obj)
        {
            if (!TryAddDependency(obj))
            {
                throw new System.Exception("Incorrect dependency.");
            }
        }
    }
}