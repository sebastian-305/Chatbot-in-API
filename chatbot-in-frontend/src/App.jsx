import { UploadFile } from  "./UploadFile"
import { FileList } from "./FileList"
import { ChatBot } from "./ChatBot"
import './App.css'

function App() {


    return (
        <>
            <h1>Chatbot:in</h1>  
            <hr></hr>
            <UploadFile></UploadFile>
            <FileList></FileList>
            <ChatBot></ChatBot>
        </>
    )
}

export default App
