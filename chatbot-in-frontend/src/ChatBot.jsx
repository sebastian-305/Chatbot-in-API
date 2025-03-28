import { useState, useEffect } from "react";
import { getCookie } from "./scripts/Chatbotin-Helper";

export function ChatBot() {
    const [userMessage, setUserMessage] = useState("");
    const [messages, setMessages] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [files, setFiles] = useState([]);
    const [selectedFile, setSelectedFile] = useState("");
    const [error, setError] = useState("");

    useEffect(() => {
        // Load files on component mount
        const sessionToken = getCookie("session");
        fetch("http://localhost:5114/files", {
            headers: {
                "Authorization": sessionToken
            }
        })
            .then(response => response.json())
            .then(data => setFiles(data.map(e => e.fileName)))
            .catch(err => {
                console.error("Error loading files:", err);
                setError("Could not load file list.");
            });
    }, []);

    async function sendMessage(event) {
        event.preventDefault();
        setError("");
        if (!userMessage.trim() || !selectedFile) return;

        setIsLoading(true);

        try {
            const sessionToken = getCookie("session");

            // Step 1: Fetch file content
            const contentResponse = await fetch(`http://localhost:5114/filecontent?filename=${selectedFile}`, {
                headers: {
                    "Authorization": sessionToken
                }
            });

            if (!contentResponse.ok) {
                throw new Error(`Could not load file content. (${contentResponse.status})`);
            }

            const fileContent = await contentResponse.text();

            // Step 2: Send to chat endpoint
            const request = new Request("http://localhost:5114/api/chat", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": sessionToken
                },
                body: JSON.stringify({
                    userMessage: userMessage,
                    documentContent: fileContent
                })
            });

            const response = await fetch(request);
            if (!response.ok) {
                throw new Error(`Chat request failed. (${response.status})`);
            }

            const result = await response.text();

            setMessages(prev => [
                ...prev,
                { role: "user", text: userMessage },
                { role: "ai", text: result }
            ]);
            setUserMessage("");
        } catch (err) {
            console.error(err);
            setError(err.message);
        } finally {
            setIsLoading(false);
        }
    }

    return (
        <div style={{ maxWidth: "600px", margin: "20px auto" }}>
            <h2>Chatbot</h2>

            {/* File Selector */}
            <select
                value={selectedFile}
                onChange={(e) => setSelectedFile(e.target.value)}
                style={{ marginBottom: "1em", width: "100%" }}
            >
                <option value="">Select a file...</option>
                {files.map((file, i) => (
                    <option key={i} value={file}>{file}</option>
                ))}
            </select>

            {/* Chat Window */}
            <div style={{
                border: "1px solid gray", padding: "1em", height: "300px", overflowY: "scroll", backgroundColor: "#f9f9f9", marginBottom: "1em"
            }}>
                {messages.map((msg, i) => (
                    <div key={i} style={{ textAlign: msg.role === "user" ? "right" : "left", marginBottom: "10px" }}>
                        <strong>{msg.role === "user" ? "You" : "AI"}:</strong> {msg.text}
                    </div>
                ))}
                {isLoading && <div><em>AI is thinking...</em> ??</div>}
            </div>

            {/* Error Message */}
            {error && <div style={{ color: "red", marginBottom: "10px" }}>{error}</div>}

            {/* Message Input */}
            <form onSubmit={sendMessage} style={{ display: "flex", gap: "0.5em" }}>
                <input
                    type="text"
                    value={userMessage}
                    onChange={(e) => setUserMessage(e.target.value)}
                    placeholder={selectedFile ? "Ask about " + selectedFile : "Select a file first"}
                    style={{ flexGrow: 1 }}
                    disabled={!selectedFile || isLoading}
                />
                <button type="submit" disabled={!selectedFile || isLoading}>
                    {isLoading ? "Sending..." : "Send"}
                </button>
            </form>
        </div>
    );
}