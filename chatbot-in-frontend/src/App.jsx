import { useState, useEffect } from "react";
import { getCookie, isLoggedIn } from "./scripts/Chatbotin-Helper";
import { UploadFile } from "./UploadFile"
import { FileList } from "./FileList"
import { ChatBot } from "./ChatBot"
import { Registration } from "./Registration"
import { Login } from "./Login"
import { Logout } from "./Logout"
import './App.css'
import { useEffect } from "react";

function App() {

    const [loggedIn, setLoggedIn] = useState(false);

    useEffect(() => {
        setLoggedIn(isLoggedIn());
    }, []);

    return (
        <>
            {!loggedIn && (
                <>
                    <Registration></Registration>
                    <Login></Login>
                </>
            )}
            {loggedIn && (
                <>
                    <Logout></Logout>
                    <hr></hr>
                    <h1>Chatbot:in</h1>
                    <hr></hr>
                    <UploadFile></UploadFile>
                    <FileList></FileList>
                    <ChatBot></ChatBot>
                </>

            )}
        </>
    );

}

export default App;
