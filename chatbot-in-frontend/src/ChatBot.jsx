import { useState } from "react";
import { getCookie } from "./scripts/Chatbotin-Helper";

export function ChatBot() {
    const [userMessage, setUserMessage] = useState("");
    const [messages, setMessages] = useState([]); // chat history
    const [isLoading, setIsLoading] = useState(false);

    async function sendMessage(event) {
        event.preventDefault();
        if (!userMessage.trim()) return;

        setIsLoading(true);

        // Step 1: Fetch file contents from backend
        const sessionToken = getCookie("session");

        const fileListResponse = await fetch("http://localhost:5114/files", {
            headers: {
                "Authorization": sessionToken
            }
        });
        const fileList = await fileListResponse.json();

        const fileContents = await Promise.all(
            fileList.map(async file => {
                const contentResponse = await fetch(`http://localhost:5114/filecontent?filename=${file.fileName}`, {
                    headers: {
                        "Authorization": sessionToken
                    }
                });
                return await contentResponse.text();
            })
        );

        const combinedContent = fileContents.join("\n");

        // Step 2: Send to chat endpoint
        const request = new Request("http://localhost:5114/api/chat", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": sessionToken
            },
            body: JSON.stringify({
                userMessage: userMessage,
                documentContent: combinedContent
            })
        });

        const response = await fetch(request);
        const result = await response.text(); // assumes plain text

        setMessages(prev => [...prev, { role: "user", text: userMessage }, { role: "ai", text: result }]);
        setUserMessage("");
        setIsLoading(false);
    }

    return (
        <div style={{ maxWidth: "600px", margin: "20px auto" }}>
            <h2>Chatbot</h2>
            <div style={{
                border: "1px solid gray", padding: "1em", height: "300px", overflowY: "scroll", backgroundColor: "#f9f9f9", marginBottom: "1em"
            }}>
                {messages.map((msg, i) => (
                    <div key={i} style={{ textAlign: msg.role === "user" ? "right" : "left", marginBottom: "10px" }}>
                        <strong>{msg.role === "user" ? "You" : "AI"}:</strong> {msg.text}
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