using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Game.UI.Interface;

namespace ElementalAdventure.Client.Game.UI;

public class UIManager {
    private readonly Stack<IViewGroup<string>> _stack;

    public UIManager() {
        _stack = [];
    }

    public void Push(IViewGroup<string> viewGroup) {
        _stack.Push(viewGroup);
    }

    public void Pop() {
        if (_stack.Count != 0)
            _stack.Pop();
    }

    public void Render(IRenderer<string> renderer) {
        if (_stack.Count == 0)
            return;
        _stack.Peek().Render(renderer);
    }
}