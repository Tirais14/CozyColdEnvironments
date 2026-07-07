#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs.Services
{
    public class Service<T>
    {
        private T? value;

#if CC_DEBUG_ENABLED
        private object? resolvedID;
#endif

        public bool IsResolved { get; private set; }

        public Type Contract { get; }

        public Service()
        {
            Contract = typeof(T);
        }

        public Service(Type contract)
        {
            Contract = contract;
        }

        public T GetValue(object? id = null)
        {
            if (!IsResolved)
            {
                value = (T)CCServices.Resolve(Contract, id)!;
                IsResolved = true;

#if CC_DEBUG_ENABLED
                resolvedID = id;
#endif
            }
#if CC_DEBUG_ENABLED
            else ValidateID(id);
#endif

            return value!;
        }

        public bool TryGetValue([NotNullWhen(true)] out T? result, object? id = null)
        {
            if (!IsResolved)
            {
                if (CCServices.TryResolveOut(Contract, out var tResult, id))
                {
                    value = (T)tResult;
                    IsResolved = true;

#if CC_DEBUG_ENABLED
                    resolvedID = id;
#endif
                }
            }
#if CC_DEBUG_ENABLED
            else ValidateID(id);
#endif

            result = value;
            return IsResolved;
        }

#if CC_DEBUG_ENABLED
        private void ValidateID(object? otherID)
        {
            if (otherID.IsNotNull() && !EqualityComparer<object?>.Default.Equals(resolvedID, otherID))
                this.PrintWarning("Static service ignores other id after resolve. It must be null or the same id");
        }
#endif
    }

//    public class Service<T, TResolve>
//        where T : TResolve
//    {
//        private T? value;

//#if CC_DEBUG_ENABLED
//        private object? resolvedID;
//#endif

//        public bool IsResolved { get; private set; }

//        public T GetValue(object? id = null)
//        {
//            if (!IsResolved)
//            {
//                value = (T)CCServices.Resolve<TResolve>(id)!;
//                IsResolved = true;

//#if CC_DEBUG_ENABLED
//                resolvedID = id;
//#endif
//            }
//#if CC_DEBUG_ENABLED
//            else ValidateID(id);
//#endif

//            return value!;
//        }

//        public bool TryGetValue([NotNullWhen(true)] out T? result, object? id = null)
//        {
//            if (!IsResolved)
//            {
//                if (CCServices.TryResolveOut<TResolve>(out var tResult, id))
//                {
//                    value = (T)tResult;
//                    IsResolved = true;

//#if CC_DEBUG_ENABLED
//                    resolvedID = id;
//#endif
//                }
//            }
//#if CC_DEBUG_ENABLED
//            else ValidateID(id);    
//#endif

//            result = value;
//            return IsResolved;
//        }

//#if CC_DEBUG_ENABLED
//        private void ValidateID(object? otherID)
//        {
//            if (!EqualityComparer<object?>.Default.Equals(resolvedID, otherID))
//                typeof(GlobalService<T>).PrintWarning("Static service ignores other id after resolve. It must be null or the same id");
//        }
//#endif
//    }
}
