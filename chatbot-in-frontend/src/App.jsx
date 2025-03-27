import { useState } from "react";
import ChatbotinContext from "./ChatbotinContext"
import './App.css'
import {Auth} from "./Auth";

export const ChatbotinContextProvider = (props) => {
    const [loggedIn, setLoggedIn] = useState(false);
    const [filesChanged, setFilesChanged] = useState(0);


    return (
        <ChatbotinContext.Provider
            value={{
                loggedIn, setLoggedIn, filesChanged, setFilesChanged,
            }}>
            {props.children}
        </ChatbotinContext.Provider>
    );
};



function App() {
    return (
        <>
            <hr></hr>
            <h1>Chatbot:in</h1>
            <hr></hr>
            <ChatbotinContextProvider>
                <Auth></Auth>
            </ChatbotinContextProvider>

        </>
    );

}

export default App;
