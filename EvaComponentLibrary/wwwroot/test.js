
//export function setupEnterKeyListener(dotNetObject) {
//    var shiftPressed = false;
//    if (window.innerWidth >= 600) {
//        document.getElementById('textbox')?.addEventListener('keydown', function (event) {



//            if (event.key === 'Enter' && !shiftPressed) {
//                event.preventDefault(); // Prevent the default action of the Enter key
//                const msg = document.getElementById('textbox').value;
//                dotNetObject.invokeMethodAsync('SendFromJs', msg); // Call the C# method
//                document.getElementById('textbox').value = "";
//                document.getElementById('scroll-to-elem').scrollIntoView({ behavior: 'smooth' });
//                ResetView();
//            }

//            if (event.key === 'Shift') {
//                shiftPressed = true;
//            }
//        });

//        document.getElementById('textbox')?.addEventListener('keyup', function (event) {


//            if (event.key === 'Shift') {
//                shiftPressed = false;
//            }
//        });

//    }
//    //document.getElementById('textbox')?.addEventListener('input', function () {
//    //        // Clear any existing debounce timers
//    //    console.log("JS IST DA");
//    //        var textbox = document.getElementById('textbox');
//    //        var chatInput = document.getElementById('chat-input');
//    //        // Use a small delay to debounce the input events

//    //        // Reset height to auto to calculate new scrollHeight correctly
//    //        textbox.style.height = 'auto';

//    //        // Determine max height for the textarea
//    //        const maxHeight = parseInt(window.getComputedStyle(textbox).lineHeight) * 4;

//    //        // Calculate new height
//    //        const newHeight = Math.min(textbox.scrollHeight, maxHeight) + 1.88;

//    //        textbox.style.height = newHeight + 'px';

//    //        if (textbox.style.height != newHeight) {
//    //            // Apply the new height to the textarea
//    //            textbox.style.height = newHeight + 'px';

//    //            // Adjust the container's height accordingly
//    //            chatInput.style.height = (100 + newHeight) + 'px';

//    //        }

//    //        // Debounce delay of 50ms
//    //});

//    function ResetView() {
//        var textbox = document.getElementById('textbox');
//        var chatinput = document.getElementById('chat-input');
//        // use a small delay to debounce the input events

//        // reset height to auto to calculate new scrollheight correctly
//        textbox.style.height = 'auto';

//        // determine max height for the textarea
//        const maxheight = parseInt(window.getComputedStyle(textbox).lineHeight) * 4;

//        // calculate new height
//        const newheight = Math.min(textbox.scrollHeight, maxheight) + 1.88;

//        textbox.style = newheight + 'px';

//        if (textbox.style.height != newheight) {
//            // apply the new height to the textarea
//            textbox.style.height = newheight + 'px';

//            // adjust the container's height accordingly
//            chatinput.style.height = (100 + newheight) + 'px';

//        }
//    }

//}

//function ResetInput() {
//    var textbox = document.getElementById('textbox');
//    var chatInput = document.getElementById('chat-input');
//    // Use a small delay to debounce the input events
//    debounceTimer = setTimeout(() => {


//        // Reset height to auto to calculate new scrollHeight correctly
//        textbox.style.height = 'auto';

//        // Determine max height for the textarea
//        const maxheight = parseInt(window.getComputedStyle(textbox).lineHeight) * 4;

//        // Calculate new height
//        const newheight = Math.min(textbox.scrollHeight, maxheight) + 1.88;

//        textbox.style.height = newHeight + 'px';

//        if (textbox.style.height != newHeight) {
//            // Apply the new height to the textarea
//            textbox.style.height = newHeight + 'px';

//            // Adjust the container's height accordingly
//            chatInput.style.height = (100 + newHeight) + 'px';

//        }
//    });

//}
