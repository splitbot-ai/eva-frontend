
    public interface IKeyboardServices
    {
        void RegisterForKeyboardNotification();
        void UnregisterForKeyboardNotification();
        event EventHandler<float> KeyboardHeightChanged;

    }

