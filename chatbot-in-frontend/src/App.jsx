import { UploadFile } from  "./UploadFile"
import { FileList } from "./FileList"
import { ChatBot } from "./ChatBot"
import './App.css'
import { Registration } from "./Registration"
import { Login } from "./Login"

function App() {


    return (
        <>
            <Registration></Registration><Login></Login>
            <hr></hr>
            <h1>Chatbot:in</h1>  
            <hr></hr>
            <UploadFile></UploadFile>
            <FileList></FileList>
            <ChatBot></ChatBot>
        </>
    )
}

export default App
