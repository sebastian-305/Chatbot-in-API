import { UploadFile } from "./UploadFile"
import { FileList } from "./FileList"
import { ChatBot } from "./ChatBot"
import { Logout } from "./Logout"
import { DisplayRequestCount } from "./DisplayRequestCount"
import { createSignalRContext } from "react-signalr/signalr";

export const SignalRContext = createSignalRContext();

export function Intern() {
    return <>
        <Logout></Logout>
        <SignalRContext.Provider
            url={"http://localhost:5114/hubs/counter"}
        >
            <DisplayRequestCount></DisplayRequestCount>
        </SignalRContext.Provider>
        <UploadFile></UploadFile>
        <FileList></FileList>
        <ChatBot></ChatBot>
    </>

}