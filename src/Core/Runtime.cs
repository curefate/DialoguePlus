namespace DialoguePlus.Core
{
    public class Runtime
    {
        public readonly VariableRegistry Variables = new();
        public readonly FunctionRegistry Functions = new();
    }

    public class VariableRegistry
    {
        internal VariableRegistry() { }

        private readonly Dictionary<string, TypedVar> _globalScope = [];
        private readonly Stack<Dictionary<string, TypedVar>> _tempScopeStack = new([new Dictionary<string, TypedVar>()]);
        private Dictionary<string, TypedVar> _tempScope
        {
            get
            {
                if (_tempScopeStack.Count > 0)
                    return _tempScopeStack.Peek();
                var newScope = new Dictionary<string, TypedVar>();
                _tempScopeStack.Push(newScope);
                return newScope;
            }
        }

        public void Clear()
        {
            _globalScope.Clear();
            _tempScopeStack.Clear();
            NewTempScope();
        }

        public void NewTempScope()
        {
            _tempScopeStack.Push(new Dictionary<string, TypedVar>());
        }

        public void PopTempScope()
        {
            if (_tempScopeStack.Count > 1)
            {
                _tempScopeStack.Pop();
            }
            else
            {
                _tempScope.Clear();
            }
        }

        public void Set(string varName, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Value cannot be null.");
            }
            var type = value.GetType();
            if (type == typeof(TypedVar))
            {
                var typedValue = (TypedVar)value;
                type = typedValue.Type;
                value = typedValue.Value;
            }
            if (type != typeof(string) && type != typeof(int) && type != typeof(float) && type != typeof(bool))
            {
                throw new ArgumentException($"Unsupported type '{type.Name}' for variable '{varName}'. Supported types are string, int, float, and bool.", nameof(value));
            }
            var currentScope = varName.StartsWith("global.") ? _globalScope : _tempScope;
            if (currentScope.ContainsKey(varName))
            {
                // Check if the type matches the existing variable
                /* if (variables[varName].Type != type)
                {
                    throw new InvalidOperationException($"Variable '{varName}' already exists with type '{variables[varName].Type.Name}', cannot assign value of type '{type.Name}'. Supported types are string, int, float, and bool.");
                } */
                currentScope[varName] = new TypedVar(value, type);
            }
            else
            {
                currentScope.Add(varName, new TypedVar(value, type));
            }
        }

        public TypedVar Get(string varName)
        {
            var currentScope = varName.StartsWith("global.") ? _globalScope : _tempScope;
            if (currentScope.TryGetValue(varName, out var variable))
            {
                return variable;
            }
            throw new KeyNotFoundException($"Variable '{varName}' not found.");
        }
    }

    public class FunctionRegistry
    {
        internal FunctionRegistry() { }
        private readonly Dictionary<string, Delegate> _functions = [];
        public void Clear() => _functions.Clear();
        public void AddFunction<TResult>(Func<TResult> func, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? func.Method.Name : funcName] = func;
        public void AddFunction<T0, TResult>(Func<T0, TResult> func, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? func.Method.Name : funcName] = func;
        public void AddFunction<T0, T1, TResult>(Func<T0, T1, TResult> func, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? func.Method.Name : funcName] = func;
        public void AddFunction<T0, T1, T2, TResult>(Func<T0, T1, T2, TResult> func, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? func.Method.Name : funcName] = func;
        public void AddFunction<T0, T1, T2, T3, TResult>(Func<T0, T1, T2, T3, TResult> func, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? func.Method.Name : funcName] = func;
        public void AddFunction<T0, T1, T2, T3, T4, TResult>(Func<T0, T1, T2, T3, T4, TResult> func, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? func.Method.Name : funcName] = func;
        public void AddFunction(Action action, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? action.Method.Name : funcName] = action;
        public void AddFunction<T0>(Action<T0> action, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? action.Method.Name : funcName] = action;
        public void AddFunction<T0, T1>(Action<T0, T1> action, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? action.Method.Name : funcName] = action;
        public void AddFunction<T0, T1, T2>(Action<T0, T1, T2> action, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? action.Method.Name : funcName] = action;
        public void AddFunction<T0, T1, T2, T3>(Action<T0, T1, T2, T3> action, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? action.Method.Name : funcName] = action;
        public void AddFunction<T0, T1, T2, T3, T4>(Action<T0, T1, T2, T3, T4> action, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? action.Method.Name : funcName] = action;
        public Delegate GetDelegate(string funcName)
        {
            if (_functions.TryGetValue(funcName, out var func))
            {
                return func;
            }
            throw new KeyNotFoundException($"Function '{funcName}' not found.");
        }
        public dynamic? Invoke(string funcName, params object[] args)
        {
            if (_functions.TryGetValue(funcName, out var func))
            {
                try
                {
                    if (args == null || args.Length == 0)
                    {
                        if (func is Action action)
                        {
                            action();
                            return null;
                        }
                        else if (func is Delegate del && del.Method.GetParameters().Length == 0)
                        {
                            return del.DynamicInvoke();
                        }
                    }
                    return func.DynamicInvoke(args);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error invoking function '{funcName}': {ex.Message}", ex);
                }
            }
            throw new KeyNotFoundException($"Function '{funcName}' not found or has incorrect signature.");
        }
        public TResult Invoke<TResult>(string funcName)
        {
            if (_functions.TryGetValue(funcName, out var func) && func is Func<TResult> function)
            {
                try
                {
                    return function();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error invoking function '{funcName}': {ex.Message}", ex);
                }
            }
            throw new KeyNotFoundException($"Function '{funcName}' not found or has incorrect signature.");
        }
        public TResult Invoke<T0, TResult>(string funcName, T0 arg0)
        {
            if (_functions.TryGetValue(funcName, out var func) && func is Func<T0, TResult> function)
            {
                try
                {
                    return function(arg0);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error invoking function '{funcName}': {ex.Message}", ex);
                }
            }
            throw new KeyNotFoundException($"Function '{funcName}' not found or has incorrect signature.");
        }
        public TResult Invoke<T0, T1, TResult>(string funcName, T0 arg0, T1 arg1)
        {
            if (_functions.TryGetValue(funcName, out var func) && func is Func<T0, T1, TResult> function)
            {
                try
                {
                    return function(arg0, arg1);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error invoking function '{funcName}': {ex.Message}", ex);
                }
            }
            throw new KeyNotFoundException($"Function '{funcName}' not found or has incorrect signature.");
        }
        public TResult Invoke<T0, T1, T2, TResult>(string funcName, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_functions.TryGetValue(funcName, out var func) && func is Func<T0, T1, T2, TResult> function)
            {
                try
                {
                    return function(arg0, arg1, arg2);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error invoking function '{funcName}': {ex.Message}", ex);
                }
            }
            throw new KeyNotFoundException($"Function '{funcName}' not found or has incorrect signature.");
        }
        public TResult Invoke<T0, T1, T2, T3, TResult>(string funcName, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_functions.TryGetValue(funcName, out var func) && func is Func<T0, T1, T2, T3, TResult> function)
            {
                try
                {
                    return function(arg0, arg1, arg2, arg3);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error invoking function '{funcName}': {ex.Message}", ex);
                }
            }
            throw new KeyNotFoundException($"Function '{funcName}' not found or has incorrect signature.");
        }
        public TResult Invoke<T0, T1, T2, T3, T4, TResult>(string funcName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (_functions.TryGetValue(funcName, out var func) && func is Func<T0, T1, T2, T3, T4, TResult> function)
            {
                try
                {
                    return function(arg0, arg1, arg2, arg3, arg4);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error invoking function '{funcName}': {ex.Message}", ex);
                }
            }
            throw new KeyNotFoundException($"Function '{funcName}' not found or has incorrect signature.");
        }
        public void Invoke(string funcName)
        {
            if (_functions.TryGetValue(funcName, out var func) && func is Action action)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error invoking function '{funcName}': {ex.Message}", ex);
                }
            }
            else
            {
                throw new KeyNotFoundException($"Function '{funcName}' not found or has incorrect signature.");
            }
        }
        public void Invoke<T0>(string funcName, T0 arg0)
        {
            if (_functions.TryGetValue(funcName, out var func) && func is Action<T0> action)
            {
                try
                {
                    action(arg0);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error invoking function '{funcName}': {ex.Message}", ex);
                }
            }
            else
            {
                throw new KeyNotFoundException($"Function '{funcName}' not found or has incorrect signature.");
            }
        }
        public void Invoke<T0, T1>(string funcName, T0 arg0, T1 arg1)
        {
            if (_functions.TryGetValue(funcName, out var func) && func is Action<T0, T1> action)
            {
                try
                {
                    action(arg0, arg1);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error invoking function '{funcName}': {ex.Message}", ex);
                }
            }
            else
            {
                throw new KeyNotFoundException($"Function '{funcName}' not found or has incorrect signature.");
            }
        }
        public void Invoke<T0, T1, T2>(string funcName, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_functions.TryGetValue(funcName, out var func) && func is Action<T0, T1, T2> action)
            {
                try
                {
                    action(arg0, arg1, arg2);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error invoking function '{funcName}': {ex.Message}", ex);
                }
            }
            else
            {
                throw new KeyNotFoundException($"Function '{funcName}' not found or has incorrect signature.");
            }
        }
        public void Invoke<T0, T1, T2, T3>(string funcName, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_functions.TryGetValue(funcName, out var func) && func is Action<T0, T1, T2, T3> action)
            {
                try
                {
                    action(arg0, arg1, arg2, arg3);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error invoking function '{funcName}': {ex.Message}", ex);
                }
            }
            else
            {
                throw new KeyNotFoundException($"Function '{funcName}' not found or has incorrect signature.");
            }
        }
        public void Invoke<T0, T1, T2, T3, T4>(string funcName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (_functions.TryGetValue(funcName, out var func) && func is Action<T0, T1, T2, T3, T4> action)
            {
                try
                {
                    action(arg0, arg1, arg2, arg3, arg4);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error invoking function '{funcName}': {ex.Message}", ex);
                }
            }
            else
            {
                throw new KeyNotFoundException($"Function '{funcName}' not found or has incorrect signature.");
            }
        }
    }

    public class TypedVar
    {
        public object Value { get; }
        public Type Type { get; }

        internal TypedVar(object value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null.");
            Type = value.GetType();
        }

        internal TypedVar(object value, Type type)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null.");
            Type = type;
        }
    }
}