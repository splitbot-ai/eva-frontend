var startHeight = 0;
var debounceTimer;


window.setupEnterKeySend = () => {
    const textbox = document.getElementById('textbox');
    const sendBtn = document.getElementById('sendBtn');
    if (!textbox || !sendBtn) return;

    textbox.addEventListener('keydown', function (e) {
        // on Enter without Shift
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();       // stop the newline
            sendBtn.click();          // fire Blazor’s @onclick
            //console.log("Enter pressed ? sendBtn.click()");
        }
    });
};

setupEnterKeySend();

document.getElementById('textbox')?.addEventListener('input', function () {
    // Clear any existing debounce timers
    clearTimeout(debounceTimer);
    var textbox = document.getElementById('textbox');
    var chatInput = document.getElementById('chat-input');
    var chatScroller = document.getElementById('chat-scroller');
    var preview = document.getElementById('preview');
    // Use a small delay to debounce the input events
    debounceTimer = setTimeout(() => {


        // Reset height to auto to calculate new scrollHeight correctly
        textbox.style.height = 'auto';

        // Determine max height for the textarea
        const maxHeight = parseInt(window.getComputedStyle(textbox).lineHeight) * 4;

        // Calculate new height
        const newHeight = Math.min(textbox.scrollHeight, maxHeight) + 1.88;

        textbox.style.height = newHeight + 'px';

        if (textbox.style.height != newHeight) {
            // Apply the new height to the textarea
            textbox.style.height = newHeight + 'px';

            // Adjust the container's height accordingly
            chatInput.style.height = (70 + newHeight) + 'px';
            if (preview) {
                preview.style.height = (70 + newHeight) + 'px';
                document.getElementById('chat-scroller').style.height = 'calc(100% -' + (70 + newHeight + 70) + 'px)';
            } else {
                document.getElementById('chat-scroller').style.height = 'calc(100% -' + (70 + newHeight) + 'px)';
            }

            //console.log(chatScroller.style.height.height + " Height");
        }

    }, 50); // Debounce delay of 50ms
});


function ResetInput() {
    var textbox = document.getElementById('textbox');
    var chatInput = document.getElementById('chat-input');
    var chatScroller = document.getElementById('chat-scroller');
    // Use a small delay to debounce the input events
    debounceTimer = setTimeout(() => {


        // Reset height to auto to calculate new scrollHeight correctly
        textbox.style.height = 'auto';

        // Determine max height for the textarea
        const maxHeight = parseInt(window.getComputedStyle(textbox).lineHeight) * 4;

        // Calculate new height
        const newHeight = Math.min(textbox.scrollHeight, maxHeight) + 1.88;

        textbox.style.height = newHeight + 'px';

        if (textbox.style.height != newHeight) {
            // Apply the new height to the textarea
            textbox.style.height = newHeight + 'px';

            // Adjust the container's height accordingly
            chatInput.style.height = (70 + newHeight) + 'px';
            chatScroller.style.height = "calc(100% -" + chatInput.style.height - 20 + ")";
        }
    });

}

