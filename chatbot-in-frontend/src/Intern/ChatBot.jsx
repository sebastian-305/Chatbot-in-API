import { useState, useContext } from "react";
import { getCookie } from "../scripts/Chatbotin-Helper";
import ChatbotinContext from "../ChatbotinContext"

export function ChatBot() {
    const [userMessage, setUserMessage] = useState("");
    const [messages, setMessages] = useState([]); // chat history
    const [isLoading, setIsLoading] = useState(false);
    const { selectedFiles } = useContext(ChatbotinContext);

    async function sendMessage(event) {
        event.preventDefault();
        if (!userMessage.trim()) return;

        setIsLoading(true);

        // Step 1: Fetch file contents from backend
        const sessionToken = getCookie("session");

        const fileList = [...selectedFiles];

        // Step 2: Send to chat endpoint
        const request = new Request("http://localhost:5114/chat", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": sessionToken
            },
            body: JSON.stringify({
                prompt: userMessage,
                fileNames: fileList
            })
        });

        const response = await fetch(request);
        const result = await response.text(); // assumes plain text

        

        setMessages(prev => [...prev, { role: "user", text: userMessage }, { role: "ai", text: result }]);
        setUserMessage("");
        setIsLoading(false);
    }



    return (
        <div style={{ margin: "20px auto", width:"100%" }}>
            <h2>Chatbot</h2>
            <div style={{
                border: "1px solid gray", padding: "1em", height: "600px", overflowY: "scroll", backgroundColor: "#f9f9f9", marginBottom: "1em", width: "100%"
            }}>
                {messages.map((msg, i) => (
                    <div key={i} style={{
                        textAlign: msg.role === "user" ? "right" : "left", border: "1px solid #EEE", marginBottom: "10px", backgroundColor: msg.role === "user" ? "#F9F9FA" : "#F9FAF9", borderRadius: "5px", padding: "0.5rem" }}>
                        <strong>{msg.role === "user" ? "You" : "AI"}:</strong> <p dangerouslySetInnerHTML={{__html: msg.text}  }></p>
                    </div>
                ))}
                {isLoading && <div><em>AI is thinking...</em></div>}
            </div>
            <form onSubmit={sendMessage} style={{ display: "flex", gap: "0.5em" }}>
                <input
                    type="text"
                    value={userMessage}
                    onChange={(e) => setUserMessage(e.target.value)}
                    placeholder="Ask something about your files..."
                    style={{ flexGrow: 1 }}
                />
                <button type="submit">Send</button>
            </form>
        </div>
    );
}