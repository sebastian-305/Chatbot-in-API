export function ChatBot() {

    const messages = ["Beep Boop I'm a bot!", "Interesting files... interssting....", "I am not implemented, stop trying.", "Processing...", "I think I spider."];


    //function getMockAnswer4() {
    //    var request = new Request('http://localhost:5114/chat', {
    //        method: 'POST'
    //    });
    //    fetch(request).then(response => response.text()).then(text => document.querySelector("#answer").innerHTML = text);
    //}

    return <><button type="button" onClick={() => document.getElementById("messages").innerHTML = messages[Math.floor(Math.random() * messages.length)]}>Chat with Ai</button><p id="messages"></p></>;
}