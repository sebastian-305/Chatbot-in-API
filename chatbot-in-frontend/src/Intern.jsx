import { UploadFile } from "./UploadFile"
import { FileList } from "./FileList"
import { ChatBot } from "./ChatBot"
import { Logout } from "./Logout"
import { DisplayRequestCount } from "./DisplayRequestCount"
import { createSignalRContext } from "react-signalr/signalr";

export const SignalRContext = createSignalRContext();

export function Intern() {
    const { token } = "counter";
    return <SignalRContext.Provider
        connectEnabled={!!token}
        accessTokenFactory={() => token}
        dependencies={[token]} //remove previous connection and create a new connection if changed
        url={"http://localhost:5114/hubs/counter"}
    >
        <Logout></Logout>
        <DisplayRequestCount></DisplayRequestCount>
        <UploadFile></UploadFile>
        <FileList></FileList>
        <ChatBot></ChatBot>
        
    </SignalRContext.Provider>
}