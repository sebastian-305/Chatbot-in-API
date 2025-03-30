import { useState } from "react";
import ChatbotinContext from "./ChatbotinContext"
import './App.css'
import { Auth } from "./Auth";


export const ChatbotinContextProvider = (props) => {
    const [loggedIn, setLoggedIn] = useState(false);
    const [filesChanged, setFilesChanged] = useState(0);
    const [selectedFiles, setSelectedFiles] = useState([]);

    // Add a file
    const addFile = (file) => {
        setSelectedFiles((prevFiles) => [...prevFiles, file]);
    };

    // Remove a file
    const removeFile = (fileToRemove) => {
        setSelectedFiles((prevFiles) => prevFiles.filter(file => file !== fileToRemove));
    };



    return (
        <ChatbotinContext.Provider
            value={{
                loggedIn, setLoggedIn, filesChanged, setFilesChanged, selectedFiles, setSelectedFiles, addFile, removeFile
            }}>
            {props.children}
        </ChatbotinContext.Provider>
    );
};



function App() {
    return (
        <>
            <hr></hr>
            <h1>Pocket Lawyer</h1>
            <hr></hr>
            <ChatbotinContextProvider>
                <Auth></Auth>
            </ChatbotinContextProvider>

        </>
    );

}

export default App;
