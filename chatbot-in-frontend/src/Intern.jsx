import { UploadFile } from "./UploadFile"
import { FileList } from "./FileList"
import { ChatBot } from "./ChatBot"
import { Logout } from "./Logout"
export function Intern() {

    return <>
        <Logout></Logout>
        <UploadFile></UploadFile>
        <FileList></FileList>
        <ChatBot></ChatBot>

    </>
}