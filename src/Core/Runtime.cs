namespace DialoguePlus.Core
{
    /// <summary>
    /// Manages runtime state including variables and function registries for dialogue execution.
    /// </summary>
    public class Runtime
    {
        /// <summary>
        /// Gets the variable registry for storing and accessing script variables.
        /// </summary>
        public readonly VariableRegistry Variables = new();
        /// <summary>
        /// Gets the function registry for registering and calling C# functions from scripts.
        /// </summary>
        public readonly FunctionRegistry Functions = new();
    }

    /// <summary>
    /// Manages variable storage with support for global and temporary (scoped) variables.
    /// </summary>
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

        /// <summary>
        /// Clears all variables in both global and temporary scopes.
        /// </summary>
        public void Clear()
        {
            _globalScope.Clear();
            _tempScopeStack.Clear();
            NewTempScope();
        }

        /// <summary>
        /// Creates a new temporary variable scope. Used when entering a new context (e.g., tour to a label).
        /// </summary>
        public void NewTempScope()
        {
            _tempScopeStack.Push(new Dictionary<string, TypedVar>());
        }

        /// <summary>
        /// Pops the current temporary variable scope, discarding all variables in it.
        /// </summary>
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

        /// <summary>
        /// Sets a variable value. Variables starting with "global." are stored in the global scope.
        /// Supported types: string, int, float, bool.
        /// </summary>
        /// <param name="varName">The variable name. Use "global." prefix for global variables.</param>
        /// <param name="value">The value to set. Must be string, int, float, bool, or TypedVar.</param>
        /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
        /// <exception cref="ArgumentException">Thrown when value type is not supported.</exception>
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

        /// <summary>
        /// Gets the value of a variable. Variables starting with "global." are retrieved from the global scope.
        /// </summary>
        /// <param name="varName">The variable name to retrieve.</param>
        /// <returns>The typed variable containing the value and type information.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the variable is not found.</exception>
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

    /// <summary>
    /// Manages registration and invocation of C# functions that can be called from DialoguePlus scripts.
    /// </summary>
    public class FunctionRegistry
    {
        internal FunctionRegistry() { }
        private readonly Dictionary<string, Delegate> _functions = [];
        /// <summary>
        /// Clears all registered functions.
        /// </summary>
        public void Clear() => _functions.Clear();
        /// <summary>
        /// Registers a function with no parameters that returns a value.
        /// </summary>
        /// <typeparam name="TResult">The return type of the function.</typeparam>
        /// <param name="func">The function to register.</param>
        /// <param name="funcName">The name to register the function under. If empty, uses the method name.</param>
        public void AddFunction<TResult>(Func<TResult> func, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? func.Method.Name : funcName] = func;
        /// <summary>
        /// Registers a function with one parameter that returns a value.
        /// </summary>
        public void AddFunction<T0, TResult>(Func<T0, TResult> func, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? func.Method.Name : funcName] = func;
        /// <summary>
        /// Registers a function with two parameters that returns a value.
        /// </summary>
        public void AddFunction<T0, T1, TResult>(Func<T0, T1, TResult> func, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? func.Method.Name : funcName] = func;
        /// <summary>
        /// Registers a function with three parameters that returns a value.
        /// </summary>
        public void AddFunction<T0, T1, T2, TResult>(Func<T0, T1, T2, TResult> func, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? func.Method.Name : funcName] = func;
        /// <summary>
        /// Registers a function with four parameters that returns a value.
        /// </summary>
        public void AddFunction<T0, T1, T2, T3, TResult>(Func<T0, T1, T2, T3, TResult> func, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? func.Method.Name : funcName] = func;
        /// <summary>
        /// Registers a function with five parameters that returns a value.
        /// </summary>
        public void AddFunction<T0, T1, T2, T3, T4, TResult>(Func<T0, T1, T2, T3, T4, TResult> func, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? func.Method.Name : funcName] = func;
        /// <summary>
        /// Registers an action (void function) with no parameters.
        /// </summary>
        public void AddFunction(Action action, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? action.Method.Name : funcName] = action;
        /// <summary>
        /// Registers an action with one parameter.
        /// </summary>
        public void AddFunction<T0>(Action<T0> action, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? action.Method.Name : funcName] = action;
        /// <summary>
        /// Registers an action with two parameters.
        /// </summary>
        public void AddFunction<T0, T1>(Action<T0, T1> action, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? action.Method.Name : funcName] = action;
        /// <summary>
        /// Registers an action with three parameters.
        /// </summary>
        public void AddFunction<T0, T1, T2>(Action<T0, T1, T2> action, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? action.Method.Name : funcName] = action;
        /// <summary>
        /// Registers an action with four parameters.
        /// </summary>
        public void AddFunction<T0, T1, T2, T3>(Action<T0, T1, T2, T3> action, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? action.Method.Name : funcName] = action;
        /// <summary>
        /// Registers an action with five parameters.
        /// </summary>
        public void AddFunction<T0, T1, T2, T3, T4>(Action<T0, T1, T2, T3, T4> action, string funcName = "")
            => _functions[string.IsNullOrEmpty(funcName) ? action.Method.Name : funcName] = action;
        /// <summary>
        /// Registers a generic delegate with a custom name.
        /// </summary>
        public void AddFunction(Delegate func, string funcName)
            => _functions[string.IsNullOrEmpty(funcName) ? func.Method.Name : funcName] = func;
        /// <summary>
        /// Gets the delegate for a registered function.
        /// </summary>
        /// <param name="funcName">The function name.</param>
        /// <returns>The registered delegate.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the function is not found.</exception>
        public Delegate GetDelegate(string funcName)
        {
            if (_functions.TryGetValue(funcName, out var func))
            {
                return func;
            }
            throw new KeyNotFoundException($"Function '{funcName}' not found.");
        }
        /// <summary>
        /// Invokes a function with dynamic arguments. Returns the function result or null for void functions.
        /// </summary>
        /// <param name="funcName">The function name.</param>
        /// <param name="args">The arguments to pass.</param>
        /// <returns>The function result or null.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the function is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown when function invocation fails.</exception>
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

    /// <summary>
    /// Represents a typed variable with its value and type information.
    /// </summary>
    public class TypedVar
    {
        /// <summary>
        /// Gets the value of the variable.
        /// </summary>
        public object Value { get; }
        /// <summary>
        /// Gets the type of the variable.
        /// </summary>
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