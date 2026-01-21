using UnityEngine;
using DialoguePlus.Core;
using UnityEngine.Scripting;
using System.Reflection;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class DialoguePlusAdapter : MonoBehaviour
{
    private static DialoguePlusAdapter _instance;
    public static DialoguePlusAdapter Instance => _instance;

    private Executer _executer = new();
    private Compiler _compiler = new();

    public Executer Executer => _executer;
    public Runtime Runtime => _executer.Runtime;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("DialoguePlusAdapter instance already exists. Destroying duplicate.");
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public async Task ExecuteToEnd(string path)
    {
        Debug.Log($"[D+] Compiling and executing script: {path}");
        var result = _compiler.Compile(path);
        Debug.Log($"[D+] Script compiled successfully: {path}");
        foreach (var diag in result.Diagnostics)
        {
            if (diag.Severity == Diagnostic.SeverityLevel.Error)
            {
                Debug.LogError($"[D+] {diag.Message} (Line {diag.Line}, Column {diag.Column})");
            }
            else if (diag.Severity == Diagnostic.SeverityLevel.Warning)
            {
                Debug.LogWarning($"[D+] {diag.Message} (Line {diag.Line}, Column {diag.Column})");
            }
            else
            {
                Debug.Log($"[D+] {diag.Message} (Line {diag.Line}, Column {diag.Column})");
            }
        }
        if (result.Success)
        {
            this.Runtime.Variables.Clear();
            _executer.Prepare(result.Labels);
            Debug.Log($"[D+] Script execution start, include labels: {string.Join(", ", result.Labels.Labels.Keys)}");
            await _executer.AutoStepAsync(0);
        }
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class DPFunctionAttribute : Attribute
{
    public string Name { get; } = string.Empty;

    public DPFunctionAttribute() { }

    public DPFunctionAttribute(string name)
    {
        Name = name;
    }
}

[Preserve]
public static class FunctionRegistrar
{
    private static readonly Type[] _allowed =
    {
        typeof(string), typeof(bool), typeof(int), typeof(float)
    };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterAll()
    {
        EnsureAdapterExists(); // 确保 Runtime 可用
        var registry = DialoguePlusAdapter.Instance.Runtime.Functions;

        var attrType = typeof(DPFunctionAttribute);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var asm in assemblies)
        {
            Type[] types;
            try { types = asm.GetTypes(); }
            catch (ReflectionTypeLoadException e) { types = e.Types.Where(t => t != null).ToArray(); }

            foreach (var type in types)
            {
                // instance methods
                var instMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                      .Where(m => m.IsDefined(attrType, false));
                foreach (var m in instMethods)
                    TryRegisterInstance(registry, m);

                // static methods
                var staticMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                        .Where(m => m.IsDefined(attrType, false));
                foreach (var m in staticMethods)
                    TryRegisterStatic(registry, m);
            }
        }
    }

    // instance methods → wrap as Action<string, ...> / Func<string, ..., TResult>
    private static void TryRegisterInstance(FunctionRegistry registry, MethodInfo method)
    {
        if (method.IsGenericMethodDefinition) { Warn(method, "Generic method is not supported."); return; }

        if (!CheckSignatureSupported(method, isInstance: true, out string reason))
        {
            Warn(method, $"Unsupported signature: {reason}");
        }

        var name = ResolveFuncName(method);
        try
        {
            var del = DelegateWrapper(method);
            AddToRegistry(registry, del, name);
#if UNITY_EDITOR
            Debug.Log($"[D+] register：{name} => {method.DeclaringType.FullName}.{method.Name}");
#endif
        }
        catch (Exception ex)
        {
            Warn(method, $"Failed to register: {ex.Message}");
        }
    }

    // static methods → register with original signature (no object name needed)
    private static void TryRegisterStatic(FunctionRegistry registry, MethodInfo method)
    {
        if (method.IsGenericMethodDefinition) { Warn(method, "Generic method is not supported."); return; }

        if (!CheckSignatureSupported(method, isInstance: false, out string reason))
        {
            Warn(method, $"Unsupported signature: {reason}");
        }

        var name = ResolveFuncName(method);
        try
        {
            var delType = BuildDelegateType(method);
            var del = Delegate.CreateDelegate(delType, method);
            AddToRegistry(registry, del, name);
#if UNITY_EDITOR
            Debug.Log($"[D+] register：{name} => {method.DeclaringType.FullName}.{method.Name}");
#endif
        }
        catch (Exception ex)
        {
            Warn(method, $"Failed to register: {ex.Message}");
        }
    }

    // Wrap instance method as Action<string, ...> / Func<string, ..., TResult>, the first parameter is the GameObject name
    private static Delegate DelegateWrapper(MethodInfo method)
    {
        var declaringType = method.DeclaringType;
        var srcParams = method.GetParameters().Select(p => p.ParameterType).ToArray();
        var retType = method.ReturnType;

        // 目标委托形参：string(对象名) + 原形参
        var wrapperParams = (new[] { typeof(string) }).Concat(srcParams).ToArray();

        // 构造委托类型
        Type delegateType = retType == typeof(void)
            ? Expression.GetActionType(wrapperParams)
            : Expression.GetFuncType(wrapperParams.Concat(new[] { retType }).ToArray());

        // 表达式参数： (string goName, A a, B b, ...)
        var paramExprs = wrapperParams.Select(Expression.Parameter).ToArray();
        var goNameExpr = paramExprs[0];
        var argExprs = paramExprs.Skip(1).ToArray(); // A,B,...

        // GameObject.Find(goName)
        var findGo = typeof(GameObject).GetMethod(nameof(GameObject.Find), new[] { typeof(string) });
        var goVar = Expression.Variable(typeof(GameObject), "go");

        // if (go == null) throw ...
        var exCtor1 = typeof(ArgumentException).GetConstructor(new[] { typeof(string) });

        // go.GetComponent(declaringType)
        var getCompGeneric = typeof(GameObject).GetMethods()
                                .First(mi => mi.Name == nameof(GameObject.GetComponent)
                                          && mi.IsGenericMethodDefinition && mi.GetParameters().Length == 0)
                                .MakeGenericMethod(declaringType);
        var compVar = Expression.Variable(declaringType, "comp");
        var exCtor2 = typeof(ArgumentException).GetConstructor(new[] { typeof(string) });

        // comp.M(a,b,...)
        var call = Expression.Call(compVar, method, argExprs);

        // 构建 Block：查找 → 取组件 → 调用
        var body = Expression.Block(
            new ParameterExpression[] { goVar, compVar },
            Expression.Assign(goVar, Expression.Call(findGo, goNameExpr)),
            Expression.IfThen(
                Expression.Equal(goVar, Expression.Constant(null, typeof(GameObject))),
                Expression.Throw(Expression.New(exCtor1, Expression.Constant($"GameObject '{goNameExpr}' not found.")))),
            Expression.Assign(compVar, Expression.Call(goVar, getCompGeneric)),
            Expression.IfThen(
                Expression.Equal(compVar, Expression.Constant(null, declaringType)),
                Expression.Throw(Expression.New(exCtor2, Expression.Constant($"Component {declaringType.Name} not found.")))),
            call // 返回值：void 直接 call；非 void 直接返回 call 的结果
        );

        return Expression.Lambda(delegateType, body, paramExprs).Compile();
    }

    private static bool CheckSignatureSupported(MethodInfo m, bool isInstance, out string reason)
    {
        if (m.ReturnType != typeof(void) && !_allowed.Contains(m.ReturnType))
        {
            reason = $"Return type not supported: {m.ReturnType.Name}（expected void/string/bool/int/float）";
            return false;
        }

        foreach (var p in m.GetParameters())
        {
            var t = p.ParameterType;
            if (!_allowed.Contains(t))
            {
                reason = $"Parameter '{p.Name}' type {t.Name} not supported（expected string/bool/int/float）";
                return false;
            }
        }

        reason = string.Empty;
        return true;
    }

    private static string ResolveFuncName(MethodInfo m)
    {
        var attr = m.GetCustomAttribute<DPFunctionAttribute>(false);
        return string.IsNullOrWhiteSpace(attr?.Name) ? m.Name : attr!.Name;
    }

    private static Type BuildDelegateType(MethodInfo method)
    {
        var ps = method.GetParameters().Select(p => p.ParameterType).ToArray();
        return method.ReturnType == typeof(void)
            ? Expression.GetActionType(ps)
            : Expression.GetFuncType(ps.Concat(new[] { method.ReturnType }).ToArray());
    }

    private static void AddToRegistry(FunctionRegistry registry, Delegate del, string key)
    {
        var addStrong = typeof(FunctionRegistry)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(mi => mi.Name == "AddFunction")
            .FirstOrDefault(mi =>
            {
                var ps = mi.GetParameters();
                return ps.Length >= 1 && ps[0].ParameterType == del.GetType();
            });

        if (addStrong != null)
        {
            addStrong.Invoke(registry, new object[] { del, key });
            return;
        }

        // if cannot find strong type, fallback to AddFunction(Delegate d, string name)
        registry.AddFunction(del, key);
    }

    private static void Warn(MethodInfo m, string msg)
        => Debug.LogWarning($"[D+] {m.DeclaringType.FullName}.{m.Name} - {msg}");

    private static void EnsureAdapterExists()
    {
        if (DialoguePlusAdapter.Instance != null) return;

        var go = GameObject.Instantiate(new GameObject("[DialoguePlusAdapter]"));
        go.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
        go.AddComponent<DialoguePlusAdapter>();
    }
}
