using System.Reflection;
using System.Reflection.Emit;

namespace ElementalAdventure.Client.Game.Utils;

public static class ListExtensions {
    static class ArrayAccessor<T> {
        public static Func<List<T>, T[]> Getter;

        static ArrayAccessor() {
            DynamicMethod dm = new DynamicMethod("get", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, typeof(T[]), [typeof(List<T>)], typeof(ArrayAccessor<T>), true);
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, typeof(List<T>).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance)!);
            il.Emit(OpCodes.Ret);
            Getter = (Func<List<T>, T[]>)dm.CreateDelegate(typeof(Func<List<T>, T[]>));
        }
    }

    public static T[] GetBackingArray<T>(this List<T> list) {
        return ArrayAccessor<T>.Getter(list);
    }
}