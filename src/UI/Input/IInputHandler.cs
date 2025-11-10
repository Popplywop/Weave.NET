namespace Weave.UI;

// Handlers return true if they handled the event and want to stop bubbling.
public interface IInputHandler
{
    bool OnInput(InputEvent e);
}